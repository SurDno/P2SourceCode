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
      characterControllerInput = AddValueInput<CharacterController>("Character");
      AddValueOutput("Velocity", () =>
      {
        CharacterController characterController = characterControllerInput.value;
        return characterController != null ? characterController.velocity : Vector3.zero;
      });
    }
  }
}
