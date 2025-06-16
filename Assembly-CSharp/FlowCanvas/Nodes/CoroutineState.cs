using System.Collections;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Name("Coroutine")]
  [Category("Flow Controllers/Repeaters")]
  [Description("Start a Coroutine that will repeat until Break is signaled")]
  public class CoroutineState : FlowControlNode
  {
    private bool activated;
    private Coroutine coroutine = (Coroutine) null;

    public override void OnGraphStoped()
    {
      if (coroutine == null)
        return;
      StopCoroutine(coroutine);
      activated = false;
    }

    protected override void RegisterPorts()
    {
      FlowOutput fStarted = AddFlowOutput("Start");
      FlowOutput fUpdate = AddFlowOutput("Update");
      FlowOutput fFinish = AddFlowOutput("Finish");
      AddFlowInput("Start", () =>
      {
        if (activated)
          return;
        activated = true;
        coroutine = StartCoroutine(DoRepeat(fStarted, fUpdate, fFinish));
      });
      AddFlowInput("Break", () => activated = false);
    }

    private IEnumerator DoRepeat(FlowOutput fStarted, FlowOutput fUpdate, FlowOutput fFinish)
    {
      fStarted.Call();
      while (activated)
      {
        while (graph.isPaused)
          yield return null;
        fUpdate.Call();
        yield return null;
      }
      fFinish.Call();
    }
  }
}
