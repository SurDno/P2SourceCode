using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public abstract class Constraint
  {
    public Transform transform;
    public float weight;

    public bool isValid => (UnityEngine.Object) this.transform != (UnityEngine.Object) null;

    public abstract void UpdateConstraint();
  }
}
