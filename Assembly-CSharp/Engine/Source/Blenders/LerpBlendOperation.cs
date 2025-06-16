using Engine.Common.Blenders;
using UnityEngine;

namespace Engine.Source.Blenders
{
  public class LerpBlendOperation : IBlendOperation, IPureBlendOperation
  {
    public float Time { get; set; }

    public Color Blend(Color a, Color b) => Color.Lerp(a, b, this.Time);

    public Vector2 Blend(Vector2 a, Vector2 b) => Vector2.Lerp(a, b, this.Time);

    public int Blend(int a, int b) => (int) Mathf.Lerp((float) a, (float) b, this.Time);

    public float Blend(float a, float b) => Mathf.Lerp(a, b, this.Time);
  }
}
