using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class SetPositionAndRotationNode : FlowControlNode
  {
    private ValueInput<Transform> transformValue;
    private ValueInput<Transform> valueValue;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        Transform transform1 = this.transformValue.value;
        if ((Object) transform1 != (Object) null)
        {
          Transform transform2 = this.valueValue.value;
          if ((Object) transform2 != (Object) null)
            transform1.SetPositionAndRotation(transform2.position, transform2.rotation);
        }
        output.Call();
      }));
      this.transformValue = this.AddValueInput<Transform>("Transform");
      this.valueValue = this.AddValueInput<Transform>("Value");
    }
  }
}
