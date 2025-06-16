// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.TaskCoroutine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

#nullable disable
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
