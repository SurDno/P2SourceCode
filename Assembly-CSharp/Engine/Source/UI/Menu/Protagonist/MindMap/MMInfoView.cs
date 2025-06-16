// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Protagonist.MindMap.MMInfoView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace Engine.Source.UI.Menu.Protagonist.MindMap
{
  public class MMInfoView : MonoBehaviour
  {
    [SerializeField]
    private Text textView;
    [SerializeField]
    private GameObject mapCallTooltip;
    [SerializeField]
    private float discoveredTime = 0.6f;
    private MMNodeView nodeView;

    public void Hide()
    {
      this.nodeView = (MMNodeView) null;
      this.gameObject.SetActive(false);
      this.CancelInvoke("MarkNodeDiscovered");
    }

    public void Hide(MMNodeView nodeView)
    {
      if (!((Object) nodeView == (Object) this.nodeView))
        return;
      this.Hide();
    }

    public void Show(MMNodeView nodeView, bool hasMapItem)
    {
      LocalizationService service = ServiceLocator.GetService<LocalizationService>();
      this.nodeView = nodeView;
      this.textView.text = service.GetText(nodeView.Node.Content.Description);
      this.mapCallTooltip.SetActive(hasMapItem);
      this.gameObject.SetActive(true);
      this.UpdatePosition();
      this.Invoke("MarkNodeDiscovered", this.discoveredTime);
    }

    private void LateUpdate() => this.UpdatePosition();

    private void UpdatePosition()
    {
      if ((Object) this.nodeView == (Object) null)
        return;
      RectTransform transform1 = (RectTransform) this.transform;
      RectTransform transform2 = (RectTransform) this.GetComponentInParent<Canvas>().transform;
      Vector2 vector2_1 = new Vector2(transform2.sizeDelta.x, transform2.sizeDelta.y);
      Vector2 vector2_2 = (Vector2) this.nodeView.transform.TransformPoint(Vector3.zero);
      vector2_2.x = Mathf.Round(vector2_2.x);
      vector2_2.y = Mathf.Round(vector2_2.y);
      vector2_2.x /= transform2.localScale.x;
      vector2_2.y /= transform2.localScale.y;
      transform1.pivot = new Vector2((double) vector2_2.x > (double) vector2_1.x * 0.699999988079071 ? 1f : 0.0f, (double) vector2_2.y > (double) vector2_1.y * 0.30000001192092896 ? 1f : 0.0f);
      transform1.anchoredPosition = vector2_2;
    }

    private void MarkNodeDiscovered() => this.nodeView.MarkNodeDiscovered();
  }
}
