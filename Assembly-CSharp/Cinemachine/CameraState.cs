// Decompiled with JetBrains decompiler
// Type: Cinemachine.CameraState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cinemachine.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Cinemachine
{
  public struct CameraState
  {
    public static Vector3 kNoPoint = new Vector3(float.NaN, float.NaN, float.NaN);
    private CameraState.CustomBlendable mCustom0;
    private CameraState.CustomBlendable mCustom1;
    private CameraState.CustomBlendable mCustom2;
    private CameraState.CustomBlendable mCustom3;
    private List<CameraState.CustomBlendable> m_CustomOverflow;

    public LensSettings Lens { get; set; }

    public Vector3 ReferenceUp { get; set; }

    public Vector3 ReferenceLookAt { get; set; }

    public bool HasLookAt => this.ReferenceLookAt == this.ReferenceLookAt;

    public Vector3 RawPosition { get; set; }

    public Quaternion RawOrientation { get; set; }

    internal Vector3 PositionDampingBypass { get; set; }

    public float ShotQuality { get; set; }

    public Vector3 PositionCorrection { get; set; }

    public Quaternion OrientationCorrection { get; set; }

    public Vector3 CorrectedPosition => this.RawPosition + this.PositionCorrection;

    public Quaternion CorrectedOrientation => this.RawOrientation * this.OrientationCorrection;

    public Vector3 FinalPosition => this.RawPosition + this.PositionCorrection;

    public Quaternion FinalOrientation
    {
      get
      {
        return (double) Mathf.Abs(this.Lens.Dutch) > 9.9999997473787516E-05 ? this.CorrectedOrientation * Quaternion.AngleAxis(this.Lens.Dutch, Vector3.forward) : this.CorrectedOrientation;
      }
    }

    public static CameraState Default
    {
      get
      {
        return new CameraState()
        {
          Lens = LensSettings.Default,
          ReferenceUp = Vector3.up,
          ReferenceLookAt = CameraState.kNoPoint,
          RawPosition = Vector3.zero,
          RawOrientation = Quaternion.identity,
          ShotQuality = 1f,
          PositionCorrection = Vector3.zero,
          OrientationCorrection = Quaternion.identity,
          PositionDampingBypass = Vector3.zero
        };
      }
    }

    public int NumCustomBlendables { get; private set; }

    public CameraState.CustomBlendable GetCustomBlendable(int index)
    {
      switch (index)
      {
        case 0:
          return this.mCustom0;
        case 1:
          return this.mCustom1;
        case 2:
          return this.mCustom2;
        case 3:
          return this.mCustom3;
        default:
          index -= 4;
          return this.m_CustomOverflow != null && index < this.m_CustomOverflow.Count ? this.m_CustomOverflow[index] : new CameraState.CustomBlendable((UnityEngine.Object) null, 0.0f);
      }
    }

    private int FindCustomBlendable(UnityEngine.Object custom)
    {
      if (this.mCustom0.m_Custom == custom)
        return 0;
      if (this.mCustom1.m_Custom == custom)
        return 1;
      if (this.mCustom2.m_Custom == custom)
        return 2;
      if (this.mCustom3.m_Custom == custom)
        return 3;
      if (this.m_CustomOverflow != null)
      {
        for (int index = 0; index < this.m_CustomOverflow.Count; ++index)
        {
          if (this.m_CustomOverflow[index].m_Custom == custom)
            return index + 4;
        }
      }
      return -1;
    }

    public void AddCustomBlendable(CameraState.CustomBlendable b)
    {
      int index = this.FindCustomBlendable(b.m_Custom);
      if (index >= 0)
        b.m_Weight += this.GetCustomBlendable(index).m_Weight;
      else
        index = this.NumCustomBlendables++;
      switch (index)
      {
        case 0:
          this.mCustom0 = b;
          break;
        case 1:
          this.mCustom1 = b;
          break;
        case 2:
          this.mCustom2 = b;
          break;
        case 3:
          this.mCustom3 = b;
          break;
        default:
          if (this.m_CustomOverflow == null)
            this.m_CustomOverflow = new List<CameraState.CustomBlendable>();
          this.m_CustomOverflow.Add(b);
          break;
      }
    }

    public static CameraState Lerp(CameraState stateA, CameraState stateB, float t)
    {
      t = Mathf.Clamp01(t);
      float t1 = t;
      CameraState cameraState = new CameraState();
      cameraState.Lens = LensSettings.Lerp(stateA.Lens, stateB.Lens, t);
      cameraState.ReferenceUp = Vector3.Slerp(stateA.ReferenceUp, stateB.ReferenceUp, t);
      cameraState.RawPosition = Vector3.Lerp(stateA.RawPosition, stateB.RawPosition, t);
      cameraState.ShotQuality = Mathf.Lerp(stateA.ShotQuality, stateB.ShotQuality, t);
      cameraState.PositionCorrection = Vector3.Lerp(stateA.PositionCorrection, stateB.PositionCorrection, t);
      cameraState.OrientationCorrection = Quaternion.Slerp(stateA.OrientationCorrection, stateB.OrientationCorrection, t);
      Vector3 vector3 = Vector3.zero;
      if (!stateA.HasLookAt || !stateB.HasLookAt)
      {
        cameraState.ReferenceLookAt = CameraState.kNoPoint;
      }
      else
      {
        float fieldOfView1 = stateA.Lens.FieldOfView;
        float fieldOfView2 = stateB.Lens.FieldOfView;
        if (!cameraState.Lens.Orthographic && !Mathf.Approximately(fieldOfView1, fieldOfView2))
        {
          LensSettings lens = cameraState.Lens with
          {
            FieldOfView = cameraState.InterpolateFOV(fieldOfView1, fieldOfView2, Mathf.Max((stateA.ReferenceLookAt - stateA.CorrectedPosition).magnitude, stateA.Lens.NearClipPlane), Mathf.Max((stateB.ReferenceLookAt - stateB.CorrectedPosition).magnitude, stateB.Lens.NearClipPlane), t)
          };
          cameraState.Lens = lens;
          t1 = Mathf.Abs((float) (((double) lens.FieldOfView - (double) fieldOfView1) / ((double) fieldOfView2 - (double) fieldOfView1)));
        }
        cameraState.ReferenceLookAt = Vector3.Lerp(stateA.ReferenceLookAt, stateB.ReferenceLookAt, t1);
        if ((double) Quaternion.Angle(stateA.RawOrientation, stateB.RawOrientation) > 9.9999997473787516E-05)
          vector3 = cameraState.ReferenceLookAt - cameraState.CorrectedPosition;
      }
      if (vector3.AlmostZero())
      {
        cameraState.RawOrientation = UnityQuaternionExtensions.SlerpWithReferenceUp(stateA.RawOrientation, stateB.RawOrientation, t, cameraState.ReferenceUp);
      }
      else
      {
        vector3 = vector3.normalized;
        if ((vector3 - cameraState.ReferenceUp).AlmostZero() || (vector3 + cameraState.ReferenceUp).AlmostZero())
        {
          cameraState.RawOrientation = UnityQuaternionExtensions.SlerpWithReferenceUp(stateA.RawOrientation, stateB.RawOrientation, t, cameraState.ReferenceUp);
        }
        else
        {
          cameraState.RawOrientation = Quaternion.LookRotation(vector3, cameraState.ReferenceUp);
          Vector2 a = -stateA.RawOrientation.GetCameraRotationToTarget(stateA.ReferenceLookAt - stateA.CorrectedPosition, stateA.ReferenceUp);
          Vector2 b = -stateB.RawOrientation.GetCameraRotationToTarget(stateB.ReferenceLookAt - stateB.CorrectedPosition, stateB.ReferenceUp);
          cameraState.RawOrientation = cameraState.RawOrientation.ApplyCameraRotation(Vector2.Lerp(a, b, t1), cameraState.ReferenceUp);
        }
      }
      for (int index = 0; index < stateA.NumCustomBlendables; ++index)
      {
        CameraState.CustomBlendable customBlendable = stateA.GetCustomBlendable(index);
        customBlendable.m_Weight *= 1f - t;
        if ((double) customBlendable.m_Weight > 9.9999997473787516E-05)
          cameraState.AddCustomBlendable(customBlendable);
      }
      for (int index = 0; index < stateB.NumCustomBlendables; ++index)
      {
        CameraState.CustomBlendable customBlendable = stateB.GetCustomBlendable(index);
        customBlendable.m_Weight *= t;
        if ((double) customBlendable.m_Weight > 9.9999997473787516E-05)
          cameraState.AddCustomBlendable(customBlendable);
      }
      return cameraState;
    }

    private float InterpolateFOV(float fovA, float fovB, float dA, float dB, float t)
    {
      float num1 = Mathf.Lerp(dA * 2f * Mathf.Tan((float) ((double) fovA * (Math.PI / 180.0) / 2.0)), dB * 2f * Mathf.Tan((float) ((double) fovB * (Math.PI / 180.0) / 2.0)), t);
      float num2 = 179f;
      float num3 = Mathf.Lerp(dA, dB, t);
      if ((double) num3 > 9.9999997473787516E-05)
        num2 = (float) (2.0 * (double) Mathf.Atan(num1 / (2f * num3)) * 57.295780181884766);
      return Mathf.Clamp(num2, Mathf.Min(fovA, fovB), Mathf.Max(fovA, fovB));
    }

    public struct CustomBlendable
    {
      public UnityEngine.Object m_Custom;
      public float m_Weight;

      public CustomBlendable(UnityEngine.Object custom, float weight)
      {
        this.m_Custom = custom;
        this.m_Weight = weight;
      }
    }
  }
}
