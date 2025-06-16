// Decompiled with JetBrains decompiler
// Type: RootMotion.Dynamics.Weight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
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
