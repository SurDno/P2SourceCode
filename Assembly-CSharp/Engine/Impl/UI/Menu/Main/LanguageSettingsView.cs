using Engine.Common.Services;
using Engine.Impl.Services;
using System;
using UnityEngine;

namespace Engine.Impl.UI.Menu.Main
{
  public class LanguageSettingsView : SettingsView
  {
    [SerializeField]
    private LanguageEnum[] languages;
    [SerializeField]
    private string[] languageNames;
    private NamedIntSettingsValueView textLanguageView;
    private NamedIntSettingsValueView voiceLanguageView;
    private NamedIntSettingsValueView subtitlesLanguageView;
    private LocalizationService localizationService;

    protected override void Awake()
    {
      this.localizationService = ServiceLocator.GetService<LocalizationService>();
      this.layout = UnityEngine.Object.Instantiate<LayoutContainer>(this.listLayoutPrefab, this.transform, false);
      this.textLanguageView = UnityEngine.Object.Instantiate<NamedIntSettingsValueView>(this.namedIntValueViewPrefab, (Transform) this.layout.Content, false);
      this.textLanguageView.SetName("{UI.Menu.Main.Settings.Language.Text}");
      this.textLanguageView.SetValueNames(this.languageNames);
      NamedIntSettingsValueView textLanguageView = this.textLanguageView;
      textLanguageView.VisibleValueChangeEvent = textLanguageView.VisibleValueChangeEvent + new Action<SettingsValueView<int>>(this.OnLanguageChange);
      this.voiceLanguageView = UnityEngine.Object.Instantiate<NamedIntSettingsValueView>(this.namedIntValueViewPrefab, (Transform) this.layout.Content, false);
      this.voiceLanguageView.SetName("{UI.Menu.Main.Settings.Language.Voice}");
      this.voiceLanguageView.SetValueNames(this.languageNames);
      NamedIntSettingsValueView voiceLanguageView = this.voiceLanguageView;
      voiceLanguageView.VisibleValueChangeEvent = voiceLanguageView.VisibleValueChangeEvent + new Action<SettingsValueView<int>>(this.OnLipSyncLanguageChange);
      this.subtitlesLanguageView = UnityEngine.Object.Instantiate<NamedIntSettingsValueView>(this.namedIntValueViewPrefab, (Transform) this.layout.Content, false);
      this.subtitlesLanguageView.SetName("{UI.Menu.Main.Settings.Language.Subtitles}");
      this.subtitlesLanguageView.SetValueNames(this.languageNames);
      NamedIntSettingsValueView subtitlesLanguageView = this.subtitlesLanguageView;
      subtitlesLanguageView.VisibleValueChangeEvent = subtitlesLanguageView.VisibleValueChangeEvent + new Action<SettingsValueView<int>>(this.OnSubTitlesLanguageChange);
      base.Awake();
    }

    private int LanguageIndex(LanguageEnum language)
    {
      for (int index = 0; index < this.languages.Length; ++index)
      {
        if (this.languages[index] == language)
          return index;
      }
      return 0;
    }

    protected override void OnButtonReset()
    {
      LanguageEnum defaultLanguage = this.localizationService.DefaultLanguage;
      this.localizationService.CurrentLanguage = defaultLanguage;
      this.localizationService.CurrentLipSyncLanguage = defaultLanguage;
      this.localizationService.CurrentSubTitlesLanguage = defaultLanguage;
      this.UpdateViews();
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.UpdateViews();
    }

    private void OnLanguageChange(SettingsValueView<int> view)
    {
      this.localizationService.CurrentLanguage = this.languages[view.VisibleValue];
    }

    private void OnLipSyncLanguageChange(SettingsValueView<int> view)
    {
      this.localizationService.CurrentLipSyncLanguage = this.languages[view.VisibleValue];
    }

    private void OnSubTitlesLanguageChange(SettingsValueView<int> view)
    {
      this.localizationService.CurrentSubTitlesLanguage = this.languages[view.VisibleValue];
    }

    private void UpdateViews()
    {
      this.textLanguageView.VisibleValue = this.LanguageIndex(this.localizationService.CurrentLanguage);
      this.voiceLanguageView.VisibleValue = this.LanguageIndex(this.localizationService.CurrentLipSyncLanguage);
      this.subtitlesLanguageView.VisibleValue = this.LanguageIndex(this.localizationService.CurrentSubTitlesLanguage);
    }
  }
}
