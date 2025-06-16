// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.AmplifyGlareCache
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace AmplifyBloom
{
  [Serializable]
  public class AmplifyGlareCache
  {
    [SerializeField]
    internal AmplifyStarlineCache[] Starlines;
    [SerializeField]
    internal Vector4 AverageWeight;
    [SerializeField]
    internal Vector4[,] CromaticAberrationMat;
    [SerializeField]
    internal int TotalRT;
    [SerializeField]
    internal GlareDefData GlareDef;
    [SerializeField]
    internal StarDefData StarDef;
    [SerializeField]
    internal int CurrentPassCount;

    public AmplifyGlareCache()
    {
      this.Starlines = new AmplifyStarlineCache[4];
      this.CromaticAberrationMat = new Vector4[4, 8];
      for (int index = 0; index < 4; ++index)
        this.Starlines[index] = new AmplifyStarlineCache();
    }

    public void Destroy()
    {
      for (int index = 0; index < 4; ++index)
        this.Starlines[index].Destroy();
      this.Starlines = (AmplifyStarlineCache[]) null;
      this.CromaticAberrationMat = (Vector4[,]) null;
    }
  }
}
