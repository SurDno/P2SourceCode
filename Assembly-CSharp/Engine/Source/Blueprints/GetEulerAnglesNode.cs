using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class GetEulerAnglesNode : FlowControlNode
  {
    private ValueInput<Transform> transformInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      transformInput = AddValueInput<Transform>("Transform");
      AddValueOutput("EulerAngles", (ValueHandler<Vector3>) (() =>
      {
        Transform transform = transformInput.value;
        return (Object) transform != (Object) null ? transform.eulerAngles : Vector3.zero;
      }));
    }
  }
}
