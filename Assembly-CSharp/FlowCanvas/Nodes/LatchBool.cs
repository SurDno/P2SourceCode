// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.LatchBool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using ParadoxNotion.FlowCanvas.Module;
using System;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("Latch Condition")]
  [Category("Flow Controllers/Flow Convert")]
  [Description("Convert a Flow signal to boolean value")]
  [ContextDefinedOutputs(new Type[] {typeof (bool)})]
  public class LatchBool : FlowControlNode
  {
    private bool latched;

    protected override void RegisterPorts()
    {
      FlowOutput o = this.AddFlowOutput("Out");
      this.AddFlowInput("True", (FlowHandler) (() =>
      {
        this.latched = true;
        o.Call();
      }));
      this.AddFlowInput("False", (FlowHandler) (() =>
      {
        this.latched = false;
        o.Call();
      }));
      this.AddValueOutput<bool>("Value", (ValueHandler<bool>) (() => this.latched));
    }
  }
}
