// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Localization.ImageLocalizerBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using System;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
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
