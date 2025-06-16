using Engine.Source.Services.Profiles;
using UnityEngine;

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
      if (!((Object) this.view != (Object) null))
        return;
      this.view.SkipAnimation();
    }

    protected override void ApplyStringValue()
    {
      if (!((Object) this.view != (Object) null) || !Application.isPlaying)
        return;
      this.view.StringValue = ProfilesUtility.ConvertProfileName(this.StringValue, this.formatTag);
    }
  }
}
