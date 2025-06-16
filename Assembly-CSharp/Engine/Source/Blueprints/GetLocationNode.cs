using Engine.Common;
using Engine.Common.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class GetLocationNode : FlowControlNode
  {
    private ValueInput<IEntity> entityInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      entityInput = AddValueInput<IEntity>("Entity");
      AddValueOutput("Location", () =>
      {
        IEntity entity = entityInput.value;
        if (entity != null)
        {
          ILocationComponent component1 = entity.GetComponent<ILocationComponent>();
          if (component1 != null)
            return component1;
          ILocationItemComponent component2 = entity.GetComponent<ILocationItemComponent>();
          if (component2 != null)
            return component2.LogicLocation;
        }
        return null;
      });
    }
  }
}
