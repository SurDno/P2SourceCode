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
      if (weight <= 0.0 || !isValid)
        return;
      if (!initiated)
      {
        defaultLocalPosition = transform.localPosition;
        lastLocalPosition = transform.localPosition;
        initiated = true;
      }
      if (positionChanged)
        defaultLocalPosition = transform.localPosition;
      transform.localPosition = defaultLocalPosition;
      transform.position += offset * weight;
      lastLocalPosition = transform.localPosition;
    }

    public ConstraintPositionOffset()
    {
    }

    public ConstraintPositionOffset(Transform transform) => this.transform = transform;

    private bool positionChanged => transform.localPosition != lastLocalPosition;
  }
}
