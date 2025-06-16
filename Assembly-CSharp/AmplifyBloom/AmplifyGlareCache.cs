using System;
using UnityEngine;

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
      Starlines = new AmplifyStarlineCache[4];
      CromaticAberrationMat = new Vector4[4, 8];
      for (int index = 0; index < 4; ++index)
        Starlines[index] = new AmplifyStarlineCache();
    }

    public void Destroy()
    {
      for (int index = 0; index < 4; ++index)
        Starlines[index].Destroy();
      Starlines = null;
      CromaticAberrationMat = null;
    }
  }
}
