// Decompiled with JetBrains decompiler
// Type: Soldier_BreakDoor_SetIsKinematic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class Soldier_BreakDoor_SetIsKinematic : MonoBehaviour
{
  private bool isKinematic;
  private Rigidbody rigidbody;

  private void Start() => this.rigidbody = this.GetComponent<Rigidbody>();

  private void Update()
  {
    if (this.rigidbody.isKinematic)
      return;
    this.rigidbody.isKinematic = true;
  }
}
