using System.Collections.Generic;

namespace Scripts.Utility
{
  public class RaycastComparer : IComparer<RaycastHit>
  {
    public static readonly RaycastComparer Instance = new RaycastComparer();

    public int Compare(RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance);
  }
}
