using Engine.Impl.UI.Controls;
using UnityEngine;

public class EnableOnEngineInitialized : EngineDependent
{
  [SerializeField]
  private HideableView view;

  protected override void OnConnectToEngine()
  {
    if (!((Object) this.view != (Object) null))
      return;
    this.view.Visible = true;
  }

  protected override void OnDisconnectFromEngine()
  {
  }
}
