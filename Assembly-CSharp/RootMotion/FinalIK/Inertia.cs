// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.Inertia
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  public class Inertia : OffsetModifier
  {
    [Tooltip("The array of Bodies")]
    public Inertia.Body[] bodies;
    [Tooltip("The array of OffsetLimits")]
    public OffsetModifier.OffsetLimits[] limits;

    public void ResetBodies()
    {
      this.lastTime = Time.time;
      foreach (Inertia.Body body in this.bodies)
        body.Reset();
    }

    protected override void OnModifyOffset()
    {
      foreach (Inertia.Body body in this.bodies)
        body.Update(this.ik.solver, this.weight, this.deltaTime);
      this.ApplyLimits(this.limits);
    }

    [Serializable]
    public class Body
    {
      [Tooltip("The Transform to follow, can be any bone of the character")]
      public Transform transform;
      [Tooltip("Linking the body to effectors. One Body can be used to offset more than one effector")]
      public Inertia.Body.EffectorLink[] effectorLinks;
      [Tooltip("The speed to follow the Transform")]
      public float speed = 10f;
      [Tooltip("The acceleration, smaller values means lazyer following")]
      public float acceleration = 3f;
      [Tooltip("Matching target velocity")]
      [Range(0.0f, 1f)]
      public float matchVelocity;
      [Tooltip("gravity applied to the Body")]
      public float gravity;
      private Vector3 delta;
      private Vector3 lazyPoint;
      private Vector3 direction;
      private Vector3 lastPosition;
      private bool firstUpdate = true;

      public void Reset()
      {
        if ((UnityEngine.Object) this.transform == (UnityEngine.Object) null)
          return;
        this.lazyPoint = this.transform.position;
        this.lastPosition = this.transform.position;
        this.direction = Vector3.zero;
      }

      public void Update(IKSolverFullBodyBiped solver, float weight, float deltaTime)
      {
        if ((UnityEngine.Object) this.transform == (UnityEngine.Object) null)
          return;
        if (this.firstUpdate)
        {
          this.Reset();
          this.firstUpdate = false;
        }
        this.direction = Vector3.Lerp(this.direction, (this.transform.position - this.lazyPoint) / deltaTime * 0.01f, deltaTime * this.acceleration);
        this.lazyPoint += this.direction * deltaTime * this.speed;
        this.delta = this.transform.position - this.lastPosition;
        this.lazyPoint += this.delta * this.matchVelocity;
        this.lazyPoint.y += this.gravity * deltaTime;
        foreach (Inertia.Body.EffectorLink effectorLink in this.effectorLinks)
          solver.GetEffector(effectorLink.effector).positionOffset += (this.lazyPoint - this.transform.position) * effectorLink.weight * weight;
        this.lastPosition = this.transform.position;
      }

      [Serializable]
      public class EffectorLink
      {
        [Tooltip("Type of the FBBIK effector to use")]
        public FullBodyBipedEffector effector;
        [Tooltip("Weight of using this effector")]
        public float weight;
      }
    }
  }
}
