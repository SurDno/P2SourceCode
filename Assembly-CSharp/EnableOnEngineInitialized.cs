using Engine.Impl.UI.Controls;

public class EnableOnEngineInitialized : EngineDependent
{
  [SerializeField]
  private HideableView view;

  protected override void OnConnectToEngine()
  {
    if (!((Object) view != (Object) null))
      return;
    view.Visible = true;
  }

  protected override void OnDisconnectFromEngine()
  {
  }
}
