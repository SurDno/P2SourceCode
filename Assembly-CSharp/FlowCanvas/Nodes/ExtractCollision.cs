// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ExtractCollision
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  public class ExtractCollision : 
    ExtractorNode<Collision, ContactPoint[], ContactPoint, GameObject, Vector3>
  {
    public override void Invoke(
      Collision collision,
      out ContactPoint[] contacts,
      out ContactPoint firstContact,
      out GameObject gameObject,
      out Vector3 velocity)
    {
      contacts = collision.contacts;
      firstContact = collision.contacts[0];
      gameObject = collision.gameObject;
      velocity = collision.relativeVelocity;
    }
  }
}
