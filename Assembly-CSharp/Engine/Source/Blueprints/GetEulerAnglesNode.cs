using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class GetEulerAnglesNode : FlowControlNode
  {
    private ValueInput<Transform> transformInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.transformInput = this.AddValueInput<Transform>("Transform");
      this.AddValueOutput<Vector3>("EulerAngles", (ValueHandler<Vector3>) (() =>
      {
        Transform transform = this.transformInput.value;
        return (Object) transform != (Object) null ? transform.eulerAngles : Vector3.zero;
      }));
    }
  }
}
