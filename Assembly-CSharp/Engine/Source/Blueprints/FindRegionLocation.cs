using Engine.Common;
using Engine.Common.Components.Locations;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class FindRegionLocation : FlowControlNode
  {
    private ValueInput<IEntity> entityInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.entityInput = this.AddValueInput<IEntity>("Entity");
      this.AddValueOutput<IEntity>("Location", (ValueHandler<IEntity>) (() =>
      {
        for (IEntity parent = this.entityInput.value; parent != null; parent = parent.Parent)
        {
          LocationComponent component = parent.GetComponent<LocationComponent>();
          if (component != null && component.LocationType == LocationType.Region)
            return component.Owner;
        }
        return (IEntity) null;
      }));
    }
  }
}
