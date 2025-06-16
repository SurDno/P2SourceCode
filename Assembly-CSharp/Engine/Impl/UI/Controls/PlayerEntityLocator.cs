// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.PlayerEntityLocator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using System;
using UnityEngine;

#nullable disable
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
