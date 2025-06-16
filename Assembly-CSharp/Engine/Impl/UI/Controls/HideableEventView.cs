using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class HideableEventView : EventView
  {
    [SerializeField]
    private HideableView hideableView;
    [SerializeField]
    private float length = 1f;

    public void Hide() => this.hideableView.Visible = false;

    public void Show() => this.hideableView.Visible = true;

    public override void Invoke()
    {
      this.Show();
      this.CancelInvoke("Hide");
      this.Invoke("Hide", this.length);
    }

    private void OnDisable()
    {
      this.CancelInvoke("Hide");
      this.Hide();
    }
  }
}
