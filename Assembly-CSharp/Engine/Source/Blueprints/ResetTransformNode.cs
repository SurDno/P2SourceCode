using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class ResetTransformNode : FlowControlNode
  {
    private ValueInput<Transform> transformInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        Transform transform = transformInput.value;
        if ((Object) transform != (Object) null)
          transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        output.Call();
      });
      transformInput = AddValueInput<Transform>("Transform");
    }
  }
}
