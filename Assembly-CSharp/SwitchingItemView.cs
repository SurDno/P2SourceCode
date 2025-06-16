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
  private StorableComponent storable = (StorableComponent) null;
  private bool needToSwap = false;
  private float phase = 1f;

  public bool IsEmptySlotActive() => this.nullImage.gameObject.activeInHierarchy;

  public override StorableComponent Storable
  {
    get => this.storable;
    set
    {
      if (this.storable == value)
        return;
      this.storable = value;
      this.needToSwap = true;
    }
  }

  private void ApplyPhase()
  {
    if ((double) this.phase >= 1.0)
    {
      this.phase = 1f;
      this.SetSprite(this.backImage, (Sprite) null);
    }
    float num1 = 1f - this.phase;
    this.SetAlpha(this.frontImage, this.phase);
    float num2 = Mathf.Pow(this.frontScale, num1);
    this.frontImage.rectTransform.localScale = new Vector3(num2, num2, num2);
    this.frontImage.rectTransform.anchoredPosition = this.frontShift * num1;
    this.SetAlpha(this.backImage, num1);
    float num3 = Mathf.Pow(this.backScale, this.phase);
    this.backImage.rectTransform.localScale = new Vector3(num3, num3, num3);
    this.backImage.rectTransform.anchoredPosition = this.backShift * this.phase;
    if (!((Object) this.nullImage != (Object) null))
      return;
    this.SetAlpha(this.nullImage, (float) (((Object) this.frontImage.sprite == (Object) null ? (double) this.phase : 0.0) + ((Object) this.backImage.sprite == (Object) null ? (double) num1 : 0.0)));
  }

  private void OnDisable() => this.SkipAnimation();

  private void SetSprite(Image image, Sprite sprite) => image.sprite = sprite;

  private void SetAlpha(Image image, float alpha)
  {
    image.gameObject.SetActive((Object) image.sprite != (Object) null && (double) alpha > 0.0);
    Color color = image.color with { a = alpha };
    image.color = color;
  }

  private void SetFrontSprite()
  {
    InventoryPlaceholder placeholder = this.storable?.Placeholder;
    if (placeholder == null)
      this.SetSprite(this.frontImage, (Sprite) null);
    else
      this.SetSprite(this.frontImage, InventoryUtility.GetSpriteByStyle(placeholder, this.imageStyle));
    this.needToSwap = false;
  }

  private void Update()
  {
    if ((double) this.phase == 1.0)
    {
      if (!this.needToSwap)
        return;
      this.SetSprite(this.backImage, this.frontImage.sprite);
      this.SetFrontSprite();
      this.phase = Time.deltaTime * this.swapSpeed;
      this.ApplyPhase();
    }
    else
    {
      this.phase += Time.deltaTime * this.swapSpeed;
      this.ApplyPhase();
    }
  }

  public override void SkipAnimation()
  {
    this.phase = 1f;
    this.SetFrontSprite();
    this.ApplyPhase();
  }
}
