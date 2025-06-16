using System;
using UnityEngine;

namespace RootMotion.Dynamics
{
  [Serializable]
  public class Weight
  {
    public Mode mode;
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
      return mode == Mode.Curve ? curve.Evaluate(param) : floatValue;
    }

    [Serializable]
    public enum Mode
    {
      Float,
      Curve,
    }
  }
}
