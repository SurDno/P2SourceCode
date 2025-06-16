using System;
using Engine.Impl.UI.Controls;
using UnityEngine;

public class CycledSwitchEventView : EventView
{
  [SerializeField]
  private Item[] items;

  private bool IsItemAvailable(int index)
  {
    HideableView availableCheck = items[index].availableCheck;
    return availableCheck == null || availableCheck.Visible;
  }

  public override void Invoke()
  {
    if (items == null || items.Length < 2)
      return;
    int index1 = -1;
    for (int index2 = 0; index2 < items.Length; ++index2)
    {
      if (IsItemAvailable(index2))
      {
        HideableView activeCheck = items[index2].activeCheck;
        if (activeCheck != null && activeCheck.Visible)
        {
          index1 = index2;
          break;
        }
      }
    }
    int index3 = WrappedIncrease(index1);
    for (bool flag = IsItemAvailable(index3); !flag; flag = IsItemAvailable(index3))
    {
      index3 = WrappedIncrease(index3);
      if (index3 == index1 || index1 == -1 && index3 == 0)
        return;
    }
    items[index3].activationAction?.Invoke();
  }

  private int WrappedIncrease(int index)
  {
    ++index;
    if (index == items.Length)
      index = 0;
    return index;
  }

  [Serializable]
  public struct Item
  {
    public HideableView availableCheck;
    public HideableView activeCheck;
    public EventView activationAction;
  }
}
