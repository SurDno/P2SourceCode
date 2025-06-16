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
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory;

[DisallowMultipleComponent]
public class StorableUI : UIControl {
	protected static IInventoryGridLimited gridSlot;
	private static List<StorableUI> s_List = new();

	[SerializeField] [FormerlySerializedAs("_Image")]
	private Image image;

	[SerializeField] [FormerlySerializedAs("_ImageBackground")]
	private Image imageBackground;

	[SerializeField] [FormerlySerializedAs("_TextCount")]
	protected Text textCount;

	[SerializeField] protected Color disabledBackgroundColor;
	[SerializeField] protected Color enabledBackgroundColor;
	[SerializeField] protected Color disabledImageColor;
	[SerializeField] protected Color enabledImageColor;
	[SerializeField] protected Image selectedImage;
	protected IStorableComponent internalStorable;
	protected InventoryCellStyle cellStyle;
	protected bool isEnabled = true;
	protected bool isSelected;
	private bool showCount = true;
	[SerializeField] protected GameObject _selectedBackground;
	private ItemsSlidingContainer _sliderParent;
	private bool _wasAttemptedToGetSlider;
	private bool dragging;

	public bool IsSelected() {
		return isSelected;
	}

	private void Start() { }

	public IStorableComponent Internal {
		get => internalStorable;
		set {
			internalStorable = value;
			CalculatePosition();
		}
	}

	public Image Image => image;

	public Image ImageBackground => imageBackground;

	public InventoryCellStyle Style {
		get => cellStyle;
		set {
			cellStyle = value;
			CalculatePosition();
		}
	}

	private Vector2 Size => !cellStyle.IsSlot
		? InventoryUtility.CalculateInnerSize(((StorableComponent)internalStorable).Placeholder.Grid, cellStyle)
		: InventoryUtility.CalculateInnerSize(gridSlot, cellStyle);

	protected virtual void CalculatePosition() {
		if (internalStorable == null || internalStorable.IsDisposed)
			return;
		var size = Size;
		Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
		Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
		ImageBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
			size.x + cellStyle.BackgroundImageOffset.x * 2f);
		ImageBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
			size.y + cellStyle.BackgroundImageOffset.x * 2f);
	}

	public static StorableUI Instantiate(
		IStorableComponent storable,
		GameObject prefab,
		InventoryCellSizeEnum size) {
		if (storable == null || storable.IsDisposed)
			throw new Exception();
		if (gridSlot == null) {
			gridSlot = ServiceLocator.GetService<IFactory>().Create<IInventoryGridLimited>();
			((InventoryGridLimited)gridSlot).Add(ProxyFactory.Create<Cell>());
		}

		var gameObject = Instantiate(prefab);
		gameObject.name = "[Storable] " + storable.Owner.Name;
		var component = gameObject.GetComponent<StorableUI>();
		component.Internal = storable;
		var placeholder = ((StorableComponent)storable).Placeholder;
		component.image.sprite = InventoryUtility.GetSpriteByStyle(placeholder, size);
		component.Transform.anchorMax = Vector3.zero;
		component.Transform.anchorMin = Vector3.zero;
		component.Transform.pivot = Vector2.zero;
		component.Update();
		component.Enable(true);
		return component;
	}

	protected override void Awake() {
		base.Awake();
		s_List.Add(this);
	}

	private void OnDestroy() {
		s_List.Remove(this);
	}

	protected virtual void Update() {
		if (internalStorable == null || internalStorable.IsDisposed)
			return;
		if (textCount != null) {
			if (internalStorable.Max > 1) {
				textCount.text = internalStorable.Count.ToString();
				textCount.gameObject?.SetActive(showCount);
			} else {
				textCount.text = null;
				textCount.gameObject?.SetActive(false);
			}
		}

		selectedImage.gameObject?.SetActive(isSelected && (InputService.Instance.JoystickUsed || IsSliderItem));
	}

	public void Enable(bool active) {
		isEnabled = active;
		var color1 = enabledImageColor;
		var color2 = enabledBackgroundColor;
		if (!active) {
			color1 = disabledImageColor;
			color2 = disabledBackgroundColor;
		}

		image.color = color1;
		ImageBackground.color = color2;
	}

	public bool IsElementHoldSelected { get; private set; }

	public void HoldSelected(bool b) {
		if (InputService.Instance.JoystickUsed) {
			if (_selectedBackground != null)
				_selectedBackground.SetActive(b);
		} else if (_selectedBackground != null)
			_selectedBackground.SetActive(false);

		IsElementHoldSelected = b;
	}

	public virtual void SetSelected(bool b) {
		isSelected = b;
		Update();
	}

	public bool IsSliderItem {
		get {
			if (_sliderParent == null && !_wasAttemptedToGetSlider) {
				_sliderParent = GetComponentInParent<ItemsSlidingContainer>();
				_wasAttemptedToGetSlider = true;
			}

			return this is StorableUITrade || _sliderParent != null;
		}
	}

	public bool Dragging {
		set {
			if (value == dragging)
				return;
			dragging = value;
			var size = Size;
			var num = Math.Min(size.x, size.y);
			var vector3 = new Vector3((float)(-(double)num * 0.10000000149011612), num * 0.1f);
			if (dragging && !IsSelected())
				SetSelected(dragging);
			HoldSelected(dragging);
			if (dragging) {
				image.transform.position = image.transform.position + vector3;
				textCount.transform.position = textCount.transform.position + vector3;
			} else {
				image.transform.position = image.transform.position - vector3;
				textCount.transform.position = textCount.transform.position - vector3;
			}
		}
	}

	public void ShowCount(bool b) {
		showCount = b;
		Update();
	}

	public StorableUI FindSelectableOnDown() {
		return FindSelectable(Vector2.down);
	}

	public StorableUI FindSelectableOnUp() {
		return FindSelectable(Vector2.up);
	}

	public StorableUI FindSelectableOnLeft() {
		return FindSelectable(Vector2.left);
	}

	public StorableUI FindSelectableOnRight() {
		return FindSelectable(Vector2.right);
	}

	private StorableUI FindSelectable(Vector2 dir) {
		var vector3 = this.transform.TransformPoint(GetPointOnRectEdge(this.transform as RectTransform, dir));
		var num1 = float.NegativeInfinity;
		StorableUI selectable = null;
		for (var index = 0; index < s_List.Count; ++index) {
			var storableUi = s_List[index];
			if (!(storableUi == this) && !(storableUi == null) && storableUi.gameObject.activeInHierarchy) {
				var transform = storableUi.transform as RectTransform;
				Vector3 position = transform != null ? transform.rect.center : Vector3.zero;
				var rhs = storableUi.transform.TransformPoint(position) - vector3;
				var num2 = Vector3.Dot(dir, rhs);
				if (num2 > 0.0) {
					var num3 = num2 / rhs.sqrMagnitude;
					if (num3 > (double)num1) {
						num1 = num3;
						selectable = storableUi;
					}
				}
			}
		}

		return selectable;
	}

	private static Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir) {
		if (rect == null)
			return Vector3.zero;
		if (dir != Vector2.zero)
			dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
		dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);
		return dir;
	}
}