// Decompiled with JetBrains decompiler
// Type: Cinemachine.CinemachineComponentBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
        if ((Object) this.m_vcamOwner == (Object) null)
          this.m_vcamOwner = this.gameObject.transform.parent.gameObject.GetComponent<CinemachineVirtualCameraBase>();
        return this.m_vcamOwner;
      }
    }

    public Transform FollowTarget
    {
      get
      {
        CinemachineVirtualCameraBase virtualCamera = this.VirtualCamera;
        return (Object) virtualCamera == (Object) null ? (Transform) null : virtualCamera.Follow;
      }
    }

    public Transform LookAtTarget
    {
      get
      {
        CinemachineVirtualCameraBase virtualCamera = this.VirtualCamera;
        return (Object) virtualCamera == (Object) null ? (Transform) null : virtualCamera.LookAt;
      }
    }

    public CameraState VcamState
    {
      get
      {
        CinemachineVirtualCameraBase virtualCamera = this.VirtualCamera;
        return (Object) virtualCamera == (Object) null ? CameraState.Default : virtualCamera.State;
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
