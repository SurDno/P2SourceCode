using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class EnableGameObjectNode : FlowControlNode
  {
    private ValueInput<GameObject> goInput;
    private ValueInput<bool> enableInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        GameObject gameObject = this.goInput.value;
        if ((Object) gameObject != (Object) null)
          gameObject.SetActive(this.enableInput.value);
        output.Call();
      }));
      this.goInput = this.AddValueInput<GameObject>("GameObject");
      this.enableInput = this.AddValueInput<bool>("Enable");
    }
  }
}
