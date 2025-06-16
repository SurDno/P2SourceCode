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

namespace Engine.Impl.UI.Menu.Protagonist.Inventory
{
  public class TradeWindow : BaseInventoryWindow<TradeWindow>, ITradeWindow, IWindow
  {
    [SerializeField]
    [FormerlySerializedAs("_Button_Trade")]
    private Button buttonTrade;
    [SerializeField]
    private CompareMeterLine meter;
    [SerializeField]
    private ItemsSlidingContainer marketContainer;
    [SerializeField]
    private TradePriceItem protagonistMoney;
    [SerializeField]
    private TradePriceItem marketMoney;
    [SerializeField]
    private GameObject joystickAccept;
    [SerializeField]
    private GameObject inventoryselectTipObject;
    private MarketComponent market;
    private StorageComponent storage;
    private IStorageComponent protagonistTable;
    private IStorageComponent marketTable;
    private Dictionary<IStorageComponent, HashSet<IStorageComponent>> disallowedTransactions = new Dictionary<IStorageComponent, HashSet<IStorageComponent>>();
    private int protagonistPrice;
    private int marketPrice;
    private bool useMoney;
    private bool reputationForGifts;
    private int protagonistCoins;
    private int marketCoins;
    private int moneyDiff;
    private int currentmarketItemsIndex = -1;
    public Dictionary<StorableComponent, int> SelectedItems = new Dictionary<StorableComponent, int>();
    private DialogModeController dialogModeController = new DialogModeController {
      TargetCameraKind = CameraKindEnum.Trade
    };
    private Modes _currentMode = Modes.None;

    private List<StorableUI> marketItems { get; set; }

    public IMarketComponent Market
    {
      get => market;
      set => market = (MarketComponent) value;
    }

    private void ClearMarketContainer()
    {
      marketContainer.Clear(containers, storables);
    }

    private void CreateMarketContainer()
    {
      List<StorableComponent> all = storage.Items.Cast<StorableComponent>().ToList().FindAll(x =>
      {
        if (x != null)
          return ValidateContainer(x.Container, x.Storage) && (ItemIsInteresting(x) || IsDebugMode());
        Debug.LogError((object) "x == null");
        return false;
      });
      ClearMarketContainer();
      marketContainer.CreateSlots(all, storage, containers, storables);
      marketItems = marketContainer.ItemsUI;
    }

    public void Accept()
    {
      if (!useMoney && protagonistPrice < marketPrice || protagonistPrice + protagonistCoins < marketPrice)
        return;
      if (useMoney)
      {
        int num = Mathf.Abs(moneyDiff);
        if (moneyDiff > 0)
          MoveMoney(storage, Actor, num);
        else if (moneyDiff < 0)
        {
          IStorableComponent storableComponent = Actor.Items.ToList().Find(x => x.Groups != null && x.Groups.Contains(StorableGroup.Money));
          if (storableComponent != null)
            num = Mathf.Min(storableComponent.Count, num);
          MoveMoney(Actor, storage, num);
        }
      }
      Dictionary<StorableComponent, int> dictionary1 = new Dictionary<StorableComponent, int>();
      Dictionary<StorableComponent, int> dictionary2 = new Dictionary<StorableComponent, int>();
      if (SelectedItems.Count > 0)
      {
        foreach (KeyValuePair<StorableComponent, int> selectedItem in SelectedItems)
        {
          StorableComponent key = selectedItem.Key;
          if (storables.ContainsKey(key))
          {
            if (storables[key] is StorableUITrade)
              (storables[key] as StorableUITrade).SetSelectedCount(0);
            if (key.Storage == Actor)
              dictionary1[key] = selectedItem.Value;
            else if (key.Storage == storage)
              dictionary2[key] = selectedItem.Value;
          }
        }
      }
      foreach (KeyValuePair<StorableComponent, int> keyValuePair in dictionary1)
      {
        StorableComponent key = keyValuePair.Key;
        if (key.Count == keyValuePair.Value)
        {
          MoveItem(key, storage, storage.Containers.First(x => ValidateContainer(x, storage)));
          selectedStorable = null;
          ModeBasedSelection(Modes.Inventory);
        }
        else
          storage.AddItem(key.Split(keyValuePair.Value), storage.Containers.First(x => ValidateContainer(x, storage)));
      }
      foreach (KeyValuePair<StorableComponent, int> keyValuePair in dictionary2)
      {
        StorableComponent key = keyValuePair.Key;
        StorableComponent storable = key;
        bool flag = false;
        if (key.Count != keyValuePair.Value)
        {
          storable = (StorableComponent) key.Split(keyValuePair.Value);
          flag = true;
        }
        IStorageComponent actor = Actor;
        Intersect intersect = StorageUtility.GetIntersect(Actor, null, storable, null);
        if (!storable.IsDisposed)
        {
          if (!intersect.IsAllowed)
          {
            ServiceLocator.GetService<DropBagService>().DropBag(storable, Actor.Owner);
            StartOpenAudio(dropItemAudio);
          }
          else if (flag)
          {
            actor.AddItem(storable, null);
          }
          else
          {
            MoveItem(storable, Actor);
            selectedStorable = null;
            ModeBasedSelection(Modes.Inventory);
          }
        }
      }
      if (protagonistPrice > 0 || marketPrice > 0)
        ServiceLocator.GetService<LogicEventService>().FireEntityEvent("TradeSuccesful", Market.Owner);
      if (!useMoney && reputationForGifts)
      {
        int gift = protagonistPrice - marketPrice;
        if (gift > 0)
          Actor.GetComponent<PlayerControllerComponent>().ComputeGiftNPC(Market?.Owner, gift);
      }
      ResetSelectedItems();
      CalculateResult();
      OnInvalidate();
      ModeBasedSelection(CurrentMode);
    }

