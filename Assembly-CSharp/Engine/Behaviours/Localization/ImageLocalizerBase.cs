using Engine.Common.Services;
using Engine.Impl.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.Behaviours.Localization;

public abstract class ImageLocalizerBase : EngineDependent {
	[FromLocator] private LocalizationService localizationService;

	private void Localize() {
		var currentLanguage = localizationService.CurrentLanguage;
		var component = GetComponent<Image>();
		component.sprite = GetSprite(currentLanguage);
		component.enabled = component.sprite != null;
	}

	protected abstract Sprite GetSprite(LanguageEnum language);

	protected override void OnConnectToEngine() {
		Localize();
		localizationService.LocalizationChanged += Localize;
	}

	protected override void OnDisconnectFromEngine() {
		localizationService.LocalizationChanged -= Localize;
	}
}