// Decompiled with JetBrains decompiler
// Type: LookAtCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class LookAtCamera : MonoBehaviour
{
  public Camera lookAtCamera;
  public bool lookOnlyOnAwake;

  public void Start()
  {
    if ((Object) this.lookAtCamera == (Object) null)
      this.lookAtCamera = Camera.main;
    if (!this.lookOnlyOnAwake)
      return;
    this.LookCam();
  }

  public void Update()
  {
    if (this.lookOnlyOnAwake)
      return;
    this.LookCam();
  }

  public void LookCam() => this.transform.LookAt(this.lookAtCamera.transform);
}
