// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.CinematicEffects.IAntiAliasing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace UnityStandardAssets.CinematicEffects
{
  public interface IAntiAliasing
  {
    void OnEnable(AntiAliasing owner);

    void OnDisable();

    void OnPreCull(Camera camera);

    void OnPostRender(Camera camera);

    void OnRenderImage(Camera camera, RenderTexture source, RenderTexture destination);
  }
}
