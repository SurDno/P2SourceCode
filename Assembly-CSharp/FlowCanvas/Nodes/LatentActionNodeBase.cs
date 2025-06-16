using ParadoxNotion.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  public abstract class LatentActionNodeBase : SimplexNode
  {
    private FlowOutput outFlow;
    private FlowOutput doing;
    private FlowOutput done;
    private Queue<IEnumerator> queue = new Queue<IEnumerator>();
    private Coroutine currentCoroutine;
    private bool graphPaused;

    public override string name
    {
      get
      {
        return this.queue.Count > 0 ? string.Format("{0} [{1}]", (object) base.name, (object) this.queue.Count.ToString()) : base.name;
      }
    }

    public override sealed void OnGraphStarted()
    {
      this.queue = new Queue<IEnumerator>();
      this.currentCoroutine = (Coroutine) null;
    }

    public override sealed void OnGraphStoped() => this.Break();

    public override sealed void OnGraphPaused() => this.graphPaused = true;

    public override sealed void OnGraphUnpaused() => this.graphPaused = false;

    protected void Begin(IEnumerator enumerator)
    {
      if (!this.queue.Contains(enumerator))
        this.queue.Enqueue(enumerator);
      if (this.currentCoroutine != null)
        return;
      this.currentCoroutine = BlueprintManager.current.StartCoroutine(this.InternalCoroutine(enumerator));
    }

    protected void Break()
    {
      if (this.currentCoroutine == null)
        return;
      BlueprintManager.current.StopCoroutine(this.currentCoroutine);
      this.queue = new Queue<IEnumerator>();
      this.currentCoroutine = (Coroutine) null;
      this.outFlow.parent.SetStatus(NodeCanvas.Status.Resting);
      this.OnBreak();
      this.done.Call();
    }

    private IEnumerator InternalCoroutine(IEnumerator enumerator)
    {
      FlowNode parentNode = this.outFlow.parent;
      parentNode.SetStatus(NodeCanvas.Status.Running);
      this.outFlow.Call();
      while (enumerator.MoveNext())
      {
        while (this.graphPaused)
          yield return (object) null;
        this.doing.Call();
        yield return enumerator.Current;
      }
      parentNode.SetStatus(NodeCanvas.Status.Resting);
      this.done.Call();
      this.currentCoroutine = (Coroutine) null;
      if (this.queue.Count > 0)
      {
        this.queue.Dequeue();
        if (this.queue.Count > 0)
          this.Begin(this.queue.Peek());
      }
    }

    protected override void OnRegisterPorts(FlowNode node)
    {
      this.outFlow = node.AddFlowOutput("Start", "Out");
      this.doing = node.AddFlowOutput("Update", "Doing");
      this.done = node.AddFlowOutput("Finish", "Done");
    }

    public virtual void OnBreak()
    {
    }
  }
}
