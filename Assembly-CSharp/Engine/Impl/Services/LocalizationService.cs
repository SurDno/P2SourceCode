using System;
using System.Collections.Generic;
using System.IO;
using Cofe.Serializations.Converters;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Settings;
using Inspectors;
using Scripts.Data;
using UnityEngine;

namespace Engine.Impl.Services
{
  [Depend(typeof (IProfilesService))]
  [RuntimeService(typeof (LocalizationService))]
  public class LocalizationService : IInitialisable
  {
    [Inspected]
    private Dictionary<LanguageEnum, Dictionary<ulong, string>> vmLanguages = new Dictionary<LanguageEnum, Dictionary<ulong, string>>();
    [Inspected]
    private Dictionary<LanguageEnum, Dictionary<string, string>> languages = new Dictionary<LanguageEnum, Dictionary<string, string>>();
    [Inspected]
    private Dictionary<LanguageEnum, Dictionary<string, string>> subTitlesLanguages = new Dictionary<LanguageEnum, Dictionary<string, string>>();
    private LanguageEnum currentLanguage;
    private LanguageEnum currentSubTitlesLanguage;
    private LanguageEnum currentLipSyncLanguage;

    [Inspected]
    public LanguageEnum DefaultLanguage => LanguageEnum.English;

    [Inspected(Mutable = true)]
    public LanguageEnum CurrentLanguage
    {
      get => currentLanguage;
      set
      {
        currentLanguage = value;
        if (currentLanguage == LanguageEnum.None)
          currentLanguage = DefaultLanguage;
        LoadLanguage(DefaultLanguage);
        if (currentLanguage != DefaultLanguage)
          LoadLanguage(currentLanguage);
        InstanceByRequest<LocalizationSettings>.Instance.Language.Value = currentLanguage;
        InstanceByRequest<LocalizationSettings>.Instance.Apply();
        FireLocalizationChanged();
      }
    }

    [Inspected(Mutable = true)]
    public LanguageEnum CurrentSubTitlesLanguage
    {
      get => currentSubTitlesLanguage;
      set
      {
        currentSubTitlesLanguage = value;
        if (currentSubTitlesLanguage == LanguageEnum.None)
          currentSubTitlesLanguage = DefaultLanguage;
        LoadSubTitlesLanguage(DefaultLanguage);
        if (currentSubTitlesLanguage != DefaultLanguage)
          LoadSubTitlesLanguage(currentSubTitlesLanguage);
        InstanceByRequest<LocalizationSettings>.Instance.SubTitlesLanguage.Value = currentSubTitlesLanguage;
        InstanceByRequest<LocalizationSettings>.Instance.Apply();
        FireLocalizationChanged();
      }
    }

    [Inspected(Mutable = true)]
    public LanguageEnum CurrentLipSyncLanguage
    {
      get => currentLipSyncLanguage;
      set
      {
        currentLipSyncLanguage = value;
        if (currentLipSyncLanguage == LanguageEnum.None)
          currentLipSyncLanguage = DefaultLanguage;
        InstanceByRequest<LocalizationSettings>.Instance.LipSyncLanguage.Value = currentLipSyncLanguage;
        InstanceByRequest<LocalizationSettings>.Instance.Apply();
        FireLocalizationChanged();
      }
    }

    public event Action LocalizationChanged;

    private void FireLocalizationChanged()
    {
      Action localizationChanged = LocalizationChanged;
      if (localizationChanged == null)
        return;
      localizationChanged();
    }

    public string GetText(LocalizedText text)
    {
      return GetVmTextImpl(text.Id, vmLanguages, currentLanguage, DefaultLanguage);
    }

    public string GetText(string tag)
    {
      return GetTextImpl(tag, languages, currentLanguage, DefaultLanguage);
    }

    public string GetSubTitlesText(string tag)
    {
      return GetTextImpl(tag, subTitlesLanguages, currentSubTitlesLanguage, DefaultLanguage);
    }

    public void Initialise()
    {
      TryOverrideSettings();
      CurrentLanguage = InstanceByRequest<LocalizationSettings>.Instance.Language.Value;
      CurrentSubTitlesLanguage = InstanceByRequest<LocalizationSettings>.Instance.SubTitlesLanguage.Value;
      CurrentLipSyncLanguage = InstanceByRequest<LocalizationSettings>.Instance.LipSyncLanguage.Value;
    }

    public void Terminate()
    {
      languages.Clear();
      vmLanguages.Clear();
      subTitlesLanguages.Clear();
    }

