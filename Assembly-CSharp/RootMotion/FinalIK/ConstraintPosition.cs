using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class ConstraintPosition : Constraint
  {
    public Vector3 position;

    public override void UpdateConstraint()
    {
      if ((double) this.weight <= 0.0 || !this.isValid)
        return;
      this.transform.position = Vector3.Lerp(this.transform.position, this.position, this.weight);
    }

    public ConstraintPosition()
    {
    }

    public ConstraintPosition(Transform transform) => this.transform = transform;
  }
}
