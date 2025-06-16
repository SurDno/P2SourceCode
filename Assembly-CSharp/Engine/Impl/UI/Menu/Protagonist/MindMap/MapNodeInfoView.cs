// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Protagonist.MindMap.MapNodeInfoView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace Engine.Impl.UI.Menu.Protagonist.MindMap
{
  public class MapNodeInfoView : MonoBehaviour
  {
    [SerializeField]
    private Text textView;
    [SerializeField]
    private GameObject mindMapCallTooltip;
    private RectTransform followedRect;

    public void Hide()
    {
      this.followedRect = (RectTransform) null;
      this.gameObject.SetActive(false);
    }

    public void Show(RectTransform followedRect, string text, bool hasMindMapNode)
    {
      this.followedRect = followedRect;
      this.textView.text = text;
      this.mindMapCallTooltip.SetActive(hasMindMapNode);
      this.gameObject.SetActive(true);
      this.UpdatePosition();
    }

    private void LateUpdate() => this.UpdatePosition();

    private void UpdatePosition()
    {
      RectTransform transform1 = (RectTransform) this.transform;
      RectTransform transform2 = (RectTransform) this.GetComponentInParent<Canvas>().transform;
      Vector2 vector2 = new Vector2(transform2.sizeDelta.x, transform2.sizeDelta.y);
      Vector2 position = (Vector2) this.followedRect.position;
      position.x = Mathf.Round(position.x);
      position.y = Mathf.Round(position.y);
      position.x /= transform2.localScale.x;
      position.y /= transform2.localScale.y;
      transform1.pivot = new Vector2((double) position.x > (double) vector2.x * 0.699999988079071 ? 1f : 0.0f, (double) position.y > (double) vector2.y * 0.30000001192092896 ? 1f : 0.0f);
      transform1.anchoredPosition = position;
    }
  }
}
