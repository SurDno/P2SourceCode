using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Components.Maps;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class SetMapFocusedItemNode : FlowControlNode
  {
    private ValueInput<IEntity> targetInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        ServiceLocator.GetService<MapService>().FocusedItem = (IMapItem) this.targetInput.value?.GetComponent<MapItemComponent>();
        output.Call();
      }));
      this.targetInput = this.AddValueInput<IEntity>("Target");
    }
  }
}
