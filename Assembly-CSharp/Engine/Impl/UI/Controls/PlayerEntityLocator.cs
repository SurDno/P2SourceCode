using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;

namespace Engine.Impl.UI.Controls
{
  public class PlayerEntityLocator : EngineDependent
  {
    [SerializeField]
    private EntityView view;
    [FromLocator]
    private Simulation simulation;

    protected override void OnConnectToEngine()
    {
      SetPlayer(simulation.Player);
      simulation.OnPlayerChanged += SetPlayer;
    }

    protected override void OnDisconnectFromEngine()
    {
      simulation.OnPlayerChanged -= SetPlayer;
      SetPlayer(null);
    }

    private void SetPlayer(IEntity entity)
    {
      if (!((UnityEngine.Object) view != (UnityEngine.Object) null))
        return;
      view.Value = entity;
    }
  }
}
