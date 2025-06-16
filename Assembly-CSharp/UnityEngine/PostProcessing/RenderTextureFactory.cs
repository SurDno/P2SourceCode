﻿// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.RenderTextureFactory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace UnityEngine.PostProcessing
{
  public sealed class RenderTextureFactory : IDisposable
  {
    private HashSet<RenderTexture> m_TemporaryRTs;

    public RenderTextureFactory() => this.m_TemporaryRTs = new HashSet<RenderTexture>();

    public RenderTexture Get(RenderTexture baseRenderTexture)
    {
      return this.Get(baseRenderTexture.width, baseRenderTexture.height, baseRenderTexture.depth, baseRenderTexture.format, baseRenderTexture.sRGB ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear, baseRenderTexture.filterMode, baseRenderTexture.wrapMode);
    }

    public RenderTexture Get(
      int width,
      int height,
      int depthBuffer = 0,
      RenderTextureFormat format = RenderTextureFormat.ARGBHalf,
      RenderTextureReadWrite rw = RenderTextureReadWrite.Default,
      FilterMode filterMode = FilterMode.Bilinear,
      TextureWrapMode wrapMode = TextureWrapMode.Clamp,
      string name = "FactoryTempTexture")
    {
      RenderTexture temporary = RenderTexture.GetTemporary(width, height, depthBuffer, format);
      temporary.filterMode = filterMode;
      temporary.wrapMode = wrapMode;
      temporary.name = name;
      this.m_TemporaryRTs.Add(temporary);
      return temporary;
    }

    public void Release(RenderTexture rt)
    {
      if ((UnityEngine.Object) rt == (UnityEngine.Object) null)
        return;
      if (!this.m_TemporaryRTs.Contains(rt))
        throw new ArgumentException(string.Format("Attempting to remove a RenderTexture that was not allocated: {0}", (object) rt));
      this.m_TemporaryRTs.Remove(rt);
      RenderTexture.ReleaseTemporary(rt);
    }

    public void ReleaseAll()
    {
      HashSet<RenderTexture>.Enumerator enumerator = this.m_TemporaryRTs.GetEnumerator();
      while (enumerator.MoveNext())
        RenderTexture.ReleaseTemporary(enumerator.Current);
      this.m_TemporaryRTs.Clear();
    }

    public void Dispose() => this.ReleaseAll();
  }
}
