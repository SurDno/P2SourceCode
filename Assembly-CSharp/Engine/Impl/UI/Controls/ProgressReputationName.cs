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

    public override void SkipAnimation() => view?.SkipAnimation();

    protected override void ApplyProgress()
    {
      if (!(view != null) || !(settings != null))
        return;
      view.StringValue = settings.GetReputationName(regionName, Progress);
    }
  }
}
