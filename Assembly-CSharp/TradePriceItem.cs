using UnityEngine;
using UnityEngine.UI;

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
