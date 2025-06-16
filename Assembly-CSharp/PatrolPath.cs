using System;
using System.Collections.Generic;

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
        return (List<Transform>) null;
      PathPart component = PointsList[pointIndex - 1].GetComponent<PathPart>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        return (List<Transform>) null;
      List<Transform> presetPath = new List<Transform>();
      presetPath.AddRange((IEnumerable<Transform>) component.PointsList);
      if (pointIndex < PointsList.Count - 1)
        presetPath.Add(PointsList[pointIndex]);
      return presetPath;
    }
    PathPart component1 = PointsList[pointIndex].GetComponent<PathPart>();
    if ((UnityEngine.Object) component1 == (UnityEngine.Object) null)
      return (List<Transform>) null;
    List<Transform> presetPath1 = new List<Transform>();
    presetPath1.AddRange((IEnumerable<Transform>) component1.PointsList);
    presetPath1.Reverse();
    presetPath1.Add(PointsList[pointIndex]);
    return presetPath1;
  }

  private void UpdateList()
  {
    pointsList.Clear();
    int childCount = this.transform.childCount;
    for (int index = 0; index < childCount; ++index)
      pointsList.Add(this.transform.GetChild(index));
  }
}
