// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.SwitchBool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("Switch Condition")]
  [Category("Flow Controllers/Switchers")]
  [Description("Branch the Flow based on a conditional boolean value")]
  [ContextDefinedInputs(new Type[] {typeof (bool)})]
  public class SwitchBool : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      ValueInput<bool> c = this.AddValueInput<bool>("Condition");
      FlowOutput tOut = this.AddFlowOutput("True");
      FlowOutput fOut = this.AddFlowOutput("False");
      this.AddFlowInput("In", (FlowHandler) (() => this.Call(c.value ? tOut : fOut)));
    }
  }
}
