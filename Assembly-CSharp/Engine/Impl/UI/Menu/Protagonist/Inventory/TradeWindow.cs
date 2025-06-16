using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Windows;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Inventory;
using Engine.Source.Services;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using InputServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory;

public class TradeWindow : BaseInventoryWindow<TradeWindow>, ITradeWindow, IWindow {
	[SerializeField] [FormerlySerializedAs("_Button_Trade")]
	private Button buttonTrade;

	[SerializeField] private CompareMeterLine meter;
	[SerializeField] private ItemsSlidingContainer marketContainer;
	[SerializeField] private TradePriceItem protagonistMoney;
	[SerializeField] private TradePriceItem marketMoney;
	[SerializeField] private GameObject joystickAccept;
	[SerializeField] private GameObject inventoryselectTipObject;
	private MarketComponent market;
	private StorageComponent storage;
	private IStorageComponent protagonistTable;
	private IStorageComponent marketTable;
	private Dictionary<IStorageComponent, HashSet<IStorageComponent>> disallowedTransactions = new();
	private int protagonistPrice;
	private int marketPrice;
	private bool useMoney;
	private bool reputationForGifts;
	private int protagonistCoins;
	private int marketCoins;
	private int moneyDiff;
	private int currentmarketItemsIndex = -1;
	public Dictionary<StorableComponent, int> SelectedItems = new();

	private DialogModeController dialogModeController = new() {
		TargetCameraKind = CameraKindEnum.Trade
	};

	private Modes _currentMode = Modes.None;

	private List<StorableUI> marketItems { get; set; }

	public IMarketComponent Market {
		get => market;
		set => market = (MarketComponent)value;
	}

	private void ClearMarketContainer() {
		marketContainer.Clear(containers, storables);
	}

	private void CreateMarketContainer() {
		var all = storage.Items.Cast<StorableComponent>().ToList().FindAll(x => {
			if (x != null)
				return ValidateContainer(x.Container, x.Storage) && (ItemIsInteresting(x) || IsDebugMode());
			Debug.LogError("x == null");
			return false;
		});
		ClearMarketContainer();
		marketContainer.CreateSlots(all, storage, containers, storables);
		marketItems = marketContainer.ItemsUI;
	}

	public void Accept() {
		if ((!useMoney && protagonistPrice < marketPrice) || protagonistPrice + protagonistCoins < marketPrice)
			return;
		if (useMoney) {
			var num = Mathf.Abs(moneyDiff);
			if (moneyDiff > 0)
				MoveMoney(storage, Actor, num);
			else if (moneyDiff < 0) {
				var storableComponent = Actor.Items.ToList()
					.Find(x => x.Groups != null && x.Groups.Contains(StorableGroup.Money));
				if (storableComponent != null)
					num = Mathf.Min(storableComponent.Count, num);
				MoveMoney(Actor, storage, num);
			}
		}

		var dictionary1 = new Dictionary<StorableComponent, int>();
		var dictionary2 = new Dictionary<StorableComponent, int>();
		if (SelectedItems.Count > 0)
			foreach (var selectedItem in SelectedItems) {
				var key = selectedItem.Key;
				if (storables.ContainsKey(key)) {
					if (storables[key] is StorableUITrade)
						(storables[key] as StorableUITrade).SetSelectedCount(0);
					if (key.Storage == Actor)
						dictionary1[key] = selectedItem.Value;
					else if (key.Storage == storage)
						dictionary2[key] = selectedItem.Value;
				}
			}

		foreach (var keyValuePair in dictionary1) {
			var key = keyValuePair.Key;
			if (key.Count == keyValuePair.Value) {
				MoveItem(key, storage, storage.Containers.First(x => ValidateContainer(x, storage)));
				selectedStorable = null;
				ModeBasedSelection(Modes.Inventory);
			} else
				storage.AddItem(key.Split(keyValuePair.Value),
					storage.Containers.First(x => ValidateContainer(x, storage)));
		}

		foreach (var keyValuePair in dictionary2) {
			var key = keyValuePair.Key;
			var storable = key;
			var flag = false;
			if (key.Count != keyValuePair.Value) {
				storable = (StorableComponent)key.Split(keyValuePair.Value);
				flag = true;
			}

			var actor = Actor;
			var intersect = StorageUtility.GetIntersect(Actor, null, storable, null);
			if (!storable.IsDisposed) {
				if (!intersect.IsAllowed) {
					ServiceLocator.GetService<DropBagService>().DropBag(storable, Actor.Owner);
					StartOpenAudio(dropItemAudio);
				} else if (flag)
					actor.AddItem(storable, null);
				else {
					MoveItem(storable, Actor);
					selectedStorable = null;
					ModeBasedSelection(Modes.Inventory);
				}
			}
		}

		if (protagonistPrice > 0 || marketPrice > 0)
			ServiceLocator.GetService<LogicEventService>().FireEntityEvent("TradeSuccesful", Market.Owner);
		if (!useMoney && reputationForGifts) {
			var gift = protagonistPrice - marketPrice;
			if (gift > 0)
				Actor.GetComponent<PlayerControllerComponent>().ComputeGiftNPC(Market?.Owner, gift);
		}

		ResetSelectedItems();
		CalculateResult();
		OnInvalidate();
		ModeBasedSelection(CurrentMode);
	}

