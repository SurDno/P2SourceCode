using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class IsDiseasedNode : FlowControlNode
  {
    [FromLocator]
    private ISimulation simulation;
    [Port("Diseased")]
    private ValueInput<DiseasedStateEnum> diseasedInput;

    [Port("Value")]
    private bool Value()
    {
      IEntity player = simulation.Player;
      if (player != null)
      {
        INavigationComponent component = player.GetComponent<INavigationComponent>();
        if (component != null && component.Region is RegionComponent region)
          return diseasedInput.value == DiseasedUtility.GetStateByLevel(region.DiseaseLevel.Value);
      }
      return false;
    }
  }
}
