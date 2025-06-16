// Decompiled with JetBrains decompiler
// Type: Cinemachine.CinemachineExternalCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cinemachine.Utility;
using UnityEngine;

#nullable disable
namespace Cinemachine
{
  [DocumentationSorting(14f, DocumentationSortingAttribute.Level.UserRef)]
  [RequireComponent(typeof (Camera))]
  [DisallowMultipleComponent]
  [ExecuteInEditMode]
  [AddComponentMenu("Cinemachine/CinemachineExternalCamera")]
  public class CinemachineExternalCamera : CinemachineVirtualCameraBase
  {
    [Tooltip("The object that the camera is looking at.  Setting this will improve the quality of the blends to and from this camera")]
    [NoSaveDuringPlay]
    public Transform m_LookAt = (Transform) null;
    private Camera m_Camera;
    private CameraState m_State = CameraState.Default;

    public override CameraState State => this.m_State;

    public override Transform LookAt
    {
      get => this.m_LookAt;
      set => this.m_LookAt = value;
    }

    public override Transform Follow { get; set; }

    public override void UpdateCameraState(Vector3 worldUp, float deltaTime)
    {
      if ((Object) this.m_Camera == (Object) null)
        this.m_Camera = this.GetComponent<Camera>();
      this.m_State = CameraState.Default;
      this.m_State.RawPosition = this.transform.position;
      this.m_State.RawOrientation = this.transform.rotation;
      this.m_State.ReferenceUp = this.m_State.RawOrientation * Vector3.up;
      if ((Object) this.m_Camera != (Object) null)
        this.m_State.Lens = LensSettings.FromCamera(this.m_Camera);
      if (!((Object) this.m_LookAt != (Object) null))
        return;
      this.m_State.ReferenceLookAt = this.m_LookAt.transform.position;
      Vector3 vector3 = this.m_State.ReferenceLookAt - this.State.RawPosition;
      if (!vector3.AlmostZero())
        this.m_State.ReferenceLookAt = this.m_State.RawPosition + Vector3.Project(vector3, this.State.RawOrientation * Vector3.forward);
    }
  }
}
