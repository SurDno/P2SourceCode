using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
  public sealed class MinAttribute(float min) : PropertyAttribute 
  {
    public readonly float min = min;
  }
}
