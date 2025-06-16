// Decompiled with JetBrains decompiler
// Type: AmplifyColorTriggerProxy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof (SphereCollider))]
[AddComponentMenu("")]
public class AmplifyColorTriggerProxy : AmplifyColorTriggerProxyBase
{
  private SphereCollider sphereCollider;
  private Rigidbody rigidBody;

  private void Start()
  {
    this.sphereCollider = this.GetComponent<SphereCollider>();
    this.sphereCollider.radius = 0.01f;
    this.sphereCollider.isTrigger = true;
    this.rigidBody = this.GetComponent<Rigidbody>();
    this.rigidBody.useGravity = false;
    this.rigidBody.isKinematic = true;
  }

  private void LateUpdate()
  {
    this.transform.position = this.Reference.position;
    this.transform.rotation = this.Reference.rotation;
  }
}
