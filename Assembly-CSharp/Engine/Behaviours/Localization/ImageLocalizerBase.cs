using Engine.Common.Services;
using Engine.Impl.Services;

namespace Engine.Behaviours.Localization
{
  public abstract class ImageLocalizerBase : EngineDependent
  {
    [FromLocator]
    private LocalizationService localizationService;

    private void Localize()
    {
      LanguageEnum currentLanguage = localizationService.CurrentLanguage;
      Image component = this.GetComponent<Image>();
      component.sprite = GetSprite(currentLanguage);
      component.enabled = (UnityEngine.Object) component.sprite != (UnityEngine.Object) null;
    }

    protected abstract Sprite GetSprite(LanguageEnum language);

    protected override void OnConnectToEngine()
    {
      Localize();
      localizationService.LocalizationChanged += Localize;
    }

    protected override void OnDisconnectFromEngine()
    {
      localizationService.LocalizationChanged -= Localize;
    }
  }
}
