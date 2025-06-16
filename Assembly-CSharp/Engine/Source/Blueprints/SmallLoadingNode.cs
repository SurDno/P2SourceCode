using Engine.Common.Services;
using Engine.Impl.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class SmallLoadingNode : FlowControlNode
  {
    private ValueInput<bool> visibleInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        ServiceLocator.GetService<UIService>().SmallLoading.gameObject.SetActive(this.visibleInput.value);
        output.Call();
      }));
      this.visibleInput = this.AddValueInput<bool>("Visible");
    }
  }
}
