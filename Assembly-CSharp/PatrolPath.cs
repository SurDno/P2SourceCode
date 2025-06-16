// Decompiled with JetBrains decompiler
// Type: PatrolPath
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[ExecuteInEditMode]
[Serializable]
public class PatrolPath : MonoBehaviour
{
  public PatrolTypeEnum PatrolType;
  public bool RestartFromClosestPoint = true;
  private List<Transform> pointsList = new List<Transform>();

  public List<Transform> PointsList => this.pointsList;

  private void Start() => this.UpdateList();

  public List<Transform> GetPresetPath(int pointIndex, bool reverse)
  {
    if (!reverse)
    {
      if (pointIndex < 1)
        return (List<Transform>) null;
      PathPart component = this.PointsList[pointIndex - 1].GetComponent<PathPart>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        return (List<Transform>) null;
      List<Transform> presetPath = new List<Transform>();
      presetPath.AddRange((IEnumerable<Transform>) component.PointsList);
      if (pointIndex < this.PointsList.Count - 1)
        presetPath.Add(this.PointsList[pointIndex]);
      return presetPath;
    }
    PathPart component1 = this.PointsList[pointIndex].GetComponent<PathPart>();
    if ((UnityEngine.Object) component1 == (UnityEngine.Object) null)
      return (List<Transform>) null;
    List<Transform> presetPath1 = new List<Transform>();
    presetPath1.AddRange((IEnumerable<Transform>) component1.PointsList);
    presetPath1.Reverse();
    presetPath1.Add(this.PointsList[pointIndex]);
    return presetPath1;
  }

  private void UpdateList()
  {
    this.pointsList.Clear();
    int childCount = this.transform.childCount;
    for (int index = 0; index < childCount; ++index)
      this.pointsList.Add(this.transform.GetChild(index));
  }
}
