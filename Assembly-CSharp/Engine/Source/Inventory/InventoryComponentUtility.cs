using Engine.Common.Components;
using Engine.Common.Components.Storable;
using Engine.Common.Types;
using Engine.Source.Components;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Source.Inventory
{
  public static class InventoryComponentUtility
  {
    public static StorableGroup GetInstrument(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).Instrument;
    }

    public static IEnumerable<StorableGroup> GetExcept(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).Except;
    }

    public static IEnumerable<StorableGroup> GetLimitations(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).Limitations;
    }

    public static InventoryGroup GetGroup(this IInventoryComponent container)
    {
      return (InventoryComponent) container == null ? InventoryGroup.None : ((InventoryComponent) container).Group;
    }

    public static IStorageComponent GetStorage(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).Storage;
    }

    public static ContainerCellKind GetKind(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).Kind;
    }

    public static IInventoryGridBase GetGrid(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).Grid;
    }

    public static SlotKind GetSlotKind(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).SlotKind;
    }

    public static Position GetPosition(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).Position;
    }

    public static Position GetPivot(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).Pivot;
    }

    public static Position GetAnchor(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).Anchor;
    }

    public static float GetDifficulty(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).Difficulty;
    }

    public static float GetInstrumentDamage(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).InstrumentDamage;
    }

    public static List<InventoryContainerOpenResource> GetOpenResources(
      this IInventoryComponent container)
    {
      return ((InventoryComponent) container).OpenResources;
    }

    public static Sprite GetImageBackground(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).ImageBackground;
    }

    public static Sprite GetImageForeground(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).ImageForeground;
    }

    public static Sprite GetImageNotAvailable(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).ImageNotAvailable;
    }

    public static Sprite GetImageInstrument(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).ImageInstrument;
    }

    public static Sprite GetImageLock(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).ImageLock;
    }

    public static float GetOpenTime(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).OpenTime;
    }

    public static AudioClip GetOpenStartAudio(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).OpenStartAudio;
    }

    public static AudioClip GetOpenProgressAudio(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).OpenProgressAudio;
    }

    public static AudioClip GetOpenCompleteAudio(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).OpenCompleteAudio;
    }

    public static AudioClip GetOpenCancelAudio(this IInventoryComponent container)
    {
      return ((InventoryComponent) container).OpenCancelAudio;
    }

    public static void Initialise(this IInventoryComponent container, StorageComponent storage)
    {
      container.Initialise(storage);
    }
  }
}
