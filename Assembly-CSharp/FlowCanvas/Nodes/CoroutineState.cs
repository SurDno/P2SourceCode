using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Name("Coroutine")]
  [Category("Flow Controllers/Repeaters")]
  [Description("Start a Coroutine that will repeat until Break is signaled")]
  public class CoroutineState : FlowControlNode
  {
    private bool activated = false;
    private Coroutine coroutine = (Coroutine) null;

    public override void OnGraphStoped()
    {
      if (this.coroutine == null)
        return;
      this.StopCoroutine(this.coroutine);
      this.activated = false;
    }

    protected override void RegisterPorts()
    {
      FlowOutput fStarted = this.AddFlowOutput("Start");
      FlowOutput fUpdate = this.AddFlowOutput("Update");
      FlowOutput fFinish = this.AddFlowOutput("Finish");
      this.AddFlowInput("Start", (FlowHandler) (() =>
      {
        if (this.activated)
          return;
        this.activated = true;
        this.coroutine = this.StartCoroutine(this.DoRepeat(fStarted, fUpdate, fFinish));
      }));
      this.AddFlowInput("Break", (FlowHandler) (() => this.activated = false));
    }

    private IEnumerator DoRepeat(FlowOutput fStarted, FlowOutput fUpdate, FlowOutput fFinish)
    {
      fStarted.Call();
      while (this.activated)
      {
        while (this.graph.isPaused)
          yield return (object) null;
        fUpdate.Call();
        yield return (object) null;
      }
      fFinish.Call();
    }
  }
}
