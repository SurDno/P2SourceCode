using System.Collections;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
  public class TaskCoroutine
  {
    private IEnumerator mCoroutineEnumerator;
    private Coroutine mCoroutine;
    private BehaviorTree mParent;
    private string mCoroutineName;
    private bool mStop = false;

    public Coroutine Coroutine => this.mCoroutine;

    public void Stop() => this.mStop = true;

    public TaskCoroutine(BehaviorTree parent, IEnumerator coroutine, string coroutineName)
    {
      this.mParent = parent;
      this.mCoroutineEnumerator = coroutine;
      this.mCoroutineName = coroutineName;
      this.mCoroutine = parent.StartCoroutine(this.RunCoroutine());
    }

    public IEnumerator RunCoroutine()
    {
      while (!this.mStop && this.mCoroutineEnumerator != null && this.mCoroutineEnumerator.MoveNext())
        yield return this.mCoroutineEnumerator.Current;
      this.mParent.TaskCoroutineEnded(this, this.mCoroutineName);
    }
  }
}
