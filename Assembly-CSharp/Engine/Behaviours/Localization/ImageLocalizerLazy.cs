// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Localization.ImageLocalizerLazy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Assets.Engine.Source.Utility;
using Engine.Common.Services;
using System;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace Engine.Behaviours.Localization
{
  public class ImageLocalizerLazy : ImageLocalizerBase
  {
    [SerializeField]
    private ImageLocalizerLazy.LanguageSprite[] sprites;

    protected override Sprite GetSprite(LanguageEnum language)
    {
      Sprite sprite1 = (Sprite) null;
      for (int index = 0; index < this.sprites.Length; ++index)
      {
        ImageLocalizerLazy.LanguageSprite sprite2 = this.sprites[index];
        if (sprite2.Language == language)
        {
          sprite1 = sprite2.Sprite;
          break;
        }
      }
      return sprite1;
    }

    protected override void OnDisconnectFromEngine()
    {
      base.OnDisconnectFromEngine();
      for (int index = 0; index < this.sprites.Length; ++index)
        this.sprites[index].Dispose();
      UnityEngine.Object.Destroy((UnityEngine.Object) this.GetComponent<Image>().sprite);
    }

    [Serializable]
    private struct LanguageSprite : IDisposable
    {
      [SerializeField]
      private LanguageEnum language;
      [SerializeField]
      private ObjectReference spriteReference;
      private Sprite sprite;

      public LanguageEnum Language => this.language;

      public Sprite Sprite
      {
        get
        {
          if ((UnityEngine.Object) this.sprite == (UnityEngine.Object) null)
            this.sprite = ObjectCreator.InstantiateFromResources<Sprite>(this.spriteReference.Path);
          return this.sprite;
        }
      }

      public void Dispose()
      {
        if (!((UnityEngine.Object) this.sprite != (UnityEngine.Object) null))
          return;
        this.sprite = (Sprite) null;
      }
    }
  }
}
