using Engine.Source.Services.Profiles;
using UnityEngine;

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
      if (!((Object) this.view != (Object) null))
        return;
      this.view.SkipAnimation();
    }

    protected override void ApplyStringValue()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.StringValue = ProfilesUtility.ConvertSaveName(this.StringValue, this.format);
    }
  }
}
