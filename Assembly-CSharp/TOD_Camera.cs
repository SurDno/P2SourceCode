// Decompiled with JetBrains decompiler
// Type: TOD_Camera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteInEditMode]
[RequireComponent(typeof (Camera))]
[AddComponentMenu("Time of Day/Camera Main Script")]
public class TOD_Camera : MonoBehaviour
{
  public TOD_Sky sky;
  public bool DomePosToCamera = true;
  public Vector3 DomePosOffset = Vector3.zero;
  public bool DomeScaleToFarClip = true;
  public float DomeScaleFactor = 0.95f;
  private Camera cameraComponent = (Camera) null;
  private Transform cameraTransform = (Transform) null;

  public bool HDR => (bool) (Object) this.cameraComponent && this.cameraComponent.allowHDR;

  protected void OnValidate()
  {
    this.DomeScaleFactor = Mathf.Clamp(this.DomeScaleFactor, 0.01f, 1f);
  }

  protected void OnEnable()
  {
    this.cameraComponent = this.GetComponent<Camera>();
    this.cameraTransform = this.GetComponent<Transform>();
  }

  protected void Update()
  {
    if (!(bool) (Object) this.sky)
      this.sky = TOD_Sky.Instance;
    if (!(bool) (Object) this.sky || !this.sky.Initialized)
      return;
    this.sky.Components.Camera = this;
    if (this.cameraComponent.clearFlags != CameraClearFlags.Color)
      this.cameraComponent.clearFlags = CameraClearFlags.Color;
    if (this.cameraComponent.backgroundColor != Color.clear)
      this.cameraComponent.backgroundColor = Color.clear;
    RenderSettings.skybox = this.sky.Resources.Skybox;
  }

  protected void OnPreCull()
  {
    if (!(bool) (Object) this.sky || !this.sky.Initialized)
      return;
    if (this.DomeScaleToFarClip)
      this.DoDomeScaleToFarClip();
    if (!this.DomePosToCamera)
      return;
    this.DoDomePosToCamera();
  }

  public void DoDomeScaleToFarClip()
  {
    float num = this.DomeScaleFactor * this.cameraComponent.farClipPlane;
    this.sky.Components.DomeTransform.localScale = new Vector3(num, num, num);
  }

  public void DoDomePosToCamera()
  {
    this.sky.Components.DomeTransform.position = this.cameraTransform.position + this.cameraTransform.rotation * this.DomePosOffset;
  }
}
