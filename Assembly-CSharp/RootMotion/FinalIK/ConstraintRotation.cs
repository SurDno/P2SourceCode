using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class ConstraintRotation : Constraint
  {
    public Quaternion rotation;

    public override void UpdateConstraint()
    {
      if ((double) this.weight <= 0.0 || !this.isValid)
        return;
      this.transform.rotation = Quaternion.Slerp(this.transform.rotation, this.rotation, this.weight);
    }

    public ConstraintRotation()
    {
    }

    public ConstraintRotation(Transform transform) => this.transform = transform;
  }
}
