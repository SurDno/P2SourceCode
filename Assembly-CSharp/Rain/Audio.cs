using System;
using UnityEngine;

namespace Rain
{
  public class Audio : MonoBehaviour
  {
    public Source[] sources;
    private bool disabled;

    private void Disable()
    {
      if (disabled)
        return;
      for (int index = 0; index < sources.Length; ++index)
        sources[index].source.enabled = false;
      disabled = true;
    }

    private void Awake() => Disable();

    private void Update()
    {
      RainManager instance = RainManager.Instance;
      float num1 = !(instance == null) ? instance.actualRainIntensity : 0.0f;
      if (num1 == 0.0)
      {
        Disable();
      }
      else
      {
        disabled = false;
        for (int index = 0; index < sources.Length; ++index)
        {
          float num2 = index < 1 ? 0.0f : sources[index - 1].threshold;
          float threshold = sources[index].threshold;
          float num3 = index == sources.Length - 1 ? float.MaxValue : sources[index + 1].threshold;
          float num4 = num1 > (double) num2 && num1 < (double) num3 ? (num1 >= (double) threshold ? (index != sources.Length - 1 && num1 != (double) threshold ? (float) ((num3 - (double) num1) / (num3 - (double) threshold)) : 1f) : (float) ((num1 - (double) num2) / (threshold - (double) num2))) : 0.0f;
          if (num4 == 0.0)
          {
            sources[index].source.enabled = false;
          }
          else
          {
            sources[index].source.volume = num4;
            sources[index].source.enabled = true;
          }
        }
      }
    }

    private void OnValidate()
    {
      for (int index = 1; index < sources.Length; ++index)
      {
        if (sources[index - 1].threshold > (double) sources[index].threshold)
          sources[index].threshold = sources[index - 1].threshold;
      }
    }

    [Serializable]
    public struct Source
    {
      [Range(0.0f, 1f)]
      public float threshold;
      public AudioSource source;
    }
  }
}
