using System.Collections.Generic;

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
      m_TemporaryRTs.Add(temporary);
      return temporary;
    }

    public void ReleaseTemporaryRenderTexture(RenderTexture rt)
    {
      if ((Object) rt == (Object) null)
        return;
      if (!m_TemporaryRTs.Contains(rt))
      {
        Debug.LogErrorFormat("Attempting to remove texture that was not allocated: {0}", (object) rt);
      }
      else
      {
        m_TemporaryRTs.Remove(rt);
        RenderTexture.ReleaseTemporary(rt);
      }
    }

    public void ReleaseAllTemporaryRenderTextures()
    {
      for (int index = 0; index < m_TemporaryRTs.Count; ++index)
        RenderTexture.ReleaseTemporary(m_TemporaryRTs[index]);
      m_TemporaryRTs.Clear();
    }
  }
}
