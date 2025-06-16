using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class MoveTransformBySpeedNode : FlowControlNode
  {
    private ValueInput<Transform> fromInput;
    private ValueInput<Transform> toInput;
    private ValueInput<Transform> targetInput;
    private ValueInput<float> speedInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        Transform target = this.targetInput.value;
        if (!((Object) target != (Object) null))
          return;
        Transform from = this.fromInput.value;
        if ((Object) from != (Object) null)
        {
          Transform to = this.toInput.value;
          if ((Object) to != (Object) null)
          {
            target.position = from.position;
            target.rotation = from.rotation;
            this.StartCoroutine(this.Move(target, from, to, this.speedInput.value, output));
          }
        }
      }));
      this.fromInput = this.AddValueInput<Transform>("From");
      this.toInput = this.AddValueInput<Transform>("To");
      this.targetInput = this.AddValueInput<Transform>("Target");
      this.speedInput = this.AddValueInput<float>("Speed");
    }

    private IEnumerator Move(
      Transform target,
      Transform from,
      Transform to,
      float speed,
      FlowOutput output)
    {
      float progress = 0.0f;
      float distance = (to.position - from.position).magnitude;
      while (true)
      {
        yield return (object) null;
        progress += Time.deltaTime * speed;
        if ((double) progress < (double) distance)
        {
          target.position = Vector3.Lerp(from.position, to.position, progress / distance);
          target.rotation = Quaternion.Lerp(from.rotation, to.rotation, progress / distance);
        }
        else
          break;
      }
      target.position = to.position;
      target.rotation = to.rotation;
      output.Call();
    }
  }
}
