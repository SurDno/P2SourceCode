using UnityEngine;

namespace Cinemachine
{
  [DocumentationSorting(23f, DocumentationSortingAttribute.Level.UserRef)]
  [AddComponentMenu("")]
  [RequireComponent(typeof (CinemachinePipeline))]
  [SaveDuringPlay]
  public class CinemachinePOV : CinemachineComponentBase
  {
    [Tooltip("The Vertical axis.  Value is -90..90. Controls the vertical orientation")]
    public AxisState m_VerticalAxis = new AxisState(300f, 0.1f, 0.1f, 0.0f, "Mouse Y", true);
    [Tooltip("The Horizontal axis.  Value is -180..180.  Controls the horizontal orientation")]
    public AxisState m_HorizontalAxis = new AxisState(300f, 0.1f, 0.1f, 0.0f, "Mouse X", false);

    public override bool IsValid => this.enabled;

    public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Aim;

    private void OnValidate()
    {
      this.m_HorizontalAxis.Validate();
      this.m_VerticalAxis.Validate();
    }

    private void OnEnable()
    {
      this.m_HorizontalAxis.SetThresholds(-180f, 180f, true);
      this.m_VerticalAxis.SetThresholds(-90f, 90f, false);
    }

    public override void MutateCameraState(ref CameraState curState, float deltaTime)
    {
      if (!this.IsValid)
        return;
      if ((double) deltaTime >= 0.0 || CinemachineCore.Instance.IsLive((ICinemachineCamera) this.VirtualCamera))
      {
        this.m_HorizontalAxis.Update(deltaTime);
        this.m_VerticalAxis.Update(deltaTime);
      }
      Quaternion quaternion = Quaternion.Euler(this.m_VerticalAxis.Value, this.m_HorizontalAxis.Value, 0.0f) * Quaternion.FromToRotation(Vector3.up, curState.ReferenceUp);
      curState.OrientationCorrection *= quaternion;
    }
  }
}