    private void MoveMoney(IStorageComponent from, IStorageComponent to, int count)
    {
      List<IStorableComponent> all = from.Items.ToList().FindAll(x => x.Groups != null && x.Groups.Contains(StorableGroup.Money));
      if (all.Count == 0)
        return;
      foreach (IStorableComponent storable in all)
      {
        int count1 = Mathf.Min(storable.Count, count);
        count -= count1;
        if (count1 > 0)
        {
          if (storable.Count == count1)
          {
            MoveItem(storable, to);
          }
          else
          {
            IStorableComponent storableComponent = storable.Split(count1);
            to.AddItem(storableComponent, to.Containers.First(x => x.GetLimitations().Contains(StorableGroup.Money)));
          }
        }
        if (count <= 0)
          break;
      }
      StorableComponentUtility.PlayUseSound(all[0]);
    }

    protected override void Update()
    {
      base.Update();
      if (!((UnityEngine.Object) windowInfoNew != (UnityEngine.Object) null) || windowInfoNew.Target == null || windowInfoNew.Target.IsDisposed)
        return;
      if (windowInfoNew.Target.Storage == Actor || windowInfoNew.Target.Storage == protagonistTable)
        windowInfoNew.Price = (int) GetPrice(market, windowInfoNew.Target, false);
      else
        windowInfoNew.Price = (int) GetPrice(market, windowInfoNew.Target, true);
    }

    private void ResetSelectedItems()
    {
      foreach (StorableUI storableUi in storables.Values)
      {
        if ((UnityEngine.Object) storableUi != (UnityEngine.Object) null && storableUi is StorableUITrade)
          (storableUi as StorableUITrade).SetSelectedCount(0);
      }
      SelectedItems = new Dictionary<StorableComponent, int>();
    }

    private void SelectItem(StorableComponent storable, int count)
    {
      int num1 = 0;
      if (SelectedItems.ContainsKey(storable))
        num1 = SelectedItems[storable];
      int num2 = Mathf.Clamp(num1 + count, 0, storable.Count);
      if (num2 > num1)
        StorableComponentUtility.PlayTakeSound(storable);
      else if (num2 < num1)
        StorableComponentUtility.PlayPutSound(storable);
      int count1 = num2;
      SelectedItems[storable] = count1;
      StorableUI storable1 = storables[storable];
      if ((UnityEngine.Object) storable1 != (UnityEngine.Object) null && storable1 is StorableUITrade)
      {
        (storable1 as StorableUITrade).SetSelectedCount(count1);
        if (marketItems.Count > 0)
          currentmarketItemsIndex = marketItems.IndexOf(storable1);
      }
      if (count1 == 0)
        SelectedItems.Remove(storable);
      CalculateResult();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
      if (InputService.Instance.JoystickUsed)
        return;
      if ((UnityEngine.Object) windowContextMenu != (UnityEngine.Object) null)
      {
        HideContextMenu();
      }
      else
      {
        if (!intersect.IsIntersected)
          return;
        StorableComponent storable = intersect.Storables.FirstOrDefault();
        if (storable == null || !ItemIsInteresting(storable))
          return;
        switch (eventData.button)
        {
          case PointerEventData.InputButton.Left:
            SelectItem(storable, 1);
            break;
          case PointerEventData.InputButton.Right:
            SelectItem(storable, -1);
            break;
        }
      }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
    }

