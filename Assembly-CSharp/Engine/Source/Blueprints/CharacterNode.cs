using Engine.Common;
using Engine.Common.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  [Description("НЕ ИСПОЛЬЗОВАТЬ, ИСПОЛЬЗОВАТЬ Player")]
  [Color("FF0000")]
  public class CharacterNode : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.AddValueOutput<IEntity>("Character", (ValueHandler<IEntity>) (() => ServiceLocator.GetService<ISimulation>().Player));
    }
  }
}
