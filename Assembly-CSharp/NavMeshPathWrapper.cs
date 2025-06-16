using Inspectors;
using UnityEngine.AI;

public class NavMeshPathWrapper
{
  private NavMeshPath path;

  public NavMeshPathWrapper(NavMeshPath path) => this.path = path;

  [Inspected]
  private int CornersCount => this.path.corners.Length;

  [Inspected]
  private NavMeshPathStatus Status => this.path.status;
}
