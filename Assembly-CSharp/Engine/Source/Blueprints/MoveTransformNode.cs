using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class MoveTransformNode : FlowControlNode
  {
    private ValueInput<Transform> targetInput;
    private ValueInput<Vector3> offsetInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        Transform transform = this.targetInput.value;
        if ((Object) transform != (Object) null)
        {
          Vector3 vector3 = transform.transform.position + this.offsetInput.value;
          transform.transform.position = vector3;
        }
        output.Call();
      }));
      this.targetInput = this.AddValueInput<Transform>("Target");
      this.offsetInput = this.AddValueInput<Vector3>("Offset");
    }
  }
}
