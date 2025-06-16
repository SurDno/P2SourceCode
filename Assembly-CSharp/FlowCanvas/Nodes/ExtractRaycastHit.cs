// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ExtractRaycastHit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  public class ExtractRaycastHit : ExtractorNode<RaycastHit, GameObject, float, Vector3, Vector3>
  {
    public override void Invoke(
      RaycastHit hit,
      out GameObject gameObject,
      out float distance,
      out Vector3 normal,
      out Vector3 point)
    {
      gameObject = hit.collider?.gameObject;
      distance = hit.distance;
      normal = hit.normal;
      point = hit.point;
    }
  }
}
