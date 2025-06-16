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
      if (dictionary == null || (Object) view == (Object) null)
        return;
      Sprite sprite = (Sprite) null;
      foreach (StringSpritePair stringSpritePair in dictionary)
      {
        if (stringSpritePair.Key == StringValue)
        {
          sprite = stringSpritePair.Value;
          break;
        }
      }
      view.SetValue(sprite, false);
    }
  }
}
