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
    if ((Object) coinsCountText != (Object) null)
      coinsCountText.text = count.ToString();
    if (!((Object) coinsChangeText != (Object) null))
      return;
    coinsChangeText.gameObject.SetActive(change != 0);
    if (change != 0)
    {
      coinsChangeText.text = change < 0 ? change.ToString() : "+" + change;
      coinsChangeText.color = change > 0 ? changePlusColor : changeMinusColor;
    }
  }
}
