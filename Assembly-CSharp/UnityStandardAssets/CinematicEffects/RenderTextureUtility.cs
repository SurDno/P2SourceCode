using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects;

public class RenderTextureUtility {
	private List<RenderTexture> m_TemporaryRTs = new();

	public RenderTexture GetTemporaryRenderTexture(
		int width,
		int height,
		int depthBuffer = 0,
		RenderTextureFormat format = RenderTextureFormat.ARGBHalf,
		FilterMode filterMode = FilterMode.Bilinear) {
		var temporary = RenderTexture.GetTemporary(width, height, depthBuffer, format);
		temporary.filterMode = filterMode;
		temporary.wrapMode = TextureWrapMode.Clamp;
		temporary.name = "RenderTextureUtilityTempTexture";
		m_TemporaryRTs.Add(temporary);
		return temporary;
	}

	public void ReleaseTemporaryRenderTexture(RenderTexture rt) {
		if (rt == null)
			return;
		if (!m_TemporaryRTs.Contains(rt))
			Debug.LogErrorFormat("Attempting to remove texture that was not allocated: {0}", rt);
		else {
			m_TemporaryRTs.Remove(rt);
			RenderTexture.ReleaseTemporary(rt);
		}
	}

	public void ReleaseAllTemporaryRenderTextures() {
		for (var index = 0; index < m_TemporaryRTs.Count; ++index)
			RenderTexture.ReleaseTemporary(m_TemporaryRTs[index]);
		m_TemporaryRTs.Clear();
	}
}