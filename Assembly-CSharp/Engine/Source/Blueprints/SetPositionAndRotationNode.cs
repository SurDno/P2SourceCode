using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

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
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        Transform transform1 = transformValue.value;
        if ((Object) transform1 != (Object) null)
        {
          Transform transform2 = valueValue.value;
          if ((Object) transform2 != (Object) null)
            transform1.SetPositionAndRotation(transform2.position, transform2.rotation);
        }
        output.Call();
      });
      transformValue = AddValueInput<Transform>("Transform");
      valueValue = AddValueInput<Transform>("Value");
    }
  }
}
