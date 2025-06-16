using Engine.Common.Components.Regions;
using System;

namespace RegionReputation
{
  [Serializable]
  public struct RegionException
  {
    public string Signature;
    public RegionEnum Region;
  }
}
