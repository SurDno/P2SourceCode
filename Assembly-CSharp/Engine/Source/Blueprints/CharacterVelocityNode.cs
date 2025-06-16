using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

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
      AddValueOutput("Velocity", (ValueHandler<Vector3>) (() =>
      {
        CharacterController characterController = characterControllerInput.value;
        return (Object) characterController != (Object) null ? characterController.velocity : Vector3.zero;
      }));
    }
  }
}
