using Engine.Common;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

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
        if (!((Object) context != (Object) null))
          return null;
        IEntity entity = EntityUtility.GetEntity(goInput.value);
        if (entity == null)
          Debug.LogError((object) ("Entity not found : " + context.name), (Object) context);
        return entity;
      });
    }
  }
}
