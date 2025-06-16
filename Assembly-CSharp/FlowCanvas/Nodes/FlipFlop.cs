// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.FlipFlop
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using ParadoxNotion.FlowCanvas.Module;
using System;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Flow Controllers/Togglers")]
  [Description("Flip Flops between the 2 outputs each time In is called")]
  [ContextDefinedOutputs(new Type[] {typeof (bool)})]
  public class FlipFlop : FlowControlNode
  {
    public bool isFlip = true;
    private bool original;

    public override string name => base.name + " " + (this.isFlip ? "[FLIP]" : "[FLOP]");

    public override void OnGraphStarted() => this.original = this.isFlip;

    public override void OnGraphStoped() => this.isFlip = this.original;

    protected override void RegisterPorts()
    {
      FlowOutput flipF = this.AddFlowOutput("Flip");
      FlowOutput flopF = this.AddFlowOutput("Flop");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        this.Call(this.isFlip ? flipF : flopF);
        this.isFlip = !this.isFlip;
      }));
      this.AddValueOutput<bool>("Is Flip", (ValueHandler<bool>) (() => this.isFlip));
    }
  }
}
