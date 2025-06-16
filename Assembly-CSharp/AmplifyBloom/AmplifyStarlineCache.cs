// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.AmplifyStarlineCache
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace AmplifyBloom
{
  [Serializable]
  public class AmplifyStarlineCache
  {
    [SerializeField]
    internal AmplifyPassCache[] Passes;

    public AmplifyStarlineCache()
    {
      this.Passes = new AmplifyPassCache[4];
      for (int index = 0; index < 4; ++index)
        this.Passes[index] = new AmplifyPassCache();
    }

    public void Destroy()
    {
      for (int index = 0; index < 4; ++index)
        this.Passes[index].Destroy();
      this.Passes = (AmplifyPassCache[]) null;
    }
  }
}
