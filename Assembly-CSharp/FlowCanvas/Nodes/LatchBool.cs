using ParadoxNotion.Design;
using ParadoxNotion.FlowCanvas.Module;

namespace FlowCanvas.Nodes
{
  [Name("Latch Condition")]
  [Category("Flow Controllers/Flow Convert")]
  [Description("Convert a Flow signal to boolean value")]
  [ContextDefinedOutputs(typeof (bool))]
  public class LatchBool : FlowControlNode
  {
    private bool latched;

    protected override void RegisterPorts()
    {
      FlowOutput o = AddFlowOutput("Out");
      AddFlowInput("True", () =>
      {
        latched = true;
        o.Call();
      });
      AddFlowInput("False", () =>
      {
        latched = false;
        o.Call();
      });
      AddValueOutput("Value", () => latched);
    }
  }
}
