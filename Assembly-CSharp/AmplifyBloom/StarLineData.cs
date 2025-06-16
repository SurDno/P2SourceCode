using System;

namespace AmplifyBloom
{
  [Serializable]
  public class StarLineData
  {
    [SerializeField]
    internal int PassCount;
    [SerializeField]
    internal float SampleLength;
    [SerializeField]
    internal float Attenuation;
    [SerializeField]
    internal float Inclination;
  }
}
