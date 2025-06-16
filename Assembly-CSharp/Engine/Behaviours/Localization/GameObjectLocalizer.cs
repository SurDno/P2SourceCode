// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Localization.GameObjectLocalizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using System;
using UnityEngine;

#nullable disable
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
