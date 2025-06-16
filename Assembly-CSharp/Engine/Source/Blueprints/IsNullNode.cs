// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.IsNullNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class IsNullNode : FlowControlNode
  {
    private ValueInput<object> inputValue;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput trueOut = this.AddFlowOutput("True");
      FlowOutput falseOut = this.AddFlowOutput("False");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        if (this.inputValue.value == null)
          trueOut.Call();
        else
          falseOut.Call();
      }));
      this.inputValue = this.AddValueInput<object>("Target");
    }
  }
}
