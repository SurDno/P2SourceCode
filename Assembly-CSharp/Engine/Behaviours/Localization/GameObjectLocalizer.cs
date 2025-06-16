using Engine.Common.Services;
using Engine.Impl.Services;
using System;
using UnityEngine;

namespace Engine.Behaviours.Localization
{
  public class GameObjectLocalizer : EngineDependent
  {
    [SerializeField]
    private GameObjectLocalizer.LanguageItem[] languages;
    [FromLocator]
    private LocalizationService localizationService;

    private void Localize()
    {
      LanguageEnum currentLanguage = this.localizationService.CurrentLanguage;
      for (int index = 0; index < this.languages.Length; ++index)
      {
        GameObjectLocalizer.LanguageItem language = this.languages[index];
        if ((UnityEngine.Object) language.GameObject != (UnityEngine.Object) null)
          language.GameObject.SetActive(language.Language == currentLanguage);
      }
    }

    protected override void OnConnectToEngine()
    {
      this.Localize();
      this.localizationService.LocalizationChanged += new Action(this.Localize);
    }

    protected override void OnDisconnectFromEngine()
    {
      this.localizationService.LocalizationChanged -= new Action(this.Localize);
    }

    [Serializable]
    private struct LanguageItem
    {
      public LanguageEnum Language;
      public GameObject GameObject;
    }
  }
}
