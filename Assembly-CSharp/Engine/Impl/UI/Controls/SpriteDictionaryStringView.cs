using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class SpriteDictionaryStringView : StringView
  {
    [SerializeField]
    private SpriteView view;
    [SerializeField]
    private StringSpritePair[] dictionary;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyStringValue()
    {
      if (this.dictionary == null || (Object) this.view == (Object) null)
        return;
      Sprite sprite = (Sprite) null;
      foreach (StringSpritePair stringSpritePair in this.dictionary)
      {
        if (stringSpritePair.Key == this.StringValue)
        {
          sprite = stringSpritePair.Value;
          break;
        }
      }
      this.view.SetValue(sprite, false);
    }
  }
}
