// Decompiled with JetBrains decompiler
// Type: CraftBrewingSlotAnchor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Connections;
using System;
using UnityEngine;

#nullable disable
public class CraftBrewingSlotAnchor : MonoBehaviour
{
  [SerializeField]
  private IEntitySerializable containerTemplate;
  [SerializeField]
  private float durabilityThreshold;
  private CraftBrewingSlot slot;

  public event Action<IInventoryComponent> CraftEvent;

  public event Action<IStorableComponent> TakeEvent;

  public CraftBrewingSlot Slot => this.slot;

  private void FireCraftEvent(IInventoryComponent container)
  {
    Action<IInventoryComponent> craftEvent = this.CraftEvent;
    if (craftEvent == null)
      return;
    craftEvent(container);
  }

  private void FireTakeEvent(IStorableComponent item)
  {
    Action<IStorableComponent> takeEvent = this.TakeEvent;
    if (takeEvent == null)
      return;
    takeEvent(item);
  }

  public void Initialize(CraftBrewingSlot slotPrefab)
  {
    this.slot = UnityEngine.Object.Instantiate<CraftBrewingSlot>(slotPrefab, this.transform, false);
    this.slot.Initialize(this.durabilityThreshold);
    this.slot.CraftEvent += new Action<IInventoryComponent>(this.FireCraftEvent);
    this.slot.TakeEvent += new Action<IStorableComponent>(this.FireTakeEvent);
  }

  public void SetTarget(IEntity target)
  {
    if (target == null)
    {
      this.slot.SetTarget((IEntity) null, (IInventoryComponent) null);
    }
    else
    {
      IStorageComponent component = target.GetComponent<IStorageComponent>();
      IInventoryComponent containerByTemplate = component != null ? StorageUtility.GetContainerByTemplate(component, this.containerTemplate.Value) : (IInventoryComponent) null;
      this.slot.SetTarget(target, containerByTemplate);
    }
  }
}
