using System.Collections.Generic;
using Cinemachine.Utility;

namespace Cinemachine
{
  [DocumentationSorting(22f, DocumentationSortingAttribute.Level.UserRef)]
  [ExecuteInEditMode]
  [AddComponentMenu("")]
  [SaveDuringPlay]
  public class CinemachineConfiner : CinemachineExtension
  {
    [Tooltip("The confiner can operate using a 2D bounding shape or a 3D bounding volume")]
    public Mode m_ConfineMode;
    [Tooltip("The volume within which the camera is to be contained")]
    public Collider m_BoundingVolume;
    [Tooltip("If camera is orthographic, screen edges will be confined to the volume.  If not checked, then only the camera center will be confined")]
    public bool m_ConfineScreenEdges = true;
    [Tooltip("How gradually to return the camera to the bounding volume if it goes beyond the borders.  Higher numbers are more gradual.")]
    [Range(0.0f, 10f)]
    public float m_Damping;
    private List<List<Vector2>> m_pathCache;

    public bool CameraWasDisplaced(CinemachineVirtualCameraBase vcam)
    {
      return GetExtraState<VcamExtraState>(vcam).confinerDisplacement > 0.0;
    }

    private void OnValidate() => m_Damping = Mathf.Max(0.0f, m_Damping);

    public bool IsValid
    {
      get
      {
        return m_ConfineMode == Mode.Confine3D && (Object) m_BoundingVolume != (Object) null;
      }
    }

    protected override void PostPipelineStageCallback(
      CinemachineVirtualCameraBase vcam,
      CinemachineCore.Stage stage,
      ref CameraState state,
      float deltaTime)
    {
      if (!IsValid || stage != CinemachineCore.Stage.Body)
        return;
      Vector3 vector3_1 = !m_ConfineScreenEdges || !state.Lens.Orthographic ? ConfinePoint(state.CorrectedPosition) : ConfineScreenEdges(vcam, ref state);
      VcamExtraState extraState = GetExtraState<VcamExtraState>(vcam);
      if (m_Damping > 0.0 && deltaTime >= 0.0)
      {
        Vector3 vector3_2 = Damper.Damp(vector3_1 - extraState.m_previousDisplacement, m_Damping, deltaTime);
        vector3_1 = extraState.m_previousDisplacement + vector3_2;
      }
      extraState.m_previousDisplacement = vector3_1;
      state.PositionCorrection += vector3_1;
      extraState.confinerDisplacement = vector3_1.magnitude;
    }

    public void InvalidatePathCache() => m_pathCache = (List<List<Vector2>>) null;

    private bool ValidatePathCache()
    {
      InvalidatePathCache();
      return false;
    }

    private Vector3 ConfinePoint(Vector3 camPos)
    {
      if (m_ConfineMode == Mode.Confine3D)
        return m_BoundingVolume.ClosestPoint(camPos) - camPos;
      if (!ValidatePathCache())
        return Vector3.zero;
      Vector2 vector2_1 = (Vector2) camPos;
      Vector2 vector2_2 = vector2_1;
      for (int index = 0; index < m_pathCache.Count; ++index)
      {
        if (m_pathCache[index].Count <= 0)
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
        Vector3 v = ConfinePoint(correctedPosition - vector3_2 - vector3_1);
        if (v.AlmostZero())
          v = ConfinePoint(correctedPosition - vector3_2 + vector3_1);
        if (v.AlmostZero())
          v = ConfinePoint(correctedPosition + vector3_2 - vector3_1);
        if (v.AlmostZero())
          v = ConfinePoint(correctedPosition + vector3_2 + vector3_1);
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
