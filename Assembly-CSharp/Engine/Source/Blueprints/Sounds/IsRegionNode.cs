using Engine.Common;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class IsRegionNode : FlowControlNode
  {
    [FromLocator]
    private ISimulation simulation;
    [Port("Region")]
    private ValueInput<RegionEnum> regionInput;

    [Port("Value")]
    private bool Value()
    {
      IEntity player = this.simulation.Player;
      if (player != null)
      {
        NavigationComponent component = player.GetComponent<NavigationComponent>();
        if (component != null)
        {
          RegionComponent region = (RegionComponent) component.Region;
          if (region != null)
            return region.Region == this.regionInput.value;
        }
      }
      return false;
    }
  }
}
