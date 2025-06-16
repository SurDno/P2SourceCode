using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class RotateByPlayerNode : FlowControlNode
  {
    private ValueInput<Transform> targetValue;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        Transform transform = targetValue.value;
        if (transform != null)
        {
          IEntity player = ServiceLocator.GetService<ISimulation>().Player;
          if (player != null)
          {
            GameObject gameObject = ((IEntityView) player).GameObject;
            if (gameObject != null && Mathf.Sign(Vector3.Dot(transform.parent.rotation * Vector3.forward, gameObject.transform.rotation * Vector3.forward)) == -1.0)
            {
              Vector3 eulerAngles = transform.rotation.eulerAngles;
              eulerAngles.y += 180f;
              transform.rotation = Quaternion.Euler(eulerAngles);
            }
          }
        }
        output.Call();
      });
      targetValue = AddValueInput<Transform>("Target");
    }
  }
}
