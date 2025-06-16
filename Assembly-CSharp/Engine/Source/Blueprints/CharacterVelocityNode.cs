using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class CharacterVelocityNode : FlowControlNode
  {
    private ValueInput<CharacterController> characterControllerInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.characterControllerInput = this.AddValueInput<CharacterController>("Character");
      this.AddValueOutput<Vector3>("Velocity", (ValueHandler<Vector3>) (() =>
      {
        CharacterController characterController = this.characterControllerInput.value;
        return (Object) characterController != (Object) null ? characterController.velocity : Vector3.zero;
      }));
    }
  }
}
