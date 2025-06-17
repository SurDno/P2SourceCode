using System;
using UnityEngine;

namespace AmplifyBloom
{
  [Serializable]
  public class AmplifyBokehData(Vector4[] offsets) {
    internal RenderTexture BokehRenderTexture;
    internal Vector4[] Offsets = offsets;

    public void Destroy()
    {
      if (BokehRenderTexture != null)
      {
        AmplifyUtils.ReleaseTempRenderTarget(BokehRenderTexture);
        BokehRenderTexture = null;
      }
      Offsets = null;
    }
  }
}
