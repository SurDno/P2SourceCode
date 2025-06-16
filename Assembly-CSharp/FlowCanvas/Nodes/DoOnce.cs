// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.DoOnce
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Category("Flow Controllers/Filters")]
  [Description("Filters Out to be called only once. After the first call, Out is no longer called until Reset is called")]
  public class DoOnce : FlowControlNode
  {
    private bool called;

    public override void OnGraphStarted() => this.called = false;

    protected override void RegisterPorts()
    {
      FlowOutput o = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        if (this.called)
          return;
        this.called = true;
        o.Call();
      }));
      this.AddFlowInput("Reset", (FlowHandler) (() => this.called = false));
    }
  }
}
