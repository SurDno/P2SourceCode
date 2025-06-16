// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.StartEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using System.Collections;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("On Start")]
  [Category("Events/Graph")]
  [Description("Called only once and the first time the Graph is enabled.\nThis is called 1 frame after all Awake events are called.")]
  public class StartEvent : EventNode
  {
    private FlowOutput start;
    private bool called = false;

    public override void OnGraphStarted()
    {
      if (this.called)
        return;
      this.called = true;
      this.StartCoroutine(this.DelayCall());
    }

    private IEnumerator DelayCall()
    {
      yield return (object) null;
      this.start.Call();
    }

    protected override void RegisterPorts() => this.start = this.AddFlowOutput("Once");
  }
}
