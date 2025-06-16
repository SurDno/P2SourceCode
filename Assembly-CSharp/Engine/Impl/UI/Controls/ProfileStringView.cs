using Engine.Source.Services.Profiles;

namespace Engine.Impl.UI.Controls
{
  public class ProfileStringView : StringView
  {
    [SerializeField]
    private StringView view;
    [SerializeField]
    private string formatTag;

    public override void SkipAnimation()
    {
      if (!((Object) view != (Object) null))
        return;
      view.SkipAnimation();
    }

    protected override void ApplyStringValue()
    {
      if (!((Object) view != (Object) null) || !Application.isPlaying)
        return;
      view.StringValue = ProfilesUtility.ConvertProfileName(StringValue, formatTag);
    }
  }
}
