// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Protagonist.Inventory.StorableUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Components;
using Engine.Source.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory.Grid;
using InputServices;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#nullable disable
namespace Engine.Impl.UI.Menu.Protagonist.Inventory
{
  [DisallowMultipleComponent]
  public class StorableUI : UIControl
  {
    protected static IInventoryGridLimited gridSlot;
    private static List<StorableUI> s_List = new List<StorableUI>();
    [SerializeField]
    [FormerlySerializedAs("_Image")]
    private Image image;
    [SerializeField]
    [FormerlySerializedAs("_ImageBackground")]
    private Image imageBackground;
    [SerializeField]
    [FormerlySerializedAs("_TextCount")]
    protected Text textCount = (Text) null;
    [SerializeField]
    protected Color disabledBackgroundColor;
    [SerializeField]
    protected Color enabledBackgroundColor;
    [SerializeField]
    protected Color disabledImageColor;
    [SerializeField]
    protected Color enabledImageColor;
    [SerializeField]
    protected Image selectedImage;
    protected IStorableComponent internalStorable;
    protected InventoryCellStyle cellStyle;
    protected bool isEnabled = true;
    protected bool isSelected = false;
    private bool showCount = true;
    [SerializeField]
    protected GameObject _selectedBackground;
    private ItemsSlidingContainer _sliderParent;
    private bool _wasAttemptedToGetSlider = false;
    private bool dragging = false;

    public bool IsSelected() => this.isSelected;

    private void Start()
    {
    }

    public IStorableComponent Internal
    {
      get => this.internalStorable;
      set
      {
        this.internalStorable = value;
        this.CalculatePosition();
      }
    }

    public Image Image => this.image;

    public Image ImageBackground => this.imageBackground;

    public InventoryCellStyle Style
    {
      get => this.cellStyle;
      set
      {
        this.cellStyle = value;
        this.CalculatePosition();
      }
    }

    private Vector2 Size
    {
      get
      {
        return !this.cellStyle.IsSlot ? InventoryUtility.CalculateInnerSize((IInventoryGridBase) ((StorableComponent) this.internalStorable).Placeholder.Grid, this.cellStyle) : InventoryUtility.CalculateInnerSize((IInventoryGridBase) StorableUI.gridSlot, this.cellStyle);
      }
    }

