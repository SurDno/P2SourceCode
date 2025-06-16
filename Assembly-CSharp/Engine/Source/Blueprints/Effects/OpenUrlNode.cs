using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

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
      this.AddFlowInput("In", (FlowHandler) (() => Application.OpenURL(this.urlInput.value)));
    }
  }
}
