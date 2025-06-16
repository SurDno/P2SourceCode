// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blenders.LerpBlendOperation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Blenders;
using UnityEngine;

#nullable disable
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
