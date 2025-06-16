// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ForLoop
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using ParadoxNotion.FlowCanvas.Module;
using System;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Description("Perform a for loop")]
  [Category("Flow Controllers/Iterators")]
  [ContextDefinedInputs(new Type[] {typeof (int)})]
  [ContextDefinedOutputs(new Type[] {typeof (int)})]
  public class ForLoop : FlowControlNode
  {
    private int current;
    private bool broken;

    protected override void RegisterPorts()
    {
      ValueInput<int> n = this.AddValueInput<int>("Loops");
      this.AddValueOutput<int>("Index", (ValueHandler<int>) (() => this.current));
      FlowOutput fCurrent = this.AddFlowOutput("Do");
      FlowOutput fFinish = this.AddFlowOutput("Done");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        this.current = 0;
        this.broken = false;
        for (int index = 0; index < n.value && !this.broken; ++index)
        {
          this.current = index;
          fCurrent.Call();
        }
        fFinish.Call();
      }));
      this.AddFlowInput("Break", (FlowHandler) (() => this.broken = true));
    }
  }
}
