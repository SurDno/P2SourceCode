using System;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class ConstraintRotation : Constraint
  {
    public Quaternion rotation;

    public override void UpdateConstraint()
    {
      if (weight <= 0.0 || !isValid)
        return;
      transform.rotation = Quaternion.Slerp(transform.rotation, rotation, weight);
    }

    public ConstraintRotation()
    {
    }

    public ConstraintRotation(Transform transform) => this.transform = transform;
  }
}
