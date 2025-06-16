using ParadoxNotion.Design;
using ParadoxNotion.FlowCanvas.Module;

namespace FlowCanvas.Nodes
{
  [Category("Flow Controllers/Togglers")]
  [Description("Flip Flops between the 2 outputs each time In is called")]
  [ContextDefinedOutputs(typeof (bool))]
  public class FlipFlop : FlowControlNode
  {
    public bool isFlip = true;
    private bool original;

    public override string name => base.name + " " + (isFlip ? "[FLIP]" : "[FLOP]");

    public override void OnGraphStarted() => original = isFlip;

    public override void OnGraphStoped() => isFlip = original;

    protected override void RegisterPorts()
    {
      FlowOutput flipF = AddFlowOutput("Flip");
      FlowOutput flopF = AddFlowOutput("Flop");
      AddFlowInput("In", () =>
      {
        Call(isFlip ? flipF : flopF);
        isFlip = !isFlip;
      });
      AddValueOutput("Is Flip", () => isFlip);
    }
  }
}
