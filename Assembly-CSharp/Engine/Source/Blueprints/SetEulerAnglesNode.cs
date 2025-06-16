// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.SetEulerAnglesNode
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
  public class SetEulerAnglesNode : FlowControlNode
  {
    private ValueInput<Transform> targetInput;
    private ValueInput<Vector3> eulerAnglesInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        Transform transform = this.targetInput.value;
        if ((Object) transform != (Object) null)
          transform.eulerAngles = this.eulerAnglesInput.value;
        output.Call();
      }));
      this.targetInput = this.AddValueInput<Transform>("Target");
      this.eulerAnglesInput = this.AddValueInput<Vector3>("EulerAngles");
    }
  }
}
