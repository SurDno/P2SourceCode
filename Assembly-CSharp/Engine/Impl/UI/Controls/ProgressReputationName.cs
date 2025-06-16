using Engine.Common.Components.Regions;
using RegionReputation;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class ProgressReputationName : ProgressView
  {
    [SerializeField]
    private StringView view;
    [SerializeField]
    private RegionReputationNames settings;
    [SerializeField]
    private RegionEnum regionName;

    public override void SkipAnimation() => this.view?.SkipAnimation();

    protected override void ApplyProgress()
    {
      if (!((Object) this.view != (Object) null) || !((Object) this.settings != (Object) null))
        return;
      this.view.StringValue = this.settings.GetReputationName(this.regionName, this.Progress);
    }
  }
}
