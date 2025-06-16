using Engine.Source.Services.Profiles;

namespace Engine.Impl.UI.Controls
{
  public class SaveFileStringView : StringView
  {
    [SerializeField]
    private StringView view;
    [SerializeField]
    private string format;

    public override void SkipAnimation()
    {
      if (!((Object) view != (Object) null))
        return;
      view.SkipAnimation();
    }

    protected override void ApplyStringValue()
    {
      if (!((Object) view != (Object) null))
        return;
      view.StringValue = ProfilesUtility.ConvertSaveName(StringValue, format);
    }
  }
}
