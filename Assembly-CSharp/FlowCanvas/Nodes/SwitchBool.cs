using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Name("Switch Condition")]
  [Category("Flow Controllers/Switchers")]
  [Description("Branch the Flow based on a conditional boolean value")]
  [ContextDefinedInputs(typeof (bool))]
  public class SwitchBool : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      ValueInput<bool> c = AddValueInput<bool>("Condition");
      FlowOutput tOut = AddFlowOutput("True");
      FlowOutput fOut = AddFlowOutput("False");
      AddFlowInput("In", () => Call(c.value ? tOut : fOut));
    }
  }
}
