// Decompiled with JetBrains decompiler
// Type: Title
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
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
