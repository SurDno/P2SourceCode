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
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        Behaviour behaviour = behaviourInput.value;
        if (behaviour != null)
          behaviour.enabled = enableInput.value;
        output.Call();
      });
      behaviourInput = AddValueInput<Behaviour>("Behaviour");
      enableInput = AddValueInput<bool>("Enable");
    }
  }
}
