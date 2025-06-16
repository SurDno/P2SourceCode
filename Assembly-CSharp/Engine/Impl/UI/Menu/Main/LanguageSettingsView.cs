using Engine.Common.Services;
using Engine.Impl.Services;
using UnityEngine;

namespace Engine.Impl.UI.Menu.Main;

public class LanguageSettingsView : SettingsView {
	[SerializeField] private LanguageEnum[] languages;
	[SerializeField] private string[] languageNames;
	private NamedIntSettingsValueView textLanguageView;
	private NamedIntSettingsValueView voiceLanguageView;
	private NamedIntSettingsValueView subtitlesLanguageView;
	private LocalizationService localizationService;

	protected override void Awake() {
		localizationService = ServiceLocator.GetService<LocalizationService>();
		layout = Instantiate(listLayoutPrefab, transform, false);
		this.textLanguageView = Instantiate(namedIntValueViewPrefab, layout.Content, false);
		this.textLanguageView.SetName("{UI.Menu.Main.Settings.Language.Text}");
		this.textLanguageView.SetValueNames(languageNames);
		var textLanguageView = this.textLanguageView;
		textLanguageView.VisibleValueChangeEvent = textLanguageView.VisibleValueChangeEvent + OnLanguageChange;
		this.voiceLanguageView = Instantiate(namedIntValueViewPrefab, layout.Content, false);
		this.voiceLanguageView.SetName("{UI.Menu.Main.Settings.Language.Voice}");
		this.voiceLanguageView.SetValueNames(languageNames);
		var voiceLanguageView = this.voiceLanguageView;
		voiceLanguageView.VisibleValueChangeEvent = voiceLanguageView.VisibleValueChangeEvent + OnLipSyncLanguageChange;
		this.subtitlesLanguageView = Instantiate(namedIntValueViewPrefab, layout.Content, false);
		this.subtitlesLanguageView.SetName("{UI.Menu.Main.Settings.Language.Subtitles}");
		this.subtitlesLanguageView.SetValueNames(languageNames);
		var subtitlesLanguageView = this.subtitlesLanguageView;
		subtitlesLanguageView.VisibleValueChangeEvent =
			subtitlesLanguageView.VisibleValueChangeEvent + OnSubTitlesLanguageChange;
		base.Awake();
	}

	private int LanguageIndex(LanguageEnum language) {
		for (var index = 0; index < languages.Length; ++index)
			if (languages[index] == language)
				return index;
		return 0;
	}

	protected override void OnButtonReset() {
		var defaultLanguage = localizationService.DefaultLanguage;
		localizationService.CurrentLanguage = defaultLanguage;
		localizationService.CurrentLipSyncLanguage = defaultLanguage;
		localizationService.CurrentSubTitlesLanguage = defaultLanguage;
		UpdateViews();
	}

	protected override void OnEnable() {
		base.OnEnable();
		UpdateViews();
	}

	private void OnLanguageChange(SettingsValueView<int> view) {
		localizationService.CurrentLanguage = languages[view.VisibleValue];
	}

	private void OnLipSyncLanguageChange(SettingsValueView<int> view) {
		localizationService.CurrentLipSyncLanguage = languages[view.VisibleValue];
	}

	private void OnSubTitlesLanguageChange(SettingsValueView<int> view) {
		localizationService.CurrentSubTitlesLanguage = languages[view.VisibleValue];
	}

	private void UpdateViews() {
		textLanguageView.VisibleValue = LanguageIndex(localizationService.CurrentLanguage);
		voiceLanguageView.VisibleValue = LanguageIndex(localizationService.CurrentLipSyncLanguage);
		subtitlesLanguageView.VisibleValue = LanguageIndex(localizationService.CurrentSubTitlesLanguage);
	}
}