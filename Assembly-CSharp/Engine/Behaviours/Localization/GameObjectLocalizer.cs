using System;
using Engine.Common.Services;
using Engine.Impl.Services;
using UnityEngine;

namespace Engine.Behaviours.Localization;

public class GameObjectLocalizer : EngineDependent {
	[SerializeField] private LanguageItem[] languages;
	[FromLocator] private LocalizationService localizationService;

	private void Localize() {
		var currentLanguage = localizationService.CurrentLanguage;
		for (var index = 0; index < languages.Length; ++index) {
			var language = languages[index];
			if (language.GameObject != null)
				language.GameObject.SetActive(language.Language == currentLanguage);
		}
	}

	protected override void OnConnectToEngine() {
		Localize();
		localizationService.LocalizationChanged += Localize;
	}

	protected override void OnDisconnectFromEngine() {
		localizationService.LocalizationChanged -= Localize;
	}

	[Serializable]
	private struct LanguageItem {
		public LanguageEnum Language;
		public GameObject GameObject;
	}
}