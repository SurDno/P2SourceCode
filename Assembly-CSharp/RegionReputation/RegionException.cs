using System;
using Engine.Common.Components.Regions;

namespace RegionReputation
{
  [Serializable]
  public struct RegionException
  {
    public string Signature;
    public RegionEnum Region;
  }
}
