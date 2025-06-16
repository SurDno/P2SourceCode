using System.Collections;

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
      Cancel();
      if (!this.gameObject.activeInHierarchy)
        return;
      coroutine = this.StartCoroutine(InvokeEvent(Visible));
    }

    private void Cancel()
    {
      if (coroutine == null)
        return;
      this.StopCoroutine(coroutine);
    }

    private IEnumerator InvokeEvent(bool value)
    {
      yield return (object) new WaitForEndOfFrame();
      if (value)
        trueEvent?.Invoke();
      else
        falseEvent?.Invoke();
    }

    public override void SkipAnimation() => Cancel();
  }
}
