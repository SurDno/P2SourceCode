using ParadoxNotion.Design;
using ParadoxNotion.FlowCanvas.Module;
using System.Collections;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Category("Flow Controllers/Filters")]
  [Description("Filters OUT so that it can't be called very frequently")]
  [ContextDefinedInputs(new System.Type[] {typeof (float)})]
  [ContextDefinedOutputs(new System.Type[] {typeof (float)})]
  public class Cooldown : FlowControlNode
  {
    private float current = 0.0f;
    private Coroutine coroutine;

    public override string name
    {
      get => base.name + string.Format(" [{0}]", (object) this.current.ToString("0.0"));
    }

    public override void OnGraphStarted()
    {
      this.current = 0.0f;
      this.coroutine = (Coroutine) null;
    }

    public override void OnGraphStoped()
    {
      if (this.coroutine == null)
        return;
      this.StopCoroutine(this.coroutine);
      this.coroutine = (Coroutine) null;
      this.current = 0.0f;
    }

    protected override void RegisterPorts()
    {
      FlowOutput o = this.AddFlowOutput("Out");
      FlowOutput ready = this.AddFlowOutput("Ready");
      ValueInput<float> time = this.AddValueInput<float>("Time");
      this.AddValueOutput<float>("Current", (ValueHandler<float>) (() => Mathf.Max(this.current, 0.0f)));
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        if ((double) this.current > 0.0 || this.coroutine != null)
          return;
        this.current = time.value;
        this.coroutine = this.StartCoroutine(this.CountDown(ready));
        o.Call();
      }));
      this.AddFlowInput("Cancel", (FlowHandler) (() =>
      {
        if (this.coroutine == null)
          return;
        this.StopCoroutine(this.coroutine);
        this.coroutine = (Coroutine) null;
        this.current = 0.0f;
      }));
    }

    private IEnumerator CountDown(FlowOutput ready)
    {
      while ((double) this.current > 0.0)
      {
        while (this.graph.isPaused)
          yield return (object) null;
        this.current -= Time.deltaTime;
        yield return (object) null;
      }
      this.coroutine = (Coroutine) null;
      ready.Call();
    }
  }
}
