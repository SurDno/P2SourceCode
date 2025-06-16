using System;

namespace SoundPropagation
{
  [Serializable]
  public struct Filtering
  {
    [Tooltip("High frequency loss")]
    public float Occlusion;

    public void AddFiltering(Filtering filteringPerMeter, float distance)
    {
      Occlusion += filteringPerMeter.Occlusion;
    }

    public void AddOcclusion(float occlusion) => Occlusion += occlusion;

    public float Loss => Occlusion;
  }
}
