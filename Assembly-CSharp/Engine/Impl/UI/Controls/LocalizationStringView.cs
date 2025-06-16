using Engine.Common.Services;
using Engine.Impl.Services;

namespace Engine.Impl.UI.Controls
{
  public class LocalizationStringView : StringView
  {
    [SerializeField]
    private StringView view;

    public override void SkipAnimation()
    {
      if (!((Object) view != (Object) null))
        return;
      view.SkipAnimation();
    }

    protected override void ApplyStringValue()
    {
      if ((Object) view == (Object) null)
        return;
      if (Application.isPlaying)
      {
        LocalizationService service = ServiceLocator.GetService<LocalizationService>();
        if (service != null)
        {
          view.StringValue = service.GetText(StringValue);
          return;
        }
      }
      view.StringValue = StringValue;
    }
  }
}
