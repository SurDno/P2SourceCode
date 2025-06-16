// Decompiled with JetBrains decompiler
// Type: ParticleBursterFire
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ParticleBursterFire : MonoBehaviour
{
  private ParticleBurster particleBurster;

  private void Awake() => this.particleBurster = this.GetComponent<ParticleBurster>();

  private void OnEnable()
  {
    if ((Object) this.particleBurster == (Object) null)
      Debug.Log((object) "particleBurster is null");
    else
      this.particleBurster.Fire();
  }
}
