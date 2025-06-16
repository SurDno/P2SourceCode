// Decompiled with JetBrains decompiler
// Type: TradePriceItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class TradePriceItem : MonoBehaviour
{
  [SerializeField]
  private Image coinImage;
  [SerializeField]
  private Text coinsCountText;
  [SerializeField]
  private Text coinsChangeText;
  [SerializeField]
  private Color changePlusColor;
  [SerializeField]
  private Color changeMinusColor;

  public void SetCount(int count, int change)
  {
    if ((Object) this.coinsCountText != (Object) null)
      this.coinsCountText.text = count.ToString();
    if (!((Object) this.coinsChangeText != (Object) null))
      return;
    this.coinsChangeText.gameObject.SetActive(change != 0);
    if (change != 0)
    {
      this.coinsChangeText.text = change < 0 ? change.ToString() : "+" + change.ToString();
      this.coinsChangeText.color = change > 0 ? this.changePlusColor : this.changeMinusColor;
    }
  }
}
