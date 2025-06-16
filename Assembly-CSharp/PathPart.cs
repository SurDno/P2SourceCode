// Decompiled with JetBrains decompiler
// Type: PathPart
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
[ExecuteInEditMode]
public class PathPart : MonoBehaviour
{
  private List<Transform> pointsList = new List<Transform>();

  private void Start() => this.UpdateList();

  public List<Transform> PointsList => this.pointsList;

  private void UpdateList()
  {
    this.pointsList.Clear();
    int childCount = this.transform.childCount;
    for (int index = 0; index < childCount; ++index)
      this.pointsList.Add(this.transform.GetChild(index));
  }
}
