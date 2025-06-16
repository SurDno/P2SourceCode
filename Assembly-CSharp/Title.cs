using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

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
    this.iconImage.sprite = icon;
    this.holdText.gameObject.SetActive(isHold);
    if (!isHold)
      return;
    this.holdText.text = new Regex("(<color(.*?)>)").Match(text).Groups[0].ToString() + this.holdText.text + "</color>";
    LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
  }
}
