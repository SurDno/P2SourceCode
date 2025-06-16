using Engine.Source.Components;
using Engine.Source.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory;
using UnityEngine;
using UnityEngine.UI;

public class SwitchingItemView : ItemView
{
  [SerializeField]
  private Image frontImage;
  [SerializeField]
  private Image backImage;
  [SerializeField]
  private Image nullImage;
  [SerializeField]
  private InventoryCellSizeEnum imageStyle;
  [SerializeField]
  private float swapSpeed = 4f;
  [SerializeField]
  private float frontScale = 1.5f;
  [SerializeField]
  private Vector2 frontShift = Vector2.zero;
  [SerializeField]
  private float backScale = 0.75f;
  [SerializeField]
  private Vector2 backShift = Vector2.zero;
  private StorableComponent storable;
  private bool needToSwap;
  private float phase = 1f;

  public bool IsEmptySlotActive() => nullImage.gameObject.activeInHierarchy;

  public override StorableComponent Storable
  {
    get => storable;
    set
    {
      if (storable == value)
        return;
      storable = value;
      needToSwap = true;
    }
  }

  private void ApplyPhase()
  {
    if (phase >= 1.0)
    {
      phase = 1f;
      SetSprite(backImage, null);
    }
    float num1 = 1f - phase;
    SetAlpha(frontImage, phase);
    float num2 = Mathf.Pow(frontScale, num1);
    frontImage.rectTransform.localScale = new Vector3(num2, num2, num2);
    frontImage.rectTransform.anchoredPosition = frontShift * num1;
    SetAlpha(backImage, num1);
    float num3 = Mathf.Pow(backScale, phase);
    backImage.rectTransform.localScale = new Vector3(num3, num3, num3);
    backImage.rectTransform.anchoredPosition = backShift * phase;
    if (!(nullImage != null))
      return;
    SetAlpha(nullImage, (float) ((frontImage.sprite == null ? phase : 0.0) + (backImage.sprite == null ? num1 : 0.0)));
  }

  private void OnDisable() => SkipAnimation();

  private void SetSprite(Image image, Sprite sprite) => image.sprite = sprite;

  private void SetAlpha(Image image, float alpha)
  {
    image.gameObject.SetActive(image.sprite != null && alpha > 0.0);
    Color color = image.color with { a = alpha };
    image.color = color;
  }

  private void SetFrontSprite()
  {
    InventoryPlaceholder placeholder = storable?.Placeholder;
    if (placeholder == null)
      SetSprite(frontImage, null);
    else
      SetSprite(frontImage, InventoryUtility.GetSpriteByStyle(placeholder, imageStyle));
    needToSwap = false;
  }

  private void Update()
  {
    if (phase == 1.0)
    {
      if (!needToSwap)
        return;
      SetSprite(backImage, frontImage.sprite);
      SetFrontSprite();
      phase = Time.deltaTime * swapSpeed;
      ApplyPhase();
    }
    else
    {
      phase += Time.deltaTime * swapSpeed;
      ApplyPhase();
    }
  }

  public override void SkipAnimation()
  {
    phase = 1f;
    SetFrontSprite();
    ApplyPhase();
  }
}
