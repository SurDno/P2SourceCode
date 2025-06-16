using Engine.Common.Services;
using Engine.Source.Services.CameraServices;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class CameraTargetNode : FlowControlNode
  {
    private ValueInput<GameObject> cameraTargetValue;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        ServiceLocator.GetService<CameraService>().GameObjectTarget = this.cameraTargetValue.value;
        output.Call();
      }));
      this.cameraTargetValue = this.AddValueInput<GameObject>("Target");
    }
  }
}
