// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.CinematicEffects.RenderTextureUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace UnityStandardAssets.CinematicEffects
{
  public class RenderTextureUtility
  {
    private List<RenderTexture> m_TemporaryRTs = new List<RenderTexture>();

    public RenderTexture GetTemporaryRenderTexture(
      int width,
      int height,
      int depthBuffer = 0,
      RenderTextureFormat format = RenderTextureFormat.ARGBHalf,
      FilterMode filterMode = FilterMode.Bilinear)
    {
      RenderTexture temporary = RenderTexture.GetTemporary(width, height, depthBuffer, format);
      temporary.filterMode = filterMode;
      temporary.wrapMode = TextureWrapMode.Clamp;
      temporary.name = "RenderTextureUtilityTempTexture";
      this.m_TemporaryRTs.Add(temporary);
      return temporary;
    }

    public void ReleaseTemporaryRenderTexture(RenderTexture rt)
    {
      if ((Object) rt == (Object) null)
        return;
      if (!this.m_TemporaryRTs.Contains(rt))
      {
        Debug.LogErrorFormat("Attempting to remove texture that was not allocated: {0}", (object) rt);
      }
      else
      {
        this.m_TemporaryRTs.Remove(rt);
        RenderTexture.ReleaseTemporary(rt);
      }
    }

    public void ReleaseAllTemporaryRenderTextures()
    {
      for (int index = 0; index < this.m_TemporaryRTs.Count; ++index)
        RenderTexture.ReleaseTemporary(this.m_TemporaryRTs[index]);
      this.m_TemporaryRTs.Clear();
    }
  }
}
