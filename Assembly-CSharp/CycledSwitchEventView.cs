// Decompiled with JetBrains decompiler
// Type: CycledSwitchEventView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Controls;
using System;
using UnityEngine;

#nullable disable
public class CycledSwitchEventView : EventView
{
  [SerializeField]
  private CycledSwitchEventView.Item[] items;

  private bool IsItemAvailable(int index)
  {
    HideableView availableCheck = this.items[index].availableCheck;
    return (UnityEngine.Object) availableCheck == (UnityEngine.Object) null || availableCheck.Visible;
  }

  public override void Invoke()
  {
    if (this.items == null || this.items.Length < 2)
      return;
    int index1 = -1;
    for (int index2 = 0; index2 < this.items.Length; ++index2)
    {
      if (this.IsItemAvailable(index2))
      {
        HideableView activeCheck = this.items[index2].activeCheck;
        if ((UnityEngine.Object) activeCheck != (UnityEngine.Object) null && activeCheck.Visible)
        {
          index1 = index2;
          break;
        }
      }
    }
    int index3 = this.WrappedIncrease(index1);
    for (bool flag = this.IsItemAvailable(index3); !flag; flag = this.IsItemAvailable(index3))
    {
      index3 = this.WrappedIncrease(index3);
      if (index3 == index1 || index1 == -1 && index3 == 0)
        return;
    }
    this.items[index3].activationAction?.Invoke();
  }

  private int WrappedIncrease(int index)
  {
    ++index;
    if (index == this.items.Length)
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
