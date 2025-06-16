using UnityEngine;

namespace Cinemachine
{
  [DocumentationSorting(24f, DocumentationSortingAttribute.Level.API)]
  public abstract class CinemachineComponentBase : MonoBehaviour
  {
    protected const float Epsilon = 0.0001f;
    private CinemachineVirtualCameraBase m_vcamOwner;

    public CinemachineVirtualCameraBase VirtualCamera
    {
      get
      {
        if (m_vcamOwner == null)
          m_vcamOwner = gameObject.transform.parent.gameObject.GetComponent<CinemachineVirtualCameraBase>();
        return m_vcamOwner;
      }
    }

    public Transform FollowTarget
    {
      get
      {
        CinemachineVirtualCameraBase virtualCamera = VirtualCamera;
        return virtualCamera == null ? null : virtualCamera.Follow;
      }
    }

    public Transform LookAtTarget
    {
      get
      {
        CinemachineVirtualCameraBase virtualCamera = VirtualCamera;
        return virtualCamera == null ? null : virtualCamera.LookAt;
      }
    }

    public CameraState VcamState
    {
      get
      {
        CinemachineVirtualCameraBase virtualCamera = VirtualCamera;
        return virtualCamera == null ? CameraState.Default : virtualCamera.State;
      }
    }

    public abstract bool IsValid { get; }

    public virtual void PrePipelineMutateCameraState(ref CameraState state)
    {
    }

    public abstract CinemachineCore.Stage Stage { get; }

    public abstract void MutateCameraState(ref CameraState curState, float deltaTime);

    public virtual void OnPositionDragged(Vector3 delta)
    {
    }
  }
}
