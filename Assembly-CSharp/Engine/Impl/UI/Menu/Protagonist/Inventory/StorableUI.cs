using System;
using System.Collections.Generic;
using Cofe.Proxies;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Components;
using Engine.Source.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory.Grid;
using InputServices;

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
    protected bool isSelected;
    private bool showCount = true;
    [SerializeField]
    protected GameObject _selectedBackground;
    private ItemsSlidingContainer _sliderParent;
    private bool _wasAttemptedToGetSlider;
    private bool dragging;

    public bool IsSelected() => isSelected;

    private void Start()
    {
    }

    public IStorableComponent Internal
    {
      get => internalStorable;
      set
      {
        internalStorable = value;
        CalculatePosition();
      }
    }

    public Image Image => image;

    public Image ImageBackground => imageBackground;

    public InventoryCellStyle Style
    {
      get => cellStyle;
      set
      {
        cellStyle = value;
        CalculatePosition();
      }
    }

    private Vector2 Size
    {
      get
      {
        return !cellStyle.IsSlot ? InventoryUtility.CalculateInnerSize(((StorableComponent) internalStorable).Placeholder.Grid, cellStyle) : InventoryUtility.CalculateInnerSize(gridSlot, cellStyle);
      }
    }

    protected virtual void CalculatePosition()
    {
      if (internalStorable == null || internalStorable.IsDisposed)
        return;
      Vector2 size = Size;
      Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
      Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
      ImageBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x + cellStyle.BackgroundImageOffset.x * 2f);
      ImageBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y + cellStyle.BackgroundImageOffset.x * 2f);
    }

    public static StorableUI Instantiate(
      IStorableComponent storable,
      GameObject prefab,
      InventoryCellSizeEnum size)
    {
      if (storable == null || storable.IsDisposed)
        throw new Exception();
      if (gridSlot == null)
      {
        gridSlot = ServiceLocator.GetService<IFactory>().Create<IInventoryGridLimited>();
        ((InventoryGridLimited) gridSlot).Add(ProxyFactory.Create<Cell>());
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
      s_List.Add(this);
    }

    private void OnDestroy() => s_List.Remove(this);

    protected virtual void Update()
    {
      if (internalStorable == null || internalStorable.IsDisposed)
        return;
      if ((UnityEngine.Object) textCount != (UnityEngine.Object) null)
      {
        if (internalStorable.Max > 1)
        {
          textCount.text = internalStorable.Count.ToString();
          textCount.gameObject?.SetActive(showCount);
        }
        else
        {
          textCount.text = (string) null;
          textCount.gameObject?.SetActive(false);
        }
      }
      selectedImage.gameObject?.SetActive(isSelected && (InputService.Instance.JoystickUsed || IsSliderItem));
    }

    public void Enable(bool active)
    {
      isEnabled = active;
      Color color1 = enabledImageColor;
      Color color2 = enabledBackgroundColor;
      if (!active)
      {
        color1 = disabledImageColor;
        color2 = disabledBackgroundColor;
      }
      image.color = color1;
      ImageBackground.color = color2;
    }

    public bool IsElementHoldSelected { get; private set; }

    public void HoldSelected(bool b)
    {
      if (InputService.Instance.JoystickUsed)
      {
        if ((UnityEngine.Object) _selectedBackground != (UnityEngine.Object) null)
          _selectedBackground.SetActive(b);
      }
      else if ((UnityEngine.Object) _selectedBackground != (UnityEngine.Object) null)
        _selectedBackground.SetActive(false);
      IsElementHoldSelected = b;
    }

    public virtual void SetSelected(bool b)
    {
      isSelected = b;
      Update();
    }

    public bool IsSliderItem
    {
      get
      {
        if ((UnityEngine.Object) _sliderParent == (UnityEngine.Object) null && !_wasAttemptedToGetSlider)
        {
          _sliderParent = this.GetComponentInParent<ItemsSlidingContainer>();
          _wasAttemptedToGetSlider = true;
        }
        return this is StorableUITrade || (UnityEngine.Object) _sliderParent != (UnityEngine.Object) null;
      }
    }

    public bool Dragging
    {
      set
      {
        if (value == dragging)
          return;
        dragging = value;
        Vector2 size = Size;
        float num = Math.Min(size.x, size.y);
        Vector3 vector3 = new Vector3((float) (-(double) num * 0.10000000149011612), num * 0.1f);
        if (dragging && !IsSelected())
          SetSelected(dragging);
        HoldSelected(dragging);
        if (dragging)
        {
          image.transform.position = image.transform.position + vector3;
          textCount.transform.position = textCount.transform.position + vector3;
        }
        else
        {
          image.transform.position = image.transform.position - vector3;
          textCount.transform.position = textCount.transform.position - vector3;
        }
      }
    }

    public void ShowCount(bool b)
    {
      showCount = b;
      Update();
    }

    public StorableUI FindSelectableOnDown() => FindSelectable(Vector2.down);

    public StorableUI FindSelectableOnUp() => FindSelectable(Vector2.up);

    public StorableUI FindSelectableOnLeft() => FindSelectable(Vector2.left);

    public StorableUI FindSelectableOnRight() => FindSelectable(Vector2.right);

    private StorableUI FindSelectable(Vector2 dir)
    {
      Vector3 vector3 = this.transform.TransformPoint(GetPointOnRectEdge(this.transform as RectTransform, dir));
      float num1 = float.NegativeInfinity;
      StorableUI selectable = null;
      for (int index = 0; index < s_List.Count; ++index)
      {
        StorableUI storableUi = s_List[index];
        if (!((UnityEngine.Object) storableUi == (UnityEngine.Object) this) && !((UnityEngine.Object) storableUi == (UnityEngine.Object) null) && storableUi.gameObject.activeInHierarchy)
        {
          RectTransform transform = storableUi.transform as RectTransform;
          Vector3 position = (UnityEngine.Object) transform != (UnityEngine.Object) null ? (Vector3) transform.rect.center : Vector3.zero;
          Vector3 rhs = storableUi.transform.TransformPoint(position) - vector3;
          float num2 = Vector3.Dot((Vector3) dir, rhs);
          if (num2 > 0.0)
          {
            float num3 = num2 / rhs.sqrMagnitude;
            if (num3 > (double) num1)
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
