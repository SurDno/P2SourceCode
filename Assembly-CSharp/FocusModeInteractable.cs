// Decompiled with JetBrains decompiler
// Type: FocusModeInteractable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Interactable;
using Engine.Source.Services;
using Inspectors;
using System;
using UnityEngine;

#nullable disable
[DisallowMultipleComponent]
public class FocusModeInteractable : MonoBehaviour, IEntityAttachable
{
  [Inspected]
  private IEntity owner;
  [Inspected]
  private InteractableComponent interactableComponent;
  private FocusEffect effect;
  private bool interactionAvailable = true;
  private bool compassEnabled = true;
  private bool visible = true;

  void IEntityAttachable.Attach(IEntity owner) => this.SetOwner(owner);

  void IEntityAttachable.Detach() => this.SetOwner((IEntity) null);

  private void OnEnable()
  {
    QuestCompassService service = ServiceLocator.GetService<QuestCompassService>();
    if (service == null)
      return;
    this.OnEnableChanged(service.IsEnabled);
    service.OnEnableChanged += new Action<bool>(this.OnEnableChanged);
  }

  private void OnDisable()
  {
    QuestCompassService service = ServiceLocator.GetService<QuestCompassService>();
    if (service == null)
      return;
    service.OnEnableChanged -= new Action<bool>(this.OnEnableChanged);
    this.OnEnableChanged(false);
  }

  private void SetOwner(IEntity value)
  {
    if (this.owner == value)
      return;
    this.owner = value;
    this.SetInteractableComponent(this.owner?.GetComponent<InteractableComponent>());
  }

  private void SetInteractableComponent(InteractableComponent value)
  {
    if (this.interactableComponent == value)
      return;
    this.interactableComponent = value;
    this.CheckInteraction();
  }

  private void CheckInteraction()
  {
    if (this.interactableComponent == null || !this.interactableComponent.IsEnabled || !this.interactableComponent.Owner.IsEnabledInHierarchy)
    {
      this.SetInteractionAvailable(false);
    }
    else
    {
      IEntity player = ServiceLocator.GetService<ISimulation>()?.Player;
      if (player == null)
      {
        this.SetInteractionAvailable(false);
      }
      else
      {
        bool flag = false;
        foreach (InteractItemInfo validateItem in this.interactableComponent.GetValidateItems(player))
        {
          if (!validateItem.Invalid && !(validateItem.Item.Blueprint.Id == Guid.Empty))
          {
            flag = true;
            break;
          }
        }
        this.SetInteractionAvailable(flag);
      }
    }
  }

  private void SetInteractionAvailable(bool value)
  {
    if (this.interactionAvailable == value)
      return;
    this.interactionAvailable = value;
    this.UpdateVisibility();
  }

  private void OnEnableChanged(bool value)
  {
    if (this.compassEnabled == value)
      return;
    this.compassEnabled = value;
    if (value)
      this.CheckInteraction();
    this.UpdateVisibility();
  }

  private void SetVisibility(bool value)
  {
    if (this.visible == value)
      return;
    this.visible = value;
    if ((UnityEngine.Object) this.effect == (UnityEngine.Object) null)
    {
      this.effect = this.GetComponent<FocusEffect>();
      if (this.visible)
      {
        if (!((UnityEngine.Object) this.effect == (UnityEngine.Object) null))
          return;
        this.effect = this.gameObject.AddComponent<FocusEffect>();
      }
      else
      {
        if (!((UnityEngine.Object) this.effect != (UnityEngine.Object) null))
          return;
        this.effect.SetActive(false);
      }
    }
    else
      this.effect.SetActive(this.visible);
  }

  private void UpdateVisibility()
  {
    this.SetVisibility(this.interactionAvailable && this.compassEnabled);
  }
}
