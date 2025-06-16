using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Components;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class IsDangerNode : FlowControlNode
  {
    [FromLocator]
    private ISimulation simulation;

    [Port("Value")]
    private bool Value()
    {
      IEntity player = simulation.Player;
      if (player != null)
      {
        PlayerControllerComponent component = player.GetComponent<PlayerControllerComponent>();
        if (component != null)
          return component.Danger;
      }
      return false;
    }
  }
}
