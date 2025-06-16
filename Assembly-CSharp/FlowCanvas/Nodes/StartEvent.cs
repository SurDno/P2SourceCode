using ParadoxNotion.Design;
using System.Collections;

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
