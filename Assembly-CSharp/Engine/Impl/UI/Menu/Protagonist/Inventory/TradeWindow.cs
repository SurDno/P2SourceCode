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
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
    private int protagonistCoins = 0;
    private int marketCoins = 0;
    private int moneyDiff = 0;
    private int currentmarketItemsIndex = -1;
    public Dictionary<StorableComponent, int> SelectedItems = new Dictionary<StorableComponent, int>();
    private DialogModeController dialogModeController = new DialogModeController()
    {
      TargetCameraKind = CameraKindEnum.Trade
    };
    private TradeWindow.Modes _currentMode = TradeWindow.Modes.None;

    private List<StorableUI> marketItems { get; set; }

    public IMarketComponent Market
    {
      get => (IMarketComponent) this.market;
      set => this.market = (MarketComponent) value;
    }

    private void ClearMarketContainer()
    {
      this.marketContainer.Clear(this.containers, this.storables);
    }

    private void CreateMarketContainer()
    {
      List<StorableComponent> all = this.storage.Items.Cast<StorableComponent>().ToList<StorableComponent>().FindAll((Predicate<StorableComponent>) (x =>
      {
        if (x != null)
          return this.ValidateContainer(x.Container, x.Storage) && (this.ItemIsInteresting((IStorableComponent) x) || this.IsDebugMode());
        Debug.LogError((object) "x == null");
        return false;
      }));
      this.ClearMarketContainer();
      this.marketContainer.CreateSlots(all, this.storage, this.containers, this.storables);
      this.marketItems = this.marketContainer.ItemsUI;
    }

    public void Accept()
    {
      if (!this.useMoney && this.protagonistPrice < this.marketPrice || this.protagonistPrice + this.protagonistCoins < this.marketPrice)
        return;
      if (this.useMoney)
      {
        int num = Mathf.Abs(this.moneyDiff);
        if (this.moneyDiff > 0)
          this.MoveMoney((IStorageComponent) this.storage, this.Actor, num);
        else if (this.moneyDiff < 0)
        {
          IStorableComponent storableComponent = this.Actor.Items.ToList<IStorableComponent>().Find((Predicate<IStorableComponent>) (x => x.Groups != null && x.Groups.Contains<StorableGroup>(StorableGroup.Money)));
          if (storableComponent != null)
            num = Mathf.Min(storableComponent.Count, num);
          this.MoveMoney(this.Actor, (IStorageComponent) this.storage, num);
        }
      }
      Dictionary<StorableComponent, int> dictionary1 = new Dictionary<StorableComponent, int>();
      Dictionary<StorableComponent, int> dictionary2 = new Dictionary<StorableComponent, int>();
      if (this.SelectedItems.Count > 0)
      {
        foreach (KeyValuePair<StorableComponent, int> selectedItem in this.SelectedItems)
        {
          StorableComponent key = selectedItem.Key;
          if (this.storables.ContainsKey((IStorableComponent) key))
          {
            if (this.storables[(IStorableComponent) key] is StorableUITrade)
              (this.storables[(IStorableComponent) key] as StorableUITrade).SetSelectedCount(0);
            if (key.Storage == this.Actor)
              dictionary1[key] = selectedItem.Value;
            else if (key.Storage == this.storage)
              dictionary2[key] = selectedItem.Value;
          }
        }
      }
      foreach (KeyValuePair<StorableComponent, int> keyValuePair in dictionary1)
      {
        StorableComponent key = keyValuePair.Key;
        if (key.Count == keyValuePair.Value)
        {
          this.MoveItem((IStorableComponent) key, (IStorageComponent) this.storage, this.storage.Containers.First<IInventoryComponent>((Func<IInventoryComponent, bool>) (x => this.ValidateContainer(x, (IStorageComponent) this.storage))));
          this.selectedStorable = (StorableUI) null;
          this.ModeBasedSelection(TradeWindow.Modes.Inventory);
        }
        else
          this.storage.AddItem(key.Split(keyValuePair.Value), this.storage.Containers.First<IInventoryComponent>((Func<IInventoryComponent, bool>) (x => this.ValidateContainer(x, (IStorageComponent) this.storage))));
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
        IStorageComponent actor = this.Actor;
        Intersect intersect = StorageUtility.GetIntersect(this.Actor, (IInventoryComponent) null, storable, (Cell) null);
        if (!storable.IsDisposed)
        {
          if (!intersect.IsAllowed)
          {
            ServiceLocator.GetService<DropBagService>().DropBag((IStorableComponent) storable, this.Actor.Owner);
            this.StartOpenAudio(this.dropItemAudio);
          }
          else if (flag)
          {
            actor.AddItem((IStorableComponent) storable, (IInventoryComponent) null);
          }
          else
          {
            this.MoveItem((IStorableComponent) storable, this.Actor);
            this.selectedStorable = (StorableUI) null;
            this.ModeBasedSelection(TradeWindow.Modes.Inventory);
          }
        }
      }
      if (this.protagonistPrice > 0 || this.marketPrice > 0)
        ServiceLocator.GetService<LogicEventService>().FireEntityEvent("TradeSuccesful", this.Market.Owner);
      if (!this.useMoney && this.reputationForGifts)
      {
        int gift = this.protagonistPrice - this.marketPrice;
        if (gift > 0)
          this.Actor.GetComponent<PlayerControllerComponent>().ComputeGiftNPC(this.Market?.Owner, (float) gift);
      }
      this.ResetSelectedItems();
      this.CalculateResult();
      this.OnInvalidate();
      this.ModeBasedSelection(this.CurrentMode);
    }

    private void MoveMoney(IStorageComponent from, IStorageComponent to, int count)
    {
      List<IStorableComponent> all = from.Items.ToList<IStorableComponent>().FindAll((Predicate<IStorableComponent>) (x => x.Groups != null && x.Groups.Contains<StorableGroup>(StorableGroup.Money)));
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
            this.MoveItem(storable, to);
          }
          else
          {
            IStorableComponent storableComponent = storable.Split(count1);
            to.AddItem(storableComponent, to.Containers.First<IInventoryComponent>((Func<IInventoryComponent, bool>) (x => x.GetLimitations().Contains<StorableGroup>(StorableGroup.Money))));
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
      if (!((UnityEngine.Object) this.windowInfoNew != (UnityEngine.Object) null) || this.windowInfoNew.Target == null || this.windowInfoNew.Target.IsDisposed)
        return;
      if (this.windowInfoNew.Target.Storage == this.Actor || this.windowInfoNew.Target.Storage == this.protagonistTable)
        this.windowInfoNew.Price = (float) (int) TradeWindow.GetPrice((IMarketComponent) this.market, this.windowInfoNew.Target, false);
      else
        this.windowInfoNew.Price = (float) (int) TradeWindow.GetPrice((IMarketComponent) this.market, this.windowInfoNew.Target, true);
    }

    private void ResetSelectedItems()
    {
      foreach (StorableUI storableUi in this.storables.Values)
      {
        if ((UnityEngine.Object) storableUi != (UnityEngine.Object) null && storableUi is StorableUITrade)
          (storableUi as StorableUITrade).SetSelectedCount(0);
      }
      this.SelectedItems = new Dictionary<StorableComponent, int>();
    }

    private void SelectItem(StorableComponent storable, int count)
    {
      int num1 = 0;
      if (this.SelectedItems.ContainsKey(storable))
        num1 = this.SelectedItems[storable];
      int num2 = Mathf.Clamp(num1 + count, 0, storable.Count);
      if (num2 > num1)
        StorableComponentUtility.PlayTakeSound((IStorableComponent) storable);
      else if (num2 < num1)
        StorableComponentUtility.PlayPutSound((IStorableComponent) storable);
      int count1 = num2;
      this.SelectedItems[storable] = count1;
      StorableUI storable1 = this.storables[(IStorableComponent) storable];
      if ((UnityEngine.Object) storable1 != (UnityEngine.Object) null && storable1 is StorableUITrade)
      {
        (storable1 as StorableUITrade).SetSelectedCount(count1);
        if (this.marketItems.Count > 0)
          this.currentmarketItemsIndex = this.marketItems.IndexOf(storable1);
      }
      if (count1 == 0)
        this.SelectedItems.Remove(storable);
      this.CalculateResult();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
      if (InputService.Instance.JoystickUsed)
        return;
      if ((UnityEngine.Object) this.windowContextMenu != (UnityEngine.Object) null)
      {
        this.HideContextMenu();
      }
      else
      {
        if (!this.intersect.IsIntersected)
          return;
        StorableComponent storable = this.intersect.Storables.FirstOrDefault<StorableComponent>();
        if (storable == null || !this.ItemIsInteresting((IStorableComponent) storable))
          return;
        switch (eventData.button)
        {
          case PointerEventData.InputButton.Left:
            this.SelectItem(storable, 1);
            break;
          case PointerEventData.InputButton.Right:
            this.SelectItem(storable, -1);
            break;
        }
      }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
    }

    protected override void OnEnable()
    {
      this.Clear();
      base.OnEnable();
      this.buttonTrade.onClick.AddListener(new UnityAction(this.Accept));
      this.Unsubscribe();
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, new GameActionHandle(this.MainControl), true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Context, new GameActionHandle(this.MainControl), true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Split, new GameActionHandle(this.MainControl), true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener), true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.BumperSelectionLeft, new GameActionHandle(this.OnChangeInventory), true);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.BumperSelectionRight, new GameActionHandle(this.OnChangeInventory), true);
      this.storage = this.market.GetComponent<StorageComponent>();
      this.useMoney = false;
      this.reputationForGifts = false;
      ParametersComponent component = this.market.GetComponent<ParametersComponent>();
      if (component != null)
      {
        IParameter<bool> byName1 = component.GetByName<bool>(ParameterNameEnum.UseMoneyInTrade);
        if (byName1 != null)
          this.useMoney = byName1.Value;
        IParameter<bool> byName2 = component.GetByName<bool>(ParameterNameEnum.ReputationForGifts);
        if (byName2 != null)
          this.reputationForGifts = byName2.Value;
      }
      this.actors.Clear();
      this.actors.Add(this.Actor);
      this.Build2();
      this.CreateMarketContainer();
      this.ResetSelectedItems();
      this.CheckInterestingItems();
      this.meter.Reset();
      this.meter.BarterMode(!this.useMoney);
      this.CalculateResult();
      this.dialogModeController.EnableCameraKind(this.Market?.Owner);
      this.dialogModeController.SetDialogMode(this.Market?.Owner, true);
      if (!InputService.Instance.JoystickUsed)
        return;
      if ((UnityEngine.Object) this.storables.Values.FirstOrDefault<StorableUI>((Func<StorableUI, bool>) (s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.Internal != null && this.ItemIsInteresting(s.Internal) && this.marketItems.Contains(s))) != (UnityEngine.Object) null)
        this.CurrentMode = TradeWindow.Modes.Market;
      else if ((UnityEngine.Object) this.storables.Values.FirstOrDefault<StorableUI>((Func<StorableUI, bool>) (s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.Internal != null && this.ItemIsInteresting(s.Internal) && !this.marketItems.Contains(s))) != (UnityEngine.Object) null)
        this.CurrentMode = TradeWindow.Modes.Inventory;
      this.inventoryselectTipObject.SetActive((UnityEngine.Object) this.storables.Values.FirstOrDefault<StorableUI>((Func<StorableUI, bool>) (s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.Internal != null && this.ItemIsInteresting(s.Internal) && this.marketItems.Contains(s))) != (UnityEngine.Object) null && (UnityEngine.Object) this.storables.Values.FirstOrDefault<StorableUI>((Func<StorableUI, bool>) (s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.Internal != null && this.ItemIsInteresting(s.Internal) && !this.marketItems.Contains(s))) != (UnityEngine.Object) null);
    }

    protected override void OnDisable()
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.MainControl));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Context, new GameActionHandle(this.MainControl));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Split, new GameActionHandle(this.MainControl));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      this.buttonTrade.onClick.RemoveListener(new UnityAction(this.Accept));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.BumperSelectionLeft, new GameActionHandle(this.OnChangeInventory));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.BumperSelectionRight, new GameActionHandle(this.OnChangeInventory));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickUp, new GameActionHandle(this.MainControl));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickDown, new GameActionHandle(this.MainControl));
      this.DestroyContainers();
      this.dialogModeController.DisableCameraKind();
      this.dialogModeController.SetDialogMode(this.Market?.Owner, false);
      this.CurrentMode = TradeWindow.Modes.None;
      this.selectedStorable = (StorableUI) null;
      this.currentmarketItemsIndex = -1;
      this.SelectedItems.Clear();
      base.OnDisable();
    }

    private void ModeBasedSelection(TradeWindow.Modes mode)
    {
      switch (mode)
      {
        case TradeWindow.Modes.None:
          this.selectedStorable = (StorableUI) null;
          return;
        case TradeWindow.Modes.Inventory:
          if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
          {
            if (this.selectedStorable.IsSelected())
              return;
            this.selectedStorable.SetSelected(true);
            return;
          }
          if (this.storables.Count > 0)
          {
            StorableUI storableUi = this.storables.Values.FirstOrDefault<StorableUI>((Func<StorableUI, bool>) (s => this.ItemIsInteresting(s.Internal) && !this.marketItems.Contains(s)));
            if ((UnityEngine.Object) storableUi != (UnityEngine.Object) null)
              this.selectedStorable = storableUi;
            break;
          }
          break;
        case TradeWindow.Modes.Market:
          this.selectedStorable?.SetSelected(false);
          this.marketItems = this.marketContainer.ItemsUI;
          this.marketItems = this.marketItems.Where<StorableUI>((Func<StorableUI, bool>) (item => (UnityEngine.Object) item != (UnityEngine.Object) null)).ToList<StorableUI>();
          if (this.marketItems.Count == 0)
            return;
          if (this.currentmarketItemsIndex < 0)
            this.currentmarketItemsIndex = 0;
          else if (this.currentmarketItemsIndex >= this.marketItems.Count)
            this.currentmarketItemsIndex = 0;
          this.selectedStorable = this.marketItems[this.currentmarketItemsIndex];
          this.marketContainer.ScrollTo(this.currentmarketItemsIndex, this.marketItems.Count);
          break;
      }
      this.selectedStorable?.SetSelected(true);
    }

    private TradeWindow.Modes CurrentMode
    {
      get => this._currentMode;
      set
      {
        if (this._currentMode == value)
          return;
        switch (value)
        {
          case TradeWindow.Modes.Inventory:
            this.SetInfoWindowShowMode();
            this.selectedStorable = (StorableUI) null;
            if (this.marketItems != null && this.marketItems.Count != 0)
            {
              using (List<StorableUI>.Enumerator enumerator = this.marketItems.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  StorableUI current = enumerator.Current;
                  if ((UnityEngine.Object) current != (UnityEngine.Object) null)
                    current.SetSelected(false);
                }
                break;
              }
            }
            else
              break;
          case TradeWindow.Modes.Market:
            this.SetInfoWindowShowMode(true);
            if (this._currentMode == TradeWindow.Modes.Inventory || this._currentMode == TradeWindow.Modes.None)
            {
              if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
                this.selectedStorable.SetSelected(false);
              this.selectedStorable = (StorableUI) null;
            }
            break;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
        this._currentMode = value;
        if (this._currentMode != 0)
          this.ModeBasedSelection(value);
        if (this._currentMode == TradeWindow.Modes.Market)
        {
          if (this.marketItems == null)
            return;
          this.selectedStorable = this.marketItems[this.currentmarketItemsIndex >= 0 ? this.currentmarketItemsIndex : this.marketItems.Count - 1];
          this.selectedStorable.SetSelected(true);
          this.ShowInfoWindow(this.selectedStorable.Internal);
        }
        else
        {
          if (!((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null))
            return;
          this.selectedStorable.SetSelected(true);
          this.ShowInfoWindow(this.selectedStorable.Internal);
        }
      }
    }

    protected override void OnNavigate(
      BaseInventoryWindow<TradeWindow>.Navigation navigation)
    {
      if (this.CurrentMode == TradeWindow.Modes.Market && (navigation == BaseInventoryWindow<TradeWindow>.Navigation.CellUp || BaseInventoryWindow<TradeWindow>.Navigation.CellDown == navigation))
      {
        this.currentmarketItemsIndex += navigation == BaseInventoryWindow<TradeWindow>.Navigation.CellUp ? -1 : 1;
        if (this.currentmarketItemsIndex < 0)
          this.currentmarketItemsIndex = this.marketItems.Count - 1;
        if (this.currentmarketItemsIndex >= this.marketItems.Count)
          this.currentmarketItemsIndex = 0;
        this.selectedStorable.SetSelected(false);
        this.selectedStorable = this.marketItems[this.currentmarketItemsIndex];
        this.selectedStorable.SetSelected(true);
        this.marketContainer.ScrollTo(this.currentmarketItemsIndex, this.marketItems.Count);
        this.ShowInfoWindow(this.selectedStorable.Internal);
      }
      else
        base.OnNavigate(navigation);
    }

    private bool MainControl(GameActionType type, bool down)
    {
      if (type == GameActionType.Context & down && this.joystickAccept.activeSelf)
      {
        this.selectedStorable = (StorableUI) null;
        this.buttonTrade?.onClick.Invoke();
        this.HideInfoWindow();
        this.CurrentMode = TradeWindow.Modes.None;
        this.CurrentMode = TradeWindow.Modes.Inventory;
        return true;
      }
      if (((type == GameActionType.Submit ? 1 : (type == GameActionType.Split ? 1 : 0)) & (down ? 1 : 0)) == 0 || (UnityEngine.Object) this.selectedStorable == (UnityEngine.Object) null)
        return false;
      StorableComponent storableComponent = (StorableComponent) this.selectedStorable.Internal;
      if (storableComponent == null || !this.ItemIsInteresting((IStorableComponent) storableComponent))
        return false;
      if (this.SelectedItems.ContainsKey(storableComponent) || type == GameActionType.Submit)
      {
        this.SelectItem(storableComponent, type == GameActionType.Submit ? 1 : -1);
        if (!this.SelectedItems.ContainsKey(storableComponent))
          this.selectedStorable.SetSelected(true);
      }
      return true;
    }

    public override bool HaveToFindSelected() => false;

    private bool OnChangeInventory(GameActionType type, bool down)
    {
      if (type == GameActionType.BumperSelectionLeft & down)
      {
        if ((UnityEngine.Object) this.storables.Values.FirstOrDefault<StorableUI>((Func<StorableUI, bool>) (s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.Internal != null && this.ItemIsInteresting(s.Internal) && this.marketItems.Contains(s))) != (UnityEngine.Object) null)
        {
          this.CurrentMode = TradeWindow.Modes.Market;
          return true;
        }
      }
      else if (type == GameActionType.BumperSelectionRight & down && (UnityEngine.Object) this.storables.Values.FirstOrDefault<StorableUI>((Func<StorableUI, bool>) (s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.Internal != null && this.ItemIsInteresting(s.Internal) && !this.marketItems.Contains(s))) != (UnityEngine.Object) null)
      {
        this.CurrentMode = TradeWindow.Modes.Inventory;
        return true;
      }
      return false;
    }

    protected override void AdditionalAfterChangeAction()
    {
      this.CurrentMode = TradeWindow.Modes.None;
      if (InputService.Instance.JoystickUsed)
      {
        if ((UnityEngine.Object) this.storables.Values.FirstOrDefault<StorableUI>((Func<StorableUI, bool>) (s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.Internal != null && this.ItemIsInteresting(s.Internal) && !this.marketItems.Contains(s))) != (UnityEngine.Object) null)
          this.CurrentMode = TradeWindow.Modes.Inventory;
        else if ((UnityEngine.Object) this.storables.Values.FirstOrDefault<StorableUI>((Func<StorableUI, bool>) (s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.Internal != null && this.ItemIsInteresting(s.Internal) && this.marketItems.Contains(s))) != (UnityEngine.Object) null)
          this.CurrentMode = TradeWindow.Modes.Market;
        this.inventoryselectTipObject.SetActive((UnityEngine.Object) this.storables.Values.FirstOrDefault<StorableUI>((Func<StorableUI, bool>) (s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.Internal != null && this.ItemIsInteresting(s.Internal) && this.marketItems.Contains(s))) != (UnityEngine.Object) null && (UnityEngine.Object) this.storables.Values.FirstOrDefault<StorableUI>((Func<StorableUI, bool>) (s => (UnityEngine.Object) s != (UnityEngine.Object) null && s.Internal != null && this.ItemIsInteresting(s.Internal) && !this.marketItems.Contains(s))) != (UnityEngine.Object) null);
      }
      LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      this.controlPanel.gameObject.SetActive(joystick);
      this.buttonTrade.gameObject.SetActive(!joystick);
      ServiceLocator.GetService<GameActionService>();
      if (this.storables != null && this.storables.Count > 0)
      {
        foreach (KeyValuePair<StorableComponent, int> selectedItem in this.SelectedItems)
        {
          KeyValuePair<StorableComponent, int> selected = selectedItem;
          StorableUI storableUi = this.storables.Values.FirstOrDefault<StorableUI>((Func<StorableUI, bool>) (s => s.Internal == selected.Key));
          if ((UnityEngine.Object) storableUi != (UnityEngine.Object) null)
            (storableUi as StorableUITrade).SetSelectedCount(selected.Value, true);
        }
      }
      if (joystick)
      {
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Trade, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, new GameActionHandle(this.MainControl), true);
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Context, new GameActionHandle(this.MainControl), true);
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Split, new GameActionHandle(this.MainControl), true);
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.BumperSelectionLeft, new GameActionHandle(this.OnChangeInventory), true);
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.BumperSelectionRight, new GameActionHandle(this.OnChangeInventory), true);
      }
      else
      {
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Trade, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.MainControl));
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Context, new GameActionHandle(this.MainControl));
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Split, new GameActionHandle(this.MainControl));
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.BumperSelectionLeft, new GameActionHandle(this.OnChangeInventory));
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.BumperSelectionRight, new GameActionHandle(this.OnChangeInventory));
      }
    }

    protected override void PositionWindow(UIControl window, IStorableComponent storable)
    {
      if (InputService.Instance.JoystickUsed)
      {
        if (!this.storables.ContainsKey(storable))
          return;
        StorableUI storable1 = this.storables[storable];
        float num1 = 20f;
        if (!((UnityEngine.Object) storable1 != (UnityEngine.Object) null))
          return;
        RectTransform component1 = window.GetComponent<RectTransform>();
        ContainerResizableWindow componentInParent = storable1.GetComponentInParent<ContainerResizableWindow>();
        float num2 = num1 * 2f;
        if ((UnityEngine.Object) componentInParent != (UnityEngine.Object) null)
          num2 = this.HintsBottomBorder;
        Rect rect = storable1.Image.rectTransform.rect;
        float num3 = rect.height / 2f * storable1.Image.rectTransform.lossyScale.y;
        double num4 = (double) storable1.Image.transform.position.y + (double) num3;
        rect = component1.rect;
        double num5 = (double) rect.height * (double) component1.lossyScale.y;
        if (num4 - num5 < (double) num2)
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
          num7 = (float) (x - num8 - (double) storable1.Image.transform.position.x - (double) num1 * (double) containerRect.lossyScale.x / 2.0);
        }
        else
        {
          RectTransform component2 = this.marketContainer.GetComponent<RectTransform>();
          rect = component2.rect;
          num7 = (float) ((double) rect.width * (double) component2.lossyScale.x / 2.0 + (double) num1 * (double) component2.lossyScale.x / 2.0);
        }
        window.Transform.position = new Vector3(storable1.Image.transform.position.x + num7, storable1.Image.transform.position.y + num3);
      }
      else
        base.PositionWindow(window, storable);
    }

    protected void CalculateResult()
    {
      this.protagonistPrice = 0;
      this.marketPrice = 0;
      foreach (KeyValuePair<StorableComponent, int> selectedItem in this.SelectedItems)
      {
        if (selectedItem.Key.Storage == this.Actor)
          this.protagonistPrice += (int) TradeWindow.GetPrice((IMarketComponent) this.market, (IStorableComponent) selectedItem.Key, false, selectedItem.Value);
        else
          this.marketPrice += (int) TradeWindow.GetPrice((IMarketComponent) this.market, (IStorableComponent) selectedItem.Key, true, selectedItem.Value);
      }
      this.protagonistPrice = (int) Mathf.Ceil((float) this.protagonistPrice);
      this.marketPrice = (int) Mathf.Ceil((float) this.marketPrice);
      this.moneyDiff = this.protagonistPrice - this.marketPrice;
      if ((UnityEngine.Object) this.meter != (UnityEngine.Object) null)
      {
        this.meter.TargetValue = (float) this.marketPrice;
        this.meter.CurrentValue = (float) this.protagonistPrice;
      }
      this.UpdateCoins();
    }

    protected override bool ItemIsInteresting(IStorableComponent item)
    {
      bool isSeller = item.Storage != this.Actor && item.Storage != this.protagonistTable;
      return (int) TradeWindow.GetPrice((IMarketComponent) this.market, item, isSeller) > 0;
    }

    private void CheckInterestingItems()
    {
      foreach (IStorableComponent key in this.storables.Keys)
      {
        bool flag = key.Storage != this.Actor && key.Storage != this.protagonistTable;
        this.storables[key].Enable(this.ItemIsInteresting(key));
      }
    }

    private void DestroyContainers()
    {
      this.ClearMarketContainer();
      this.storage = (StorageComponent) null;
    }

    protected override bool ValidateContainer(
      IInventoryComponent container,
      IStorageComponent storage)
    {
      return base.ValidateContainer(container, storage) && (container.GetGroup() == InventoryGroup.Trade || storage == this.Actor);
    }

    public override void Initialize()
    {
      this.RegisterLayer<ITradeWindow>((ITradeWindow) this);
      base.Initialize();
    }

    public override System.Type GetWindowType() => typeof (ITradeWindow);

    protected override void OnSelectObject(GameObject selected)
    {
      base.OnSelectObject(selected);
      if (!((UnityEngine.Object) selected != (UnityEngine.Object) null))
        return;
      this.CurrentMode = !((UnityEngine.Object) selected.GetComponentInParent<ItemsSlidingContainer>() != (UnityEngine.Object) null) ? TradeWindow.Modes.Inventory : TradeWindow.Modes.Market;
    }

    protected override bool AdditionalConditionOfSelectableList(StorableUI storable)
    {
      return base.AdditionalConditionOfSelectableList(storable) && this.ItemIsInteresting(storable.Internal);
    }

    private static float GetPrice(
      IMarketComponent storage,
      IStorableComponent storable,
      bool isSeller,
      int count = 1)
    {
      return isSeller ? (float) count * storable.Invoice.SellPrice : (float) count * storable.Invoice.BuyPrice * TradeWindow.GetDurabilityModifier(storable);
    }

    private static float GetDurabilityModifier(IStorableComponent storable)
    {
      float durabilityModifier = 1f;
      if (storable.Durability != null && (double) storable.Durability.MaxValue > 0.0)
        durabilityModifier = Mathf.Max(storable.Durability.Value / storable.Durability.MaxValue, 0.1f);
      return durabilityModifier;
    }

    protected override bool WithPrice() => true;

    private void CountCoins()
    {
      this.protagonistCoins = 0;
      foreach (IStorableComponent storableComponent in this.Actor.Items)
      {
        if (storableComponent.Groups.Contains<StorableGroup>(StorableGroup.Money))
          this.protagonistCoins += storableComponent.Count;
      }
      this.marketCoins = 0;
      foreach (IStorableComponent storableComponent in this.storage.Items)
      {
        if (storableComponent.Groups.Contains<StorableGroup>(StorableGroup.Money))
          this.marketCoins += storableComponent.Count;
      }
      this.meter.StoredCoins = this.useMoney ? this.protagonistCoins : 0;
      this.meter.MarketCoins = this.useMoney ? this.marketCoins : 0;
    }

    protected override void UpdateCoins()
    {
      this.CountCoins();
      this.marketMoney.gameObject.SetActive(this.useMoney);
      if (this.useMoney)
      {
        int change = Mathf.Min(this.moneyDiff, this.marketCoins);
        this.protagonistMoney.SetCount(this.protagonistCoins, change);
        this.marketMoney.SetCount(this.marketCoins, -change);
      }
      else
        this.protagonistMoney.SetCount(this.protagonistCoins, 0);
      bool flag = true;
      if (this.protagonistPrice == 0 && this.marketPrice == 0)
        flag = false;
      if (!this.useMoney && this.protagonistPrice < this.marketPrice)
        flag = false;
      if (this.useMoney && this.protagonistPrice + this.protagonistCoins < this.marketPrice)
        flag = false;
      this.buttonTrade.interactable = flag;
      this.joystickAccept.SetActive(flag);
    }

    protected override void ShowInfoWindow(IStorableComponent storable)
    {
      LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
      CoroutineService.Instance.WaitFrame(1, (Action) (() =>
      {
        base.ShowInfoWindow(storable);
        if (!((UnityEngine.Object) this.windowInfoNew != (UnityEngine.Object) null))
          return;
        this.windowInfoNew.BarterMode(!this.useMoney);
      }));
    }

    protected override void SelectFirstStorableInContainer(List<StorableUI> storables)
    {
    }

    protected override void OnInvalidate()
    {
      this.CreateMarketContainer();
      base.OnInvalidate();
      this.CheckInterestingItems();
    }

    protected override void AddActionsToInfoWindow(
      InfoWindowNew window,
      IStorableComponent storable)
    {
      if (!this.ItemIsInteresting(storable))
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
