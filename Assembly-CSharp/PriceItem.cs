// Decompiled with JetBrains decompiler
// Type: PriceItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
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
      this.unityPrice.gameObject.SetActive(false);
      this.unityPriceImage.gameObject.SetActive(false);
    }
    else
    {
      this.unityPrice.gameObject.SetActive(true);
      this.unityPriceImage.gameObject.SetActive(true);
      string message = price.ToString();
      this.unityPrice.text = message;
      if (this.NeedMoveCoin)
        this.unityPriceImage.transform.localPosition = this.unityPriceImageBasePlace.localPosition + new Vector3((float) -this.CalculateLengthOfMessage(this.unityPrice, message), 0.0f, 0.0f);
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