    private void TryOverrideSettings()
    {
      string path = PlatformUtility.GetPath("Data/Settings/DefaultLocalization.txt");
      LanguageEnum result;
      if (!File.Exists(path) || !DefaultConverter.TryParseEnum(File.ReadAllText(path), out result) || InstanceByRequest<LocalizationSettings>.Instance.OverrideLanguage.Value == result)
        return;
      InstanceByRequest<LocalizationSettings>.Instance.OverrideLanguage.Value = result;
      InstanceByRequest<LocalizationSettings>.Instance.Language.Value = result;
      InstanceByRequest<LocalizationSettings>.Instance.SubTitlesLanguage.Value = result;
      InstanceByRequest<LocalizationSettings>.Instance.LipSyncLanguage.Value = result;
    }

    private void LoadLanguage(LanguageEnum language)
    {
      string fileName = language.ToString();
      LoadLanguageImpl(languages, language, fileName);
      LoadVmLanguageImpl(vmLanguages, language, fileName);
    }

    private void LoadSubTitlesLanguage(LanguageEnum language)
    {
      string fileName = "SubTitles_" + language;
      LoadLanguageImpl(subTitlesLanguages, language, fileName);
    }

    private static void LoadLanguageImpl(
      Dictionary<LanguageEnum, Dictionary<string, string>> languages,
      LanguageEnum language,
      string fileName)
    {
      Dictionary<string, string> result;
      if (!languages.TryGetValue(language, out result))
      {
        result = new Dictionary<string, string>();
        languages.Add(language, result);
      }
      else
        result.Clear();
      TextAsset textAsset = Resources.Load<TextAsset>(fileName);
      if (textAsset != null && textAsset.text != null)
        LoadText(textAsset.text, result);
      else
        Debug.LogError("Engine localization file '" + fileName + "' not found");
    }

    private static void LoadVmLanguageImpl(
      Dictionary<LanguageEnum, Dictionary<ulong, string>> languages,
      LanguageEnum language,
      string fileName)
    {
      Dictionary<ulong, string> result;
      if (!languages.TryGetValue(language, out result))
      {
        result = new Dictionary<ulong, string>();
        languages.Add(language, result);
      }
      else
        result.Clear();
      foreach (GameDataInfo gameDataInfo in BuildSettingsUtility.GetAllGameData())
      {
        if (BuildSettingsUtility.IsDataExist(gameDataInfo.Name))
        {
          string path = PlatformUtility.GetPath("Data/VirtualMachine/{ProjectName}/Localizations/".Replace("{ProjectName}", gameDataInfo.Name) + fileName.ToLowerInvariant() + ".txt");
          if (!File.Exists(path))
            Debug.LogError("Vm localization file '" + path + "' not found");
          else
            LoadTextVm(File.ReadAllText(path), result);
        }
      }
    }

    private static string GetTextImpl(
      string tag,
      Dictionary<LanguageEnum, Dictionary<string, string>> languages,
      LanguageEnum language,
      LanguageEnum defaultLanguage)
    {
      if (tag == null)
        return "";
      Dictionary<string, string> dictionary;
      string textImpl;
      if (languages.TryGetValue(language, out dictionary) && dictionary.TryGetValue(tag, out textImpl) && !textImpl.IsNullOrEmpty())
        return textImpl;
      string str;
      return language != defaultLanguage && languages.TryGetValue(defaultLanguage, out dictionary) && dictionary.TryGetValue(tag, out str) && !str.IsNullOrEmpty() ? str : tag;
    }

    private static string GetVmTextImpl(
      ulong id,
      Dictionary<LanguageEnum, Dictionary<ulong, string>> languages,
      LanguageEnum language,
      LanguageEnum defaultLanguage)
    {
      if (id == 0UL)
        return "";
      Dictionary<ulong, string> dictionary;
      string vmTextImpl;
      if (languages.TryGetValue(language, out dictionary) && dictionary.TryGetValue(id, out vmTextImpl) && !vmTextImpl.IsNullOrEmpty())
        return vmTextImpl;
      string str;
      return language != defaultLanguage && languages.TryGetValue(defaultLanguage, out dictionary) && dictionary.TryGetValue(id, out str) && !str.IsNullOrEmpty() ? str : id.ToString();
    }

    private static void LoadText(string data, Dictionary<string, string> result)
    {
      string str1 = data;
      char[] separator = new char[2]{ '\n', '\r' };
      foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        int length = str2.IndexOf(' ');
        string key = str2;
        string str3 = "";
        if (length != -1)
        {
          key = str2.Substring(0, length);
          str3 = str2.Substring(length + 1).Replace("\\n", "\n");
        }
        result[key] = str3;
      }
    }

    private static void LoadTextVm(string data, Dictionary<ulong, string> result)
    {
      string str1 = data;
      char[] separator = new char[2]{ '\n', '\r' };
      foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        int length = str2.IndexOf(' ');
        string str3 = str2;
        string str4 = "";
        if (length != -1)
        {
          str3 = str2.Substring(0, length);
          str4 = str2.Substring(length + 1).Replace("\\n", "\n");
        }
        ulong result1;
        if (DefaultConverter.TryParseUlong(str3, out result1))
          result[result1] = str4;
      }
    }
  }
}
