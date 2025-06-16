using System.Collections.Generic;

[ExecuteInEditMode]
public class PathPart : MonoBehaviour
{
  private List<Transform> pointsList = new List<Transform>();

  private void Start() => UpdateList();

  public List<Transform> PointsList => pointsList;

  private void UpdateList()
  {
    pointsList.Clear();
    int childCount = this.transform.childCount;
    for (int index = 0; index < childCount; ++index)
      pointsList.Add(this.transform.GetChild(index));
  }
}
