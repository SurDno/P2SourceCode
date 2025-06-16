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
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        Transform transform = this.targetValue.value;
        if ((Object) transform != (Object) null)
        {
          IEntity player = ServiceLocator.GetService<ISimulation>().Player;
          if (player != null)
          {
            GameObject gameObject = ((IEntityView) player).GameObject;
            if ((Object) gameObject != (Object) null && (double) Mathf.Sign(Vector3.Dot(transform.parent.rotation * Vector3.forward, gameObject.transform.rotation * Vector3.forward)) == -1.0)
            {
              Vector3 eulerAngles = transform.rotation.eulerAngles;
              eulerAngles.y += 180f;
              transform.rotation = Quaternion.Euler(eulerAngles);
            }
          }
        }
        output.Call();
      }));
      this.targetValue = this.AddValueInput<Transform>("Target");
    }
  }
}
