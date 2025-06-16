// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Sounds.SmoothNode
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
  public class SmoothNode : FlowControlNode
  {
    [Port("Value")]
    private ValueInput<float> valueInput;
    [Port("Time")]
    private ValueInput<float> timeInput;
    private float time;
    private float prevValue;

    [Port("Value")]
    private float Value()
    {
      this.UpdateValue();
      return this.prevValue;
    }

    public override void OnGraphStarted()
    {
      base.OnGraphStarted();
      this.time = Time.time;
    }

    private void UpdateValue()
    {
      float num1 = this.valueInput.value;
      if ((double) num1 == (double) this.prevValue)
      {
        this.time = Time.time;
      }
      else
      {
        if ((double) this.time == (double) Time.time)
          return;
        float num2 = (Time.time - this.time) / this.timeInput.value;
        if ((double) num1 > (double) this.prevValue)
        {
          this.prevValue += num2;
          if ((double) this.prevValue > (double) num1)
            this.prevValue = num1;
        }
        else
        {
          this.prevValue -= num2;
          if ((double) this.prevValue < (double) num1)
            this.prevValue = num1;
        }
        this.time = Time.time;
      }
    }
  }
}
