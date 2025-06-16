// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.UpdateEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using NodeCanvas.Framework;
using ParadoxNotion.Design;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("On Update")]
  [Category("Events/Graph")]
  [Description("Called per-frame")]
  public class UpdateEvent : EventNode, IUpdatable
  {
    private FlowOutput update;

    protected override void RegisterPorts() => this.update = this.AddFlowOutput("Out");

    public void Update() => this.update.Call();
  }
}
