// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ConstructionEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("On Awake")]
  [Category("Events/Graph")]
  [Description("Called only once and the first time the Graph is enabled.\nUse this only for initialization of this graph.")]
  public class ConstructionEvent : EventNode
  {
    private FlowOutput awake;
    private bool called = false;

    public override void OnGraphStarted()
    {
      if (this.called)
        return;
      this.called = true;
      this.awake.Call();
    }

    protected override void RegisterPorts() => this.awake = this.AddFlowOutput("Once");
  }
}
