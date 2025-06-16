using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class CopyTransformNode : FlowControlNode
  {
    private ValueInput<Transform> fromInput;
    private ValueInput<Transform> toInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        Transform transform1 = fromInput.value;
        Transform transform2 = toInput.value;
        if (transform1 != null && transform2 != null)
          transform2.SetPositionAndRotation(transform1.position, transform1.rotation);
        output.Call();
      });
      fromInput = AddValueInput<Transform>("From");
      toInput = AddValueInput<Transform>("To");
    }
  }
}
