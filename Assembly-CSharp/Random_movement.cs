// Decompiled with JetBrains decompiler
// Type: Random_movement
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class Random_movement : MonoBehaviour
{
  private Vector3 dir;

  private void Start()
  {
    this.dir = Quaternion.Euler(0.0f, Random.Range(0.0f, 360f), 0.0f) * Vector3.forward;
  }

  private void FixedUpdate() => this.transform.Translate(this.dir / 20f);
}