    protected override void OnEnable()
    {
      Clear();
      base.OnEnable();
      buttonTrade.onClick.AddListener(new UnityAction(Accept));
      Unsubscribe();
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, MainControl, true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Context, MainControl, true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Split, MainControl, true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener), true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.BumperSelectionLeft, OnChangeInventory, true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.BumperSelectionRight, OnChangeInventory, true);
      storage = market.GetComponent<StorageComponent>();
      useMoney = false;
      reputationForGifts = false;
      ParametersComponent component = market.GetComponent<ParametersComponent>();
      if (component != null)
      {
        IParameter<bool> byName1 = component.GetByName<bool>(ParameterNameEnum.UseMoneyInTrade);
        if (byName1 != null)
          useMoney = byName1.Value;
        IParameter<bool> byName2 = component.GetByName<bool>(ParameterNameEnum.ReputationForGifts);
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
      if ((UnityEngine.Object) storables.Values.FirstOrDefault(s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.Internal != null && ItemIsInteresting(s.Internal) && marketItems.Contains(s)) != (UnityEngine.Object) null)
        CurrentMode = Modes.Market;
      else if ((UnityEngine.Object) storables.Values.FirstOrDefault(s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.Internal != null && ItemIsInteresting(s.Internal) && !marketItems.Contains(s)) != (UnityEngine.Object) null)
        CurrentMode = Modes.Inventory;
      inventoryselectTipObject.SetActive((UnityEngine.Object) storables.Values.FirstOrDefault(s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.Internal != null && ItemIsInteresting(s.Internal) && marketItems.Contains(s)) != (UnityEngine.Object) null && (UnityEngine.Object) storables.Values.FirstOrDefault(s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.Internal != null && ItemIsInteresting(s.Internal) && !marketItems.Contains(s)) != (UnityEngine.Object) null);
    }

