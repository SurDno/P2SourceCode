// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.EnableEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("On Enable")]
  [Category("Events/Graph")]
  [Description("Called when the Graph is enabled")]
  public class EnableEvent : EventNode
  {
    private FlowOutput enable;

    public override void OnGraphStarted() => this.enable.Call();

    protected override void RegisterPorts() => this.enable = this.AddFlowOutput("Out");
  }
}
