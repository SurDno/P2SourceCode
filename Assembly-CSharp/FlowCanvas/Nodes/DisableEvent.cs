// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.DisableEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("On Disable")]
  [Category("Events/Graph")]
  [Description("Called when the Graph is Disabled")]
  public class DisableEvent : EventNode
  {
    private FlowOutput disable;

    public override void OnGraphStoped() => this.disable.Call();

    protected override void RegisterPorts() => this.disable = this.AddFlowOutput("Out");
  }
}
