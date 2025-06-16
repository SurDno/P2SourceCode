using System;
using Assets.Engine.Source.Utility;
using Engine.Common.Services;

namespace Engine.Behaviours.Localization
{
  public class ImageLocalizerLazy : ImageLocalizerBase
  {
    [SerializeField]
    private LanguageSprite[] sprites;

    protected override Sprite GetSprite(LanguageEnum language)
    {
      Sprite sprite1 = (Sprite) null;
      for (int index = 0; index < sprites.Length; ++index)
      {
        LanguageSprite sprite2 = sprites[index];
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
      for (int index = 0; index < sprites.Length; ++index)
        sprites[index].Dispose();
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

      public LanguageEnum Language => language;

      public Sprite Sprite
      {
        get
        {
          if ((UnityEngine.Object) sprite == (UnityEngine.Object) null)
            sprite = ObjectCreator.InstantiateFromResources<Sprite>(spriteReference.Path);
          return sprite;
        }
      }

      public void Dispose()
      {
        if (!((UnityEngine.Object) sprite != (UnityEngine.Object) null))
          return;
        sprite = (Sprite) null;
      }
    }
  }
}
