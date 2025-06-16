// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.IsPlayerNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  [Description("НЕ ИСПОЛЬЗОВАТЬ, ЗАМЕНИТЬ НА IsPlayer2Node")]
  [Color("FF0000")]
  public class IsPlayerNode : FlowControlNode
  {
    [Port("Target")]
    private ValueInput<GameObject> inputValue;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput trueOut = this.AddFlowOutput("True");
      FlowOutput falseOut = this.AddFlowOutput("False");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        GameObject gameObject = this.inputValue.value;
        if ((Object) gameObject == (Object) null)
          falseOut.Call();
        else if ((Object) gameObject.GetComponent<PivotPlayer>() == (Object) null)
          falseOut.Call();
        else
          trueOut.Call();
      }));
    }
  }
}
