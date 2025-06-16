// Decompiled with JetBrains decompiler
// Type: CenterOfMass
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class CenterOfMass : MonoBehaviour
{
  public Vector3 Offset = new Vector3();

  private void Start() => this.GetComponent<Rigidbody>().centerOfMass += this.Offset;
}
