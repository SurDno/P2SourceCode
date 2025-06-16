using Engine.Common.Services;
using Engine.Impl.Services;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class LocalizationStringView : StringView
  {
    [SerializeField]
    private StringView view;

    public override void SkipAnimation()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.SkipAnimation();
    }

    protected override void ApplyStringValue()
    {
      if ((Object) this.view == (Object) null)
        return;
      if (Application.isPlaying)
      {
        LocalizationService service = ServiceLocator.GetService<LocalizationService>();
        if (service != null)
        {
          this.view.StringValue = service.GetText(this.StringValue);
          return;
        }
      }
      this.view.StringValue = this.StringValue;
    }
  }
}
