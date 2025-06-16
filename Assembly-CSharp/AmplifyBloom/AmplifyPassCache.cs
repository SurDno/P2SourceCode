﻿using System;
using UnityEngine;

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
      this.Offsets = new Vector4[16];
      this.Weights = new Vector4[16];
    }

    public void Destroy()
    {
      this.Offsets = (Vector4[]) null;
      this.Weights = (Vector4[]) null;
    }
  }
}
