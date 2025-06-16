using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

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
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        Transform transform = targetInput.value;
        if ((Object) transform != (Object) null)
          transform.eulerAngles = eulerAnglesInput.value;
        output.Call();
      });
      targetInput = AddValueInput<Transform>("Target");
      eulerAnglesInput = AddValueInput<Vector3>("EulerAngles");
    }
  }
}
