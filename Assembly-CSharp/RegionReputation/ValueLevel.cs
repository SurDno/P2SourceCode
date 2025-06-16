using System;

namespace RegionReputation
{
  [Serializable]
  public struct ValueLevel
  {
    public string Signature;
    public float Threshold;

    public static string GetSignature(ValueLevel[] levels, float value, bool greater = false)
    {
      for (int index = 0; index < levels.Length; ++index)
      {
        if (greater ? (double) value > (double) levels[index].Threshold : (double) value < (double) levels[index].Threshold)
          return levels[index].Signature;
      }
      return (string) null;
    }
  }
}
