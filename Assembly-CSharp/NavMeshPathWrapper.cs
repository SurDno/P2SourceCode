using Inspectors;
using UnityEngine.AI;

public class NavMeshPathWrapper(NavMeshPath path) {
  [Inspected]
  private int CornersCount => path.corners.Length;

  [Inspected]
  private NavMeshPathStatus Status => path.status;
}
