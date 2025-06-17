using System.Collections;
using System.Collections.Generic;
using NodeCanvas;
using ParadoxNotion.Services;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  public abstract class LatentActionNodeBase : SimplexNode
  {
    private FlowOutput outFlow;
    private FlowOutput doing;
    private FlowOutput done;
    private Queue<IEnumerator> queue = new();
    private Coroutine currentCoroutine;
    private bool graphPaused;

    public override string name => queue.Count > 0 ? string.Format("{0} [{1}]", base.name, queue.Count.ToString()) : base.name;

    public override sealed void OnGraphStarted()
    {
      queue = new Queue<IEnumerator>();
      currentCoroutine = null;
    }

    public override sealed void OnGraphStoped() => Break();

    public override sealed void OnGraphPaused() => graphPaused = true;

    public override sealed void OnGraphUnpaused() => graphPaused = false;

    protected void Begin(IEnumerator enumerator)
    {
      if (!queue.Contains(enumerator))
        queue.Enqueue(enumerator);
      if (currentCoroutine != null)
        return;
      currentCoroutine = BlueprintManager.current.StartCoroutine(InternalCoroutine(enumerator));
    }

    protected void Break()
    {
      if (currentCoroutine == null)
        return;
      BlueprintManager.current.StopCoroutine(currentCoroutine);
      queue = new Queue<IEnumerator>();
      currentCoroutine = null;
      outFlow.parent.SetStatus(Status.Resting);
      OnBreak();
      done.Call();
    }

    private IEnumerator InternalCoroutine(IEnumerator enumerator)
    {
      FlowNode parentNode = outFlow.parent;
      parentNode.SetStatus(Status.Running);
      outFlow.Call();
      while (enumerator.MoveNext())
      {
        while (graphPaused)
          yield return null;
        doing.Call();
        yield return enumerator.Current;
      }
      parentNode.SetStatus(Status.Resting);
      done.Call();
      currentCoroutine = null;
      if (queue.Count > 0)
      {
        queue.Dequeue();
        if (queue.Count > 0)
          Begin(queue.Peek());
      }
    }

    protected override void OnRegisterPorts(FlowNode node)
    {
      outFlow = node.AddFlowOutput("Start", "Out");
      doing = node.AddFlowOutput("Update", "Doing");
      done = node.AddFlowOutput("Finish", "Done");
    }

    public virtual void OnBreak()
    {
    }
  }
}
