// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Localization.ImageLocalizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Behaviours.Localization
{
  public class ImageLocalizer : ImageLocalizerBase
  {
    [SerializeField]
    private ImageLocalizer.LanguageSprite[] sprites;

    protected override Sprite GetSprite(LanguageEnum language)
    {
      Sprite sprite1 = (Sprite) null;
      for (int index = 0; index < this.sprites.Length; ++index)
      {
        ImageLocalizer.LanguageSprite sprite2 = this.sprites[index];
        if (sprite2.Language == language)
        {
          sprite1 = sprite2.Sprite;
          break;
        }
      }
      return sprite1;
    }

    [Serializable]
    private struct LanguageSprite
    {
      [SerializeField]
      private LanguageEnum language;
      [SerializeField]
      private Sprite sprite;

      public LanguageEnum Language => this.language;

      public Sprite Sprite => this.sprite;
    }
  }
}
