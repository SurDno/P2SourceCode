// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.DelayNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class DelayNode : FlowControlNode
  {
    private ValueInput<float> delay;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.delay = this.AddValueInput<float>("Delay");
      this.AddFlowInput("In", (FlowHandler) (() => this.StartCoroutine(this.Delay(this.delay.value, output))));
    }

    private IEnumerator Delay(float delay, FlowOutput output)
    {
      yield return (object) new WaitForSeconds(delay);
      if ((Object) this.graphAgent != (Object) null)
        output.Call();
    }
  }
}
