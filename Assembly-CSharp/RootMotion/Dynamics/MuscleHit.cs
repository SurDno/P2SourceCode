using UnityEngine;

namespace RootMotion.Dynamics
{
  public struct MuscleHit(int muscleIndex, float unPin, Vector3 force, Vector3 position) {
    public int muscleIndex = muscleIndex;
    public float unPin = unPin;
    public Vector3 force = force;
    public Vector3 position = position;
  }
}
