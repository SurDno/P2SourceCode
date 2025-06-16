using System;
using UnityEngine;

namespace RootMotion.Dynamics
{
  [Serializable]
  public class Weight
  {
    public Weight.Mode mode;
    public float floatValue;
    public AnimationCurve curve;
    public string tooltip = "";

    public Weight(float floatValue) => this.floatValue = floatValue;

    public Weight(float floatValue, string tooltip)
    {
      this.floatValue = floatValue;
      this.tooltip = tooltip;
    }

    public float GetValue(float param)
    {
      return this.mode == Weight.Mode.Curve ? this.curve.Evaluate(param) : this.floatValue;
    }

    [Serializable]
    public enum Mode
    {
      Float,
      Curve,
    }
  }
}
