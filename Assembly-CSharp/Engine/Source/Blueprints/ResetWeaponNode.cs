using Engine.Common.Components;
using Engine.Common.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class ResetWeaponNode : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        ServiceLocator.GetService<ISimulation>().Player?.GetComponent<IAttackerPlayerComponent>()?.ResetWeapon();
        output.Call();
      });
    }
  }
}
