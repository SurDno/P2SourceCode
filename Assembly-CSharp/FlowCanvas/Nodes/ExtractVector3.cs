// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ExtractVector3
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  public class ExtractVector3 : ExtractorNode<Vector3, float, float, float>
  {
    public override void Invoke(Vector3 vector, out float x, out float y, out float z)
    {
      x = vector.x;
      y = vector.y;
      z = vector.z;
    }
  }
}
