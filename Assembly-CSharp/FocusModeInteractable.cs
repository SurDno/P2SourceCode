using System;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Interactable;
using Engine.Source.Services;
using Inspectors;

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

  void IEntityAttachable.Attach(IEntity owner) => SetOwner(owner);

  void IEntityAttachable.Detach() => SetOwner(null);

  private void OnEnable()
  {
    QuestCompassService service = ServiceLocator.GetService<QuestCompassService>();
    if (service == null)
      return;
    OnEnableChanged(service.IsEnabled);
    service.OnEnableChanged += OnEnableChanged;
  }

  private void OnDisable()
  {
    QuestCompassService service = ServiceLocator.GetService<QuestCompassService>();
    if (service == null)
      return;
    service.OnEnableChanged -= OnEnableChanged;
    OnEnableChanged(false);
  }

  private void SetOwner(IEntity value)
  {
    if (owner == value)
      return;
    owner = value;
    SetInteractableComponent(owner?.GetComponent<InteractableComponent>());
  }

  private void SetInteractableComponent(InteractableComponent value)
  {
    if (interactableComponent == value)
      return;
    interactableComponent = value;
    CheckInteraction();
  }

  private void CheckInteraction()
  {
    if (interactableComponent == null || !interactableComponent.IsEnabled || !interactableComponent.Owner.IsEnabledInHierarchy)
    {
      SetInteractionAvailable(false);
    }
    else
    {
      IEntity player = ServiceLocator.GetService<ISimulation>()?.Player;
      if (player == null)
      {
        SetInteractionAvailable(false);
      }
      else
      {
        bool flag = false;
        foreach (InteractItemInfo validateItem in interactableComponent.GetValidateItems(player))
        {
          if (!validateItem.Invalid && !(validateItem.Item.Blueprint.Id == Guid.Empty))
          {
            flag = true;
            break;
          }
        }
        SetInteractionAvailable(flag);
      }
    }
  }

  private void SetInteractionAvailable(bool value)
  {
    if (interactionAvailable == value)
      return;
    interactionAvailable = value;
    UpdateVisibility();
  }

  private void OnEnableChanged(bool value)
  {
    if (compassEnabled == value)
      return;
    compassEnabled = value;
    if (value)
      CheckInteraction();
    UpdateVisibility();
  }

  private void SetVisibility(bool value)
  {
    if (visible == value)
      return;
    visible = value;
    if ((UnityEngine.Object) effect == (UnityEngine.Object) null)
    {
      effect = this.GetComponent<FocusEffect>();
      if (visible)
      {
        if (!((UnityEngine.Object) effect == (UnityEngine.Object) null))
          return;
        effect = this.gameObject.AddComponent<FocusEffect>();
      }
      else
      {
        if (!((UnityEngine.Object) effect != (UnityEngine.Object) null))
          return;
        effect.SetActive(false);
      }
    }
    else
      effect.SetActive(visible);
  }

  private void UpdateVisibility()
  {
    SetVisibility(interactionAvailable && compassEnabled);
  }
}
