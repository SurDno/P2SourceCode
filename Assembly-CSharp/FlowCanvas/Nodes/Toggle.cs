using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Flow Controllers/Togglers")]
  [Description("When In is called, calls On or Off depending on the current toggle state. Whenever Toggle input is called the state changes.")]
  public class Toggle : FlowControlNode
  {
    public bool open = true;
    private bool original;

    public override string name => base.name + " " + (this.open ? "[ON]" : "[OFF]");

    public override void OnGraphStarted() => this.original = this.open;

    public override void OnGraphStoped() => this.open = this.original;

    protected override void RegisterPorts()
    {
      FlowOutput tOut = this.AddFlowOutput("On");
      FlowOutput fOut = this.AddFlowOutput("Off");
      this.AddFlowInput("In", (FlowHandler) (() => this.Call(this.open ? tOut : fOut)));
      this.AddFlowInput(nameof (Toggle), (FlowHandler) (() => this.open = !this.open));
    }
  }
}
