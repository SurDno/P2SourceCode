using System.Collections;
using ParadoxNotion.Design;
using ParadoxNotion.FlowCanvas.Module;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Category("Flow Controllers/Filters")]
  [Description("Filters OUT so that it can't be called very frequently")]
  [ContextDefinedInputs(typeof (float))]
  [ContextDefinedOutputs(typeof (float))]
  public class Cooldown : FlowControlNode
  {
    private float current;
    private Coroutine coroutine;

    public override string name
    {
      get => base.name + string.Format(" [{0}]", current.ToString("0.0"));
    }

    public override void OnGraphStarted()
    {
      current = 0.0f;
      coroutine = null;
    }

    public override void OnGraphStoped()
    {
      if (coroutine == null)
        return;
      StopCoroutine(coroutine);
      coroutine = null;
      current = 0.0f;
    }

    protected override void RegisterPorts()
    {
      FlowOutput o = AddFlowOutput("Out");
      FlowOutput ready = AddFlowOutput("Ready");
      ValueInput<float> time = AddValueInput<float>("Time");
      AddValueOutput("Current", () => Mathf.Max(current, 0.0f));
      AddFlowInput("In", () =>
      {
        if (current > 0.0 || coroutine != null)
          return;
        current = time.value;
        coroutine = StartCoroutine(CountDown(ready));
        o.Call();
      });
      AddFlowInput("Cancel", () =>
      {
        if (coroutine == null)
          return;
        StopCoroutine(coroutine);
        coroutine = null;
        current = 0.0f;
      });
    }

    private IEnumerator CountDown(FlowOutput ready)
    {
      while (current > 0.0)
      {
        while (graph.isPaused)
          yield return null;
        current -= Time.deltaTime;
        yield return null;
      }
      coroutine = null;
      ready.Call();
    }
  }
}
