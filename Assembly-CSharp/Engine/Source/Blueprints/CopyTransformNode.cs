using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

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
        if ((Object) transform1 != (Object) null && (Object) transform2 != (Object) null)
          transform2.SetPositionAndRotation(transform1.position, transform1.rotation);
        output.Call();
      });
      fromInput = AddValueInput<Transform>("From");
      toInput = AddValueInput<Transform>("To");
    }
  }
}
