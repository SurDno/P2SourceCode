using Engine.Impl.UI.Menu.Main;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class LoadWindowVisibleNode : FlowControlNode
  {
    private ValueInput<bool> visibleInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() => LoadWindow.Instance.Show = this.visibleInput.value));
      this.visibleInput = this.AddValueInput<bool>("Visible");
    }
  }
}
