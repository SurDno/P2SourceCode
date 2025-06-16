// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.DestroyGameObjectNode
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
  public class DestroyGameObjectNode : FlowControlNode
  {
    private ValueInput<GameObject> goInput;
    private ValueInput<float> delayInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        GameObject gameObject = this.goInput.value;
        if (!((Object) gameObject != (Object) null))
          return;
        Object.Destroy((Object) gameObject, this.delayInput.value);
      }));
      this.goInput = this.AddValueInput<GameObject>("GameObject");
      this.delayInput = this.AddValueInput<float>("Delay");
    }
  }
}
