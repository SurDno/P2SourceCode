using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Utility
{
  public class RaycastComparer : IComparer<RaycastHit>
  {
    public static readonly RaycastComparer Instance = new();

    public int Compare(RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance);
  }
}
