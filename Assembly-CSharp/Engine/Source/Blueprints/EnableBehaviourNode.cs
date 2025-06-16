using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class EnableBehaviourNode : FlowControlNode
  {
    private ValueInput<Behaviour> behaviourInput;
    private ValueInput<bool> enableInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        Behaviour behaviour = this.behaviourInput.value;
        if ((Object) behaviour != (Object) null)
          behaviour.enabled = this.enableInput.value;
        output.Call();
      }));
      this.behaviourInput = this.AddValueInput<Behaviour>("Behaviour");
      this.enableInput = this.AddValueInput<bool>("Enable");
    }
  }
}
