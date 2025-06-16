// Decompiled with JetBrains decompiler
// Type: Cinemachine.CinemachineMixingCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
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
          return this.m_Weight0;
        case 1:
          return this.m_Weight1;
        case 2:
          return this.m_Weight2;
        case 3:
          return this.m_Weight3;
        case 4:
          return this.m_Weight4;
        case 5:
          return this.m_Weight5;
        case 6:
          return this.m_Weight6;
        case 7:
          return this.m_Weight7;
        default:
          Debug.LogError((object) ("CinemachineMixingCamera: Invalid index: " + (object) index));
          return 0.0f;
      }
    }

    public void SetWeight(int index, float w)
    {
      switch (index)
      {
        case 0:
          this.m_Weight0 = w;
          break;
        case 1:
          this.m_Weight1 = w;
          break;
        case 2:
          this.m_Weight2 = w;
          break;
        case 3:
          this.m_Weight3 = w;
          break;
        case 4:
          this.m_Weight4 = w;
          break;
        case 5:
          this.m_Weight5 = w;
          break;
        case 6:
          this.m_Weight6 = w;
          break;
        case 7:
          this.m_Weight7 = w;
          break;
        default:
          Debug.LogError((object) ("CinemachineMixingCamera: Invalid index: " + (object) index));
          break;
      }
    }

    public float GetWeight(CinemachineVirtualCameraBase vcam)
    {
      int index;
      if (this.m_indexMap.TryGetValue(vcam, out index))
        return this.GetWeight(index);
      Debug.LogError((object) ("CinemachineMixingCamera: Invalid child: " + ((Object) vcam != (Object) null ? vcam.Name : "(null)")));
      return 0.0f;
    }

    public void SetWeight(CinemachineVirtualCameraBase vcam, float w)
    {
      int index;
      if (this.m_indexMap.TryGetValue(vcam, out index))
        this.SetWeight(index, w);
      else
        Debug.LogError((object) ("CinemachineMixingCamera: Invalid child: " + ((Object) vcam != (Object) null ? vcam.Name : "(null)")));
    }

    private ICinemachineCamera LiveChild { set; get; }

    public override CameraState State => this.m_State;

    public override Transform LookAt { get; set; }

    public override Transform Follow { get; set; }

    public override ICinemachineCamera LiveChildOrSelf => this.LiveChild;

    public override void RemovePostPipelineStageHook(
      CinemachineVirtualCameraBase.OnPostPipelineStageDelegate d)
    {
      base.RemovePostPipelineStageHook(d);
      this.ValidateListOfChildren();
      foreach (CinemachineVirtualCameraBase childCamera in this.m_ChildCameras)
        childCamera.RemovePostPipelineStageHook(d);
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.InvalidateListOfChildren();
    }

    public void OnTransformChildrenChanged() => this.InvalidateListOfChildren();

    protected override void OnValidate()
    {
      base.OnValidate();
      for (int index = 0; index < 8; ++index)
        this.SetWeight(index, Mathf.Max(0.0f, this.GetWeight(index)));
    }

    public override bool IsLiveChild(ICinemachineCamera vcam)
    {
      CinemachineVirtualCameraBase[] childCameras = this.ChildCameras;
      for (int index = 0; index < 8 && index < childCameras.Length; ++index)
      {
        if (childCameras[index] == vcam)
          return (double) this.GetWeight(index) > 9.9999997473787516E-05 && childCameras[index].isActiveAndEnabled;
      }
      return false;
    }

    public CinemachineVirtualCameraBase[] ChildCameras
    {
      get
      {
        this.ValidateListOfChildren();
        return this.m_ChildCameras;
      }
    }

    protected void InvalidateListOfChildren()
    {
      this.m_ChildCameras = (CinemachineVirtualCameraBase[]) null;
      this.m_indexMap = (Dictionary<CinemachineVirtualCameraBase, int>) null;
      this.LiveChild = (ICinemachineCamera) null;
    }

    protected void ValidateListOfChildren()
    {
      if (this.m_ChildCameras != null)
        return;
      this.m_indexMap = new Dictionary<CinemachineVirtualCameraBase, int>();
      List<CinemachineVirtualCameraBase> virtualCameraBaseList = new List<CinemachineVirtualCameraBase>();
      foreach (CinemachineVirtualCameraBase componentsInChild in this.GetComponentsInChildren<CinemachineVirtualCameraBase>(true))
      {
        if ((Object) componentsInChild.transform.parent == (Object) this.transform)
        {
          int count = virtualCameraBaseList.Count;
          virtualCameraBaseList.Add(componentsInChild);
          if (count < 8)
            this.m_indexMap.Add(componentsInChild, count);
        }
      }
      this.m_ChildCameras = virtualCameraBaseList.ToArray();
    }

    public override void UpdateCameraState(Vector3 worldUp, float deltaTime)
    {
      CinemachineVirtualCameraBase[] childCameras = this.ChildCameras;
      this.LiveChild = (ICinemachineCamera) null;
      float num1 = 0.0f;
      float num2 = 0.0f;
      for (int index = 0; index < 8 && index < childCameras.Length; ++index)
      {
        CinemachineVirtualCameraBase virtualCameraBase = childCameras[index];
        if (virtualCameraBase.isActiveAndEnabled)
        {
          float num3 = Mathf.Max(0.0f, this.GetWeight(index));
          if ((double) num3 > 9.9999997473787516E-05)
          {
            num2 += num3;
            this.m_State = (double) num2 != (double) num3 ? CameraState.Lerp(this.m_State, virtualCameraBase.State, num3 / num2) : virtualCameraBase.State;
            if ((double) num3 > (double) num1)
            {
              num1 = num3;
              this.LiveChild = (ICinemachineCamera) virtualCameraBase;
            }
          }
        }
      }
    }
  }
}
