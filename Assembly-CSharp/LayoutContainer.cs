using UnityEngine;

public class LayoutContainer : MonoBehaviour
{
  [SerializeField]
  private RectTransform content;

  public RectTransform Content => this.content;
}
