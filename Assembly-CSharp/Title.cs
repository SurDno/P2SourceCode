using System.Text.RegularExpressions;

public class Title : MonoBehaviour
{
  [SerializeField]
  private UnityEngine.UI.Text holdText;
  [SerializeField]
  private UnityEngine.UI.Text text;
  [SerializeField]
  private Image iconImage;

  public void SetText(string text, Sprite icon, bool isHold)
  {
    this.text.gameObject.SetActive(text.Length > 0);
    this.text.text = text;
    iconImage.sprite = icon;
    holdText.gameObject.SetActive(isHold);
    if (!isHold)
      return;
    holdText.text = new Regex("(<color(.*?)>)").Match(text).Groups[0] + holdText.text + "</color>";
    LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
  }
}
