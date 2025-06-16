using System;
using System.Collections;
using UnityEngine;

namespace RootMotion.FinalIK
{
  public abstract class OffsetModifier : MonoBehaviour
  {
    [Tooltip("The master weight")]
    public float weight = 1f;
    [Tooltip("Reference to the FBBIK component")]
    public FullBodyBipedIK ik;
    protected float lastTime;

    protected float deltaTime => Time.time - lastTime;

    protected abstract void OnModifyOffset();

    protected virtual void Start() => StartCoroutine(Initiate());

    private IEnumerator Initiate()
    {
      while (ik == null)
        yield return null;
      IKSolverFullBodyBiped solver = ik.solver;
      solver.OnPreUpdate = solver.OnPreUpdate + ModifyOffset;
      lastTime = Time.time;
    }

    private void ModifyOffset()
    {
      if (!enabled || weight <= 0.0 || deltaTime <= 0.0 || ik == null)
        return;
      weight = Mathf.Clamp(weight, 0.0f, 1f);
      OnModifyOffset();
      lastTime = Time.time;
    }

    protected void ApplyLimits(OffsetLimits[] limits)
    {
      foreach (OffsetLimits limit in limits)
        limit.Apply(ik.solver.GetEffector(limit.effector), transform.rotation);
    }

    protected virtual void OnDestroy()
    {
      if (!(ik != null))
        return;
      IKSolverFullBodyBiped solver = ik.solver;
      solver.OnPreUpdate = solver.OnPreUpdate - ModifyOffset;
    }

    [Serializable]
    public class OffsetLimits
    {
      [Tooltip("The effector type (this is just an enum)")]
      public FullBodyBipedEffector effector;
      [Tooltip("Spring force, if zero then this is a hard limit, if not, offset can exceed the limit.")]
      public float spring;
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
        if (spring <= 0.0)
        {
          if (x)
            vector3.x = Mathf.Clamp(vector3.x, minX, maxX);
          if (y)
            vector3.y = Mathf.Clamp(vector3.y, minY, maxY);
          if (z)
            vector3.z = Mathf.Clamp(vector3.z, minZ, maxZ);
        }
        else
        {
          if (x)
            vector3.x = SpringAxis(vector3.x, minX, maxX);
          if (y)
            vector3.y = SpringAxis(vector3.y, minY, maxY);
          if (z)
            vector3.z = SpringAxis(vector3.z, minZ, maxZ);
        }
        e.positionOffset = rootRotation * vector3;
      }

      private float SpringAxis(float value, float min, float max)
      {
        if (value > (double) min && value < (double) max)
          return value;
        return value < (double) min ? Spring(value, min, true) : Spring(value, max, false);
      }

      private float Spring(float value, float limit, bool negative)
      {
        float max = value - limit;
        float num = max * spring;
        return negative ? value + Mathf.Clamp(-num, 0.0f, -max) : value - Mathf.Clamp(num, 0.0f, max);
      }
    }
  }
}
