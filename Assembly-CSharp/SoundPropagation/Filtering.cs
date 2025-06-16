using System;
using UnityEngine;

namespace SoundPropagation
{
  [Serializable]
  public struct Filtering
  {
    [Tooltip("High frequency loss")]
    public float Occlusion;

    public void AddFiltering(Filtering filteringPerMeter, float distance)
    {
      this.Occlusion += filteringPerMeter.Occlusion;
    }

    public void AddOcclusion(float occlusion) => this.Occlusion += occlusion;

    public float Loss => this.Occlusion;
  }
}
