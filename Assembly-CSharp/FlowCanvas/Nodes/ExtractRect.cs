// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ExtractRect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  public class ExtractRect : ExtractorNode<Rect, Vector2, float, float, float, float>
  {
    public override void Invoke(
      Rect rect,
      out Vector2 center,
      out float xMin,
      out float xMax,
      out float yMin,
      out float yMax)
    {
      center = rect.center;
      xMin = rect.xMin;
      xMax = rect.xMax;
      yMin = rect.yMin;
      yMax = rect.yMax;
    }
  }
}
