using Engine.Common.Services;
using Engine.Impl.Services;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.Behaviours.Localization
{
  public abstract class ImageLocalizerBase : EngineDependent
  {
    [FromLocator]
    private LocalizationService localizationService;

    private void Localize()
    {
      LanguageEnum currentLanguage = this.localizationService.CurrentLanguage;
      Image component = this.GetComponent<Image>();
      component.sprite = this.GetSprite(currentLanguage);
      component.enabled = (UnityEngine.Object) component.sprite != (UnityEngine.Object) null;
    }

    protected abstract Sprite GetSprite(LanguageEnum language);

    protected override void OnConnectToEngine()
    {
      this.Localize();
      this.localizationService.LocalizationChanged += new Action(this.Localize);
    }

    protected override void OnDisconnectFromEngine()
    {
      this.localizationService.LocalizationChanged -= new Action(this.Localize);
    }
  }
}
