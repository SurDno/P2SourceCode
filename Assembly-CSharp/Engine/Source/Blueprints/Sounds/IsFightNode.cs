using Engine.Common.Services;
using Engine.Source.Services;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class IsFightNode : FlowControlNode
  {
    [FromLocator]
    private ISimulation simulation;

    [Port("Value")]
    private bool Value()
    {
      if (simulation.Player != null)
      {
        CombatService service = ServiceLocator.GetService<CombatService>();
        if (service != null)
          return service.PlayerIsFighting;
      }
      return false;
    }
  }
}
