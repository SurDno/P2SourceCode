using Engine.Common.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class IsDoorSendEnterWithoutKnockNode : FlowControlNode
  {
    private ValueInput<IDoorComponent> doorInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.AddValueOutput<bool>("SendEnterWithoutKnock", (ValueHandler<bool>) (() =>
      {
        IDoorComponent doorComponent = this.doorInput.value;
        return doorComponent != null && doorComponent.SendEnterWithoutKnock.Value;
      }));
      this.doorInput = this.AddValueInput<IDoorComponent>("Door");
    }
  }
}
