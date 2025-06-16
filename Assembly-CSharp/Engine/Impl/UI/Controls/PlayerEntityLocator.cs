using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using System;
using UnityEngine;

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
      this.SetPlayer(this.simulation.Player);
      this.simulation.OnPlayerChanged += new Action<IEntity>(this.SetPlayer);
    }

    protected override void OnDisconnectFromEngine()
    {
      this.simulation.OnPlayerChanged -= new Action<IEntity>(this.SetPlayer);
      this.SetPlayer((IEntity) null);
    }

    private void SetPlayer(IEntity entity)
    {
      if (!((UnityEngine.Object) this.view != (UnityEngine.Object) null))
        return;
      this.view.Value = entity;
    }
  }
}
