using System;
using Engine.Common;
using Engine.Common.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Connections;
using UnityEngine;

public class CraftBrewingSlotAnchor : MonoBehaviour
{
  [SerializeField]
  private IEntitySerializable containerTemplate;
  [SerializeField]
  private float durabilityThreshold;
  private CraftBrewingSlot slot;

  public event Action<IInventoryComponent> CraftEvent;

  public event Action<IStorableComponent> TakeEvent;

  public CraftBrewingSlot Slot => slot;

  private void FireCraftEvent(IInventoryComponent container)
  {
    Action<IInventoryComponent> craftEvent = CraftEvent;
    if (craftEvent == null)
      return;
    craftEvent(container);
  }

  private void FireTakeEvent(IStorableComponent item)
  {
    Action<IStorableComponent> takeEvent = TakeEvent;
    if (takeEvent == null)
      return;
    takeEvent(item);
  }

  public void Initialize(CraftBrewingSlot slotPrefab)
  {
    slot = Instantiate(slotPrefab, transform, false);
    slot.Initialize(durabilityThreshold);
    slot.CraftEvent += FireCraftEvent;
    slot.TakeEvent += FireTakeEvent;
  }

  public void SetTarget(IEntity target)
  {
    if (target == null)
    {
      slot.SetTarget(null, null);
    }
    else
    {
      IStorageComponent component = target.GetComponent<IStorageComponent>();
      IInventoryComponent containerByTemplate = component != null ? StorageUtility.GetContainerByTemplate(component, containerTemplate.Value) : null;
      slot.SetTarget(target, containerByTemplate);
    }
  }
}
