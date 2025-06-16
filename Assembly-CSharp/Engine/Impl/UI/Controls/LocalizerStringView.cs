using Engine.Behaviours.Localization;
using UnityEngine;

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
      if (!((Object) this.localizer != (Object) null))
        return;
      this.localizer.Signature = this.StringValue;
    }
  }
}
