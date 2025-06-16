using System;
using UnityEngine;

namespace Rain
{
  public class Audio : MonoBehaviour
  {
    public Audio.Source[] sources;
    private bool disabled;

    private void Disable()
    {
      if (this.disabled)
        return;
      for (int index = 0; index < this.sources.Length; ++index)
        this.sources[index].source.enabled = false;
      this.disabled = true;
    }

    private void Awake() => this.Disable();

    private void Update()
    {
      RainManager instance = RainManager.Instance;
      float num1 = !((UnityEngine.Object) instance == (UnityEngine.Object) null) ? instance.actualRainIntensity : 0.0f;
      if ((double) num1 == 0.0)
      {
        this.Disable();
      }
      else
      {
        this.disabled = false;
        for (int index = 0; index < this.sources.Length; ++index)
        {
          float num2 = index < 1 ? 0.0f : this.sources[index - 1].threshold;
          float threshold = this.sources[index].threshold;
          float num3 = index == this.sources.Length - 1 ? float.MaxValue : this.sources[index + 1].threshold;
          float num4 = (double) num1 > (double) num2 && (double) num1 < (double) num3 ? ((double) num1 >= (double) threshold ? (index != this.sources.Length - 1 && (double) num1 != (double) threshold ? (float) (((double) num3 - (double) num1) / ((double) num3 - (double) threshold)) : 1f) : (float) (((double) num1 - (double) num2) / ((double) threshold - (double) num2))) : 0.0f;
          if ((double) num4 == 0.0)
          {
            this.sources[index].source.enabled = false;
          }
          else
          {
            this.sources[index].source.volume = num4;
            this.sources[index].source.enabled = true;
          }
        }
      }
    }

    private void OnValidate()
    {
      for (int index = 1; index < this.sources.Length; ++index)
      {
        if ((double) this.sources[index - 1].threshold > (double) this.sources[index].threshold)
          this.sources[index].threshold = this.sources[index - 1].threshold;
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
