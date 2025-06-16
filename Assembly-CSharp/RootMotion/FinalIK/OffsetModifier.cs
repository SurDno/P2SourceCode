// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.OffsetModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  public abstract class OffsetModifier : MonoBehaviour
  {
    [Tooltip("The master weight")]
    public float weight = 1f;
    [Tooltip("Reference to the FBBIK component")]
    public FullBodyBipedIK ik;
    protected float lastTime;

    protected float deltaTime => Time.time - this.lastTime;

    protected abstract void OnModifyOffset();

    protected virtual void Start() => this.StartCoroutine(this.Initiate());

    private IEnumerator Initiate()
    {
      while ((UnityEngine.Object) this.ik == (UnityEngine.Object) null)
        yield return (object) null;
      IKSolverFullBodyBiped solver = this.ik.solver;
      solver.OnPreUpdate = solver.OnPreUpdate + new IKSolver.UpdateDelegate(this.ModifyOffset);
      this.lastTime = Time.time;
    }

    private void ModifyOffset()
    {
      if (!this.enabled || (double) this.weight <= 0.0 || (double) this.deltaTime <= 0.0 || (UnityEngine.Object) this.ik == (UnityEngine.Object) null)
        return;
      this.weight = Mathf.Clamp(this.weight, 0.0f, 1f);
      this.OnModifyOffset();
      this.lastTime = Time.time;
    }

    protected void ApplyLimits(OffsetModifier.OffsetLimits[] limits)
    {
      foreach (OffsetModifier.OffsetLimits limit in limits)
        limit.Apply(this.ik.solver.GetEffector(limit.effector), this.transform.rotation);
    }

    protected virtual void OnDestroy()
    {
      if (!((UnityEngine.Object) this.ik != (UnityEngine.Object) null))
        return;
      IKSolverFullBodyBiped solver = this.ik.solver;
      solver.OnPreUpdate = solver.OnPreUpdate - new IKSolver.UpdateDelegate(this.ModifyOffset);
    }

    [Serializable]
    public class OffsetLimits
    {
      [Tooltip("The effector type (this is just an enum)")]
      public FullBodyBipedEffector effector;
      [Tooltip("Spring force, if zero then this is a hard limit, if not, offset can exceed the limit.")]
      public float spring = 0.0f;
      [Tooltip("Which axes to limit the offset on?")]
      public bool x;
      [Tooltip("Which axes to limit the offset on?")]
      public bool y;
      [Tooltip("Which axes to limit the offset on?")]
      public bool z;
      [Tooltip("The limits")]
      public float minX;
      [Tooltip("The limits")]
      public float maxX;
      [Tooltip("The limits")]
      public float minY;
      [Tooltip("The limits")]
      public float maxY;
      [Tooltip("The limits")]
      public float minZ;
      [Tooltip("The limits")]
      public float maxZ;

      public void Apply(IKEffector e, Quaternion rootRotation)
      {
        Vector3 vector3 = Quaternion.Inverse(rootRotation) * e.positionOffset;
        if ((double) this.spring <= 0.0)
        {
          if (this.x)
            vector3.x = Mathf.Clamp(vector3.x, this.minX, this.maxX);
          if (this.y)
            vector3.y = Mathf.Clamp(vector3.y, this.minY, this.maxY);
          if (this.z)
            vector3.z = Mathf.Clamp(vector3.z, this.minZ, this.maxZ);
        }
        else
        {
          if (this.x)
            vector3.x = this.SpringAxis(vector3.x, this.minX, this.maxX);
          if (this.y)
            vector3.y = this.SpringAxis(vector3.y, this.minY, this.maxY);
          if (this.z)
            vector3.z = this.SpringAxis(vector3.z, this.minZ, this.maxZ);
        }
        e.positionOffset = rootRotation * vector3;
      }

      private float SpringAxis(float value, float min, float max)
      {
        if ((double) value > (double) min && (double) value < (double) max)
          return value;
        return (double) value < (double) min ? this.Spring(value, min, true) : this.Spring(value, max, false);
      }

      private float Spring(float value, float limit, bool negative)
      {
        float max = value - limit;
        float num = max * this.spring;
        return negative ? value + Mathf.Clamp(-num, 0.0f, -max) : value - Mathf.Clamp(num, 0.0f, max);
      }
    }
  }
}
