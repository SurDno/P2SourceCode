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
      AddFlowInput("In", () =>
      {
        GameObject gameObject = goInput.value;
        if (!(gameObject != null))
          return;
        Object.Destroy(gameObject, delayInput.value);
      });
      goInput = AddValueInput<GameObject>("GameObject");
      delayInput = AddValueInput<float>("Delay");
    }
  }
}
