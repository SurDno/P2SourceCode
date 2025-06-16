// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ExtractBounds
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  public class ExtractBounds : ExtractorNode<Bounds, Vector3, Vector3, Vector3, Vector3, Vector3>
  {
    public override void Invoke(
      Bounds bounds,
      out Vector3 center,
      out Vector3 extents,
      out Vector3 max,
      out Vector3 min,
      out Vector3 size)
    {
      center = bounds.center;
      extents = bounds.extents;
      max = bounds.max;
      min = bounds.min;
      size = bounds.size;
    }
  }
}
