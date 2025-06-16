using System;
using Cinemachine.Utility;

namespace Cinemachine
{
  [DocumentationSorting(16f, DocumentationSortingAttribute.Level.UserRef)]
  [ExecuteInEditMode]
  [AddComponentMenu("")]
  [SaveDuringPlay]
  public class CinemachineFollowZoom : CinemachineExtension
  {
    [Tooltip("The shot width to maintain, in world units, at target distance.")]
    public float m_Width = 2f;
    [Range(0.0f, 20f)]
    [Tooltip("Increase this value to soften the aggressiveness of the follow-zoom.  Small numbers are more responsive, larger numbers give a more heavy slowly responding camera.")]
    public float m_Damping = 1f;
    [Range(1f, 179f)]
    [Tooltip("Lower limit for the FOV that this behaviour will generate.")]
    public float m_MinFOV = 3f;
    [Range(1f, 179f)]
    [Tooltip("Upper limit for the FOV that this behaviour will generate.")]
    public float m_MaxFOV = 60f;

    private void OnValidate()
    {
      m_Width = Mathf.Max(0.0f, m_Width);
      m_MaxFOV = Mathf.Clamp(m_MaxFOV, 1f, 179f);
      m_MinFOV = Mathf.Clamp(m_MinFOV, 1f, m_MaxFOV);
    }

    protected override void PostPipelineStageCallback(
      CinemachineVirtualCameraBase vcam,
      CinemachineCore.Stage stage,
      ref CameraState state,
      float deltaTime)
    {
      VcamExtraState extraState = GetExtraState<VcamExtraState>(vcam);
      if (!this.enabled || deltaTime < 0.0)
        extraState.m_previousFrameZoom = state.Lens.FieldOfView;
      if (stage != CinemachineCore.Stage.Body)
        return;
      float num1 = Mathf.Max(m_Width, 0.0f);
      float num2 = 179f;
      float num3 = Vector3.Distance(state.CorrectedPosition, state.ReferenceLookAt);
      if (num3 > 9.9999997473787516E-05)
      {
        float min = num3 * 2f * Mathf.Tan((float) (m_MinFOV * (Math.PI / 180.0) / 2.0));
        float max = num3 * 2f * Mathf.Tan((float) (m_MaxFOV * (Math.PI / 180.0) / 2.0));
        float num4 = Mathf.Clamp(num1, min, max);
        if (deltaTime >= 0.0 && m_Damping > 0.0)
        {
          float num5 = num3 * 2f * Mathf.Tan((float) (extraState.m_previousFrameZoom * (Math.PI / 180.0) / 2.0));
          float num6 = Damper.Damp(num4 - num5, m_Damping, deltaTime);
          num4 = num5 + num6;
        }
        num2 = (float) (2.0 * (double) Mathf.Atan(num4 / (2f * num3)) * 57.295780181884766);
      }
      LensSettings lens = state.Lens with
      {
        FieldOfView = extraState.m_previousFrameZoom = Mathf.Clamp(num2, m_MinFOV, m_MaxFOV)
      };
      state.Lens = lens;
    }

    private class VcamExtraState
    {
      public float m_previousFrameZoom;
    }
  }
}
