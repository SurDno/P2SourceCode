// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.DelayFramesNode
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
  public class DelayFramesNode : FlowControlNode
  {
    private ValueInput<int> frames;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.frames = this.AddValueInput<int>("Frames");
      this.AddFlowInput("In", (FlowHandler) (() => this.StartCoroutine(this.Delay((float) this.frames.value, output))));
    }

    private IEnumerator Delay(float frames, FlowOutput output)
    {
      for (int index = 0; (double) index < (double) frames; ++index)
        yield return (object) new WaitForEndOfFrame();
      output.Call();
    }
  }
}
