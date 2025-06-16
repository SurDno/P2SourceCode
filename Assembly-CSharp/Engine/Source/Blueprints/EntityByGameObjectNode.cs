using Engine.Common;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class EntityByGameObjectNode : FlowControlNode
  {
    private ValueInput<GameObject> goInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.goInput = this.AddValueInput<GameObject>("GameObject");
      this.AddValueOutput<IEntity>("Entity", (ValueHandler<IEntity>) (() =>
      {
        GameObject context = this.goInput.value;
        if (!((Object) context != (Object) null))
          return (IEntity) null;
        IEntity entity = EntityUtility.GetEntity(this.goInput.value);
        if (entity == null)
          Debug.LogError((object) ("Entity not found : " + context.name), (Object) context);
        return entity;
      }));
    }
  }
}
