// Decompiled with JetBrains decompiler
// Type: Cinemachine.CinemachineConfiner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cinemachine.Utility;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Cinemachine
{
  [DocumentationSorting(22f, DocumentationSortingAttribute.Level.UserRef)]
  [ExecuteInEditMode]
  [AddComponentMenu("")]
  [SaveDuringPlay]
  public class CinemachineConfiner : CinemachineExtension
  {
    [Tooltip("The confiner can operate using a 2D bounding shape or a 3D bounding volume")]
    public CinemachineConfiner.Mode m_ConfineMode;
    [Tooltip("The volume within which the camera is to be contained")]
    public Collider m_BoundingVolume;
    [Tooltip("If camera is orthographic, screen edges will be confined to the volume.  If not checked, then only the camera center will be confined")]
    public bool m_ConfineScreenEdges = true;
    [Tooltip("How gradually to return the camera to the bounding volume if it goes beyond the borders.  Higher numbers are more gradual.")]
    [Range(0.0f, 10f)]
    public float m_Damping = 0.0f;
    private List<List<Vector2>> m_pathCache;

    public bool CameraWasDisplaced(CinemachineVirtualCameraBase vcam)
    {
      return (double) this.GetExtraState<CinemachineConfiner.VcamExtraState>((ICinemachineCamera) vcam).confinerDisplacement > 0.0;
    }

    private void OnValidate() => this.m_Damping = Mathf.Max(0.0f, this.m_Damping);

    public bool IsValid
    {
      get
      {
        return this.m_ConfineMode == CinemachineConfiner.Mode.Confine3D && (Object) this.m_BoundingVolume != (Object) null;
      }
    }

    protected override void PostPipelineStageCallback(
      CinemachineVirtualCameraBase vcam,
      CinemachineCore.Stage stage,
      ref CameraState state,
      float deltaTime)
    {
      if (!this.IsValid || stage != CinemachineCore.Stage.Body)
        return;
      Vector3 vector3_1 = !this.m_ConfineScreenEdges || !state.Lens.Orthographic ? this.ConfinePoint(state.CorrectedPosition) : this.ConfineScreenEdges(vcam, ref state);
      CinemachineConfiner.VcamExtraState extraState = this.GetExtraState<CinemachineConfiner.VcamExtraState>((ICinemachineCamera) vcam);
      if ((double) this.m_Damping > 0.0 && (double) deltaTime >= 0.0)
      {
        Vector3 vector3_2 = Damper.Damp(vector3_1 - extraState.m_previousDisplacement, this.m_Damping, deltaTime);
        vector3_1 = extraState.m_previousDisplacement + vector3_2;
      }
      extraState.m_previousDisplacement = vector3_1;
      state.PositionCorrection += vector3_1;
      extraState.confinerDisplacement = vector3_1.magnitude;
    }

    public void InvalidatePathCache() => this.m_pathCache = (List<List<Vector2>>) null;

    private bool ValidatePathCache()
    {
      this.InvalidatePathCache();
      return false;
    }

    private Vector3 ConfinePoint(Vector3 camPos)
    {
      if (this.m_ConfineMode == CinemachineConfiner.Mode.Confine3D)
        return this.m_BoundingVolume.ClosestPoint(camPos) - camPos;
      if (!this.ValidatePathCache())
        return Vector3.zero;
      Vector2 vector2_1 = (Vector2) camPos;
      Vector2 vector2_2 = vector2_1;
      for (int index = 0; index < this.m_pathCache.Count; ++index)
      {
        if (this.m_pathCache[index].Count <= 0)
          ;
      }
      return (Vector3) (vector2_2 - vector2_1);
    }

    private Vector3 ConfineScreenEdges(CinemachineVirtualCameraBase vcam, ref CameraState state)
    {
      Quaternion quaternion = Quaternion.Inverse(state.CorrectedOrientation);
      float orthographicSize = state.Lens.OrthographicSize;
      float num = orthographicSize * state.Lens.Aspect;
      Vector3 vector3_1 = quaternion * Vector3.right * num;
      Vector3 vector3_2 = quaternion * Vector3.up * orthographicSize;
      Vector3 zero = Vector3.zero;
      Vector3 correctedPosition = state.CorrectedPosition;
      for (int index = 0; index < 12; ++index)
      {
        Vector3 v = this.ConfinePoint(correctedPosition - vector3_2 - vector3_1);
        if (v.AlmostZero())
          v = this.ConfinePoint(correctedPosition - vector3_2 + vector3_1);
        if (v.AlmostZero())
          v = this.ConfinePoint(correctedPosition + vector3_2 - vector3_1);
        if (v.AlmostZero())
          v = this.ConfinePoint(correctedPosition + vector3_2 + vector3_1);
        if (!v.AlmostZero())
        {
          zero += v;
          correctedPosition += v;
        }
        else
          break;
      }
      return zero;
    }

    public enum Mode
    {
      Confine2D,
      Confine3D,
    }

    private class VcamExtraState
    {
      public Vector3 m_previousDisplacement;
      public float confinerDisplacement;
    }
  }
}
