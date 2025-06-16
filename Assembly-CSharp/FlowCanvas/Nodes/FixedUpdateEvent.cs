// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.FixedUpdateEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using ParadoxNotion.Services;
using System;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("On Fixed Update")]
  [Category("Events/Graph")]
  [Description("Called every fixed framerate frame, which should be used when dealing with Physics")]
  public class FixedUpdateEvent : EventNode
  {
    private FlowOutput fixedUpdate;

    protected override void RegisterPorts() => this.fixedUpdate = this.AddFlowOutput("Out");

    public override void OnGraphStarted()
    {
      BlueprintManager.current.onFixedUpdate += new Action(this.FixedUpdate);
    }

    public override void OnGraphStoped()
    {
      BlueprintManager.current.onFixedUpdate -= new Action(this.FixedUpdate);
    }

    private void FixedUpdate() => this.fixedUpdate.Call();
  }
}
