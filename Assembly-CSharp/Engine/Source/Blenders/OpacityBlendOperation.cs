using Engine.Common.Blenders;
using UnityEngine;

namespace Engine.Source.Blenders
{
  public class OpacityBlendOperation : IBlendOperation, IPureBlendOperation
  {
    public float Opacity { get; set; }

    public Color Blend(Color a, Color b) => a * (1f - this.Opacity) + b * this.Opacity;

    public Vector2 Blend(Vector2 a, Vector2 b) => a * (1f - this.Opacity) + b * this.Opacity;

    public int Blend(int a, int b)
    {
      return (int) ((double) a * (1.0 - (double) this.Opacity) + (double) b * (double) this.Opacity);
    }

    public float Blend(float a, float b)
    {
      return (float) ((double) a * (1.0 - (double) this.Opacity) + (double) b * (double) this.Opacity);
    }
  }
}
