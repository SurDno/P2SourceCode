using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class GetAttachedGameObjectNode : FlowControlNode
  {
    private ValueInput<GameObject> goInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.goInput = this.AddValueInput<GameObject>("GameObject");
      this.AddValueOutput<GameObject>("Attached", (ValueHandler<GameObject>) (() =>
      {
        GameObject context = this.goInput.value;
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
