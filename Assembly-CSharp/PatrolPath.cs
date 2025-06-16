using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[Serializable]
public class PatrolPath : MonoBehaviour
{
  public PatrolTypeEnum PatrolType;
  public bool RestartFromClosestPoint = true;
  private List<Transform> pointsList = new List<Transform>();

  public List<Transform> PointsList => pointsList;

  private void Start() => UpdateList();

  public List<Transform> GetPresetPath(int pointIndex, bool reverse)
  {
    if (!reverse)
    {
      if (pointIndex < 1)
        return null;
      PathPart component = PointsList[pointIndex - 1].GetComponent<PathPart>();
      if (component == null)
        return null;
      List<Transform> presetPath = new List<Transform>();
      presetPath.AddRange(component.PointsList);
      if (pointIndex < PointsList.Count - 1)
        presetPath.Add(PointsList[pointIndex]);
      return presetPath;
    }
    PathPart component1 = PointsList[pointIndex].GetComponent<PathPart>();
    if (component1 == null)
      return null;
    List<Transform> presetPath1 = new List<Transform>();
    presetPath1.AddRange(component1.PointsList);
    presetPath1.Reverse();
    presetPath1.Add(PointsList[pointIndex]);
    return presetPath1;
  }

  private void UpdateList()
  {
    pointsList.Clear();
    int childCount = transform.childCount;
    for (int index = 0; index < childCount; ++index)
      pointsList.Add(transform.GetChild(index));
  }
}
