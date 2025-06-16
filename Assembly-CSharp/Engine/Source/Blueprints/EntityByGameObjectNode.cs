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
      goInput = AddValueInput<GameObject>("GameObject");
      AddValueOutput("Entity", () =>
      {
        GameObject context = goInput.value;
        if (!(context != null))
          return null;
        IEntity entity = EntityUtility.GetEntity(goInput.value);
        if (entity == null)
          Debug.LogError("Entity not found : " + context.name, context);
        return entity;
      });
    }
  }
}
