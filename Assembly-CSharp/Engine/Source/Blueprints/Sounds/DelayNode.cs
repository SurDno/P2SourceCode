// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Sounds.DelayNode
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
  public class DelayNode : FlowControlNode
  {
    [Port("StartDelay")]
    private ValueInput<float> startDelayInput;
    [Port("EndDelay")]
    private ValueInput<float> endDelayInput;
    [Port("Value")]
    private ValueInput<bool> valueInput;
    private bool prevValue;
    private float accumulate;

    [Port("Value")]
    private bool Value() => this.Compute(this.valueInput.value);

    public bool Compute(bool value)
    {
      if (value)
      {
        if (this.prevValue)
        {
          this.accumulate = 0.0f;
        }
        else
        {
          this.accumulate += Time.deltaTime;
          if ((double) this.accumulate >= (double) this.startDelayInput.value)
          {
            this.prevValue = true;
            this.accumulate = 0.0f;
          }
        }
      }
      else if (this.prevValue)
      {
        this.accumulate += Time.deltaTime;
        if ((double) this.accumulate >= (double) this.endDelayInput.value)
        {
          this.prevValue = false;
          this.accumulate = 0.0f;
        }
      }
      else
        this.accumulate = 0.0f;
      return this.prevValue;
    }
  }
}
