using UnityEngine;
using UnityEngine.UI;

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
      followedRect = null;
      gameObject.SetActive(false);
    }

    public void Show(RectTransform followedRect, string text, bool hasMindMapNode)
    {
      this.followedRect = followedRect;
      textView.text = text;
      mindMapCallTooltip.SetActive(hasMindMapNode);
      gameObject.SetActive(true);
      UpdatePosition();
    }

    private void LateUpdate() => UpdatePosition();

    private void UpdatePosition()
    {
      RectTransform transform1 = (RectTransform) transform;
      RectTransform transform2 = (RectTransform) GetComponentInParent<Canvas>().transform;
      Vector2 vector2 = new Vector2(transform2.sizeDelta.x, transform2.sizeDelta.y);
      Vector2 position = followedRect.position;
      position.x = Mathf.Round(position.x);
      position.y = Mathf.Round(position.y);
      position.x /= transform2.localScale.x;
      position.y /= transform2.localScale.y;
      transform1.pivot = new Vector2(position.x > vector2.x * 0.699999988079071 ? 1f : 0.0f, position.y > vector2.y * 0.30000001192092896 ? 1f : 0.0f);
      transform1.anchoredPosition = position;
    }
  }
}
