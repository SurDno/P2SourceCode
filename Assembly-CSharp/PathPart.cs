using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PathPart : MonoBehaviour
{
  private List<Transform> pointsList = new List<Transform>();

  private void Start() => UpdateList();

  public List<Transform> PointsList => pointsList;

  private void UpdateList()
  {
    pointsList.Clear();
    int childCount = transform.childCount;
    for (int index = 0; index < childCount; ++index)
      pointsList.Add(transform.GetChild(index));
  }
}
