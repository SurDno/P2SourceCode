// Decompiled with JetBrains decompiler
// Type: DisableInIsolatedIndoor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Components;
using Engine.Source.Services;
using Inspectors;
using System;
using UnityEngine;

#nullable disable
public class DisableInIsolatedIndoor : EngineDependent
{
  [Inspected]
  private bool insideIndoor;
  [Inspected]
  private bool isolatedIndoor;
  private bool connected;

  protected override void OnConnectToEngine()
  {
    if (this.connected)
      return;
    this.insideIndoor = ServiceLocator.GetService<InsideIndoorListener>().InsideIndoor;
    this.isolatedIndoor = ServiceLocator.GetService<InsideIndoorListener>().IsolatedIndoor;
    this.Apply();
    ServiceLocator.GetService<InsideIndoorListener>().OnInsideIndoorChanged += new Action<bool>(this.OnInsideIndoorChanged);
    ServiceLocator.GetService<InsideIndoorListener>().OnPlayerBeginsExit += new Action(this.OnPlayerBeginsExit);
  }

  protected override void OnDisconnectFromEngine()
  {
    ServiceLocator.GetService<InsideIndoorListener>().OnInsideIndoorChanged -= new Action<bool>(this.OnInsideIndoorChanged);
    ServiceLocator.GetService<InsideIndoorListener>().OnPlayerBeginsExit -= new Action(this.OnPlayerBeginsExit);
    this.connected = false;
  }

  private void OnPlayerBeginsExit()
  {
    if (this.insideIndoor)
    {
      this.insideIndoor = false;
      this.isolatedIndoor = false;
    }
    this.Apply();
  }

  private void OnInsideIndoorChanged(bool inside)
  {
    this.insideIndoor = false;
    this.isolatedIndoor = false;
    IEntity player = ServiceLocator.GetService<ISimulation>().Player;
    if (player != null)
    {
      LocationItemComponent component = player.GetComponent<LocationItemComponent>();
      if (component != null)
      {
        this.insideIndoor = component.IsIndoor;
        IBuildingComponent building = player.GetComponent<NavigationComponent>().Building;
        if (building != null)
          this.isolatedIndoor = ScriptableObjectInstance<IndoorSettingsData>.Instance.IsIndoorIsolated(building.Building);
      }
      else
        Debug.LogError((object) ("LocationItemComponent not found, owner : " + player.GetInfo()));
    }
    this.Apply();
  }

  private void Apply() => this.gameObject.SetActive(!this.isolatedIndoor);

  protected override void OnDisable()
  {
  }

  private void OnDestroy() => base.OnDisable();
}
