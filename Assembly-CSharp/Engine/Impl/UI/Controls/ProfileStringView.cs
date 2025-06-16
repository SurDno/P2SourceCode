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
      if (!(view != null))
        return;
      view.SkipAnimation();
    }

    protected override void ApplyStringValue()
    {
      if (!(view != null) || !Application.isPlaying)
        return;
      view.StringValue = ProfilesUtility.ConvertProfileName(StringValue, formatTag);
    }
  }
}