    protected override void OnDisable()
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, MainControl);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Context, MainControl);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Split, MainControl);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      buttonTrade.onClick.RemoveListener(new UnityAction(Accept));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.BumperSelectionLeft, OnChangeInventory);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.BumperSelectionRight, OnChangeInventory);
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

    private void ModeBasedSelection(Modes mode)
    {
      switch (mode)
      {
        case Modes.None:
          selectedStorable = null;
          return;
        case Modes.Inventory:
          if ((UnityEngine.Object) selectedStorable != (UnityEngine.Object) null)
          {
            if (selectedStorable.IsSelected())
              return;
            selectedStorable.SetSelected(true);
            return;
          }
          if (storables.Count > 0)
          {
            StorableUI storableUi = storables.Values.FirstOrDefault(s => ItemIsInteresting(s.Internal) && !marketItems.Contains(s));
            if ((UnityEngine.Object) storableUi != (UnityEngine.Object) null)
              selectedStorable = storableUi;
          }
          break;
        case Modes.Market:
          selectedStorable?.SetSelected(false);
          marketItems = marketContainer.ItemsUI;
          marketItems = marketItems.Where(item => (UnityEngine.Object) item != (UnityEngine.Object) null).ToList();
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

    private Modes CurrentMode
    {
      get => _currentMode;
      set
      {
        if (_currentMode == value)
          return;
        switch (value)
        {
          case Modes.Inventory:
            SetInfoWindowShowMode();
            selectedStorable = null;
            if (marketItems != null && marketItems.Count != 0)
            {
              using (List<StorableUI>.Enumerator enumerator = marketItems.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  StorableUI current = enumerator.Current;
                  if ((UnityEngine.Object) current != (UnityEngine.Object) null)
                    current.SetSelected(false);
                }
              }
            }

            break;
          case Modes.Market:
            SetInfoWindowShowMode(true);
            if (_currentMode == Modes.Inventory || _currentMode == Modes.None)
            {
              if ((UnityEngine.Object) selectedStorable != (UnityEngine.Object) null)
                selectedStorable.SetSelected(false);
              selectedStorable = null;
            }
            break;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
        _currentMode = value;
        if (_currentMode != 0)
          ModeBasedSelection(value);
        if (_currentMode == Modes.Market)
        {
          if (marketItems == null)
            return;
          selectedStorable = marketItems[currentmarketItemsIndex >= 0 ? currentmarketItemsIndex : marketItems.Count - 1];
          selectedStorable.SetSelected(true);
          ShowInfoWindow(selectedStorable.Internal);
        }
        else
        {
          if (!((UnityEngine.Object) selectedStorable != (UnityEngine.Object) null))
            return;
          selectedStorable.SetSelected(true);
          ShowInfoWindow(selectedStorable.Internal);
        }
      }
    }

    protected override void OnNavigate(
      Navigation navigation)
    {
      if (CurrentMode == Modes.Market && (navigation == Navigation.CellUp || Navigation.CellDown == navigation))
      {
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
      }
      else
        base.OnNavigate(navigation);
    }

    private bool MainControl(GameActionType type, bool down)
    {
      if (type == GameActionType.Context & down && joystickAccept.activeSelf)
      {
        selectedStorable = null;
        buttonTrade?.onClick.Invoke();
        HideInfoWindow();
        CurrentMode = Modes.None;
        CurrentMode = Modes.Inventory;
        return true;
      }
      if (((type == GameActionType.Submit ? 1 : (type == GameActionType.Split ? 1 : 0)) & (down ? 1 : 0)) == 0 || (UnityEngine.Object) selectedStorable == (UnityEngine.Object) null)
        return false;
      StorableComponent storableComponent = (StorableComponent) selectedStorable.Internal;
      if (storableComponent == null || !ItemIsInteresting(storableComponent))
        return false;
      if (SelectedItems.ContainsKey(storableComponent) || type == GameActionType.Submit)
      {
        SelectItem(storableComponent, type == GameActionType.Submit ? 1 : -1);
        if (!SelectedItems.ContainsKey(storableComponent))
          selectedStorable.SetSelected(true);
      }
      return true;
    }

    public override bool HaveToFindSelected() => false;

    private bool OnChangeInventory(GameActionType type, bool down)
    {
      if (type == GameActionType.BumperSelectionLeft & down)
      {
        if ((UnityEngine.Object) storables.Values.FirstOrDefault(s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.Internal != null && ItemIsInteresting(s.Internal) && marketItems.Contains(s)) != (UnityEngine.Object) null)
        {
          CurrentMode = Modes.Market;
          return true;
        }
      }
      else if (type == GameActionType.BumperSelectionRight & down && (UnityEngine.Object) storables.Values.FirstOrDefault(s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.Internal != null && ItemIsInteresting(s.Internal) && !marketItems.Contains(s)) != (UnityEngine.Object) null)
      {
        CurrentMode = Modes.Inventory;
        return true;
      }
      return false;
    }

    protected override void AdditionalAfterChangeAction()
    {
      CurrentMode = Modes.None;
      if (InputService.Instance.JoystickUsed)
      {
        if ((UnityEngine.Object) storables.Values.FirstOrDefault(s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.Internal != null && ItemIsInteresting(s.Internal) && !marketItems.Contains(s)) != (UnityEngine.Object) null)
          CurrentMode = Modes.Inventory;
        else if ((UnityEngine.Object) storables.Values.FirstOrDefault(s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.Internal != null && ItemIsInteresting(s.Internal) && marketItems.Contains(s)) != (UnityEngine.Object) null)
          CurrentMode = Modes.Market;
        inventoryselectTipObject.SetActive((UnityEngine.Object) storables.Values.FirstOrDefault(s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.Internal != null && ItemIsInteresting(s.Internal) && marketItems.Contains(s)) != (UnityEngine.Object) null && (UnityEngine.Object) storables.Values.FirstOrDefault(s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.Internal != null && ItemIsInteresting(s.Internal) && !marketItems.Contains(s)) != (UnityEngine.Object) null);
      }
      LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      controlPanel.gameObject.SetActive(joystick);
      buttonTrade.gameObject.SetActive(!joystick);
      ServiceLocator.GetService<GameActionService>();
      if (storables != null && storables.Count > 0)
      {
        foreach (KeyValuePair<StorableComponent, int> selectedItem in SelectedItems)
        {
          KeyValuePair<StorableComponent, int> selected = selectedItem;
          StorableUI storableUi = storables.Values.FirstOrDefault(s => s.Internal == selected.Key);
          if ((UnityEngine.Object) storableUi != (UnityEngine.Object) null)
            (storableUi as StorableUITrade).SetSelectedCount(selected.Value, true);
        }
      }
      if (joystick)
      {
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Trade, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, MainControl, true);
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Context, MainControl, true);
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Split, MainControl, true);
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.BumperSelectionLeft, OnChangeInventory, true);
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.BumperSelectionRight, OnChangeInventory, true);
      }
      else
      {
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Trade, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, MainControl);
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Context, MainControl);
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Split, MainControl);
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.BumperSelectionLeft, OnChangeInventory);
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.BumperSelectionRight, OnChangeInventory);
      }
    }

    protected override void PositionWindow(UIControl window, IStorableComponent storable)
    {
      if (InputService.Instance.JoystickUsed)
      {
        if (!storables.ContainsKey(storable))
          return;
        StorableUI storable1 = storables[storable];
        float num1 = 20f;
        if (!((UnityEngine.Object) storable1 != (UnityEngine.Object) null))
          return;
        RectTransform component1 = window.GetComponent<RectTransform>();
        ContainerResizableWindow componentInParent = storable1.GetComponentInParent<ContainerResizableWindow>();
        float num2 = num1 * 2f;
        if ((UnityEngine.Object) componentInParent != (UnityEngine.Object) null)
          num2 = HintsBottomBorder;
        Rect rect = storable1.Image.rectTransform.rect;
        float num3 = rect.height / 2f * storable1.Image.rectTransform.lossyScale.y;
        double num4 = (double) storable1.Image.transform.position.y + num3;
        rect = component1.rect;
        double num5 = (double) rect.height * (double) component1.lossyScale.y;
        if (num4 - num5 < num2)
        {
          rect = component1.rect;
          double height = (double) rect.height;
          rect = storable1.Image.rectTransform.rect;
          double num6 = (double) rect.height / 2.0;
          num3 = (float) (height - num6) * storable1.Image.rectTransform.lossyScale.y;
        }
        float num7;
        if ((UnityEngine.Object) componentInParent != (UnityEngine.Object) null)
        {
          RectTransform containerRect = componentInParent.GetContainerRect();
          double x = (double) containerRect.position.x;
          rect = component1.rect;
          double num8 = (double) rect.width * (double) component1.lossyScale.x;
          num7 = (float) (x - num8 - (double) storable1.Image.transform.position.x - num1 * (double) containerRect.lossyScale.x / 2.0);
        }
        else
        {
          RectTransform component2 = marketContainer.GetComponent<RectTransform>();
          rect = component2.rect;
          num7 = (float) ((double) rect.width * (double) component2.lossyScale.x / 2.0 + num1 * (double) component2.lossyScale.x / 2.0);
        }
        window.Transform.position = new Vector3(storable1.Image.transform.position.x + num7, storable1.Image.transform.position.y + num3);
      }
      else
        base.PositionWindow(window, storable);
    }

    protected void CalculateResult()
    {
      protagonistPrice = 0;
      marketPrice = 0;
      foreach (KeyValuePair<StorableComponent, int> selectedItem in SelectedItems)
      {
        if (selectedItem.Key.Storage == Actor)
          protagonistPrice += (int) GetPrice(market, selectedItem.Key, false, selectedItem.Value);
        else
          marketPrice += (int) GetPrice(market, selectedItem.Key, true, selectedItem.Value);
      }
      protagonistPrice = (int) Mathf.Ceil((float) protagonistPrice);
      marketPrice = (int) Mathf.Ceil((float) marketPrice);
      moneyDiff = protagonistPrice - marketPrice;
      if ((UnityEngine.Object) meter != (UnityEngine.Object) null)
      {
        meter.TargetValue = marketPrice;
        meter.CurrentValue = protagonistPrice;
      }
      UpdateCoins();
    }

    protected override bool ItemIsInteresting(IStorableComponent item)
    {
      bool isSeller = item.Storage != Actor && item.Storage != protagonistTable;
      return (int) GetPrice(market, item, isSeller) > 0;
    }

    private void CheckInterestingItems()
    {
      foreach (IStorableComponent key in storables.Keys)
      {
        bool flag = key.Storage != Actor && key.Storage != protagonistTable;
        storables[key].Enable(ItemIsInteresting(key));
      }
    }

    private void DestroyContainers()
    {
      ClearMarketContainer();
      storage = null;
    }

    protected override bool ValidateContainer(
      IInventoryComponent container,
      IStorageComponent storage)
    {
      return base.ValidateContainer(container, storage) && (container.GetGroup() == InventoryGroup.Trade || storage == Actor);
    }

    public override void Initialize()
    {
      RegisterLayer((ITradeWindow) this);
      base.Initialize();
    }

    public override Type GetWindowType() => typeof (ITradeWindow);

    protected override void OnSelectObject(GameObject selected)
    {
      base.OnSelectObject(selected);
      if (!((UnityEngine.Object) selected != (UnityEngine.Object) null))
        return;
      CurrentMode = !((UnityEngine.Object) selected.GetComponentInParent<ItemsSlidingContainer>() != (UnityEngine.Object) null) ? Modes.Inventory : Modes.Market;
    }

    protected override bool AdditionalConditionOfSelectableList(StorableUI storable)
    {
      return base.AdditionalConditionOfSelectableList(storable) && ItemIsInteresting(storable.Internal);
    }

    private static float GetPrice(
      IMarketComponent storage,
      IStorableComponent storable,
      bool isSeller,
      int count = 1)
    {
      return isSeller ? count * storable.Invoice.SellPrice : count * storable.Invoice.BuyPrice * GetDurabilityModifier(storable);
    }

    private static float GetDurabilityModifier(IStorableComponent storable)
    {
      float durabilityModifier = 1f;
      if (storable.Durability != null && storable.Durability.MaxValue > 0.0)
        durabilityModifier = Mathf.Max(storable.Durability.Value / storable.Durability.MaxValue, 0.1f);
      return durabilityModifier;
    }

    protected override bool WithPrice() => true;

    private void CountCoins()
    {
      protagonistCoins = 0;
      foreach (IStorableComponent storableComponent in Actor.Items)
      {
        if (storableComponent.Groups.Contains(StorableGroup.Money))
          protagonistCoins += storableComponent.Count;
      }
      marketCoins = 0;
      foreach (IStorableComponent storableComponent in storage.Items)
      {
        if (storableComponent.Groups.Contains(StorableGroup.Money))
          marketCoins += storableComponent.Count;
      }
      meter.StoredCoins = useMoney ? protagonistCoins : 0;
      meter.MarketCoins = useMoney ? marketCoins : 0;
    }

    protected override void UpdateCoins()
    {
      CountCoins();
      marketMoney.gameObject.SetActive(useMoney);
      if (useMoney)
      {
        int change = Mathf.Min(moneyDiff, marketCoins);
        protagonistMoney.SetCount(protagonistCoins, change);
        marketMoney.SetCount(marketCoins, -change);
      }
      else
        protagonistMoney.SetCount(protagonistCoins, 0);
      bool flag = true;
      if (protagonistPrice == 0 && marketPrice == 0)
        flag = false;
      if (!useMoney && protagonistPrice < marketPrice)
        flag = false;
      if (useMoney && protagonistPrice + protagonistCoins < marketPrice)
        flag = false;
      buttonTrade.interactable = flag;
      joystickAccept.SetActive(flag);
    }

    protected override void ShowInfoWindow(IStorableComponent storable)
    {
      LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
      CoroutineService.Instance.WaitFrame(1, (Action) (() =>
      {
        base.ShowInfoWindow(storable);
        if (!((UnityEngine.Object) windowInfoNew != (UnityEngine.Object) null))
          return;
        windowInfoNew.BarterMode(!useMoney);
      }));
    }

    protected override void SelectFirstStorableInContainer(List<StorableUI> storables)
    {
    }

    protected override void OnInvalidate()
    {
      CreateMarketContainer();
      base.OnInvalidate();
      CheckInterestingItems();
    }

    protected override void AddActionsToInfoWindow(
      InfoWindowNew window,
      IStorableComponent storable)
    {
      if (!ItemIsInteresting(storable))
        return;
      window.AddActionTooltip(GameActionType.Submit, "{StorableTooltip.CountUp}");
      if (InputService.Instance.JoystickUsed)
        window.AddActionTooltip(GameActionType.Split, "{StorableTooltip.CountDown}");
      else
        window.AddActionTooltip(GameActionType.Context, "{StorableTooltip.CountDown}");
    }

    private enum Modes
    {
      None,
      Inventory,
      Market,
    }
  }
}
