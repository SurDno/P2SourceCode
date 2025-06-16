// Decompiled with JetBrains decompiler
// Type: DrawGizmoIcon
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class DrawGizmoIcon : MonoBehaviour
{
  [SerializeField]
  private string textureName;
  [SerializeField]
  private Vector3 offset;

  private void OnDrawGizmos()
  {
    Gizmos.DrawIcon(this.transform.position + this.offset, this.textureName);
  }
}
