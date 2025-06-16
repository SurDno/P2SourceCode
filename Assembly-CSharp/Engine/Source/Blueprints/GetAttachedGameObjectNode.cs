using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class GetAttachedGameObjectNode : FlowControlNode
  {
    private ValueInput<GameObject> goInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      goInput = AddValueInput<GameObject>("GameObject");
      AddValueOutput("Attached", (ValueHandler<GameObject>) (() =>
      {
        GameObject context = goInput.value;
        if ((Object) context != (Object) null)
        {
          AttachedGameObject component = context.GetComponent<AttachedGameObject>();
          if ((Object) component != (Object) null)
            return component.Attached;
          Debug.LogError((object) ("Attached object not found : " + context.name), (Object) context);
        }
        return (GameObject) null;
      }));
    }
  }
}
