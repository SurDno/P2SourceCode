using System;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class ConstraintPosition : Constraint
  {
    public Vector3 position;

    public override void UpdateConstraint()
    {
      if (weight <= 0.0 || !isValid)
        return;
      transform.position = Vector3.Lerp(transform.position, position, weight);
    }

    public ConstraintPosition()
    {
    }

    public ConstraintPosition(Transform transform) => this.transform = transform;
  }
}
