using Engine.Impl.UI.Menu.Protagonist.Inventory;
using Engine.Source.Components;
using Engine.Source.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory;
using InputServices;
using UnityEngine;

public class StorableUITrade : StorableUI
{
  [SerializeField]
  private GameObject _selectedImage;
  private int selectedCount = 0;

  private void Start() => this._selectedImage.SetActive(false);

  protected override void Update()
  {
    if (this.internalStorable.Max > 1)
    {
      this.textCount.text = this.selectedCount == 0 ? this.internalStorable.Count.ToString() : this.selectedCount.ToString() + "/" + this.internalStorable.Count.ToString();
      if ((Object) this.textCount.gameObject != (Object) null)
        this.textCount.gameObject.SetActive(true);
    }
    else
    {
      this.textCount.text = (string) null;
      if ((Object) this.textCount.gameObject != (Object) null)
        this.textCount.gameObject.SetActive(false);
    }
    this.selectedImage.gameObject.SetActive(this.isSelected);
    Color color = this.enabledBackgroundColor;
    if (!this.isEnabled)
      color = this.disabledBackgroundColor;
    this.ImageBackground.color = color;
  }

  protected override void CalculatePosition()
  {
    if (this.internalStorable == null || this.internalStorable.IsDisposed)
      return;
    base.CalculatePosition();
    Vector2 vector2 = !this.cellStyle.IsSlot ? InventoryUtility.CalculateInnerSize((IInventoryGridBase) ((StorableComponent) this.internalStorable).Placeholder.Grid, this.cellStyle) : InventoryUtility.CalculateInnerSize((IInventoryGridBase) StorableUI.gridSlot, this.cellStyle);
    this.selectedImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, vector2.x + this.cellStyle.BackgroundImageOffset.x * 2f);
    this.selectedImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, vector2.y + this.cellStyle.BackgroundImageOffset.x * 2f);
  }

  public int GetSelectedCount() => this.selectedCount;

  public void SetSelectedCount(int count, bool isInit = false)
  {
    this.selectedCount = count;
    if (InputService.Instance.JoystickUsed)
    {
      this._selectedImage.SetActive(count > 0);
      if (isInit)
        this.isSelected = false;
    }
    else
    {
      this.isSelected = count > 0;
      this._selectedImage.SetActive(false);
    }
    this.Update();
  }
}
