using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class SetEulerAnglesNode : FlowControlNode
  {
    private ValueInput<Transform> targetInput;
    private ValueInput<Vector3> eulerAnglesInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        Transform transform = this.targetInput.value;
        if ((Object) transform != (Object) null)
          transform.eulerAngles = this.eulerAnglesInput.value;
        output.Call();
      }));
      this.targetInput = this.AddValueInput<Transform>("Target");
      this.eulerAnglesInput = this.AddValueInput<Vector3>("EulerAngles");
    }
  }
}
