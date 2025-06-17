using System;
using UnityEngine;

namespace AmplifyBloom
{
  [Serializable]
  public class AmplifyPassCache
  {
    [SerializeField]
    internal Vector4[] Offsets = new Vector4[16];
    [SerializeField]
    internal Vector4[] Weights = new Vector4[16];

    public void Destroy()
    {
      Offsets = null;
      Weights = null;
    }
  }
}
