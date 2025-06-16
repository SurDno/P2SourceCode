using Engine.Common;
using Engine.Source.Commons;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class GameObjectByEntityNode : FlowControlNode
  {
    private ValueInput<IEntity> entityInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.entityInput = this.AddValueInput<IEntity>("Entity");
      this.AddValueOutput<GameObject>("GameObject", (ValueHandler<GameObject>) (() => ((IEntityView) this.entityInput.value)?.GameObject));
    }
  }
}
