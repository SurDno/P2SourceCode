using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class LogicEventWithEntityNode : FlowControlNode
  {
    [Port("EventName")]
    private ValueInput<string> eventNameInput;
    [Port("Entity")]
    private ValueInput<IEntity> eventEntityInput;
    [Port("Out")]
    private FlowOutput output;

    [Port("In")]
    private void In()
    {
      string name = this.eventNameInput.value;
      if (name != null)
        ServiceLocator.GetService<LogicEventService>().FireEntityEvent(name, this.eventEntityInput.value);
      this.output.Call();
    }
  }
}
