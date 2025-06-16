using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Components;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class SetMapFastTravelOriginNode : FlowControlNode
  {
    private ValueInput<IEntity> targetInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        ServiceLocator.GetService<MapService>().FastTravelOrigin = targetInput.value?.GetComponent<FastTravelComponent>();
        output.Call();
      });
      targetInput = AddValueInput<IEntity>("Target");
    }
  }
}
