using System;
using UnityEngine;

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
      if ((UnityEngine.Object) this.window == (UnityEngine.Object) null)
        this.window = UnityEngine.Object.Instantiate<ConfirmationWindow>(this.prefab, (bool) (UnityEngine.Object) this.layout ? this.layout : this.transform, false);
      this.window.Show(this.text, new Action(this.OnAccept), new Action(this.OnCancel));
    }

    private void OnAccept() => this.acceptAction?.Invoke();

    private void OnCancel() => this.cancelAction?.Invoke();
  }
}
