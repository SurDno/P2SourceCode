using UnityEngine;

namespace RootMotion.Dynamics
{
  public struct MuscleCollision(int muscleIndex, Collision collision, bool isStay = false) {
    public int muscleIndex = muscleIndex;
    public Collision collision = collision;
    public bool isStay = isStay;
  }
}
