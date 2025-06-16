namespace Engine.Impl.UI.Controls
{
  public class ConfirmationWindowEventView : EventView
  {
    [SerializeField]
    private ConfirmationWindow prefab;
    [SerializeField]
    private Transform layout;
    [SerializeField]
    private EventView acceptAction;
    [SerializeField]
    private EventView cancelAction;
    [SerializeField]
    private string text;
    private ConfirmationWindow window;

    public override void Invoke()
    {
      if ((UnityEngine.Object) window == (UnityEngine.Object) null)
        window = UnityEngine.Object.Instantiate<ConfirmationWindow>(prefab, (bool) (UnityEngine.Object) layout ? layout : this.transform, false);
      window.Show(text, OnAccept, OnCancel);
    }

    private void OnAccept() => acceptAction?.Invoke();

    private void OnCancel() => cancelAction?.Invoke();
  }
}