    protected virtual void CalculatePosition()
    {
      if (this.internalStorable == null || this.internalStorable.IsDisposed)
        return;
      Vector2 size = this.Size;
      this.Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
      this.Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
      this.ImageBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x + this.cellStyle.BackgroundImageOffset.x * 2f);
      this.ImageBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y + this.cellStyle.BackgroundImageOffset.x * 2f);
    }

    public static StorableUI Instantiate(
      IStorableComponent storable,
      GameObject prefab,
      InventoryCellSizeEnum size)
    {
      if (storable == null || storable.IsDisposed)
        throw new Exception();
      if (StorableUI.gridSlot == null)
      {
        StorableUI.gridSlot = ServiceLocator.GetService<IFactory>().Create<IInventoryGridLimited>();
        ((InventoryGridLimited) StorableUI.gridSlot).Add(ProxyFactory.Create<Cell>());
      }
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
      gameObject.name = "[Storable] " + storable.Owner.Name;
      StorableUI component = gameObject.GetComponent<StorableUI>();
      component.Internal = storable;
      InventoryPlaceholder placeholder = ((StorableComponent) storable).Placeholder;
      component.image.sprite = InventoryUtility.GetSpriteByStyle(placeholder, size);
      component.Transform.anchorMax = (Vector2) Vector3.zero;
      component.Transform.anchorMin = (Vector2) Vector3.zero;
      component.Transform.pivot = Vector2.zero;
      component.Update();
      component.Enable(true);
      return component;
    }

    protected override void Awake()
    {
      base.Awake();
      StorableUI.s_List.Add(this);
    }

    private void OnDestroy() => StorableUI.s_List.Remove(this);

    protected virtual void Update()
    {
      if (this.internalStorable == null || this.internalStorable.IsDisposed)
        return;
      if ((UnityEngine.Object) this.textCount != (UnityEngine.Object) null)
      {
        if (this.internalStorable.Max > 1)
        {
          this.textCount.text = this.internalStorable.Count.ToString();
          this.textCount.gameObject?.SetActive(this.showCount);
        }
        else
        {
          this.textCount.text = (string) null;
          this.textCount.gameObject?.SetActive(false);
        }
      }
      this.selectedImage.gameObject?.SetActive(this.isSelected && (InputService.Instance.JoystickUsed || this.IsSliderItem));
    }

    public void Enable(bool active)
    {
      this.isEnabled = active;
      Color color1 = this.enabledImageColor;
      Color color2 = this.enabledBackgroundColor;
      if (!active)
      {
        color1 = this.disabledImageColor;
        color2 = this.disabledBackgroundColor;
      }
      this.image.color = color1;
      this.ImageBackground.color = color2;
    }

    public bool IsElementHoldSelected { get; private set; } = false;

    public void HoldSelected(bool b)
    {
      if (InputService.Instance.JoystickUsed)
      {
        if ((UnityEngine.Object) this._selectedBackground != (UnityEngine.Object) null)
          this._selectedBackground.SetActive(b);
      }
      else if ((UnityEngine.Object) this._selectedBackground != (UnityEngine.Object) null)
        this._selectedBackground.SetActive(false);
      this.IsElementHoldSelected = b;
    }

    public virtual void SetSelected(bool b)
    {
      this.isSelected = b;
      this.Update();
    }

    public bool IsSliderItem
    {
      get
      {
        if ((UnityEngine.Object) this._sliderParent == (UnityEngine.Object) null && !this._wasAttemptedToGetSlider)
        {
          this._sliderParent = this.GetComponentInParent<ItemsSlidingContainer>();
          this._wasAttemptedToGetSlider = true;
        }
        return this is StorableUITrade || (UnityEngine.Object) this._sliderParent != (UnityEngine.Object) null;
      }
    }

    public bool Dragging
    {
      set
      {
        if (value == this.dragging)
          return;
        this.dragging = value;
        Vector2 size = this.Size;
        float num = Math.Min(size.x, size.y);
        Vector3 vector3 = new Vector3((float) (-(double) num * 0.10000000149011612), num * 0.1f);
        if (this.dragging && !this.IsSelected())
          this.SetSelected(this.dragging);
        this.HoldSelected(this.dragging);
        if (this.dragging)
        {
          this.image.transform.position = this.image.transform.position + vector3;
          this.textCount.transform.position = this.textCount.transform.position + vector3;
        }
        else
        {
          this.image.transform.position = this.image.transform.position - vector3;
          this.textCount.transform.position = this.textCount.transform.position - vector3;
        }
      }
    }

    public void ShowCount(bool b)
    {
      this.showCount = b;
      this.Update();
    }

    public StorableUI FindSelectableOnDown() => this.FindSelectable(Vector2.down);

    public StorableUI FindSelectableOnUp() => this.FindSelectable(Vector2.up);

    public StorableUI FindSelectableOnLeft() => this.FindSelectable(Vector2.left);

    public StorableUI FindSelectableOnRight() => this.FindSelectable(Vector2.right);

    private StorableUI FindSelectable(Vector2 dir)
    {
      Vector3 vector3 = this.transform.TransformPoint(StorableUI.GetPointOnRectEdge(this.transform as RectTransform, dir));
      float num1 = float.NegativeInfinity;
      StorableUI selectable = (StorableUI) null;
      for (int index = 0; index < StorableUI.s_List.Count; ++index)
      {
        StorableUI storableUi = StorableUI.s_List[index];
        if (!((UnityEngine.Object) storableUi == (UnityEngine.Object) this) && !((UnityEngine.Object) storableUi == (UnityEngine.Object) null) && storableUi.gameObject.activeInHierarchy)
        {
          RectTransform transform = storableUi.transform as RectTransform;
          Vector3 position = (UnityEngine.Object) transform != (UnityEngine.Object) null ? (Vector3) transform.rect.center : Vector3.zero;
          Vector3 rhs = storableUi.transform.TransformPoint(position) - vector3;
          float num2 = Vector3.Dot((Vector3) dir, rhs);
          if ((double) num2 > 0.0)
          {
            float num3 = num2 / rhs.sqrMagnitude;
            if ((double) num3 > (double) num1)
            {
              num1 = num3;
              selectable = storableUi;
            }
          }
        }
      }
      return selectable;
    }

    private static Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir)
    {
      if ((UnityEngine.Object) rect == (UnityEngine.Object) null)
        return Vector3.zero;
      if (dir != Vector2.zero)
        dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
      dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);
      return (Vector3) dir;
    }
  }
}
