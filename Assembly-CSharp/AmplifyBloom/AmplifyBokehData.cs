using System;
using UnityEngine;

namespace AmplifyBloom
{
  [Serializable]
  public class AmplifyBokehData
  {
    internal RenderTexture BokehRenderTexture;
    internal Vector4[] Offsets;

    public AmplifyBokehData(Vector4[] offsets) => this.Offsets = offsets;

    public void Destroy()
    {
      if ((UnityEngine.Object) this.BokehRenderTexture != (UnityEngine.Object) null)
      {
        AmplifyUtils.ReleaseTempRenderTarget(this.BokehRenderTexture);
        this.BokehRenderTexture = (RenderTexture) null;
      }
      this.Offsets = (Vector4[]) null;
    }
  }
}
