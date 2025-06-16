// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.AmplifyPassCache
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
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
