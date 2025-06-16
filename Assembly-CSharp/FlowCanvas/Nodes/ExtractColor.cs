// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ExtractColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  public class ExtractColor : ExtractorNode<Color, float, float, float, float>
  {
    public override void Invoke(Color color, out float r, out float g, out float b, out float a)
    {
      r = color.r;
      g = color.g;
      b = color.b;
      a = color.a;
    }
  }
}
