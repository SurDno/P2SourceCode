// Decompiled with JetBrains decompiler
// Type: HerbRootsSpawnPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class HerbRootsSpawnPoint : MonoBehaviour
{
  private void OnDrawGizmos()
  {
    Gizmos.color = Color.white;
    Gizmos.DrawCube(this.transform.position + new Vector3(0.0f, 0.5f, 0.0f), new Vector3(0.05f, 1f, 0.05f));
    Gizmos.color = Color.white;
    Gizmos.DrawSphere(this.transform.position, 0.1f);
    Gizmos.color = Color.red;
    Gizmos.DrawCube(this.transform.position + Vector3.up, new Vector3(0.2f, 0.05f, 0.2f));
    Gizmos.color = Color.yellow;
    Gizmos.DrawCube(this.transform.position + Vector3.up * 1.05f, new Vector3(0.05f, 0.05f, 0.05f));
  }
}