	private void MoveMoney(IStorageComponent from, IStorageComponent to, int count) {
		var all = from.Items.ToList().FindAll(x => x.Groups != null && x.Groups.Contains(StorableGroup.Money));
		if (all.Count == 0)
			return;
		foreach (var storable in all) {
			var count1 = Mathf.Min(storable.Count, count);
			count -= count1;
			if (count1 > 0) {
				if (storable.Count == count1)
					MoveItem(storable, to);
				else {
					var storableComponent = storable.Split(count1);
					to.AddItem(storableComponent,
						to.Containers.First(x => x.GetLimitations().Contains(StorableGroup.Money)));
				}
			}

			if (count <= 0)
				break;
		}

		StorableComponentUtility.PlayUseSound(all[0]);
	}

	protected override void Update() {
		base.Update();
		if (!(windowInfoNew != null) || windowInfoNew.Target == null || windowInfoNew.Target.IsDisposed)
			return;
		if (windowInfoNew.Target.Storage == Actor || windowInfoNew.Target.Storage == protagonistTable)
			windowInfoNew.Price = (int)GetPrice(market, windowInfoNew.Target, false);
		else
			windowInfoNew.Price = (int)GetPrice(market, windowInfoNew.Target, true);
	}

	private void ResetSelectedItems() {
		foreach (var storableUi in storables.Values)
			if (storableUi != null && storableUi is StorableUITrade)
				(storableUi as StorableUITrade).SetSelectedCount(0);
		SelectedItems = new Dictionary<StorableComponent, int>();
	}

	private void SelectItem(StorableComponent storable, int count) {
		var num1 = 0;
		if (SelectedItems.ContainsKey(storable))
			num1 = SelectedItems[storable];
		var num2 = Mathf.Clamp(num1 + count, 0, storable.Count);
		if (num2 > num1)
			StorableComponentUtility.PlayTakeSound(storable);
		else if (num2 < num1)
			StorableComponentUtility.PlayPutSound(storable);
		var count1 = num2;
		SelectedItems[storable] = count1;
		var storable1 = storables[storable];
		if (storable1 != null && storable1 is StorableUITrade) {
			(storable1 as StorableUITrade).SetSelectedCount(count1);
			if (marketItems.Count > 0)
				currentmarketItemsIndex = marketItems.IndexOf(storable1);
		}

		if (count1 == 0)
			SelectedItems.Remove(storable);
		CalculateResult();
	}

	public override void OnPointerDown(PointerEventData eventData) {
		if (InputService.Instance.JoystickUsed)
			return;
		if (windowContextMenu != null)
			HideContextMenu();
		else {
			if (!intersect.IsIntersected)
				return;
			var storable = intersect.Storables.FirstOrDefault();
			if (storable == null || !ItemIsInteresting(storable))
				return;
			switch (eventData.button) {
				case PointerEventData.InputButton.Left:
					SelectItem(storable, 1);
					break;
				case PointerEventData.InputButton.Right:
					SelectItem(storable, -1);
					break;
			}
		}
	}

	public override void OnPointerClick(PointerEventData eventData) { }

