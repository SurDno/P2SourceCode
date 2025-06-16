// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Sounds.CurveNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class CurveNode : FlowControlNode
  {
    [Port("Value")]
    private ValueInput<float> valueInput;
    [Port("Curve")]
    private ValueInput<AnimationCurve> curveInput;

    [Port("Value")]
    private float Value()
    {
      AnimationCurve animationCurve = this.curveInput.value;
      return animationCurve != null ? animationCurve.Evaluate(this.valueInput.value) : this.valueInput.value;
    }
  }
}
