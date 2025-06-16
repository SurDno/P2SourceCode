using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class ConstraintPositionOffset : Constraint
  {
    public Vector3 offset;
    private Vector3 defaultLocalPosition;
    private Vector3 lastLocalPosition;
    private bool initiated;

    public override void UpdateConstraint()
    {
      if ((double) this.weight <= 0.0 || !this.isValid)
        return;
      if (!this.initiated)
      {
        this.defaultLocalPosition = this.transform.localPosition;
        this.lastLocalPosition = this.transform.localPosition;
        this.initiated = true;
      }
      if (this.positionChanged)
        this.defaultLocalPosition = this.transform.localPosition;
      this.transform.localPosition = this.defaultLocalPosition;
      this.transform.position += this.offset * this.weight;
      this.lastLocalPosition = this.transform.localPosition;
    }

    public ConstraintPositionOffset()
    {
    }

    public ConstraintPositionOffset(Transform transform) => this.transform = transform;

    private bool positionChanged => this.transform.localPosition != this.lastLocalPosition;
  }
}
