// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ExtractContactPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  public class ExtractContactPoint : 
    ExtractorNode<ContactPoint, Vector3, Vector3, Collider, Collider>
  {
    public override void Invoke(
      ContactPoint contactPoint,
      out Vector3 normal,
      out Vector3 point,
      out Collider colliderA,
      out Collider colliderB)
    {
      normal = contactPoint.normal;
      point = contactPoint.point;
      colliderA = contactPoint.thisCollider;
      colliderB = contactPoint.otherCollider;
    }
  }
}
