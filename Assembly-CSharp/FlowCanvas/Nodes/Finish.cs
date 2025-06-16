// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.Finish
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Description("Stops and cease execution of the FlowSript")]
  public class Finish : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      ValueInput<bool> c = this.AddValueInput<bool>("Success");
      this.AddFlowInput("In", (FlowHandler) (() => this.graph.Stop(c.value)));
    }
  }
}
