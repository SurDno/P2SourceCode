// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.RotationLimitPolygonal
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  [HelpURL("http://www.root-motion.com/finalikdox/html/page12.html")]
  [AddComponentMenu("Scripts/RootMotion.FinalIK/Rotation Limits/Rotation Limit Polygonal")]
  public class RotationLimitPolygonal : RotationLimit
  {
    [Range(0.0f, 180f)]
    public float twistLimit = 180f;
    [Range(0.0f, 3f)]
    public int smoothIterations = 0;
    [SerializeField]
    [HideInInspector]
    public RotationLimitPolygonal.LimitPoint[] points;
    [SerializeField]
    [HideInInspector]
    public Vector3[] P;
    [SerializeField]
    [HideInInspector]
    public RotationLimitPolygonal.ReachCone[] reachCones = new RotationLimitPolygonal.ReachCone[0];

    [ContextMenu("User Manual")]
    private void OpenUserManual()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/page12.html");
    }

    [ContextMenu("Scrpt Reference")]
    private void OpenScriptReference()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_rotation_limit_polygonal.html");
    }

    [ContextMenu("Support Group")]
    private void SupportGroup()
    {
      Application.OpenURL("https://groups.google.com/forum/#!forum/final-ik");
    }

    [ContextMenu("Asset Store Thread")]
    private void ASThread()
    {
      Application.OpenURL("http://forum.unity3d.com/threads/final-ik-full-body-ik-aim-look-at-fabrik-ccd-ik-1-0-released.222685/");
    }

    public void SetLimitPoints(RotationLimitPolygonal.LimitPoint[] points)
    {
      if (points.Length < 3)
      {
        this.LogWarning("The polygon must have at least 3 Limit Points.");
      }
      else
      {
        this.points = points;
        this.BuildReachCones();
      }
    }

    protected override Quaternion LimitRotation(Quaternion rotation)
    {
      if (this.reachCones.Length == 0)
        this.Start();
      return RotationLimit.LimitTwist(this.LimitSwing(rotation), this.axis, this.secondaryAxis, this.twistLimit);
    }

    private void Start()
    {
      if (this.points.Length < 3)
        this.ResetToDefault();
      for (int index = 0; index < this.reachCones.Length; ++index)
      {
        if (!this.reachCones[index].isValid)
        {
          if (this.smoothIterations <= 0)
          {
            int num = index >= this.reachCones.Length - 1 ? 0 : index + 1;
            this.LogWarning("Reach Cone {point " + (object) index + ", point " + (object) num + ", Origin} has negative volume. Make sure Axis vector is in the reachable area and the polygon is convex.");
          }
          else
            this.LogWarning("One of the Reach Cones in the polygon has negative volume. Make sure Axis vector is in the reachable area and the polygon is convex.");
        }
      }
      this.axis = this.axis.normalized;
    }

    public void ResetToDefault()
    {
      this.points = new RotationLimitPolygonal.LimitPoint[4];
      for (int index = 0; index < this.points.Length; ++index)
        this.points[index] = new RotationLimitPolygonal.LimitPoint();
      Quaternion rotation1 = Quaternion.AngleAxis(45f, Vector3.right);
      Quaternion rotation2 = Quaternion.AngleAxis(45f, Vector3.up);
      this.points[0].point = rotation1 * rotation2 * this.axis;
      this.points[1].point = Quaternion.Inverse(rotation1) * rotation2 * this.axis;
      this.points[2].point = Quaternion.Inverse(rotation1) * Quaternion.Inverse(rotation2) * this.axis;
      this.points[3].point = rotation1 * Quaternion.Inverse(rotation2) * this.axis;
      this.BuildReachCones();
    }

    public void BuildReachCones()
    {
      this.smoothIterations = Mathf.Clamp(this.smoothIterations, 0, 3);
      this.P = new Vector3[this.points.Length];
      for (int index = 0; index < this.points.Length; ++index)
        this.P[index] = this.points[index].point.normalized;
      for (int index = 0; index < this.smoothIterations; ++index)
        this.P = this.SmoothPoints();
      this.reachCones = new RotationLimitPolygonal.ReachCone[this.P.Length];
      for (int index = 0; index < this.reachCones.Length - 1; ++index)
        this.reachCones[index] = new RotationLimitPolygonal.ReachCone(Vector3.zero, this.axis.normalized, this.P[index], this.P[index + 1]);
      this.reachCones[this.P.Length - 1] = new RotationLimitPolygonal.ReachCone(Vector3.zero, this.axis.normalized, this.P[this.P.Length - 1], this.P[0]);
      for (int index = 0; index < this.reachCones.Length; ++index)
        this.reachCones[index].Calculate();
    }

    private Vector3[] SmoothPoints()
    {
      Vector3[] vector3Array = new Vector3[this.P.Length * 2];
      float scalar = this.GetScalar(this.P.Length);
      for (int index = 0; index < vector3Array.Length; index += 2)
        vector3Array[index] = this.PointToTangentPlane(this.P[index / 2], 1f);
      for (int index = 1; index < vector3Array.Length; index += 2)
      {
        Vector3 zero1 = Vector3.zero;
        Vector3 zero2 = Vector3.zero;
        Vector3 zero3 = Vector3.zero;
        if (index > 1 && index < vector3Array.Length - 2)
        {
          zero1 = vector3Array[index - 2];
          zero3 = vector3Array[index + 1];
        }
        else if (index == 1)
        {
          zero1 = vector3Array[vector3Array.Length - 2];
          zero3 = vector3Array[index + 1];
        }
        else if (index == vector3Array.Length - 1)
        {
          zero1 = vector3Array[index - 2];
          zero3 = vector3Array[0];
        }
        Vector3 vector3 = index >= vector3Array.Length - 1 ? vector3Array[0] : vector3Array[index + 1];
        int num = vector3Array.Length / this.points.Length;
        vector3Array[index] = 0.5f * (vector3Array[index - 1] + vector3) + scalar * this.points[index / num].tangentWeight * (vector3 - zero1) + scalar * this.points[index / num].tangentWeight * (vector3Array[index - 1] - zero3);
      }
      for (int index = 0; index < vector3Array.Length; ++index)
        vector3Array[index] = this.TangentPointToSphere(vector3Array[index], 1f);
      return vector3Array;
    }

    private float GetScalar(int k)
    {
      if (k <= 3)
        return 0.1667f;
      if (k == 4)
        return 0.1036f;
      if (k == 5)
        return 0.085f;
      if (k == 6)
        return 0.0773f;
      return k == 7 ? 0.07f : 1f / 16f;
    }

    private Vector3 PointToTangentPlane(Vector3 p, float r)
    {
      float num1 = Vector3.Dot(this.axis, p);
      float num2 = (float) (2.0 * (double) r * (double) r / ((double) r * (double) r + (double) num1));
      return num2 * p + (1f - num2) * -this.axis;
    }

    private Vector3 TangentPointToSphere(Vector3 q, float r)
    {
      float num1 = Vector3.Dot(q - this.axis, q - this.axis);
      float num2 = (float) (4.0 * (double) r * (double) r / (4.0 * (double) r * (double) r + (double) num1));
      return num2 * q + (1f - num2) * -this.axis;
    }

    private Quaternion LimitSwing(Quaternion rotation)
    {
      if (rotation == Quaternion.identity)
        return rotation;
      Vector3 vector3 = rotation * this.axis;
      int reachCone = this.GetReachCone(vector3);
      if (reachCone == -1)
      {
        if (!Warning.logged)
          this.LogWarning("RotationLimitPolygonal reach cones are invalid.");
        return rotation;
      }
      if ((double) Vector3.Dot(this.reachCones[reachCone].B, vector3) > 0.0)
        return rotation;
      Vector3 rhs = Vector3.Cross(this.axis, vector3);
      Vector3 toDirection = Vector3.Cross(-this.reachCones[reachCone].B, rhs);
      return Quaternion.FromToRotation(rotation * this.axis, toDirection) * rotation;
    }

    private int GetReachCone(Vector3 L)
    {
      float num1 = Vector3.Dot(this.reachCones[0].S, L);
      for (int reachCone = 0; reachCone < this.reachCones.Length; ++reachCone)
      {
        float num2 = num1;
        num1 = reachCone >= this.reachCones.Length - 1 ? Vector3.Dot(this.reachCones[0].S, L) : Vector3.Dot(this.reachCones[reachCone + 1].S, L);
        if ((double) num2 >= 0.0 && (double) num1 < 0.0)
          return reachCone;
      }
      return -1;
    }

    [Serializable]
    public class ReachCone
    {
      public Vector3[] tetrahedron;
      public float volume;
      public Vector3 S;
      public Vector3 B;

      public Vector3 o => this.tetrahedron[0];

      public Vector3 a => this.tetrahedron[1];

      public Vector3 b => this.tetrahedron[2];

      public Vector3 c => this.tetrahedron[3];

      public ReachCone(Vector3 _o, Vector3 _a, Vector3 _b, Vector3 _c)
      {
        this.tetrahedron = new Vector3[4];
        this.tetrahedron[0] = _o;
        this.tetrahedron[1] = _a;
        this.tetrahedron[2] = _b;
        this.tetrahedron[3] = _c;
        this.volume = 0.0f;
        this.S = Vector3.zero;
        this.B = Vector3.zero;
      }

      public bool isValid => (double) this.volume > 0.0;

      public void Calculate()
      {
        this.volume = Vector3.Dot(Vector3.Cross(this.a, this.b), this.c) / 6f;
        this.S = Vector3.Cross(this.a, this.b).normalized;
        this.B = Vector3.Cross(this.b, this.c).normalized;
      }
    }

    [Serializable]
    public class LimitPoint
    {
      public Vector3 point;
      public float tangentWeight;

      public LimitPoint()
      {
        this.point = Vector3.forward;
        this.tangentWeight = 1f;
      }
    }
  }
}
