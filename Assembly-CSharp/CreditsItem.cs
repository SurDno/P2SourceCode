// Decompiled with JetBrains decompiler
// Type: CreditsItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Controls;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class CreditsItem : MonoBehaviour
{
  [SerializeField]
  private StringView[] lineViews;
  private int count = 0;

  public void AddLine(string line)
  {
    line = line.Replace("\\n", "\n");
    StringView lineView = this.lineViews[this.count];
    lineView.StringValue = line;
    lineView.gameObject.SetActive(true);
    ++this.count;
  }

  public float UpdateBottomEnd()
  {
    RectTransform transform = (RectTransform) this.transform;
    float preferredHeight = LayoutUtility.GetPreferredHeight(transform);
    Vector2 sizeDelta = transform.sizeDelta with
    {
      y = preferredHeight
    };
    transform.sizeDelta = sizeDelta;
    return preferredHeight - transform.anchoredPosition.y;
  }

  public bool IsFull() => this.count >= this.lineViews.Length;

  public void SetPosition(float value)
  {
    ((RectTransform) this.transform).anchoredPosition = new Vector2(0.0f, -value);
  }
}
