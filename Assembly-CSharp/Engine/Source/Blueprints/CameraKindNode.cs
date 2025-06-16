using Engine.Common.Services;
using Engine.Source.Services.CameraServices;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class CameraKindNode : FlowControlNode
  {
    private ValueInput<CameraKindEnum> cameraKindValue;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        ServiceLocator.GetService<CameraService>().Kind = this.cameraKindValue.value;
        output.Call();
      }));
      this.cameraKindValue = this.AddValueInput<CameraKindEnum>("Kind");
    }
  }
}
