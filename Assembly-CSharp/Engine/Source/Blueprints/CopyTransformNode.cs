// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.CopyTransformNode
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
  public class CopyTransformNode : FlowControlNode
  {
    private ValueInput<Transform> fromInput;
    private ValueInput<Transform> toInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        Transform transform1 = this.fromInput.value;
        Transform transform2 = this.toInput.value;
        if ((Object) transform1 != (Object) null && (Object) transform2 != (Object) null)
          transform2.SetPositionAndRotation(transform1.position, transform1.rotation);
        output.Call();
      }));
      this.fromInput = this.AddValueInput<Transform>("From");
      this.toInput = this.AddValueInput<Transform>("To");
    }
  }
}
