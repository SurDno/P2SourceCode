using Engine.Common.Services;
using Engine.Source.Services.CameraServices;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class CameraTargetNode : FlowControlNode
  {
    private ValueInput<GameObject> cameraTargetValue;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        ServiceLocator.GetService<CameraService>().GameObjectTarget = cameraTargetValue.value;
        output.Call();
      });
      cameraTargetValue = AddValueInput<GameObject>("Target");
    }
  }
}
