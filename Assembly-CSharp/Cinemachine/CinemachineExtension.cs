using System.Collections.Generic;
using UnityEngine;

namespace Cinemachine
{
  [DocumentationSorting(23f, DocumentationSortingAttribute.Level.API)]
  public abstract class CinemachineExtension : MonoBehaviour
  {
    protected const float Epsilon = 0.0001f;
    private CinemachineVirtualCameraBase m_vcamOwner;
    private Dictionary<ICinemachineCamera, object> mExtraState;

    public CinemachineVirtualCameraBase VirtualCamera
    {
      get
      {
        if ((Object) this.m_vcamOwner == (Object) null)
          this.m_vcamOwner = this.GetComponent<CinemachineVirtualCameraBase>();
        return this.m_vcamOwner;
      }
    }

    protected virtual void Awake() => this.ConnectToVcam();

    protected virtual void OnDestroy()
    {
      if (!((Object) this.VirtualCamera != (Object) null))
        return;
      this.VirtualCamera.RemovePostPipelineStageHook(new CinemachineVirtualCameraBase.OnPostPipelineStageDelegate(this.PostPipelineStageCallback));
    }

    private void ConnectToVcam()
    {
      if ((Object) this.VirtualCamera == (Object) null)
        Debug.LogError((object) "CinemachineExtension requires a Cinemachine Virtual Camera component");
      else
        this.VirtualCamera.AddPostPipelineStageHook(new CinemachineVirtualCameraBase.OnPostPipelineStageDelegate(this.PostPipelineStageCallback));
      this.mExtraState = (Dictionary<ICinemachineCamera, object>) null;
    }

    protected abstract void PostPipelineStageCallback(
      CinemachineVirtualCameraBase vcam,
      CinemachineCore.Stage stage,
      ref CameraState state,
      float deltaTime);

    protected T GetExtraState<T>(ICinemachineCamera vcam) where T : class, new()
    {
      if (this.mExtraState == null)
        this.mExtraState = new Dictionary<ICinemachineCamera, object>();
      object extraState = (object) null;
      if (!this.mExtraState.TryGetValue(vcam, out extraState))
        extraState = this.mExtraState[vcam] = (object) new T();
      return extraState as T;
    }

    protected List<T> GetAllExtraStates<T>() where T : class, new()
    {
      List<T> allExtraStates = new List<T>();
      if (this.mExtraState != null)
      {
        foreach (KeyValuePair<ICinemachineCamera, object> keyValuePair in this.mExtraState)
          allExtraStates.Add(keyValuePair.Value as T);
      }
      return allExtraStates;
    }
  }
}
