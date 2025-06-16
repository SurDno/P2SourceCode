// Decompiled with JetBrains decompiler
// Type: OcclusionCullingSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Controls;
using UnityEngine;

#nullable disable
public class OcclusionCullingSettings : HideableView
{
  [SerializeField]
  private Camera targetCamera;

  protected override void ApplyVisibility()
  {
    if (!((Object) this.targetCamera != (Object) null))
      return;
    this.targetCamera.useOcclusionCulling = this.Visible;
  }
}
