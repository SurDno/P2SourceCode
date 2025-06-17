using System.Collections.Generic;
using UnityEngine;

namespace Cinemachine
{
  [DocumentationSorting(20f, DocumentationSortingAttribute.Level.UserRef)]
  [ExecuteInEditMode]
  [DisallowMultipleComponent]
  [AddComponentMenu("Cinemachine/CinemachineMixingCamera")]
  public class CinemachineMixingCamera : CinemachineVirtualCameraBase
  {
    public const int MaxCameras = 8;
    [Tooltip("The weight of the first tracked camera")]
    public float m_Weight0 = 0.5f;
    [Tooltip("The weight of the second tracked camera")]
    public float m_Weight1 = 0.5f;
    [Tooltip("The weight of the third tracked camera")]
    public float m_Weight2 = 0.5f;
    [Tooltip("The weight of the fourth tracked camera")]
    public float m_Weight3 = 0.5f;
    [Tooltip("The weight of the fifth tracked camera")]
    public float m_Weight4 = 0.5f;
    [Tooltip("The weight of the sixth tracked camera")]
    public float m_Weight5 = 0.5f;
    [Tooltip("The weight of the seventh tracked camera")]
    public float m_Weight6 = 0.5f;
    [Tooltip("The weight of the eighth tracked camera")]
    public float m_Weight7 = 0.5f;
    private CameraState m_State = CameraState.Default;
    private CinemachineVirtualCameraBase[] m_ChildCameras;
    private Dictionary<CinemachineVirtualCameraBase, int> m_indexMap;

    public float GetWeight(int index)
    {
      switch (index)
      {
        case 0:
          return m_Weight0;
        case 1:
          return m_Weight1;
        case 2:
          return m_Weight2;
        case 3:
          return m_Weight3;
        case 4:
          return m_Weight4;
        case 5:
          return m_Weight5;
        case 6:
          return m_Weight6;
        case 7:
          return m_Weight7;
        default:
          Debug.LogError("CinemachineMixingCamera: Invalid index: " + index);
          return 0.0f;
      }
    }

    public void SetWeight(int index, float w)
    {
      switch (index)
      {
        case 0:
          m_Weight0 = w;
          break;
        case 1:
          m_Weight1 = w;
          break;
        case 2:
          m_Weight2 = w;
          break;
        case 3:
          m_Weight3 = w;
          break;
        case 4:
          m_Weight4 = w;
          break;
        case 5:
          m_Weight5 = w;
          break;
        case 6:
          m_Weight6 = w;
          break;
        case 7:
          m_Weight7 = w;
          break;
        default:
          Debug.LogError("CinemachineMixingCamera: Invalid index: " + index);
          break;
      }
    }

    public float GetWeight(CinemachineVirtualCameraBase vcam)
    {
      if (m_indexMap.TryGetValue(vcam, out int index))
        return GetWeight(index);
      Debug.LogError("CinemachineMixingCamera: Invalid child: " + (vcam != null ? vcam.Name : "(null)"));
      return 0.0f;
    }

    public void SetWeight(CinemachineVirtualCameraBase vcam, float w)
    {
      if (m_indexMap.TryGetValue(vcam, out int index))
        SetWeight(index, w);
      else
        Debug.LogError("CinemachineMixingCamera: Invalid child: " + (vcam != null ? vcam.Name : "(null)"));
    }

    private ICinemachineCamera LiveChild { set; get; }

    public override CameraState State => m_State;

    public override Transform LookAt { get; set; }

    public override Transform Follow { get; set; }

    public override ICinemachineCamera LiveChildOrSelf => LiveChild;

    public override void RemovePostPipelineStageHook(
      OnPostPipelineStageDelegate d)
    {
      base.RemovePostPipelineStageHook(d);
      ValidateListOfChildren();
      foreach (CinemachineVirtualCameraBase childCamera in m_ChildCameras)
        childCamera.RemovePostPipelineStageHook(d);
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      InvalidateListOfChildren();
    }

    public void OnTransformChildrenChanged() => InvalidateListOfChildren();

    protected override void OnValidate()
    {
      base.OnValidate();
      for (int index = 0; index < 8; ++index)
        SetWeight(index, Mathf.Max(0.0f, GetWeight(index)));
    }

    public override bool IsLiveChild(ICinemachineCamera vcam)
    {
      CinemachineVirtualCameraBase[] childCameras = ChildCameras;
      for (int index = 0; index < 8 && index < childCameras.Length; ++index)
      {
        if (childCameras[index] == vcam)
          return GetWeight(index) > 9.9999997473787516E-05 && childCameras[index].isActiveAndEnabled;
      }
      return false;
    }

    public CinemachineVirtualCameraBase[] ChildCameras
    {
      get
      {
        ValidateListOfChildren();
        return m_ChildCameras;
      }
    }

    protected void InvalidateListOfChildren()
    {
      m_ChildCameras = null;
      m_indexMap = null;
      LiveChild = null;
    }

    protected void ValidateListOfChildren()
    {
      if (m_ChildCameras != null)
        return;
      m_indexMap = new Dictionary<CinemachineVirtualCameraBase, int>();
      List<CinemachineVirtualCameraBase> virtualCameraBaseList = [];
      foreach (CinemachineVirtualCameraBase componentsInChild in GetComponentsInChildren<CinemachineVirtualCameraBase>(true))
      {
        if (componentsInChild.transform.parent == transform)
        {
          int count = virtualCameraBaseList.Count;
          virtualCameraBaseList.Add(componentsInChild);
          if (count < 8)
            m_indexMap.Add(componentsInChild, count);
        }
      }
      m_ChildCameras = virtualCameraBaseList.ToArray();
    }

    public override void UpdateCameraState(Vector3 worldUp, float deltaTime)
    {
      CinemachineVirtualCameraBase[] childCameras = ChildCameras;
      LiveChild = null;
      float num1 = 0.0f;
      float num2 = 0.0f;
      for (int index = 0; index < 8 && index < childCameras.Length; ++index)
      {
        CinemachineVirtualCameraBase virtualCameraBase = childCameras[index];
        if (virtualCameraBase.isActiveAndEnabled)
        {
          float num3 = Mathf.Max(0.0f, GetWeight(index));
          if (num3 > 9.9999997473787516E-05)
          {
            num2 += num3;
            m_State = num2 != (double) num3 ? CameraState.Lerp(m_State, virtualCameraBase.State, num3 / num2) : virtualCameraBase.State;
            if (num3 > (double) num1)
            {
              num1 = num3;
              LiveChild = virtualCameraBase;
            }
          }
        }
      }
    }
  }
}
