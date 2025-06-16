// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blenders.IBlendOperation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Blenders;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blenders
{
  public interface IBlendOperation : IPureBlendOperation
  {
    float Blend(float a, float b);

    int Blend(int a, int b);

    Color Blend(Color a, Color b);

    Vector2 Blend(Vector2 a, Vector2 b);
  }
}
