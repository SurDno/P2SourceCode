// Decompiled with JetBrains decompiler
// Type: LockpickTooltip
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Components;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class LockpickTooltip : MonoBehaviour
{
  [SerializeField]
  private GameObject tooltip;
  [SerializeField]
  private Image itemIcon;
  [SerializeField]
  private Text countText;

  public void SetActive(bool active) => this.gameObject.SetActive(active);

  public void SetItem(StorableComponent storable)
  {
    this.itemIcon.sprite = storable?.Placeholder?.ImageInventorySlot.Value;
    this.itemIcon.gameObject.SetActive((Object) this.itemIcon.sprite != (Object) null);
  }

  public void SetCount(int count) => this.countText.text = count.ToString();
}
