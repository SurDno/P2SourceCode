using System.Collections;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class HideableViewEvents : HideableView
  {
    [SerializeField]
    private EventView trueEvent;
    [SerializeField]
    private EventView falseEvent;
    private Coroutine coroutine = (Coroutine) null;

    protected override void ApplyVisibility()
    {
      this.Cancel();
      if (!this.gameObject.activeInHierarchy)
        return;
      this.coroutine = this.StartCoroutine(this.InvokeEvent(this.Visible));
    }

    private void Cancel()
    {
      if (this.coroutine == null)
        return;
      this.StopCoroutine(this.coroutine);
    }

    private IEnumerator InvokeEvent(bool value)
    {
      yield return (object) new WaitForEndOfFrame();
      if (value)
        this.trueEvent?.Invoke();
      else
        this.falseEvent?.Invoke();
    }

    public override void SkipAnimation() => this.Cancel();
  }
}
