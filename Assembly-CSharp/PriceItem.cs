using UnityEngine;
using UnityEngine.UI;

public class PriceItem : MonoBehaviour
{
  [SerializeField]
  private bool NeedMoveCoin;
  [SerializeField]
  private Text unityPrice;
  [SerializeField]
  private Image unityPriceImage;
  [SerializeField]
  private Transform unityPriceImageBasePlace;

  public void SetPrice(int price)
  {
    if (price == 0)
    {
      unityPrice.gameObject.SetActive(false);
      unityPriceImage.gameObject.SetActive(false);
    }
    else
    {
      unityPrice.gameObject.SetActive(true);
      unityPriceImage.gameObject.SetActive(true);
      string message = price.ToString();
      unityPrice.text = message;
      if (NeedMoveCoin)
        unityPriceImage.transform.localPosition = unityPriceImageBasePlace.localPosition + new Vector3(-CalculateLengthOfMessage(unityPrice, message), 0.0f, 0.0f);
    }
  }

  private int CalculateLengthOfMessage(Text textfield, string message)
  {
    int lengthOfMessage = 0;
    Font font = textfield.font;
    CharacterInfo info = new CharacterInfo();
    char[] charArray = message.ToCharArray();
    font.RequestCharactersInTexture(message, textfield.fontSize, textfield.fontStyle);
    foreach (char ch in charArray)
    {
      font.GetCharacterInfo(ch, out info, textfield.fontSize);
      lengthOfMessage += info.advance;
    }
    return lengthOfMessage;
  }
}
