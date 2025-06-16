using Engine.Impl.UI.Controls;
using UnityEngine;

public class EnableOnEngineInitialized : EngineDependent
{
  [SerializeField]
  private HideableView view;

  protected override void OnConnectToEngine()
  {
    if (!(view != null))
      return;
    view.Visible = true;
  }

  protected override void OnDisconnectFromEngine()
  {
  }
}
