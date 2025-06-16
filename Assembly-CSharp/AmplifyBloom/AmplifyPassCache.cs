using System;

namespace AmplifyBloom
{
  [Serializable]
  public class AmplifyPassCache
  {
    [SerializeField]
    internal Vector4[] Offsets;
    [SerializeField]
    internal Vector4[] Weights;

    public AmplifyPassCache()
    {
      Offsets = new Vector4[16];
      Weights = new Vector4[16];
    }

    public void Destroy()
    {
      Offsets = (Vector4[]) null;
      Weights = (Vector4[]) null;
    }
  }
}