	protected override void OnEnable() {
		Clear();
		base.OnEnable();
		buttonTrade.onClick.AddListener(Accept);
		Unsubscribe();
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, MainControl, true);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Context, MainControl, true);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Split, MainControl, true);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, CancelListener, true);
		ServiceLocator.GetService<GameActionService>()
			.AddListener(GameActionType.BumperSelectionLeft, OnChangeInventory, true);
		ServiceLocator.GetService<GameActionService>()
			.AddListener(GameActionType.BumperSelectionRight, OnChangeInventory, true);
		storage = market.GetComponent<StorageComponent>();
		useMoney = false;
		reputationForGifts = false;
		var component = market.GetComponent<ParametersComponent>();
		if (component != null) {
			var byName1 = component.GetByName<bool>(ParameterNameEnum.UseMoneyInTrade);
			if (byName1 != null)
				useMoney = byName1.Value;
			var byName2 = component.GetByName<bool>(ParameterNameEnum.ReputationForGifts);
			if (byName2 != null)
				reputationForGifts = byName2.Value;
		}

		actors.Clear();
		actors.Add(Actor);
		Build2();
		CreateMarketContainer();
		ResetSelectedItems();
		CheckInterestingItems();
		meter.Reset();
		meter.BarterMode(!useMoney);
		CalculateResult();
		dialogModeController.EnableCameraKind(Market?.Owner);
		dialogModeController.SetDialogMode(Market?.Owner, true);
		if (!InputService.Instance.JoystickUsed)
			return;
		if (storables.Values.FirstOrDefault(s =>
			    s != null && s.Internal != null && ItemIsInteresting(s.Internal) && marketItems.Contains(s)) != null)
			CurrentMode = Modes.Market;
		else if (storables.Values.FirstOrDefault(s =>
			         s != null && s.Internal != null && ItemIsInteresting(s.Internal) && !marketItems.Contains(s)) !=
		         null)
			CurrentMode = Modes.Inventory;
		inventoryselectTipObject.SetActive(
			storables.Values.FirstOrDefault(s =>
				s != null && s.Internal != null && ItemIsInteresting(s.Internal) && marketItems.Contains(s)) != null &&
			storables.Values.FirstOrDefault(s =>
				s != null && s.Internal != null && ItemIsInteresting(s.Internal) && !marketItems.Contains(s)) != null);
	}

	protected override void OnDisable() {
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, MainControl);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Context, MainControl);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Split, MainControl);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, CancelListener);
		buttonTrade.onClick.RemoveListener(Accept);
		ServiceLocator.GetService<GameActionService>()
			.RemoveListener(GameActionType.BumperSelectionLeft, OnChangeInventory);
		ServiceLocator.GetService<GameActionService>()
			.RemoveListener(GameActionType.BumperSelectionRight, OnChangeInventory);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickUp, MainControl);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickDown, MainControl);
		DestroyContainers();
		dialogModeController.DisableCameraKind();
		dialogModeController.SetDialogMode(Market?.Owner, false);
		CurrentMode = Modes.None;
		selectedStorable = null;
		currentmarketItemsIndex = -1;
		SelectedItems.Clear();
		base.OnDisable();
	}

	private void ModeBasedSelection(Modes mode) {
		switch (mode) {
			case Modes.None:
				selectedStorable = null;
				return;
			case Modes.Inventory:
				if (selectedStorable != null) {
					if (selectedStorable.IsSelected())
						return;
					selectedStorable.SetSelected(true);
					return;
				}

				if (storables.Count > 0) {
					var storableUi =
						storables.Values.FirstOrDefault(s => ItemIsInteresting(s.Internal) && !marketItems.Contains(s));
					if (storableUi != null)
						selectedStorable = storableUi;
				}

				break;
			case Modes.Market:
				selectedStorable?.SetSelected(false);
				marketItems = marketContainer.ItemsUI;
				marketItems = marketItems.Where(item => item != null).ToList();
				if (marketItems.Count == 0)
					return;
				if (currentmarketItemsIndex < 0)
					currentmarketItemsIndex = 0;
				else if (currentmarketItemsIndex >= marketItems.Count)
					currentmarketItemsIndex = 0;
				selectedStorable = marketItems[currentmarketItemsIndex];
				marketContainer.ScrollTo(currentmarketItemsIndex, marketItems.Count);
				break;
		}

		selectedStorable?.SetSelected(true);
	}

	private Modes CurrentMode {
		get => _currentMode;
		set {
			if (_currentMode == value)
				return;
			switch (value) {
				case Modes.Inventory:
					SetInfoWindowShowMode();
					selectedStorable = null;
					if (marketItems != null && marketItems.Count != 0)
						using (var enumerator = marketItems.GetEnumerator()) {
							while (enumerator.MoveNext()) {
								var current = enumerator.Current;
								if (current != null)
									current.SetSelected(false);
							}

							break;
						}

					break;
				case Modes.Market:
					SetInfoWindowShowMode(true);
					if (_currentMode == Modes.Inventory || _currentMode == Modes.None) {
						if (selectedStorable != null)
							selectedStorable.SetSelected(false);
						selectedStorable = null;
					}

					break;
			}

			LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
			_currentMode = value;
			if (_currentMode != 0)
				ModeBasedSelection(value);
			if (_currentMode == Modes.Market) {
				if (marketItems == null)
					return;
				selectedStorable =
					marketItems[currentmarketItemsIndex >= 0 ? currentmarketItemsIndex : marketItems.Count - 1];
				selectedStorable.SetSelected(true);
				ShowInfoWindow(selectedStorable.Internal);
			} else {
				if (!(selectedStorable != null))
					return;
				selectedStorable.SetSelected(true);
				ShowInfoWindow(selectedStorable.Internal);
			}
		}
	}

	protected override void OnNavigate(
		Navigation navigation) {
		if (CurrentMode == Modes.Market && (navigation == Navigation.CellUp || Navigation.CellDown == navigation)) {
			currentmarketItemsIndex += navigation == Navigation.CellUp ? -1 : 1;
			if (currentmarketItemsIndex < 0)
				currentmarketItemsIndex = marketItems.Count - 1;
			if (currentmarketItemsIndex >= marketItems.Count)
				currentmarketItemsIndex = 0;
			selectedStorable.SetSelected(false);
			selectedStorable = marketItems[currentmarketItemsIndex];
			selectedStorable.SetSelected(true);
			marketContainer.ScrollTo(currentmarketItemsIndex, marketItems.Count);
			ShowInfoWindow(selectedStorable.Internal);
		} else
			base.OnNavigate(navigation);
	}

	private bool MainControl(GameActionType type, bool down) {
		if ((type == GameActionType.Context) & down && joystickAccept.activeSelf) {
			selectedStorable = null;
			buttonTrade?.onClick.Invoke();
			HideInfoWindow();
			CurrentMode = Modes.None;
			CurrentMode = Modes.Inventory;
			return true;
		}

		if (((type == GameActionType.Submit ? 1 : type == GameActionType.Split ? 1 : 0) & (down ? 1 : 0)) == 0 ||
		    selectedStorable == null)
			return false;
		var storableComponent = (StorableComponent)selectedStorable.Internal;
		if (storableComponent == null || !ItemIsInteresting(storableComponent))
			return false;
		if (SelectedItems.ContainsKey(storableComponent) || type == GameActionType.Submit) {
			SelectItem(storableComponent, type == GameActionType.Submit ? 1 : -1);
			if (!SelectedItems.ContainsKey(storableComponent))
				selectedStorable.SetSelected(true);
		}

		return true;
	}

	public override bool HaveToFindSelected() {
		return false;
	}

	private bool OnChangeInventory(GameActionType type, bool down) {
		if ((type == GameActionType.BumperSelectionLeft) & down) {
			if (storables.Values.FirstOrDefault(s =>
				    s != null && s.Internal != null && ItemIsInteresting(s.Internal) && marketItems.Contains(s)) !=
			    null) {
				CurrentMode = Modes.Market;
				return true;
			}
		} else if ((type == GameActionType.BumperSelectionRight) & down && storables.Values.FirstOrDefault(s =>
			           s != null && s.Internal != null && ItemIsInteresting(s.Internal) && !marketItems.Contains(s)) !=
		           null) {
			CurrentMode = Modes.Inventory;
			return true;
		}

		return false;
	}

	protected override void AdditionalAfterChangeAction() {
		CurrentMode = Modes.None;
		if (InputService.Instance.JoystickUsed) {
			if (storables.Values.FirstOrDefault(s =>
				    s != null && s.Internal != null && ItemIsInteresting(s.Internal) && !marketItems.Contains(s)) !=
			    null)
				CurrentMode = Modes.Inventory;
			else if (storables.Values.FirstOrDefault(s =>
				         s != null && s.Internal != null && ItemIsInteresting(s.Internal) && marketItems.Contains(s)) !=
			         null)
				CurrentMode = Modes.Market;
			inventoryselectTipObject.SetActive(
				storables.Values.FirstOrDefault(s =>
					s != null && s.Internal != null && ItemIsInteresting(s.Internal) && marketItems.Contains(s)) !=
				null && storables.Values.FirstOrDefault(s =>
					s != null && s.Internal != null && ItemIsInteresting(s.Internal) && !marketItems.Contains(s)) !=
				null);
		}

		LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
	}

	protected override void OnJoystick(bool joystick) {
		base.OnJoystick(joystick);
		controlPanel.gameObject.SetActive(joystick);
		buttonTrade.gameObject.SetActive(!joystick);
		ServiceLocator.GetService<GameActionService>();
		if (storables != null && storables.Count > 0)
			foreach (var selectedItem in SelectedItems) {
				var selected = selectedItem;
				var storableUi = storables.Values.FirstOrDefault(s => s.Internal == selected.Key);
				if (storableUi != null)
					(storableUi as StorableUITrade).SetSelectedCount(selected.Value, true);
			}

		if (joystick) {
			ServiceLocator.GetService<GameActionService>()
				.RemoveListener(GameActionType.Trade, WithoutJoystickCancelListener);
			ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, MainControl, true);
			ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Context, MainControl, true);
			ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Split, MainControl, true);
			ServiceLocator.GetService<GameActionService>()
				.AddListener(GameActionType.BumperSelectionLeft, OnChangeInventory, true);
			ServiceLocator.GetService<GameActionService>()
				.AddListener(GameActionType.BumperSelectionRight, OnChangeInventory, true);
		} else {
			ServiceLocator.GetService<GameActionService>()
				.AddListener(GameActionType.Trade, WithoutJoystickCancelListener);
			ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, MainControl);
			ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Context, MainControl);
			ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Split, MainControl);
			ServiceLocator.GetService<GameActionService>()
				.RemoveListener(GameActionType.BumperSelectionLeft, OnChangeInventory);
			ServiceLocator.GetService<GameActionService>()
				.RemoveListener(GameActionType.BumperSelectionRight, OnChangeInventory);
		}
	}

	protected override void PositionWindow(UIControl window, IStorableComponent storable) {
		if (InputService.Instance.JoystickUsed) {
			if (!storables.ContainsKey(storable))
				return;
			var storable1 = storables[storable];
			var num1 = 20f;
			if (!(storable1 != null))
				return;
			var component1 = window.GetComponent<RectTransform>();
			var componentInParent = storable1.GetComponentInParent<ContainerResizableWindow>();
			var num2 = num1 * 2f;
			if (componentInParent != null)
				num2 = HintsBottomBorder;
			var rect = storable1.Image.rectTransform.rect;
			var num3 = rect.height / 2f * storable1.Image.rectTransform.lossyScale.y;
			var num4 = storable1.Image.transform.position.y + (double)num3;
			rect = component1.rect;
			var num5 = rect.height * (double)component1.lossyScale.y;
			if (num4 - num5 < num2) {
				rect = component1.rect;
				double height = rect.height;
				rect = storable1.Image.rectTransform.rect;
				var num6 = rect.height / 2.0;
				num3 = (float)(height - num6) * storable1.Image.rectTransform.lossyScale.y;
			}

			float num7;
			if (componentInParent != null) {
				var containerRect = componentInParent.GetContainerRect();
				double x = containerRect.position.x;
				rect = component1.rect;
				var num8 = rect.width * (double)component1.lossyScale.x;
				num7 = (float)(x - num8 - storable1.Image.transform.position.x -
				               num1 * (double)containerRect.lossyScale.x / 2.0);
			} else {
				var component2 = marketContainer.GetComponent<RectTransform>();
				rect = component2.rect;
				num7 = (float)(rect.width * (double)component2.lossyScale.x / 2.0 +
				               num1 * (double)component2.lossyScale.x / 2.0);
			}

			window.Transform.position = new Vector3(storable1.Image.transform.position.x + num7,
				storable1.Image.transform.position.y + num3);
		} else
			base.PositionWindow(window, storable);
	}

	protected void CalculateResult() {
		protagonistPrice = 0;
		marketPrice = 0;
		foreach (var selectedItem in SelectedItems)
			if (selectedItem.Key.Storage == Actor)
				protagonistPrice += (int)GetPrice(market, selectedItem.Key, false, selectedItem.Value);
			else
				marketPrice += (int)GetPrice(market, selectedItem.Key, true, selectedItem.Value);
		protagonistPrice = (int)Mathf.Ceil(protagonistPrice);
		marketPrice = (int)Mathf.Ceil(marketPrice);
		moneyDiff = protagonistPrice - marketPrice;
		if (meter != null) {
			meter.TargetValue = marketPrice;
			meter.CurrentValue = protagonistPrice;
		}

		UpdateCoins();
	}

	protected override bool ItemIsInteresting(IStorableComponent item) {
		var isSeller = item.Storage != Actor && item.Storage != protagonistTable;
		return (int)GetPrice(market, item, isSeller) > 0;
	}

	private void CheckInterestingItems() {
		foreach (var key in storables.Keys) {
			var flag = key.Storage != Actor && key.Storage != protagonistTable;
			storables[key].Enable(ItemIsInteresting(key));
		}
	}

	private void DestroyContainers() {
		ClearMarketContainer();
		storage = null;
	}

	protected override bool ValidateContainer(
		IInventoryComponent container,
		IStorageComponent storage) {
		return base.ValidateContainer(container, storage) &&
		       (container.GetGroup() == InventoryGroup.Trade || storage == Actor);
	}

	public override void Initialize() {
		RegisterLayer<ITradeWindow>(this);
		base.Initialize();
	}

	public override Type GetWindowType() {
		return typeof(ITradeWindow);
	}

	protected override void OnSelectObject(GameObject selected) {
		base.OnSelectObject(selected);
		if (!(selected != null))
			return;
		CurrentMode = !(selected.GetComponentInParent<ItemsSlidingContainer>() != null)
			? Modes.Inventory
			: Modes.Market;
	}

	protected override bool AdditionalConditionOfSelectableList(StorableUI storable) {
		return base.AdditionalConditionOfSelectableList(storable) && ItemIsInteresting(storable.Internal);
	}

	private static float GetPrice(
		IMarketComponent storage,
		IStorableComponent storable,
		bool isSeller,
		int count = 1) {
		return isSeller
			? count * storable.Invoice.SellPrice
			: count * storable.Invoice.BuyPrice * GetDurabilityModifier(storable);
	}

	private static float GetDurabilityModifier(IStorableComponent storable) {
		var durabilityModifier = 1f;
		if (storable.Durability != null && storable.Durability.MaxValue > 0.0)
			durabilityModifier = Mathf.Max(storable.Durability.Value / storable.Durability.MaxValue, 0.1f);
		return durabilityModifier;
	}

	protected override bool WithPrice() {
		return true;
	}

	private void CountCoins() {
		protagonistCoins = 0;
		foreach (var storableComponent in Actor.Items)
			if (storableComponent.Groups.Contains(StorableGroup.Money))
				protagonistCoins += storableComponent.Count;
		marketCoins = 0;
		foreach (var storableComponent in storage.Items)
			if (storableComponent.Groups.Contains(StorableGroup.Money))
				marketCoins += storableComponent.Count;
		meter.StoredCoins = useMoney ? protagonistCoins : 0;
		meter.MarketCoins = useMoney ? marketCoins : 0;
	}

	protected override void UpdateCoins() {
		CountCoins();
		marketMoney.gameObject.SetActive(useMoney);
		if (useMoney) {
			var change = Mathf.Min(moneyDiff, marketCoins);
			protagonistMoney.SetCount(protagonistCoins, change);
			marketMoney.SetCount(marketCoins, -change);
		} else
			protagonistMoney.SetCount(protagonistCoins, 0);

		var flag = true;
		if (protagonistPrice == 0 && marketPrice == 0)
			flag = false;
		if (!useMoney && protagonistPrice < marketPrice)
			flag = false;
		if (useMoney && protagonistPrice + protagonistCoins < marketPrice)
			flag = false;
		buttonTrade.interactable = flag;
		joystickAccept.SetActive(flag);
	}

	protected override void ShowInfoWindow(IStorableComponent storable) {
		LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
		CoroutineService.Instance.WaitFrame(1, (Action)(() => {
			base.ShowInfoWindow(storable);
			if (!(windowInfoNew != null))
				return;
			windowInfoNew.BarterMode(!useMoney);
		}));
	}

	protected override void SelectFirstStorableInContainer(List<StorableUI> storables) { }

	protected override void OnInvalidate() {
		CreateMarketContainer();
		base.OnInvalidate();
		CheckInterestingItems();
	}

	protected override void AddActionsToInfoWindow(
		InfoWindowNew window,
		IStorableComponent storable) {
		if (!ItemIsInteresting(storable))
			return;
		window.AddActionTooltip(GameActionType.Submit, "{StorableTooltip.CountUp}");
		if (InputService.Instance.JoystickUsed)
			window.AddActionTooltip(GameActionType.Split, "{StorableTooltip.CountDown}");
		else
			window.AddActionTooltip(GameActionType.Context, "{StorableTooltip.CountDown}");
	}

	private enum Modes {
		None,
		Inventory,
		Market
	}
}