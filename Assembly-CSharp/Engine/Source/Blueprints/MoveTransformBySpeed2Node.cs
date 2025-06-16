// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.MoveTransformBySpeed2Node
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class MoveTransformBySpeed2Node : FlowControlNode
  {
    private ValueInput<Transform> fromInput;
    private ValueInput<Transform> toInput;
    private ValueInput<Transform> targetInput;
    private ValueInput<float> speedInput;
    private ValueInput<AnimationCurve> curveInput;
    private ValueInput<Vector3> characterSpeedInput;
    private ValueInput<float> speedOutInput;
    private const float minTime = 0.3f;

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
            this.StartCoroutine(this.Move(target, from, to, this.speedInput.value, output, this.characterSpeedInput.value, this.curveInput.value, this.speedOutInput.value));
          }
        }
      }));
      this.fromInput = this.AddValueInput<Transform>("From");
      this.toInput = this.AddValueInput<Transform>("To");
      this.targetInput = this.AddValueInput<Transform>("Target");
      this.speedInput = this.AddValueInput<float>("Speed");
      this.curveInput = this.AddValueInput<AnimationCurve>("Curve");
      this.characterSpeedInput = this.AddValueInput<Vector3>("Character Speed");
      this.speedOutInput = this.AddValueInput<float>("Speed Out");
    }

    private Vector3 EvaluateCubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
      return (float) ((1.0 - (double) t) * (1.0 - (double) t) * (1.0 - (double) t)) * p0 + (float) (3.0 * (double) t * (1.0 - (double) t) * (1.0 - (double) t)) * p1 + (float) (3.0 * (double) t * (double) t * (1.0 - (double) t)) * p2 + t * t * t * p3;
    }

    private Vector3 EvaluateCubicBezierDerivative(
      Vector3 p0,
      Vector3 p1,
      Vector3 p2,
      Vector3 p3,
      float t)
    {
      return (float) (-3.0 * (1.0 - (double) t) * (1.0 - (double) t)) * p0 + (float) (3.0 * (1.0 - (double) t) * (1.0 - (double) t) - 6.0 * (1.0 - (double) t) * (double) t) * p1 + (float) (6.0 * (1.0 - (double) t) * (double) t - 3.0 * (double) t * (double) t) * p2 + 3f * t * t * p3;
    }

    private Vector3 EvaluateCubicBezierDerivative2(
      Vector3 p0,
      Vector3 p1,
      Vector3 p2,
      Vector3 p3,
      float t)
    {
      return (float) (6.0 * (1.0 - (double) t)) * p0 + (float) (-6.0 * (1.0 - (double) t) - 6.0 * (1.0 - 2.0 * (double) t)) * p1 + (float) (6.0 * (1.0 - 2.0 * (double) t) - 6.0 * (double) t) * p2 + 6f * t * p3;
    }

    private Vector3 EvaluateQuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
      return (float) ((1.0 - (double) t) * (1.0 - (double) t)) * p0 + (float) (2.0 * (double) t * (1.0 - (double) t)) * p1 + t * t * p2;
    }

    private Vector3 EvaluateQuadraticBezierDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
      return (float) (-2.0 * (1.0 - (double) t)) * p0 + (float) (2.0 * (1.0 - 2.0 * (double) t)) * p1 + 2f * t * p2;
    }

    private IEnumerator Move(
      Transform target,
      Transform from,
      Transform to,
      float speed,
      FlowOutput output,
      Vector3 characterSpeedInput,
      AnimationCurve curve,
      float speedOut)
    {
      float progress = 0.0f;
      Vector3 vector3 = to.position - from.position;
      float distance = vector3.magnitude;
      float enterTime = distance / speed;
      if ((double) enterTime < 0.30000001192092896)
        enterTime = 0.3f;
      enterTime *= 5f;
      Vector3 p0 = from.transform.position;
      Vector3 p1 = p0 + from.transform.forward * 0.3f * distance;
      Vector3 p3 = to.transform.position;
      Vector3 p2 = p3 - to.transform.forward * 0.3f * distance;
      vector3 = this.EvaluateCubicBezierDerivative(p0, p1, p2, p3, 0.0f);
      float derivative0 = vector3.magnitude;
      float scale = characterSpeedInput.magnitude / derivative0;
      float currentSpeed = characterSpeedInput.magnitude;
      while (true)
      {
        yield return (object) null;
        float derivative = this.EvaluateCubicBezierDerivative(p0, p1, p2, p3, progress).magnitude;
        currentSpeed += 5f * Time.deltaTime;
        progress += Time.deltaTime / enterTime;
        if ((double) progress < 1.0)
        {
          target.position = this.EvaluateCubicBezier(p0, p1, p2, p3, progress);
          target.rotation = Quaternion.LerpUnclamped(from.rotation, to.rotation, progress);
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
