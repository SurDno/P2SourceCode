using System.Collections;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Name("While True")]
  [Category("Flow Controllers/Repeaters")]
  [Description("Once called, will continuously call 'Do' while the input boolean condition is true. Once condition becomes or is false, 'Done' is called")]
  [ContextDefinedInputs(typeof (bool))]
  public class While : FlowControlNode
  {
    private Coroutine coroutine;

    public override void OnGraphStarted() => coroutine = (Coroutine) null;

    public override void OnGraphStoped()
    {
      if (coroutine == null)
        return;
      StopCoroutine(coroutine);
      coroutine = (Coroutine) null;
    }

    protected override void RegisterPorts()
    {
      ValueInput<bool> c = AddValueInput<bool>("Condition");
      FlowOutput fCurrent = AddFlowOutput("Do");
      FlowOutput fFinish = AddFlowOutput("Done");
      AddFlowInput("In", () =>
      {
        if (coroutine != null)
          return;
        coroutine = StartCoroutine(DoWhile(fCurrent, fFinish, c));
      });
    }

    private IEnumerator DoWhile(
      FlowOutput fCurrent,
      FlowOutput fFinish,
      ValueInput<bool> condition)
    {
      while (condition.value)
      {
        while (graph.isPaused)
          yield return null;
        fCurrent.Call();
        yield return null;
      }
      coroutine = (Coroutine) null;
      fFinish.Call();
    }
  }
}
