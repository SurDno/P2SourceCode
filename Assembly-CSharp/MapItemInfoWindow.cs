using Engine.Behaviours.Localization;
using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components.BoundCharacters;
using UnityEngine;
using UnityEngine.UI;

public class MapItemInfoWindow : MonoBehaviour
{
  [SerializeField]
  private Localizer title = (Localizer) null;
  [SerializeField]
  private Text text = (Text) null;
  [Space]
  [SerializeField]
  private GameObject icons;
  [SerializeField]
  private GameObject craftIcon;
  [SerializeField]
  private GameObject saveIcon;
  [SerializeField]
  private GameObject sleepIcon;
  [SerializeField]
  private GameObject storeIcon;
  [SerializeField]
  private GameObject tradeIcon;
  [Space]
  [SerializeField]
  private GameObject portrait = (GameObject) null;
  [SerializeField]
  private Image portraitImage = (Image) null;
  [SerializeField]
  private StringView portraitText = (StringView) null;
  [SerializeField]
  private Sprite fallbackPortraitSprite = (Sprite) null;
  [SerializeField]
  private Color dangerColor;
  [SerializeField]
  private Color diseasedColor;
  [SerializeField]
  private Color deadColor;
  [SerializeField]
  private GameObject medicated;
  private MapItemView targetView;

  public void Show(MapItemView itemView)
  {
    if ((Object) itemView == (Object) this.targetView)
      return;
    if ((Object) this.targetView == (Object) null)
      this.gameObject.SetActive(true);
    else
      this.targetView.SetHightlight(false);
    this.targetView = itemView;
    this.targetView.SetHightlight(true);
    IMapItem mapItem = this.targetView.Item;
    if (mapItem.Resource == null)
      return;
    this.GetComponent<LayoutElement>();
    LocalizationService service = ServiceLocator.GetService<LocalizationService>();
    if (mapItem.Title != LocalizedText.Empty)
    {
      this.title.Signature = service.GetText(mapItem.Title);
      this.title.gameObject.SetActive(true);
    }
    else
      this.title.gameObject.SetActive(false);
    if (mapItem.Text != LocalizedText.Empty)
    {
      this.text.text = service.GetText(mapItem.Text);
      this.text.gameObject.SetActive(true);
    }
    else
      this.text.gameObject.SetActive(false);
    BoundCharacterComponent component = mapItem.BoundCharacter?.GetComponent<BoundCharacterComponent>();
    if (component != null)
    {
      BoundHealthStateEnum boundHealthStateEnum = BoundCharacterUtility.PerceivedHealth(component);
      this.portraitImage.sprite = BoundCharacterUtility.StateSprite(component, boundHealthStateEnum) ?? this.fallbackPortraitSprite;
      string str = BoundCharacterUtility.StateText(component, boundHealthStateEnum);
      if (str != null)
      {
        switch (boundHealthStateEnum)
        {
          case BoundHealthStateEnum.Danger:
            str = "<color=" + this.dangerColor.ToRGBHex() + ">" + str + "</color>";
            break;
          case BoundHealthStateEnum.Diseased:
            str = "<color=" + this.diseasedColor.ToRGBHex() + ">" + str + "</color>";
            break;
          case BoundHealthStateEnum.Dead:
            str = "<color=" + this.deadColor.ToRGBHex() + ">" + str + "</color>";
            break;
        }
        this.portraitText.StringValue = str;
        this.portraitText.gameObject.SetActive(true);
      }
      else
        this.portraitText.gameObject.SetActive(false);
      this.medicated.SetActive(BoundCharacterUtility.MedicineAttempted(component));
      this.portrait.SetActive(true);
    }
    else
      this.portrait.SetActive(false);
    this.craftIcon.SetActive(mapItem.CraftIcon.Value);
    this.saveIcon.SetActive(mapItem.SavePointIcon.Value);
    this.sleepIcon.SetActive(mapItem.SleepIcon.Value);
    this.storeIcon.SetActive(mapItem.StorageIcon.Value);
    this.tradeIcon.SetActive(mapItem.MerchantIcon.Value);
    this.icons.SetActive(this.craftIcon.activeSelf || this.saveIcon.activeSelf || this.sleepIcon.activeSelf || this.storeIcon.activeSelf || this.tradeIcon.activeSelf);
    this.UpdatePosition();
  }

  public void Hide(MapItemView mapItemView)
  {
    if ((Object) this.targetView != (Object) mapItemView)
      return;
    this.gameObject.SetActive(false);
    this.targetView.SetHightlight(false);
    this.targetView = (MapItemView) null;
    this.portraitImage.sprite = (Sprite) null;
  }

  private void UpdatePosition()
  {
    if ((Object) this.targetView == (Object) null)
      return;
    RectTransform transform1 = (RectTransform) this.transform;
    RectTransform transform2 = (RectTransform) this.GetComponentInParent<Canvas>().transform;
    Vector2 vector2 = new Vector2(transform2.sizeDelta.x, transform2.sizeDelta.y);
    Vector2 worldPosition = this.targetView.WorldPosition;
    worldPosition.x = Mathf.Round(worldPosition.x);
    worldPosition.y = Mathf.Round(worldPosition.y);
    worldPosition.x /= transform2.localScale.x;
    worldPosition.y /= transform2.localScale.y;
    transform1.pivot = new Vector2((double) worldPosition.x > (double) vector2.x * 0.699999988079071 ? 1f : 0.0f, (double) worldPosition.y > (double) vector2.y * 0.30000001192092896 ? 1f : 0.0f);
    transform1.anchoredPosition = worldPosition;
  }

  private void LateUpdate() => this.UpdatePosition();
}
