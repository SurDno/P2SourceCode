// Decompiled with JetBrains decompiler
// Type: CameraFarClippingPlaneSwitch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Controls;
using UnityEngine;

#nullable disable
public class CameraFarClippingPlaneSwitch : HideableView
{
  [SerializeField]
  private Camera targetCamera;
  [SerializeField]
  private float offDistance;
  [SerializeField]
  private float onDistance;

  protected override void ApplyVisibility()
  {
    if (!((Object) this.targetCamera != (Object) null))
      return;
    this.targetCamera.farClipPlane = this.Visible ? this.onDistance : this.offDistance;
  }
}
