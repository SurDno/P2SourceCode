namespace Engine.Impl.UI.Controls
{
  public class HideableEventView : EventView
  {
    [SerializeField]
    private HideableView hideableView;
    [SerializeField]
    private float length = 1f;

    public void Hide() => hideableView.Visible = false;

    public void Show() => hideableView.Visible = true;

    public override void Invoke()
    {
      Show();
      this.CancelInvoke("Hide");
      this.Invoke("Hide", length);
    }

    private void OnDisable()
    {
      this.CancelInvoke("Hide");
      Hide();
    }
  }
}
