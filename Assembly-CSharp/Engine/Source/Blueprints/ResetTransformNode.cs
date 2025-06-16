using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class ResetTransformNode : FlowControlNode
  {
    private ValueInput<Transform> transformInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        Transform transform = this.transformInput.value;
        if ((Object) transform != (Object) null)
          transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        output.Call();
      }));
      this.transformInput = this.AddValueInput<Transform>("Transform");
    }
  }
}
