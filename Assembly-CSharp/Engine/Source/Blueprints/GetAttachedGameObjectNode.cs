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
      goInput = AddValueInput<GameObject>("GameObject");
      AddValueOutput("Attached", () =>
      {
        GameObject context = goInput.value;
        if (context != null)
        {
          AttachedGameObject component = context.GetComponent<AttachedGameObject>();
          if (component != null)
            return component.Attached;
          Debug.LogError("Attached object not found : " + context.name, context);
        }
        return null;
      });
    }
  }
}
