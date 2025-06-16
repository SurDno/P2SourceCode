using Engine.Behaviours.Localization;

namespace Engine.Impl.UI.Controls
{
  public class LocalizerStringView : StringView
  {
    [SerializeField]
    private Localizer localizer;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyStringValue()
    {
      if (!((Object) localizer != (Object) null))
        return;
      localizer.Signature = StringValue;
    }
  }
}
