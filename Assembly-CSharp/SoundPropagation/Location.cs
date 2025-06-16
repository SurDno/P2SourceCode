using UnityEngine;

namespace SoundPropagation
{
  public struct Location
  {
    public bool PathFound;
    public float PathLength;
    public Vector3 NearestCorner;
    public Filtering Filtering;
  }
}
