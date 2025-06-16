using System.Collections.Generic;
using UnityEngine;

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
