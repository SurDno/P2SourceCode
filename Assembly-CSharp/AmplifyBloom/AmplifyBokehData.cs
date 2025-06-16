using System;

namespace AmplifyBloom
{
  [Serializable]
  public class AmplifyBokehData
  {
    internal RenderTexture BokehRenderTexture;
    internal Vector4[] Offsets;

    public AmplifyBokehData(Vector4[] offsets) => Offsets = offsets;

    public void Destroy()
    {
      if ((UnityEngine.Object) BokehRenderTexture != (UnityEngine.Object) null)
      {
        AmplifyUtils.ReleaseTempRenderTarget(BokehRenderTexture);
        BokehRenderTexture = (RenderTexture) null;
      }
      Offsets = (Vector4[]) null;
    }
  }
}
