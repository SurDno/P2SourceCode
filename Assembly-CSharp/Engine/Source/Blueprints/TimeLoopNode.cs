// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.TimeLoopNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class TimeLoopNode : FlowControlNode
  {
    [Port("Length")]
    private ValueInput<float> lengthInput;
    private float prevTime;
    private float value;

    [Port("Value")]
    private float Value()
    {
      this.UpdateValue();
      return this.value;
    }

    private void UpdateValue()
    {
      float time = Time.time;
      if ((double) this.prevTime == (double) time)
        return;
      if ((double) this.lengthInput.value != 0.0)
        this.value = Mathf.Repeat(this.value + (time - this.prevTime) / this.lengthInput.value, 1f);
      this.prevTime = time;
    }
  }
}
