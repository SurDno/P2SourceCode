using System;

namespace RegionReputation
{
  [Serializable]
  public class RegionReputationNameItem
  {
    public string DataName;
    public ValueLevel[] ReputationLevels;
    public RegionException[] RegionExceptions;
  }
}
