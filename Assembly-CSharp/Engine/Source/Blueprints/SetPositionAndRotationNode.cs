// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.SetPositionAndRotationNode
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
  public class SetPositionAndRotationNode : FlowControlNode
  {
    private ValueInput<Transform> transformValue;
    private ValueInput<Transform> valueValue;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        Transform transform1 = this.transformValue.value;
        if ((Object) transform1 != (Object) null)
        {
          Transform transform2 = this.valueValue.value;
          if ((Object) transform2 != (Object) null)
            transform1.SetPositionAndRotation(transform2.position, transform2.rotation);
        }
        output.Call();
      }));
      this.transformValue = this.AddValueInput<Transform>("Transform");
      this.valueValue = this.AddValueInput<Transform>("Value");
    }
  }
}
