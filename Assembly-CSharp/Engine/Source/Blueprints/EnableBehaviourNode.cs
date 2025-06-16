// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.EnableBehaviourNode
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
  public class EnableBehaviourNode : FlowControlNode
  {
    private ValueInput<Behaviour> behaviourInput;
    private ValueInput<bool> enableInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        Behaviour behaviour = this.behaviourInput.value;
        if ((Object) behaviour != (Object) null)
          behaviour.enabled = this.enableInput.value;
        output.Call();
      }));
      this.behaviourInput = this.AddValueInput<Behaviour>("Behaviour");
      this.enableInput = this.AddValueInput<bool>("Enable");
    }
  }
}
