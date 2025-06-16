// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.WeaponVisibleListener
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using Engine.Source.Components;
using System;

#nullable disable
namespace Engine.Source.Services
{
  [GameService(new Type[] {typeof (WeaponVisibleListener)})]
  public class WeaponVisibleListener : IInitialisable
  {
    private ControllerComponent controller;

    public event Action<WeaponKind, bool> OnWeaponVisibleChanged;

    public void Initialise()
    {
      ServiceLocator.GetService<Simulation>().OnPlayerChanged += new Action<IEntity>(this.OnPlayerChanged);
      this.OnPlayerChanged(ServiceLocator.GetService<ISimulation>().Player);
    }

    public void Terminate()
    {
      ServiceLocator.GetService<Simulation>().OnPlayerChanged -= new Action<IEntity>(this.OnPlayerChanged);
    }

    private void OnPlayerChanged(IEntity player)
    {
      if (this.controller != null)
      {
        this.controller.OnWeaponEnableChanged -= new Action<WeaponKind, bool>(this.OnWeaponEnableChanged);
        this.controller = (ControllerComponent) null;
      }
      if (player != null)
        this.controller = player.GetComponent<ControllerComponent>();
      if (this.controller == null)
        return;
      this.controller.OnWeaponEnableChanged += new Action<WeaponKind, bool>(this.OnWeaponEnableChanged);
    }

    private void OnWeaponEnableChanged(WeaponKind kind, bool value)
    {
      Action<WeaponKind, bool> weaponVisibleChanged = this.OnWeaponVisibleChanged;
      if (weaponVisibleChanged == null)
        return;
      weaponVisibleChanged(kind, value);
    }
  }
}
