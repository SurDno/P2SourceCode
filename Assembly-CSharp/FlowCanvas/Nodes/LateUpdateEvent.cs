// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.LateUpdateEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using ParadoxNotion.Services;
using System;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("On Late Update")]
  [Category("Events/Graph")]
  [Description("Called per-frame, but after normal Update")]
  public class LateUpdateEvent : EventNode
  {
    private FlowOutput lateUpdate;

    protected override void RegisterPorts() => this.lateUpdate = this.AddFlowOutput("Out");

    public override void OnGraphStarted()
    {
      BlueprintManager.current.onLateUpdate += new Action(this.LateUpdate);
    }

    public override void OnGraphStoped()
    {
      BlueprintManager.current.onLateUpdate -= new Action(this.LateUpdate);
    }

    private void LateUpdate() => this.lateUpdate.Call();
  }
}
