using Engine.Common;
using Engine.Source.Commons;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class GameObjectByEntityNode : FlowControlNode
  {
    private ValueInput<IEntity> entityInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      entityInput = AddValueInput<IEntity>("Entity");
      AddValueOutput("GameObject", () => ((IEntityView) entityInput.value)?.GameObject);
    }
  }
}
