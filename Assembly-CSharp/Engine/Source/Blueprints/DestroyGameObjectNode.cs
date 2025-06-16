using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class DestroyGameObjectNode : FlowControlNode
  {
    private ValueInput<GameObject> goInput;
    private ValueInput<float> delayInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        GameObject gameObject = this.goInput.value;
        if (!((Object) gameObject != (Object) null))
          return;
        Object.Destroy((Object) gameObject, this.delayInput.value);
      }));
      this.goInput = this.AddValueInput<GameObject>("GameObject");
      this.delayInput = this.AddValueInput<float>("Delay");
    }
  }
}
