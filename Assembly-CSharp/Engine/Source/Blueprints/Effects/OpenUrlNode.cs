using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Engine")]
  public class OpenUrlNode : FlowControlNode
  {
    [Port("Url")]
    private ValueInput<string> urlInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      AddFlowInput("In", () => Application.OpenURL(urlInput.value));
    }
  }
}
