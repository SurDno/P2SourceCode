// Decompiled with JetBrains decompiler
// Type: Engine.Impl.Services.LocalizationService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#nullable disable
namespace Engine.Impl.Services
{
  [Depend(typeof (IProfilesService))]
  [RuntimeService(new System.Type[] {typeof (LocalizationService)})]
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
      get => this.currentLanguage;
      set
      {
        this.currentLanguage = value;
        if (this.currentLanguage == LanguageEnum.None)
          this.currentLanguage = this.DefaultLanguage;
        this.LoadLanguage(this.DefaultLanguage);
        if (this.currentLanguage != this.DefaultLanguage)
          this.LoadLanguage(this.currentLanguage);
        InstanceByRequest<LocalizationSettings>.Instance.Language.Value = this.currentLanguage;
        InstanceByRequest<LocalizationSettings>.Instance.Apply();
        this.FireLocalizationChanged();
      }
    }

    [Inspected(Mutable = true)]
    public LanguageEnum CurrentSubTitlesLanguage
    {
      get => this.currentSubTitlesLanguage;
      set
      {
        this.currentSubTitlesLanguage = value;
        if (this.currentSubTitlesLanguage == LanguageEnum.None)
          this.currentSubTitlesLanguage = this.DefaultLanguage;
        this.LoadSubTitlesLanguage(this.DefaultLanguage);
        if (this.currentSubTitlesLanguage != this.DefaultLanguage)
          this.LoadSubTitlesLanguage(this.currentSubTitlesLanguage);
        InstanceByRequest<LocalizationSettings>.Instance.SubTitlesLanguage.Value = this.currentSubTitlesLanguage;
        InstanceByRequest<LocalizationSettings>.Instance.Apply();
        this.FireLocalizationChanged();
      }
    }

    [Inspected(Mutable = true)]
    public LanguageEnum CurrentLipSyncLanguage
    {
      get => this.currentLipSyncLanguage;
      set
      {
        this.currentLipSyncLanguage = value;
        if (this.currentLipSyncLanguage == LanguageEnum.None)
          this.currentLipSyncLanguage = this.DefaultLanguage;
        InstanceByRequest<LocalizationSettings>.Instance.LipSyncLanguage.Value = this.currentLipSyncLanguage;
        InstanceByRequest<LocalizationSettings>.Instance.Apply();
        this.FireLocalizationChanged();
      }
    }

    public event Action LocalizationChanged;

    private void FireLocalizationChanged()
    {
      Action localizationChanged = this.LocalizationChanged;
      if (localizationChanged == null)
        return;
      localizationChanged();
    }

    public string GetText(LocalizedText text)
    {
      return LocalizationService.GetVmTextImpl(text.Id, this.vmLanguages, this.currentLanguage, this.DefaultLanguage);
    }

    public string GetText(string tag)
    {
      return LocalizationService.GetTextImpl(tag, this.languages, this.currentLanguage, this.DefaultLanguage);
    }

    public string GetSubTitlesText(string tag)
    {
      return LocalizationService.GetTextImpl(tag, this.subTitlesLanguages, this.currentSubTitlesLanguage, this.DefaultLanguage);
    }

    public void Initialise()
    {
      this.TryOverrideSettings();
      this.CurrentLanguage = InstanceByRequest<LocalizationSettings>.Instance.Language.Value;
      this.CurrentSubTitlesLanguage = InstanceByRequest<LocalizationSettings>.Instance.SubTitlesLanguage.Value;
      this.CurrentLipSyncLanguage = InstanceByRequest<LocalizationSettings>.Instance.LipSyncLanguage.Value;
    }

    public void Terminate()
    {
      this.languages.Clear();
      this.vmLanguages.Clear();
      this.subTitlesLanguages.Clear();
    }

    private void TryOverrideSettings()
    {
      string path = PlatformUtility.GetPath("Data/Settings/DefaultLocalization.txt");
      LanguageEnum result;
      if (!File.Exists(path) || !DefaultConverter.TryParseEnum<LanguageEnum>(File.ReadAllText(path), out result) || InstanceByRequest<LocalizationSettings>.Instance.OverrideLanguage.Value == result)
        return;
      InstanceByRequest<LocalizationSettings>.Instance.OverrideLanguage.Value = result;
      InstanceByRequest<LocalizationSettings>.Instance.Language.Value = result;
      InstanceByRequest<LocalizationSettings>.Instance.SubTitlesLanguage.Value = result;
      InstanceByRequest<LocalizationSettings>.Instance.LipSyncLanguage.Value = result;
    }

    private void LoadLanguage(LanguageEnum language)
    {
      string fileName = language.ToString();
      LocalizationService.LoadLanguageImpl(this.languages, language, fileName);
      LocalizationService.LoadVmLanguageImpl(this.vmLanguages, language, fileName);
    }

    private void LoadSubTitlesLanguage(LanguageEnum language)
    {
      string fileName = "SubTitles_" + language.ToString();
      LocalizationService.LoadLanguageImpl(this.subTitlesLanguages, language, fileName);
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
      if ((UnityEngine.Object) textAsset != (UnityEngine.Object) null && textAsset.text != null)
        LocalizationService.LoadText(textAsset.text, result);
      else
        Debug.LogError((object) ("Engine localization file '" + fileName + "' not found"));
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
            Debug.LogError((object) ("Vm localization file '" + path + "' not found"));
          else
            LocalizationService.LoadTextVm(File.ReadAllText(path), result);
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
