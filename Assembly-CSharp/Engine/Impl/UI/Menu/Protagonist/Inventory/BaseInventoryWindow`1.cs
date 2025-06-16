using Cofe.Proxies;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Container;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Grid;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Windows;
using Engine.Source.Audio;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Connections;
using Engine.Source.Inventory;
using Engine.Source.Services;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using Engine.Source.UI.Menu.Protagonist.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory.Grid;
using Engine.Source.Utility;
using InputServices;
using Inspectors;
using Pingle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory
{
  public abstract class BaseInventoryWindow<T> : 
    UIWindow,
    IBaseInventoryWindow,
    IPointerClickHandler,
    IEventSystemHandler,
    IPointerDownHandler,
    ITargetInventoryWindow,
    IEntityEventsListener
    where T : BaseInventoryWindow<T>
  {
    [SerializeField]
    [FormerlySerializedAs("_ActorAnchors")]
    private List<UIControl> actorAnchors = new List<UIControl>();
    [SerializeField]
    [FormerlySerializedAs("_Button_Cancel")]
    private Button buttonCancel;
    [SerializeField]
    [FormerlySerializedAs("_DragAnchor")]
    private UIControl dragAnchor;
    [SerializeField]
    [FormerlySerializedAs("_ShowInfo")]
    private bool showInfo;
    [SerializeField]
    private InventoryCellStyle styleOneCellOneStorable;
    [SerializeField]
    private InventoryCellStyle styleMultipleCellOneStorable;
    [SerializeField]
    [FormerlySerializedAs("_SlotAnchor")]
    private List<SlotAnchor> slotAnchor = new List<SlotAnchor>();
    [SerializeField]
    private Sprite nonAvailableIcon;
    [SerializeField]
    [FormerlySerializedAs("openProgressAudio")]
    private AudioClip overrideUnlockProgressAudio;
    [SerializeField]
    [FormerlySerializedAs("openCompleteAudio")]
    private AudioClip overrideUnlockCompleteAudio;
    [SerializeField]
    [Tooltip("Set -1 to use container time")]
    private float overrideUnlockTime = -1f;
    [SerializeField]
    protected AudioClip dropItemAudio;
    [SerializeField]
    private AudioMixerGroup mixer;
    [SerializeField]
    private GameObject inventoryContainerInfinitedPrefab;
    [SerializeField]
    private GameObject inventoryContainerLimitedPrefab;
    [SerializeField]
    private GameObject inventoryInfoPrefabNew;
    [SerializeField]
    private GameObject contextMenuPrefab;
    [SerializeField]
    private GameObject joystickContextMenuPrefab;
    [SerializeField]
    private GameObject inventorySplitPrefab;
    [SerializeField]
    private GameObject inventoryStorablePrefab;
    [SerializeField]
    private Text moneyText;
    [SerializeField]
    private ContainerResizableWindow actorContainerWindow;
    [SerializeField]
    private IEntitySerializable splitContainerTemplate;
    protected IStorageComponent splitContainer;
    [Inspected]
    protected List<IStorageComponent> actors = new List<IStorageComponent>();
    protected Dictionary<InventoryContainerUI, IStorageComponent> containers = new Dictionary<InventoryContainerUI, IStorageComponent>();
    protected Dictionary<IInventoryComponent, InventoryContainerUI> containerViews = new Dictionary<IInventoryComponent, InventoryContainerUI>();
    private CameraKindEnum lastCameraKind;
    [Inspected]
    protected Dictionary<IStorableComponent, StorableUI> storables = new Dictionary<IStorableComponent, StorableUI>();
    protected InfoWindowNew windowInfoNew;
    protected SplitGraphic windowSplit;
    protected ContextMenuWindowNew windowContextMenu;
    private AudioState currentOpenedAudioState;
    [Inspected]
    protected DragInventoryCell drag = new DragInventoryCell();
    [Inspected]
    protected Intersect intersect;
    protected bool splitEnabled = false;
    [SerializeField]
    protected GameObject controlPanel;
    [SerializeField]
    protected GameObject closeButtonObject;
    protected ControlTipsController tipsPannelController;
    protected bool CanShowInfoWindows = true;
    private bool ShowSimplifiedInfoWindows = false;
    private bool ShowActionTooltips = true;
    [SerializeField]
    protected Image selectionFrame;
    private StorableUI _selectedStorable;
    private bool isConsoleDragging = false;
    private Vector2 newStorableDraggingDelta = Vector2.zero;
    protected Vector3 lastStorablePosition;
    private Guid lastSelectedTemplateId;
    private float xPrev;
    private float yPrev = 0.0f;
    private const float MAX_ACCELERATION_RATE = 7.5f;
    private float currentAcceleration = 0.0f;
    private float accelerationSpeed = 3f;
    private Vector2 lastMoveDirecton;
    private Coroutine scrollCoroutine;
    private BaseInventoryWindow<T>.Navigation coroutineNavigation = BaseInventoryWindow<T>.Navigation.None;
    protected List<GameObject> selectableList = new List<GameObject>();
    private const float CellDistanceThreshold = 500f;
    private bool shouldDeselectStorable = false;
    public ContainerResizableWindow currentInventory = (ContainerResizableWindow) null;
    public LimitedInventoryContainerUI currentContainer = (LimitedInventoryContainerUI) null;
    private const float ContainerDistanceThreshold = 800f;

    [Inspected]
    public IStorageComponent Actor { get; set; }

    private void Awake()
    {
    }

    protected virtual void HideMainNavigationPanel()
    {
      this.tipsPannelController?.HidePannel("Main");
      this.closeButtonObject.GetSceneInstance()?.SetActive(false);
    }

    protected virtual void ShowMainNavigationPanel()
    {
      this.tipsPannelController?.ShowPannel("Main");
      this.closeButtonObject.GetSceneInstance()?.SetActive(true);
    }

    protected virtual void OnDragBegin()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      this.HideMainNavigationPanel();
      this.tipsPannelController?.ShowPannel("Move");
      service.AddListener(GameActionType.Cancel, new GameActionHandle(this.DragCancel));
      service.RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
    }

    protected virtual void OnDragEnd()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      this.tipsPannelController?.HidePannel("Move");
      this.ShowMainNavigationPanel();
      service.RemoveListener(GameActionType.Cancel, new GameActionHandle(this.DragCancel));
      service.AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      this.splitEnabled = false;
    }

    private bool DragCancel(GameActionType type, bool down)
    {
      if (down)
        this.DragCancel();
      return down;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
      switch (eventData.button)
      {
        case PointerEventData.InputButton.Left:
          if ((UnityEngine.Object) this.windowContextMenu != (UnityEngine.Object) null)
          {
            this.HideContextMenu();
            break;
          }
          if (!this.drag.IsEnabled)
          {
            if (this.intersect.Storables == null)
            {
              Debug.LogError((object) "intersect.Storables == null");
              break;
            }
            StorableComponent storable = this.intersect.Storables.FirstOrDefault<StorableComponent>();
            if (storable == null)
              break;
            if (StorableComponentUtility.IsSplittable((IStorableComponent) storable) && this.splitEnabled)
            {
              this.Split((IStorableComponent) storable);
              break;
            }
            if (this.ItemIsInteresting((IStorableComponent) storable))
              this.DragBegin((IStorableComponent) storable);
            break;
          }
          this.DragCheck(ref this.intersect);
          this.DragEnd(this.intersect);
          break;
        case PointerEventData.InputButton.Right:
          StorableComponent storable1 = this.drag.Storable;
          this.DragCancel();
          if (this.drag.Storable == null || !StorableComponentUtility.IsSplittable((IStorableComponent) this.drag.Storable))
            break;
          this.Split((IStorableComponent) storable1);
          break;
      }
    }

    protected virtual void Update()
    {
      if (this.isConsoleDragging)
        this.ConsoleDraggingUpdate();
      this.UpdateInternal();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
      this.intersect = !this.drag.IsEnabled ? this.GetIntersect(CursorService.Instance.Position) : this.GetIntersect((IStorableComponent) this.drag.Storable);
      switch (eventData.button)
      {
        case PointerEventData.InputButton.Right:
          if (this.drag.IsEnabled)
          {
            this.DragCancel();
            break;
          }
          if ((UnityEngine.Object) this.windowContextMenu != (UnityEngine.Object) null)
          {
            this.HideContextMenu();
            break;
          }
          StorableComponent storable = this.intersect.Storables.FirstOrDefault<StorableComponent>();
          if (storable != null)
          {
            bool flag = false;
            if (this.Actor.Items.Contains<IStorableComponent>((IStorableComponent) storable))
              this.ShowContextMenu(storable);
            if (!flag)
              this.InteractItem((IStorableComponent) storable);
          }
          break;
        case PointerEventData.InputButton.Middle:
          if (!this.drag.IsEnabled)
            break;
          break;
      }
      eventData.Use();
    }

    private void ShowContextMenu(StorableComponent storable)
    {
      this.HideInfoWindow();
      this.HideContextMenu();
      this.HideMainNavigationPanel();
      this.windowContextMenu = ContextMenuWindowNew.Instantiate(InputService.Instance.JoystickUsed ? this.joystickContextMenuPrefab : this.contextMenuPrefab);
      this.windowContextMenu.Transform.SetParent(this.transform, false);
      this.windowContextMenu.Target = (IStorableComponent) storable;
      StorableUI storableUi;
      if (this.storables.TryGetValue((IStorableComponent) storable, out storableUi))
      {
        if ((UnityEngine.Object) this.selectedStorable == (UnityEngine.Object) null)
          this.selectedStorable = storableUi;
        if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) storableUi && !this.selectedStorable.IsSliderItem)
          this.selectedStorable = storableUi;
        if (InputService.Instance.JoystickUsed)
        {
          this.PositionWindow((UIControl) this.windowContextMenu, (IStorableComponent) storable);
        }
        else
        {
          float y;
          switch (storable.Container.GetKind())
          {
            case ContainerCellKind.MultipleCellToOneStorable:
              y = (storableUi.Image.rectTransform.localPosition.y + this.styleMultipleCellOneStorable.Size.y / 4f) * storableUi.Image.rectTransform.lossyScale.y;
              break;
            case ContainerCellKind.OneCellToOneStorable:
              y = 0.0f;
              break;
            default:
              throw new Exception();
          }
          this.windowContextMenu.Transform.position = storableUi.Image.transform.position - new Vector3(0.0f, y, 0.0f);
        }
      }
      this.windowContextMenu.OnButtonInvestigate += new Action<IStorableComponent>(this.ShowInvestigationWindow);
      this.windowContextMenu.OnButtonDrop += new Action<IStorableComponent>(this.OnButtonDrop);
      this.windowContextMenu.OnButtonWear += new Action<IStorableComponent>(this.OnButtonDress);
      this.windowContextMenu.OnButtonUse += new Action<IStorableComponent>(this.OnButtonUse);
      this.windowContextMenu.OnButtonPourOut += new Action<IStorableComponent>(this.OnButtonPourOut);
      this.windowContextMenu.OnButtonSplit += new Action<IStorableComponent>(this.OnButtonSplit);
      this.windowContextMenu.OnClose += new Action(this.HideContextMenu);
      this.Unsubscribe();
      this.UnsubscribeNavigation();
    }

    protected void HideContextMenu()
    {
      if (!((UnityEngine.Object) this.windowContextMenu != (UnityEngine.Object) null))
        return;
      this.windowContextMenu.OnButtonInvestigate -= new Action<IStorableComponent>(this.ShowInvestigationWindow);
      this.windowContextMenu.OnButtonDrop -= new Action<IStorableComponent>(this.OnButtonDrop);
      this.windowContextMenu.OnButtonWear -= new Action<IStorableComponent>(this.OnButtonDress);
      this.windowContextMenu.OnButtonUse -= new Action<IStorableComponent>(this.OnButtonUse);
      this.windowContextMenu.OnButtonSplit -= new Action<IStorableComponent>(this.OnButtonSplit);
      this.windowContextMenu.OnClose -= new Action(this.HideContextMenu);
      UnityEngine.Object.Destroy((UnityEngine.Object) this.windowContextMenu.gameObject);
      this.windowContextMenu = (ContextMenuWindowNew) null;
      this.SubscribeNavigation();
      this.Subscribe();
      if (this.storables.Count > 0 && (UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
        this.ShowInfoWindow(this.selectedStorable.Internal);
      else
        this.selectedStorable = (StorableUI) null;
      this.ShowMainNavigationPanel();
    }

    private void ShowInvestigationWindow(IStorableComponent storable)
    {
      this.HideInfoWindow();
      this.HideContextMenu();
      IInvestigationWindow investigationWindow = ServiceLocator.GetService<UIService>().Get<IInvestigationWindow>();
      investigationWindow.Actor = this.Actor;
      investigationWindow.Target = storable;
      ServiceLocator.GetService<UIService>().Push<IInvestigationWindow>();
    }

    private void UICancelClickHandler() => ServiceLocator.GetService<UIService>().Pop();

    private void StorableDisposeEvent(IEntity sender)
    {
      StorableComponent component = sender.GetComponent<StorableComponent>();
      this.RemoveItemFromView((IStorableComponent) component);
      if (!this.drag.IsEnabled || component != this.drag.Storable)
        return;
      this.drag.Reset();
    }

    protected void RemoveItemFromView(IStorableComponent storable)
    {
      if (storable == null)
        return;
      StorableUI storableUi;
      if (this.storables.TryGetValue(storable, out storableUi))
      {
        this.storables.Remove(storable);
        UnityEngine.Object.Destroy((UnityEngine.Object) storableUi.gameObject);
      }
      if (storable.IsDisposed)
        return;
      ((Entity) storable.Owner).RemoveListener((IEntityEventsListener) this);
    }

    private void RemoveOpenResources(IInventoryComponent container)
    {
      if (container.GetOpenResources() == null)
        return;
      foreach (InventoryContainerOpenResource openResource in container.GetOpenResources())
      {
        if (StorageUtility.GetItemAmount(this.Actor.Items, openResource.ResourceType.Value) >= openResource.Amount)
        {
          this.RemoveItemsAmount(this.Actor, openResource.ResourceType.Value, openResource.Amount);
          break;
        }
      }
    }

    protected void RemoveItemsAmount(IStorageComponent storage, IEntity removingItem, int amount)
    {
      Guid itemId = StorageUtility.GetItemId(removingItem);
      int num = amount;
      List<KeyValuePair<IStorableComponent, int>> keyValuePairList = new List<KeyValuePair<IStorableComponent, int>>();
      foreach (IStorableComponent key in storage.Items)
      {
        if (StorageUtility.GetItemId(key.Owner) == itemId)
        {
          num -= Mathf.Min(key.Count, amount);
          keyValuePairList.Add(new KeyValuePair<IStorableComponent, int>(key, key.Count - Mathf.Min(key.Count, amount)));
        }
        if (num <= 0)
          break;
      }
      List<IStorableComponent> storableComponentList = new List<IStorableComponent>();
      foreach (KeyValuePair<IStorableComponent, int> keyValuePair in keyValuePairList)
      {
        KeyValuePair<IStorableComponent, int> k = keyValuePair;
        IStorableComponent storableComponent = storage.Items.First<IStorableComponent>((Func<IStorableComponent, bool>) (x => x.Equals((object) k.Key)));
        storableComponent.Count = k.Value;
        if (storableComponent.Count == 0)
          storableComponentList.Add(storableComponent);
      }
      foreach (IComponent component in storableComponentList)
        component.Owner.Dispose();
    }

    protected virtual void Clear()
    {
      this.ClearItems();
      this.drag.Reset();
      this.HideInfoWindow();
      if ((UnityEngine.Object) this.windowInfoNew != (UnityEngine.Object) null)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.windowInfoNew.gameObject);
        this.windowInfoNew = (InfoWindowNew) null;
      }
      if ((UnityEngine.Object) this.windowSplit != (UnityEngine.Object) null)
      {
        this.windowSplit.Dispose();
        this.windowSplit = (SplitGraphic) null;
      }
      this.HideContextMenu();
      if (this.splitContainer != null && this.splitContainer.Owner != null)
        this.splitContainer.Owner.Dispose();
      this.splitContainer = (IStorageComponent) null;
    }

    protected override void OnEnable()
    {
      this.tipsPannelController = this.controlPanel.GetComponent<ControlTipsController>();
      base.OnEnable();
      LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
      if ((UnityEngine.Object) this.selectionFrame != (UnityEngine.Object) null)
        this.selectionFrame.gameObject.SetActive(false);
      this.lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
      ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      this.buttonCancel?.onClick.AddListener(new UnityAction(this.UICancelClickHandler));
      this.SubscribeNavigation();
      this.Subscribe();
    }

    protected override void OnDisable()
    {
      PlayerUtility.ShowPlayerHands(true);
      if (this.drag.IsEnabled)
        this.DragCancel();
      this.Clear();
      this.Actor?.Owner?.GetComponent<AttackerPlayerComponent>()?.ApplyInventoryChange();
      ServiceLocator.GetService<CameraService>().Kind = this.lastCameraKind;
      this.buttonCancel?.onClick.RemoveListener(new UnityAction(this.UICancelClickHandler));
      this.UnsubscribeNavigation();
      this.UnsubscribeConsoleDragNavigation();
      this.Unsubscribe();
      this.isConsoleDragging = false;
      this.newStorableDraggingDelta = Vector2.zero;
      this.SetInfoWindowShowMode();
      this.SaveCurrentSelectedInstance((StorableUI) null);
      this.selectedStorable = (StorableUI) null;
      base.OnDisable();
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      if (this.drag.IsEnabled)
        this.DragCancel();
      this.controlPanel.gameObject.SetActive(joystick);
      if (!((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null))
        return;
      this.selectedStorable.SetSelected(joystick);
      if ((UnityEngine.Object) this.windowContextMenu != (UnityEngine.Object) null && this.windowContextMenu.IsEnabled)
      {
        this.HideContextMenu();
        this.ShowContextMenu((StorableComponent) this.selectedStorable.Internal);
      }
      if (InputService.Instance.JoystickUsed && !this.selectedStorable.IsSliderItem && ((UnityEngine.Object) this.windowContextMenu == (UnityEngine.Object) null || (UnityEngine.Object) this.windowContextMenu != (UnityEngine.Object) null && !this.windowContextMenu.IsEnabled))
      {
        this.HideInfoWindow();
        this.ShowInfoWindow(this.selectedStorable.Internal);
      }
      if ((UnityEngine.Object) this.windowInfoNew != (UnityEngine.Object) null && this.windowInfoNew.IsEnabled)
        this.PositionWindow((UIControl) this.windowInfoNew, this.selectedStorable.Internal);
    }

    protected virtual void GetFirstStorable()
    {
      this.SelectFirstStorableInContainer(this.storables.Values.ToList<StorableUI>());
      if ((UnityEngine.Object) this._selectedStorable == (UnityEngine.Object) null && this.storables.Count > 0)
        this.selectedStorable = this.GetComponentInChildren<StorableUI>();
      if (!((UnityEngine.Object) this._selectedStorable != (UnityEngine.Object) null))
        return;
      this.selectedStorable.SetSelected(true);
      this.ShowInfoWindow(this._selectedStorable.Internal);
    }

    protected virtual bool WithPrice() => false;

    protected void Build2()
    {
      PlayerUtility.ShowPlayerHands(false);
      this.Clear();
      this.HideInfoWindow();
      if ((UnityEngine.Object) this.windowInfoNew == (UnityEngine.Object) null && (UnityEngine.Object) this.inventoryInfoPrefabNew != (UnityEngine.Object) null)
      {
        this.windowInfoNew = InfoWindowNew.Instantiate(this.WithPrice(), this.inventoryInfoPrefabNew);
        this.windowInfoNew.Transform.SetParent(this.transform, false);
        this.windowInfoNew.IsEnabled = false;
      }
      if ((UnityEngine.Object) this.windowSplit == (UnityEngine.Object) null)
      {
        this.windowSplit = SplitGraphic.Instantiate(this.inventorySplitPrefab);
        if ((UnityEngine.Object) this.windowSplit != (UnityEngine.Object) null)
        {
          this.windowSplit.Transform.SetParent(this.transform, false);
          this.windowSplit.IsEnabled = false;
          this.windowSplit.Disable_Event += new Action<BaseGraphic>(this.SplitDisableHandler);
        }
      }
      this.CreateItems();
      this.OnInvalidate();
    }

    protected void OpenBegin(IInventoryComponent container) => this.OnContainerOpenBegin(container);

    private void OpenBegin(InventoryContainerUI uiContainer)
    {
      this.OpenBegin(uiContainer.InventoryContainer);
    }

    protected void OpenEnd(IInventoryComponent container, bool complete, bool keepClosedOnUnlock)
    {
      if (complete)
      {
        if (container.OpenState.Value == ContainerOpenStateEnum.Locked)
          this.RemoveOpenResources(container);
        int num = !keepClosedOnUnlock ? 0 : (container.OpenState.Value == ContainerOpenStateEnum.Locked ? 1 : 0);
        container.OpenState.Value = num == 0 ? ContainerOpenStateEnum.Open : ContainerOpenStateEnum.Closed;
      }
      this.OnContainerOpenEnd(container, complete);
      this.OnInvalidate();
    }

    protected virtual void OpenEnd(IInventoryComponent container, bool complete)
    {
      this.OpenEnd(container, complete, false);
    }

    protected void OpenEnd(
      InventoryContainerUI uiContainer,
      bool complete,
      bool keepClosedOnUnlock)
    {
      this.OpenEnd(uiContainer.InventoryContainer, complete, keepClosedOnUnlock);
    }

    protected virtual void OpenEnd(InventoryContainerUI uiContainer, bool complete)
    {
      this.OpenEnd(uiContainer, complete, false);
    }

    protected virtual void UpdateContainerStates()
    {
      foreach (KeyValuePair<InventoryContainerUI, IStorageComponent> container in this.containers)
        this.UpdateContainerStates(container.Key);
      this.UpdateCoins();
      this.ResizeActorContainersWindow();
    }

    private void UpdateContainerStates(InventoryContainerUI uiContainer)
    {
      IInventoryComponent inventoryContainer = uiContainer.InventoryContainer;
      if (inventoryContainer == null)
        return;
      bool b = this.CanOpenContainer(inventoryContainer);
      if ((UnityEngine.Object) uiContainer.ImageForeground != (UnityEngine.Object) null)
      {
        uiContainer.ImageForeground.sprite = inventoryContainer.OpenState.Value == ContainerOpenStateEnum.Open ? (Sprite) null : inventoryContainer.GetImageForeground();
        uiContainer.ImageForeground.gameObject.SetActive((UnityEngine.Object) uiContainer.ImageForeground.sprite != (UnityEngine.Object) null);
      }
      if ((UnityEngine.Object) uiContainer.ImageIcon != (UnityEngine.Object) null)
      {
        if (!inventoryContainer.Available.Value)
        {
          Sprite imageNotAvailable = inventoryContainer.GetImageNotAvailable();
          uiContainer.ImageIcon.sprite = imageNotAvailable ?? this.nonAvailableIcon;
        }
        else if (inventoryContainer.OpenState.Value == ContainerOpenStateEnum.Locked && inventoryContainer.GetInstrument() != 0)
        {
          uiContainer.ImageIcon.sprite = inventoryContainer.GetImageInstrument();
          uiContainer.SetIconEnabled(b);
        }
        else
          uiContainer.ImageIcon.sprite = (Sprite) null;
        uiContainer.ImageIcon.gameObject.SetActive((UnityEngine.Object) uiContainer.ImageIcon.sprite != (UnityEngine.Object) null);
      }
      if ((UnityEngine.Object) uiContainer.ImageDisease != (UnityEngine.Object) null)
        uiContainer.ImageDisease.gameObject.SetActive(inventoryContainer.OpenState.Value != ContainerOpenStateEnum.Open && (double) inventoryContainer.Disease.Value > 0.0);
      if ((UnityEngine.Object) uiContainer.ImageLock != (UnityEngine.Object) null)
      {
        if (inventoryContainer.OpenState.Value == ContainerOpenStateEnum.Locked)
        {
          uiContainer.ImageLock.sprite = inventoryContainer.GetImageLock();
          uiContainer.SetLockEnabled(b);
        }
        else
          uiContainer.ImageLock.sprite = (Sprite) null;
        uiContainer.ImageLock.gameObject.SetActive((UnityEngine.Object) uiContainer.ImageLock.sprite != (UnityEngine.Object) null);
      }
      if ((UnityEngine.Object) uiContainer.Button != (UnityEngine.Object) null)
      {
        bool flag = ((!inventoryContainer.Available.Value ? 0 : (inventoryContainer.OpenState.Value != ContainerOpenStateEnum.Open ? 1 : 0)) & (b ? 1 : 0)) != 0;
        if (flag)
        {
          int num = inventoryContainer.OpenState.Value != ContainerOpenStateEnum.Locked ? 0 : ((double) this.overrideUnlockTime >= 0.0 ? 1 : 0);
          uiContainer.Button.HoldTime = num == 0 ? inventoryContainer.GetOpenTime() : this.overrideUnlockTime;
        }
        uiContainer.Button.gameObject.SetActive(flag);
      }
      IStorableComponent itemInContainer = this.GetItemInContainer(inventoryContainer, inventoryContainer.GetStorage());
      if (itemInContainer == null || this.drag.IsEnabled && this.drag.Storable == itemInContainer)
        uiContainer.ImageBackground.gameObject.SetActive((UnityEngine.Object) uiContainer.ImageBackground.sprite != (UnityEngine.Object) null);
      else
        uiContainer.ImageBackground.gameObject.SetActive(false);
    }

    protected virtual void UpdateCoins()
    {
      if ((UnityEngine.Object) this.moneyText == (UnityEngine.Object) null)
        return;
      int num = 0;
      foreach (IStorableComponent storableComponent in this.Actor.Items)
      {
        if (storableComponent.Groups.Contains<StorableGroup>(StorableGroup.Money))
          num += storableComponent.Count;
      }
      this.moneyText.text = num.ToString();
    }

    private bool ItemsContainsOpenResources(
      IEnumerable<IStorableComponent> items,
      List<InventoryContainerOpenResource> openResources)
    {
      if (openResources == null || openResources.Count == 0)
        return true;
      foreach (InventoryContainerOpenResource openResource in openResources)
      {
        foreach (IStorableComponent storableComponent in items)
        {
          if (StorageUtility.GetItemAmount(items, openResource.ResourceType.Value) >= openResource.Amount)
            return true;
        }
      }
      return false;
    }

    protected bool CanOpenContainer(IInventoryComponent container)
    {
      if (container.OpenState.Value != ContainerOpenStateEnum.Locked)
        return true;
      bool flag = this.ItemsContainsOpenResources(this.Actor.Items, container.GetOpenResources());
      return (container.GetInstrument() == StorableGroup.None || this.GetInstrument(container.GetInstrument()) != null) & flag;
    }

    protected virtual void DamageInstrument(IInventoryComponent container)
    {
      IEntity instrument = this.GetInstrument(container.GetInstrument());
      if (instrument == null)
        return;
      this.DamageInstrument(instrument, container);
    }

    protected void DamageInstrument(IEntity item, IInventoryComponent container)
    {
      ParametersComponent component = item.GetComponent<ParametersComponent>();
      if (component == null)
        return;
      IParameter<float> byName1 = component.GetByName<float>(ParameterNameEnum.Durability);
      if (byName1 == null)
        return;
      float instrumentDamage = container.GetInstrumentDamage();
      IParameter<float> byName2 = component.GetByName<float>(ParameterNameEnum.Quality);
      if (byName2 != null)
        instrumentDamage *= Mathf.Clamp01(1f - byName2.Value);
      byName1.Value -= instrumentDamage;
    }

    protected virtual IEntity GetInstrument(StorableGroup instrumentGroup, bool brokenEnabled = false)
    {
      Dictionary<IEntity, IParameter<float>> source = new Dictionary<IEntity, IParameter<float>>();
      foreach (IStorableComponent storableComponent in this.Actor.Items)
      {
        if (storableComponent.Groups.Contains<StorableGroup>(instrumentGroup))
        {
          IEntity owner = storableComponent.Owner;
          ParametersComponent component = owner.GetComponent<ParametersComponent>();
          if (component == null)
            return owner;
          IParameter<float> byName = component.GetByName<float>(ParameterNameEnum.Durability);
          if (byName == null)
            return owner;
          if ((double) byName.Value > 0.0 | brokenEnabled)
            source.Add(owner, byName);
        }
      }
      if (source.Count <= 0)
        return (IEntity) null;
      IEntity key = source.First<KeyValuePair<IEntity, IParameter<float>>>().Key;
      if (brokenEnabled)
        return key;
      IParameter<float> parameter = source.First<KeyValuePair<IEntity, IParameter<float>>>().Value;
      foreach (KeyValuePair<IEntity, IParameter<float>> keyValuePair in source)
      {
        if ((double) keyValuePair.Value.Value < (double) parameter.Value)
        {
          key = keyValuePair.Key;
          parameter = keyValuePair.Value;
        }
      }
      return key;
    }

    protected virtual bool ValidateComputeActor(IStorageComponent actor) => actor == this.Actor;

    protected virtual bool ValidateContainer(
      IInventoryComponent container,
      IStorageComponent storage)
    {
      if (container.GetStorage() == this.Actor)
      {
        if (!container.Enabled.Value)
          return false;
        IEnumerable<StorableGroup> limitations = container.GetLimitations();
        if (limitations.Contains<StorableGroup>(StorableGroup.Money) || limitations.Contains<StorableGroup>(StorableGroup.Key) || limitations.Contains<StorableGroup>(StorableGroup.Weapons_Hands))
          return false;
      }
      return true;
    }

    protected virtual bool ItemIsInteresting(IStorableComponent item) => true;

    protected void PlaceTo(
      IStorageComponent storage,
      IInventoryComponent container,
      StorableComponent storable,
      Cell cell)
    {
      if (!this.storables.ContainsKey((IStorableComponent) storable))
      {
        this.AddItemToView((IStorableComponent) storable);
      }
      else
      {
        StorableUI storable1 = this.storables[(IStorableComponent) storable];
        InventoryContainerUI container1 = this.GetContainer(storage, container);
        if ((UnityEngine.Object) container1 == (UnityEngine.Object) null)
        {
          storable1.gameObject.SetActive(false);
        }
        else
        {
          InventoryCellStyle style;
          Sprite spriteByStyle;
          switch (container.GetKind())
          {
            case ContainerCellKind.MultipleCellToOneStorable:
              style = this.styleMultipleCellOneStorable;
              spriteByStyle = InventoryUtility.GetSpriteByStyle(storable.Placeholder, this.styleMultipleCellOneStorable.imageStyle);
              break;
            case ContainerCellKind.OneCellToOneStorable:
              style = this.styleOneCellOneStorable;
              spriteByStyle = InventoryUtility.GetSpriteByStyle(storable.Placeholder, this.styleOneCellOneStorable.imageStyle);
              break;
            default:
              throw new Exception();
          }
          storable1.Style = style;
          storable1.Image.sprite = spriteByStyle;
          storable1.transform.SetParent(container1.Storables.transform, false);
          Vector2 storablePosition = InventoryUtility.CalculateStorablePosition(storable.Cell, style);
          storable1.Transform.localPosition = (Vector3) storablePosition;
        }
      }
    }

    private void OnButtonDrop(IStorableComponent storable)
    {
      ServiceLocator.GetService<DropBagService>().DropBag(storable, this.Actor.Owner);
      this.RemoveItemFromView(storable);
      this.PlayAudio(this.dropItemAudio);
      this.HideContextMenu();
      this.HideInfoWindow();
      this.UpdateContainerStates();
      this.OnNavigate(BaseInventoryWindow<T>.Navigation.CellClosest);
    }

    private InventoryContainerUI GetContainer(
      IStorageComponent storage,
      IInventoryComponent container)
    {
      foreach (KeyValuePair<InventoryContainerUI, IStorageComponent> container1 in this.containers)
      {
        if (container1.Value == storage && container1.Key.InventoryContainer == container)
          return container1.Key;
      }
      return (InventoryContainerUI) null;
    }

    private void RegisterItemInView(IStorableComponent storable, InventoryCellSizeEnum size)
    {
      if (storable == null || storable.IsDisposed)
        return;
      StorableUI storableUi = StorableUI.Instantiate(storable, this.inventoryStorablePrefab, size);
      storableUi.Style = this.styleMultipleCellOneStorable;
      this.storables[storable] = storableUi;
      ((Entity) storable.Owner).AddListener((IEntityEventsListener) this);
    }

    protected bool AddItemToView(IStorableComponent storable)
    {
      InventoryContainerUI container = this.GetContainer(storable.Storage, storable.Container);
      if ((UnityEngine.Object) container == (UnityEngine.Object) null)
        return false;
      InventoryCellStyle inventoryCellStyle;
      switch (container.InventoryContainer.GetKind())
      {
        case ContainerCellKind.MultipleCellToOneStorable:
          inventoryCellStyle = this.styleMultipleCellOneStorable;
          break;
        case ContainerCellKind.OneCellToOneStorable:
          inventoryCellStyle = this.styleOneCellOneStorable;
          break;
        default:
          inventoryCellStyle = new InventoryCellStyle();
          break;
      }
      this.RegisterItemInView(storable, inventoryCellStyle.imageStyle);
      if (!this.storables.ContainsKey(storable))
        return false;
      this.storables[storable].Style = inventoryCellStyle;
      this.PlaceTo(storable.Storage, storable.Container, (StorableComponent) storable, ((StorableComponent) storable).Cell);
      return true;
    }

    protected virtual bool DragBegin(IStorableComponent storable)
    {
      if (this.drag.IsEnabled || storable == null || storable.IsDisposed || !this.storables.ContainsKey(storable) || storable.Container != null && this.containerViews.ContainsKey(storable.Container) && !this.containerViews[storable.Container].ClickEnabled)
        return false;
      StorableComponentUtility.PlayTakeSound(storable);
      if (!StorableComponentUtility.IsDraggable(storable))
      {
        this.MoveItem(storable, this.Actor);
        return false;
      }
      this.OnDragBegin();
      this.drag.Reset();
      this.drag.IsEnabled = true;
      CursorService.Instance.Visible = false;
      this.windowSplit.Actor = (IStorableComponent) null;
      if (storable.Container != null)
      {
        this.drag.Storage = storable.Storage;
        this.drag.Container = storable.Container;
        this.drag.Storable = (StorableComponent) storable;
        this.drag.Cell = ((StorableComponent) storable).Cell;
      }
      else
      {
        this.drag.Storage = (IStorageComponent) null;
        this.drag.Container = (IInventoryComponent) null;
        this.drag.Storable = (StorableComponent) storable;
        this.drag.Cell = (Cell) null;
      }
      StorableUI storable1 = this.storables[storable];
      this.drag.MouseOffset = InventoryUtility.GetCenter((UIControl) storable1);
      storable1.Dragging = true;
      Vector3 vector3 = Vector3.zero;
      if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
        vector3 = this.selectedStorable.transform.position;
      storable1.transform.SetParent(this.dragAnchor.transform, false);
      if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
        storable1.transform.position = vector3;
      if (storable.Container == null)
        return true;
      this.intersect = StorageUtility.GetIntersect(storable.Storage, storable.Container, (StorableComponent) storable, ((StorableComponent) storable).Cell);
      foreach (CellInfo cell1 in this.intersect.Cells)
      {
        InventoryContainerUI container = this.GetContainer(this.drag.Storage, this.drag.Container);
        if (!((UnityEngine.Object) container == (UnityEngine.Object) null))
        {
          foreach (KeyValuePair<Cell, InventoryCellUI> cell2 in (IEnumerable<KeyValuePair<Cell, InventoryCellUI>>) container.Cells)
          {
            if (cell1.Cell.Column == cell2.Key.Column && cell1.Cell.Row == cell2.Key.Row)
            {
              cell2.Value.State = CellState.Current;
              this.drag.BaseCells.Add(cell2.Value);
            }
          }
        }
      }
      this.UpdateContainerStates();
      return true;
    }

    protected virtual void Drag()
    {
      if (!this.drag.IsEnabled)
        return;
      if (!this.storables.ContainsKey((IStorableComponent) this.drag.Storable))
      {
        Debug.LogError((object) "!storables.ContainsKey(drag.Storable), Такого быть недолжно, воспроизвести, разобраться в чем дело и пофиксить нормально");
      }
      else
      {
        StorableUI storable = this.storables[(IStorableComponent) this.drag.Storable];
        storable.Dragging = true;
        this.drag.MouseOffset = InventoryUtility.GetCenter((UIControl) storable);
        if ((UnityEngine.Object) storable.Transform.parent != (UnityEngine.Object) this.dragAnchor.transform.transform)
        {
          if ((UnityEngine.Object) this.selectedStorable == (UnityEngine.Object) null)
            this.selectedStorable = storable;
          Vector3 position = this.selectedStorable.transform.position;
          storable.Transform.SetParent(this.dragAnchor.transform.transform, false);
          storable.transform.position = position;
        }
        this.drag.MouseOffset = InventoryUtility.GetCenter((UIControl) storable);
        if (InputService.Instance.JoystickUsed)
          return;
        storable.Transform.position = (Vector3) (CursorService.Instance.Position + this.drag.MouseOffset);
      }
    }

    private void HoverShadow(Intersect intersect)
    {
      if (!this.drag.IsEnabled)
        return;
      if (!this.storables.ContainsKey((IStorableComponent) this.drag.Storable))
      {
        Debug.LogError((object) "!storables.ContainsKey(drag.Storable), Такого быть недолжно, воспроизвести, разобраться в чем дело и пофиксить нормально");
      }
      else
      {
        if (!this.storables[(IStorableComponent) this.drag.Storable].Transform.hasChanged)
          return;
        foreach (InventoryCellUI actionCell in this.drag.ActionCells)
        {
          if ((UnityEngine.Object) actionCell != (UnityEngine.Object) null)
            actionCell.State = CellState.Default;
        }
        this.drag.ActionCells.Clear();
        foreach (InventoryCellUI baseCell in this.drag.BaseCells)
          baseCell.State = CellState.Current;
        if (!intersect.IsIntersected)
          return;
        InventoryContainerUI container = this.GetContainer(intersect.Storage, intersect.Container);
        foreach (CellInfo cell1 in intersect.Cells)
        {
          InventoryCellUI inventoryCellUi = (InventoryCellUI) null;
          foreach (KeyValuePair<Cell, InventoryCellUI> cell2 in (IEnumerable<KeyValuePair<Cell, InventoryCellUI>>) container.Cells)
          {
            if (cell2.Key.Column == cell1.Cell.Column && cell2.Key.Row == cell1.Cell.Row)
            {
              inventoryCellUi = cell2.Value;
              break;
            }
          }
          if (!((UnityEngine.Object) inventoryCellUi == (UnityEngine.Object) null))
          {
            this.drag.ActionCells.Add(inventoryCellUi);
            if (cell1.State == CellState.Disabled)
            {
              inventoryCellUi.State = CellState.Disabled;
            }
            else
            {
              switch (cell1.State)
              {
                case CellState.Occupied:
                  switch (intersect.Storables.Count)
                  {
                    case 0:
                      inventoryCellUi.State = cell1.State;
                      break;
                    case 1:
                      StorableComponent storableComponent = intersect.Storables.FirstOrDefault<StorableComponent>();
                      if (storableComponent == null)
                        throw new Exception();
                      if (this.drag.Storable.Owner.TemplateId == storableComponent.Owner.TemplateId && storableComponent.Count < storableComponent.Max)
                      {
                        inventoryCellUi.State = CellState.Stack;
                        break;
                      }
                      int num1 = container.InventoryContainer.GetKind() == ContainerCellKind.OneCellToOneStorable ? 1 : (intersect.Cells.Count == ((InventoryGridLimited) this.drag.Storable.Placeholder.Grid).Cells.Count ? 1 : 0);
                      inventoryCellUi.State = num1 == 0 ? CellState.NotFull : CellState.Swap;
                      break;
                    default:
                      int num2 = container.InventoryContainer.GetKind() == ContainerCellKind.OneCellToOneStorable ? 1 : (intersect.Cells.Count == ((InventoryGridLimited) this.drag.Storable.Placeholder.Grid).Cells.Count ? 1 : 0);
                      inventoryCellUi.State = num2 == 0 ? CellState.NotFull : CellState.Partial;
                      break;
                  }
                  break;
                case CellState.Stack:
                  inventoryCellUi.State = intersect.Storables.Count <= 1 ? cell1.State : CellState.Occupied;
                  break;
                default:
                  int num3 = container.InventoryContainer.GetKind() == ContainerCellKind.OneCellToOneStorable ? 1 : (intersect.Cells.Count == ((InventoryGridLimited) this.drag.Storable.Placeholder.Grid).Cells.Count ? 1 : 0);
                  inventoryCellUi.State = num3 == 0 ? CellState.NotFull : CellState.Allowed;
                  break;
              }
            }
          }
        }
      }
    }

    protected virtual void DragCheck(ref Intersect intersect)
    {
    }

    protected virtual void DragEnd(Intersect intersect)
    {
      bool isSuccess;
      this.DragEnd(intersect, out isSuccess);
      if (!isSuccess)
        return;
      this.Subscribe();
      this.SubscribeNavigation();
      this.isConsoleDragging = false;
      this.SaveCurrentSelectedInstance(this.selectedStorable);
      if ((UnityEngine.Object) this._selectedStorable != (UnityEngine.Object) null)
        this.currentInventory = this.selectedStorable.GetComponentInParent<ContainerResizableWindow>();
    }

    protected virtual void DragEnd(Intersect intersect, out bool isSuccess)
    {
      isSuccess = false;
      if (!this.drag.IsEnabled || !intersect.IsIntersected || intersect.Container != null && this.containerViews.ContainsKey(intersect.Container) && !this.containerViews[intersect.Container].ClickEnabled)
        return;
      if (!this.storables.ContainsKey((IStorableComponent) this.drag.Storable))
      {
        Debug.LogError((object) "!storables.ContainsKey(drag.Storable), Такого быть недолжно, воспроизвести, разобраться в чем дело и пофиксить нормально");
      }
      else
      {
        StorableUI storable = this.storables[(IStorableComponent) this.drag.Storable];
        storable.Dragging = false;
        foreach (CellInfo cell in intersect.Cells)
        {
          if (cell.State == CellState.Disabled)
            return;
        }
        StorableComponentUtility.PlayPutSound((IStorableComponent) this.drag.Storable);
        switch (intersect.Storables.Count)
        {
          case 0:
            if (!intersect.IsAllowed)
            {
              this.drag.IsEnabled = true;
              return;
            }
            if (!(this.drag.Storage == null ? intersect.Storage.AddItem((IStorableComponent) intersect.Storable, intersect.Container) : ((StorageComponent) this.drag.Storage).MoveItem((IStorableComponent) this.drag.Storable, intersect.Storage, intersect.Container, intersect.Cell.To())))
              throw new Exception();
            this.PlaceTo(intersect.Storage, intersect.Container, intersect.Storable, intersect.Cell.To());
            isSuccess = true;
            this.OnDragEnd();
            this.drag.Reset();
            break;
          case 1:
            StorableComponent storableComponent1 = intersect.Storables.FirstOrDefault<StorableComponent>();
            if (this.drag.Storable.Owner.TemplateId == storableComponent1.Owner.TemplateId && this.drag.Storable.Count + storableComponent1.Count <= storableComponent1.Max)
            {
              storableComponent1.Count += this.drag.Storable.Count;
              this.drag.Reset();
              if (intersect.Storable != null && !intersect.Storable.IsDisposed)
                intersect.Storable.Owner.Dispose();
              isSuccess = true;
              this.OnDragEnd();
              storable = this.storables[(IStorableComponent) storableComponent1];
              break;
            }
            if (intersect.Container.GetKind() != ContainerCellKind.OneCellToOneStorable && ((InventoryGridLimited) this.drag.Storable.Placeholder.Grid).Cells.Count != intersect.Cells.Count)
              return;
            StorableComponent storableComponent2 = intersect.Storables.First<StorableComponent>();
            if (this.drag.Storable.Owner.TemplateId == storableComponent2.Owner.TemplateId && this.drag.Storable.Max > 1)
            {
              int a = storableComponent2.Max - storableComponent2.Count;
              if (a > 0)
              {
                int num = Mathf.Min(a, this.drag.Storable.Count);
                storableComponent2.Count += num;
                this.drag.Storable.Count -= num;
                break;
              }
            }
            storableComponent1.Container = (IInventoryComponent) null;
            storableComponent1.Cell = (Cell) null;
            storableComponent1.Storage = (IStorageComponent) null;
            if (!(this.drag.Storage == null ? intersect.Storage.AddItem((IStorableComponent) intersect.Storable, intersect.Container) : ((StorageComponent) this.drag.Storage).MoveItem((IStorableComponent) this.drag.Storable, intersect.Storage, intersect.Container, intersect.Cell.To())))
              return;
            this.PlaceTo(intersect.Storage, intersect.Container, intersect.Storable, intersect.Cell.To());
            InventoryCellStyle inventoryCellStyle;
            switch (intersect.Container.GetKind())
            {
              case ContainerCellKind.MultipleCellToOneStorable:
                inventoryCellStyle = this.styleMultipleCellOneStorable;
                break;
              case ContainerCellKind.OneCellToOneStorable:
                inventoryCellStyle = this.styleOneCellOneStorable;
                break;
              default:
                inventoryCellStyle = new InventoryCellStyle();
                break;
            }
            if (!this.storables.TryGetValue((IStorableComponent) storableComponent1, out storable))
            {
              Debug.LogError((object) ("Item not found, storable : " + storableComponent1.Owner.GetInfo() + " , count : " + (object) this.storables.Count));
              return;
            }
            Vector3 lossyScale = storable.Transform.lossyScale;
            Vector2 vector2 = inventoryCellStyle.Size * 0.5f;
            vector2.Scale((Vector2) lossyScale);
            storable.Transform.position = (Vector3) (CursorService.Instance.Position - vector2);
            this.ClearShadow();
            this.drag.Reset();
            storableComponent1.Container = intersect.Container;
            storableComponent1.Cell = intersect.Cell.To();
            storableComponent1.Storage = intersect.Storage;
            if (this.DragBegin((IStorableComponent) storableComponent1))
            {
              this.drag.Storage = intersect.Storage;
            }
            else
            {
              isSuccess = true;
              this.drag.Reset();
              this.OnDragEnd();
            }
            this.HideContextMenu();
            this.HideInfoWindow();
            break;
        }
        if (!this.drag.IsEnabled)
        {
          this.windowSplit.Actor = (IStorableComponent) null;
          this.ClearShadow();
          CursorService.Instance.Visible = true;
        }
        else
          CursorService.Instance.Visible = false;
        this.selectedStorable = InputService.Instance.JoystickUsed ? storable : (StorableUI) null;
        this.UpdateContainerStates();
      }
    }

    protected void ClearShadow()
    {
      foreach (InventoryCellUI actionCell in this.drag.ActionCells)
        actionCell.State = CellState.Default;
      this.drag.ActionCells.Clear();
      foreach (InventoryCellUI baseCell in this.drag.BaseCells)
        baseCell.State = CellState.Default;
      this.drag.BaseCells.Clear();
    }

    protected virtual void DragCancel()
    {
      if (!this.drag.IsEnabled)
        return;
      this.OnDragEnd();
      if (this.drag.Storage == this.splitContainer)
      {
        StorableUI storableUi;
        if (this.storables.TryGetValue((IStorableComponent) this.drag.Storable, out storableUi))
        {
          storableUi.Dragging = false;
          this.storables.Remove((IStorableComponent) this.drag.Storable);
          UnityEngine.Object.Destroy((UnityEngine.Object) storableUi.gameObject);
          Debug.LogError((object) "-");
        }
        else
          Debug.LogError((object) ("View not found : " + this.drag.Storable.Owner.GetInfo()));
        this.MoveItem((IStorableComponent) this.drag.Storable, this.Actor);
      }
      else
      {
        this.drag.Storage = this.drag.Storable.Storage;
        if (!StorageUtility.GetIntersect(this.drag.Storage, this.drag.Container, this.drag.Storable, this.drag.Cell).IsAllowed)
        {
          if (StorageUtility.GetIntersect(this.drag.Storage, (IInventoryComponent) null, this.drag.Storable, (Cell) null).IsAllowed)
            this.MoveItem((IStorableComponent) this.drag.Storable, this.drag.Storage, this.drag.Container);
          else if (this.drag.Storable != null)
          {
            ServiceLocator.GetService<DropBagService>().DropBag((IStorableComponent) this.drag.Storable, this.Actor.Owner);
            this.RemoveItemFromView((IStorableComponent) this.drag.Storable);
            this.PlayAudio(this.dropItemAudio);
          }
        }
        else
          this.PlaceTo(this.drag.Storage, this.drag.Container, this.drag.Storable, this.drag.Cell);
      }
      StorableComponentUtility.PlayPutSound((IStorableComponent) this.drag.Storable);
      StorableUI storableUi1;
      if (this.drag.Storable != null && this.storables.TryGetValue((IStorableComponent) this.drag.Storable, out storableUi1))
      {
        storableUi1.Dragging = false;
        storableUi1.SetSelected(false);
      }
      this.drag.Reset();
      this.Subscribe();
      this.SubscribeNavigation();
      this.isConsoleDragging = false;
      this.SaveCurrentSelectedInstance(this.selectedStorable);
      CursorService.Instance.Visible = true;
      this.windowSplit.Actor = (IStorableComponent) null;
      if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
        this.OnSelectObject(this.selectedStorable.gameObject);
      this.ClearShadow();
      this.OnInvalidate();
    }

    protected void SaveCurrentSelectedInstance(StorableUI selectedStorable)
    {
      if ((UnityEngine.Object) selectedStorable != (UnityEngine.Object) null)
      {
        this.lastStorablePosition = selectedStorable.transform.position;
        try
        {
          this.lastSelectedTemplateId = selectedStorable.Internal.Owner.TemplateId;
        }
        catch
        {
          this.lastSelectedTemplateId = new Guid();
        }
      }
      else
      {
        float num = 0.0f;
        if ((UnityEngine.Object) this.currentInventory != (UnityEngine.Object) null)
          num = this.currentInventory.GetComponent<RectTransform>().sizeDelta.y;
        this.lastStorablePosition = new Vector3(0.0f, (UnityEngine.Object) this.currentContainer != (UnityEngine.Object) null ? this.currentContainer.transform.position.y + num / 2f : 0.0f);
        this.lastSelectedTemplateId = new Guid();
      }
    }

    private Pair<int, int> PositionToGrid(
      IInventoryComponent container,
      Vector2 position,
      Vector2 scale)
    {
      InventoryCellStyle inventoryCellStyle;
      switch (container.GetKind())
      {
        case ContainerCellKind.MultipleCellToOneStorable:
          inventoryCellStyle = this.styleMultipleCellOneStorable;
          break;
        case ContainerCellKind.OneCellToOneStorable:
          inventoryCellStyle = this.styleOneCellOneStorable;
          break;
        default:
          inventoryCellStyle = new InventoryCellStyle();
          break;
      }
      Vector2 size = inventoryCellStyle.Size;
      Vector2 vector2 = inventoryCellStyle.Size * 0.5f;
      Vector2 offset = inventoryCellStyle.Offset;
      size.Scale(scale);
      vector2.Scale(scale);
      offset.Scale(scale);
      position -= vector2;
      return new Pair<int, int>()
      {
        Item1 = (int) Math.Round((double) position.x / ((double) size.x + (double) offset.x)),
        Item2 = (int) Math.Round((double) position.y / ((double) size.y + (double) offset.y))
      };
    }

    private Cell CellIntersect(Vector2 position, InventoryContainerUI container)
    {
      foreach (KeyValuePair<Cell, InventoryCellUI> cell in (IEnumerable<KeyValuePair<Cell, InventoryCellUI>>) container.Cells)
      {
        Vector3 lossyScale = cell.Value.Transform.lossyScale;
        Rect scaledCoordinates = InventoryUtility.GetScaledCoordinates(cell.Value.Transform);
        Vector2 position1 = (Vector2) cell.Value.Transform.position;
        scaledCoordinates.position += position1;
        if (scaledCoordinates.Contains((Vector3) position, true))
          return cell.Key;
      }
      return (Cell) null;
    }

    private InventoryContainerUI ContainerIntersect(Vector2 position)
    {
      foreach (KeyValuePair<InventoryContainerUI, IStorageComponent> container in this.containers)
      {
        Rect scaledCoordinates1 = InventoryUtility.GetScaledCoordinates(container.Key.Transform);
        Vector2 position1 = (Vector2) container.Key.Transform.position;
        scaledCoordinates1.position += position1;
        bool flag = true;
        if (container.Key is ComplexInventoryContainerUI)
        {
          ComplexInventoryContainerUI key = container.Key as ComplexInventoryContainerUI;
          Rect scaledCoordinates2 = InventoryUtility.GetScaledCoordinates(key.Mask);
          Vector2 position2 = (Vector2) key.Mask.position;
          scaledCoordinates2.position += position2;
          flag = scaledCoordinates2.Contains((Vector3) position, true);
        }
        if (scaledCoordinates1.Contains((Vector3) position, true) & flag)
          return container.Key;
      }
      return (InventoryContainerUI) null;
    }

    private InventoryContainerUI ContainerIntersect(Rect rectangle)
    {
      foreach (KeyValuePair<InventoryContainerUI, IStorageComponent> container in this.containers)
      {
        if (container.Key.InventoryContainer.GetKind() == ContainerCellKind.OneCellToOneStorable)
        {
          Rect scaledCoordinates = InventoryUtility.GetScaledCoordinates(container.Key.Transform);
          Vector2 position = (Vector2) container.Key.Transform.position;
          scaledCoordinates.position += position;
          if (scaledCoordinates.Contains(rectangle.center))
            return container.Key;
        }
      }
      foreach (KeyValuePair<InventoryContainerUI, IStorageComponent> container in this.containers)
      {
        Rect scaledCoordinates = InventoryUtility.GetScaledCoordinates(container.Key.Transform);
        Vector2 position = (Vector2) container.Key.Transform.position;
        scaledCoordinates.position += position;
        if (scaledCoordinates.Overlaps(rectangle, true))
          return container.Key;
      }
      return (InventoryContainerUI) null;
    }

    private Intersect GetIntersect(IStorableComponent storable)
    {
      if (!this.storables.ContainsKey(storable))
        return new Intersect();
      StorableUI storable1 = this.storables[storable];
      Vector3 lossyScale1 = storable1.Transform.lossyScale;
      InventoryContainerUI key = this.ContainerIntersect(new Rect()
      {
        position = storable1.PivotedPosition,
        size = Vector2.Scale((Vector2) (storable1.Transform.rotation * (Vector3) storable1.Transform.sizeDelta), (Vector2) lossyScale1)
      });
      if ((UnityEngine.Object) key == (UnityEngine.Object) null || !this.containers.ContainsKey(key))
        return new Intersect();
      if (!key.InventoryContainer.Available.Value || key.InventoryContainer.OpenState.Value != ContainerOpenStateEnum.Open)
        return new Intersect();
      InventoryCellStyle inventoryCellStyle;
      switch (key.InventoryContainer.GetKind())
      {
        case ContainerCellKind.MultipleCellToOneStorable:
          inventoryCellStyle = this.styleMultipleCellOneStorable;
          break;
        case ContainerCellKind.OneCellToOneStorable:
          inventoryCellStyle = this.styleOneCellOneStorable;
          break;
        default:
          inventoryCellStyle = new InventoryCellStyle();
          break;
      }
      Vector3 lossyScale2 = key.Transform.lossyScale;
      Vector2 vector2_1 = inventoryCellStyle.Size * 0.5f;
      Vector2 offset = inventoryCellStyle.Offset;
      vector2_1.Scale((Vector2) lossyScale2);
      offset.Scale((Vector2) lossyScale2);
      Rect scaledCoordinates = InventoryUtility.GetScaledCoordinates(key.Transform);
      Vector2 position = scaledCoordinates.position;
      Vector2 size = scaledCoordinates.size;
      Vector2 vector2_2 = (Vector2) (storable1.Transform.rotation * (Vector3) vector2_1);
      Vector2 vector2_3 = (Vector2) storable1.Transform.position + vector2_2;
      Vector2 vector2_4 = (Vector2) key.Content.Transform.position + position;
      Pair<int, int> grid = this.PositionToGrid(key.InventoryContainer, vector2_3 - vector2_4, (Vector2) key.Transform.lossyScale);
      Cell cell = ProxyFactory.Create<Cell>();
      cell.Column = grid.Item1;
      cell.Row = grid.Item2;
      return StorageUtility.GetIntersect(this.containers[key], key.InventoryContainer, (StorableComponent) storable, cell);
    }

    private Intersect GetIntersect(Vector2 position)
    {
      InventoryContainerUI key = this.ContainerIntersect(position);
      if ((UnityEngine.Object) key == (UnityEngine.Object) null || !this.containers.ContainsKey(key))
        return new Intersect();
      if (key is ComplexInventoryContainerUI)
      {
        ComplexInventoryContainerUI container = key as ComplexInventoryContainerUI;
        Cell cell = this.CellIntersect(position, (InventoryContainerUI) container);
        if (cell == null)
          return new Intersect();
        IStorableComponent cellItem = container.GetCellItem(cell);
        return cellItem != null ? StorageUtility.GetIntersect(cellItem.Storage, cellItem.Container, (StorableComponent) null, cell) : new Intersect();
      }
      if (key.InventoryContainer.OpenState.Value != ContainerOpenStateEnum.Open)
        this.ShowClosedContainerInfo(key.InventoryContainer);
      if (!key.InventoryContainer.Available.Value || key.InventoryContainer.OpenState.Value != ContainerOpenStateEnum.Open)
        return new Intersect();
      Rect scaledCoordinates = InventoryUtility.GetScaledCoordinates(key.Transform);
      Vector2 position1 = scaledCoordinates.position;
      Vector2 size = scaledCoordinates.size;
      Vector3 lossyScale = key.Transform.lossyScale;
      Pair<int, int> grid = this.PositionToGrid(key.InventoryContainer, position - ((Vector2) key.Content.Transform.position + position1), (Vector2) lossyScale);
      Cell cell1 = ProxyFactory.Create<Cell>();
      cell1.Column = grid.Item1;
      cell1.Row = grid.Item2;
      return StorageUtility.GetIntersect(this.containers[key], key.InventoryContainer, (StorableComponent) null, cell1);
    }

    private void UpdateInternal()
    {
      if (!InputService.Instance.JoystickUsed)
        this.HideClosedContainerInfo();
      this.Drag();
      this.intersect = !this.drag.IsEnabled ? this.GetIntersect(CursorService.Instance.Position) : this.GetIntersect((IStorableComponent) this.drag.Storable);
      if (this.drag.IsEnabled)
      {
        if (this.intersect.Container != null)
        {
          StorableUI storable = this.storables[(IStorableComponent) this.drag.Storable];
          if (this.intersect.Container.GetKind() == ContainerCellKind.OneCellToOneStorable)
          {
            storable.Style = this.styleOneCellOneStorable;
            storable.Image.sprite = InventoryUtility.GetSpriteByStyle(this.drag.Storable.Placeholder, this.styleOneCellOneStorable.imageStyle);
          }
          else
          {
            storable.Style = this.styleMultipleCellOneStorable;
            storable.Image.sprite = InventoryUtility.GetSpriteByStyle(this.drag.Storable.Placeholder, this.styleMultipleCellOneStorable.imageStyle);
          }
        }
        this.DragCheck(ref this.intersect);
        this.HoverShadow(this.intersect);
      }
      if (!this.showInfo)
        return;
      IStorableComponent storable1 = !InputService.Instance.JoystickUsed || !((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null) ? (IStorableComponent) this.intersect.Storables.FirstOrDefault<StorableComponent>() : this.selectedStorable.Internal;
      if ((UnityEngine.Object) this.windowContextMenu == (UnityEngine.Object) null && !this.drag.IsEnabled && this.intersect.IsIntersected && storable1 != null && ((UnityEngine.Object) this.windowSplit == (UnityEngine.Object) null || !this.windowSplit.IsEnabled))
        this.ShowInfoWindow(storable1);
      else if (!InputService.Instance.JoystickUsed && (UnityEngine.Object) this.windowContextMenu == (UnityEngine.Object) null)
        this.HideInfoWindow();
    }

    protected virtual void HideInfoWindow()
    {
      if (!((UnityEngine.Object) this.windowInfoNew != (UnityEngine.Object) null))
        return;
      this.windowInfoNew.IsEnabled = false;
      this.windowInfoNew.Target = (IStorableComponent) null;
      this.windowInfoNew.ClearActionTooltips();
    }

    protected virtual void ShowClosedContainerInfo(IInventoryComponent container)
    {
    }

    protected virtual void HideClosedContainerInfo()
    {
    }

    public void SetInfoWindowShowMode(bool isSimplified = false, bool showActionTooltips = true)
    {
      this.ShowSimplifiedInfoWindows = isSimplified;
      this.ShowActionTooltips = showActionTooltips;
    }

    protected virtual void ShowInfoWindow(IStorableComponent storable)
    {
      if (!this.CanShowInfoWindows || (UnityEngine.Object) this.windowInfoNew == (UnityEngine.Object) null || this.drag != null && this.drag.IsEnabled)
        return;
      bool flag = false;
      if (this.windowInfoNew.IsEnabled)
      {
        if (this.windowInfoNew.Target != storable)
          this.HideInfoWindow();
        else
          flag = true;
      }
      if (!flag)
      {
        this.windowInfoNew.Target = storable;
        this.AddActionsToInfoWindow(this.windowInfoNew, storable);
        this.windowInfoNew.IsEnabled = true;
        StorableUI storableByComponent = this.GetStorableByComponent(storable);
        if ((UnityEngine.Object) storableByComponent != (UnityEngine.Object) null && !storableByComponent.IsSliderItem)
          this.selectedStorable = storableByComponent;
      }
      this.windowInfoNew.ShowSimpliedWindow(this.ShowSimplifiedInfoWindows, this.ShowActionTooltips);
      this.PositionWindow((UIControl) this.windowInfoNew, storable);
    }

    protected virtual float HintsBottomBorder
    {
      get
      {
        return (UnityEngine.Object) this.actorContainerWindow == (UnityEngine.Object) null ? 40f * this.transform.lossyScale.y : this.GetComponent<RectTransform>().position.y - 450f * this.actorContainerWindow.transform.lossyScale.y;
      }
    }

    protected virtual void PositionWindow(UIControl window, IStorableComponent storable)
    {
      StorableUI storableUi;
      if (!this.storables.TryGetValue(storable, out storableUi))
        return;
      RectTransform component = window.GetComponent<RectTransform>();
      float x1 = component.lossyScale.x;
      float num1 = 20f * x1;
      float num2 = 10f * x1;
      float num3;
      float num4;
      if (!InputService.Instance.JoystickUsed)
      {
        switch (storable.Container.GetKind())
        {
          case ContainerCellKind.MultipleCellToOneStorable:
            num3 = storableUi.Image.rectTransform.localPosition.x * x1 + num2;
            Rect rect1;
            if ((double) storableUi.Image.transform.position.x + (double) num3 + (double) component.rect.width * (double) x1 > (double) Screen.width - (double) num1)
            {
              double x2 = (double) storableUi.Image.rectTransform.localPosition.x;
              rect1 = storableUi.Image.rectTransform.rect;
              double width1 = (double) rect1.width;
              double num5 = x2 - width1;
              rect1 = component.rect;
              double width2 = (double) rect1.width;
              num3 = (float) (num5 - width2) * x1 - num2;
            }
            rect1 = storableUi.Image.rectTransform.rect;
            num4 = rect1.height / 2f * x1;
            double num6 = (double) storableUi.Image.transform.position.y + (double) num4;
            rect1 = component.rect;
            double num7 = (double) rect1.height * (double) x1;
            if (num6 - num7 < (double) num1)
            {
              rect1 = storableUi.Image.rectTransform.rect;
              double num8 = (double) rect1.height / 2.0;
              rect1 = component.rect;
              double height1 = (double) rect1.height;
              double num9 = num8 + height1;
              rect1 = storableUi.Image.rectTransform.rect;
              double height2 = (double) rect1.height;
              num4 = (float) (num9 - height2) * x1;
              break;
            }
            break;
          case ContainerCellKind.OneCellToOneStorable:
            num3 = storableUi.Image.rectTransform.rect.width / 2f * x1 + num2;
            if ((double) storableUi.Image.transform.position.x + (double) num3 + (double) component.rect.width * (double) x1 > (double) Screen.width - (double) num1)
              num3 = ((float) (-(double) storableUi.Image.rectTransform.rect.width / 2.0) - component.rect.width) * x1 - num2;
            num4 = storableUi.Image.rectTransform.rect.height / 2f * x1;
            double num10 = (double) storableUi.Image.transform.position.y + (double) num4;
            Rect rect2 = component.rect;
            double num11 = (double) rect2.height * (double) x1;
            if (num10 - num11 < (double) num1)
            {
              rect2 = component.rect;
              double height3 = (double) rect2.height;
              rect2 = storableUi.Image.rectTransform.rect;
              double height4 = (double) rect2.height;
              num4 = (float) ((height3 - height4) * (double) x1 / 2.0);
              break;
            }
            break;
          default:
            throw new Exception();
        }
      }
      else
      {
        ContainerResizableWindow componentInParent = storableUi.GetComponentInParent<ContainerResizableWindow>();
        if ((UnityEngine.Object) componentInParent == (UnityEngine.Object) null)
          return;
        RectTransform containerRect = componentInParent.GetContainerRect();
        float hintsBottomBorder = this.HintsBottomBorder;
        Rect rect3 = storableUi.Image.rectTransform.rect;
        num4 = rect3.height / 2f * x1;
        double num12 = (double) storableUi.Image.transform.position.y + (double) num4;
        rect3 = component.rect;
        double num13 = (double) rect3.height * (double) x1;
        if (num12 - num13 < (double) hintsBottomBorder)
        {
          rect3 = component.rect;
          double height = (double) rect3.height;
          rect3 = storableUi.Image.rectTransform.rect;
          double num14 = (double) rect3.height / 2.0;
          num4 = (float) (height - num14) * x1;
        }
        if ((UnityEngine.Object) componentInParent == (UnityEngine.Object) this.actorContainerWindow)
        {
          double x3 = (double) containerRect.position.x;
          rect3 = component.rect;
          double num15 = (double) rect3.width * (double) x1;
          num3 = (float) (x3 - num15 - (double) storableUi.Image.transform.position.x - (double) num1 / 2.0);
        }
        else
        {
          double x4 = (double) containerRect.position.x;
          rect3 = containerRect.rect;
          double num16 = (double) rect3.width * (double) x1;
          num3 = (float) (x4 + num16) + num1 - storableUi.Image.transform.position.x;
        }
      }
      window.Transform.position = new Vector3(storableUi.Image.transform.position.x + num3, storableUi.Image.transform.position.y + num4);
    }

    private void OnButtonUse(IStorableComponent storable)
    {
      if (!StorableComponentUtility.IsUsable(storable) || !this.Use(storable))
        return;
      this.HideContextMenu();
      this.HideInfoWindow();
    }

    private bool Use(IStorableComponent storable)
    {
      StorableComponentUtility.PlayUseSound(storable);
      IEntity template = (IEntity) storable.Owner.Template;
      StorableComponentUtility.Use(storable);
      ServiceLocator.GetService<LogicEventService>().FireEntityEvent(nameof (Use), template);
      this.OnInvalidate();
      return true;
    }

    private void OnButtonPourOut(IStorableComponent storable)
    {
      if (!StorableComponentUtility.IsBottled(storable) || !this.PourOut(storable))
        return;
      this.HideContextMenu();
      this.HideInfoWindow();
    }

    private bool PourOut(IStorableComponent storable)
    {
      StorableComponentUtility.PlayPourOutSound(storable);
      IEntity template = (IEntity) storable.Owner.Template;
      --storable.Count;
      if (storable.Count <= 0)
        storable.Owner.Dispose();
      ServiceLocator.GetService<LogicEventService>().FireEntityEvent(nameof (PourOut), template);
      this.OnInvalidate();
      return true;
    }

    private void OnButtonDress(IStorableComponent storable)
    {
      if (!StorableComponentUtility.IsWearable(storable) || !this.Dress(storable))
        return;
      this.HideContextMenu();
      this.HideInfoWindow();
    }

    protected bool Dress(IStorableComponent storable)
    {
      if (storable == null || storable.IsDisposed)
        return false;
      IInventoryComponent container1 = (IInventoryComponent) null;
      foreach (IInventoryComponent container2 in this.Actor.Containers.Where<IInventoryComponent>((Func<IInventoryComponent, bool>) (o => o.GetGroup() == InventoryGroup.Clothes || o.GetGroup() == InventoryGroup.Weapons)))
      {
        foreach (StorableGroup group in storable.Groups)
        {
          if (container2.GetLimitations().Contains<StorableGroup>(group))
          {
            container1 = container2;
            break;
          }
        }
        if (container1 != null)
          break;
      }
      if (container1 == null)
        return false;
      Intersect intersect1 = StorageUtility.GetIntersect(this.Actor, container1, (StorableComponent) storable, (Cell) null);
      if (intersect1.IsAllowed)
      {
        if (((StorageComponent) storable.Storage).MoveItem(storable, intersect1.Storage, intersect1.Container, intersect1.Cell.To()))
          this.PlaceTo(intersect1.Storage, intersect1.Container, intersect1.Storable, intersect1.Cell.To());
      }
      else
      {
        IStorableComponent itemInContainer = this.GetItemInContainer(container1, this.Actor);
        if (itemInContainer != null)
        {
          this.Actor.RemoveItem(itemInContainer);
          Intersect intersect2 = StorageUtility.GetIntersect(this.Actor, container1, (StorableComponent) storable, (Cell) null);
          if (intersect2.IsAllowed && ((StorageComponent) storable.Storage).MoveItem(storable, intersect2.Storage, intersect2.Container, intersect2.Cell.To()))
            this.PlaceTo(intersect2.Storage, intersect2.Container, intersect2.Storable, intersect2.Cell.To());
          if (StorageUtility.GetIntersect(this.Actor, (IInventoryComponent) null, (StorableComponent) itemInContainer, (Cell) null).IsAllowed)
            this.Actor.AddItem(itemInContainer, (IInventoryComponent) null);
          else
            ServiceLocator.GetService<DropBagService>().DropBag(itemInContainer, this.Actor.Owner);
        }
      }
      return true;
    }

    private void OnButtonSplit(IStorableComponent storable)
    {
      if (!this.Split(storable))
        return;
      this.HideContextMenu();
      this.HideInfoWindow();
      this.Unsubscribe();
      this.UnsubscribeNavigation();
    }

    private bool Split(IStorableComponent storable)
    {
      if (!StorableComponentUtility.IsSplittable(storable))
        return false;
      this.HideInfoWindow();
      this.HideContextMenu();
      this.HideMainNavigationPanel();
      this.windowSplit.Actor = storable;
      this.windowSplit.IsEnabled = true;
      return true;
    }

    protected virtual void InteractItem(IStorableComponent storable)
    {
    }

    protected virtual void SplitDisableHandler(BaseGraphic base2)
    {
      this.FindCurrentCellInstance();
      SplitGraphic splitGraphic = (SplitGraphic) base2;
      if (splitGraphic.IsCanceled || splitGraphic.Target == null || splitGraphic.Target.IsDisposed)
      {
        this.Subscribe();
        this.SubscribeConsoleDragNavigation();
        this.splitEnabled = false;
        if (InputService.Instance.JoystickUsed)
          this.ShowContextMenu((StorableComponent) this.selectedStorable.Internal);
        this.ShowMainNavigationPanel();
      }
      else
      {
        IStorableComponent storableComponent = splitGraphic.Actor != splitGraphic.Target ? splitGraphic.Target : splitGraphic.Actor;
        if (this.splitContainer == null)
          this.CreateSplitContainer();
        this.splitContainer.AddItem(storableComponent, this.splitContainer.Containers.FirstOrDefault<IInventoryComponent>());
        this.RegisterItemInView(storableComponent, this.styleOneCellOneStorable.imageStyle);
        if (InputService.Instance.JoystickUsed)
        {
          this.storables[storableComponent].Transform.position = this.selectedStorable.Transform.position;
          this.selectedStorable = this.storables[storableComponent];
          ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Context, new GameActionHandle(this.DragListener));
          this.DragListener(GameActionType.Context, true);
        }
        else
        {
          this.storables[storableComponent].Transform.position = (Vector3) CursorService.Instance.Position;
          this.DragBegin(storableComponent);
        }
        if (this.drag.Storable.Container != null)
          return;
        this.drag.Storage = this.splitContainer;
        this.drag.Storable.Storage = this.splitContainer;
        this.drag.Storable.Container = this.splitContainer.Containers.FirstOrDefault<IInventoryComponent>();
      }
    }

    protected void CreateSplitContainer()
    {
      IEntity template = this.splitContainerTemplate.Value;
      IEntity entity = ServiceLocator.GetService<IFactory>().Instantiate<IEntity>(template);
      this.splitContainer = (IStorageComponent) entity.GetComponent<StorageComponent>();
      ServiceLocator.GetService<ISimulation>().Add(entity, ServiceLocator.GetService<ISimulation>().Others);
      this.actors.Add(this.splitContainer);
    }

    private bool SplitListener(GameActionType type, bool down)
    {
      if (this.drag.IsEnabled)
        return false;
      if (down)
      {
        if (!this.IsEnabled)
          return false;
        this.splitEnabled = true;
        IStorableComponent storable = (IStorableComponent) null;
        if (InputService.Instance.JoystickUsed)
        {
          if (!((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null))
            return false;
          storable = this.selectedStorable.Internal;
        }
        else
        {
          this.intersect = this.GetIntersect(CursorService.Instance.Position);
          if (this.intersect.Storables != null)
            storable = (IStorableComponent) this.intersect.Storables.FirstOrDefault<StorableComponent>();
          if (storable == null)
            return false;
        }
        if (this.Split(storable))
        {
          this.Unsubscribe();
          this.UnsubscribeNavigation();
        }
        return true;
      }
      this.Subscribe();
      this.SubscribeNavigation();
      this.splitEnabled = false;
      return false;
    }

    protected void MoveAllItems(IStorageComponent fromStorage, IStorageComponent toStorage)
    {
      foreach (IStorableComponent storableComponent in fromStorage.Items.ToList<IStorableComponent>())
        this.MoveItem(storableComponent, toStorage, invalidate: false);
      this.OnInvalidate();
    }

    protected void MoveItem(
      IStorableComponent item,
      IStorageComponent toStorage,
      IInventoryComponent toContainer = null,
      bool invalidate = true)
    {
      IInventoryComponent container = (IInventoryComponent) null;
      if (toContainer != null && toContainer.GetStorage() == toStorage)
        container = toContainer;
      Intersect intersect = StorageUtility.GetIntersect(toStorage, container, (StorableComponent) item, (Cell) null);
      if (item.IsDisposed)
      {
        if (!invalidate)
          return;
        this.OnInvalidate();
      }
      else
      {
        if (!intersect.IsAllowed)
        {
          ServiceLocator.GetService<DropBagService>().DropBag(item, this.Actor.Owner);
          this.RemoveItemFromView(item);
          this.PlayAudio(this.dropItemAudio);
        }
        else
        {
          if (!((StorageComponent) item.Storage).MoveItem(item, intersect.Storage, intersect.Container, intersect.Cell.To()))
            throw new Exception();
          StorableComponent storableComponent = intersect.Storables.FirstOrDefault<StorableComponent>();
          if (storableComponent == null || storableComponent.IsDisposed)
            this.PlaceTo(intersect.Storage, intersect.Container, intersect.Storable, intersect.Cell.To());
        }
        if (!invalidate)
          return;
        this.OnInvalidate();
      }
    }

    [Inspected]
    protected virtual void OnInvalidate() => this.UpdateContainerStates();

    private void OnContainerOpenBegin(IInventoryComponent container)
    {
      this.StartOpenAudio(container.GetOpenStartAudio());
      if (container.OpenState.Value == ContainerOpenStateEnum.Locked && (UnityEngine.Object) this.overrideUnlockProgressAudio != (UnityEngine.Object) null)
        this.StartOpenAudio(this.overrideUnlockProgressAudio);
      else
        this.StartOpenAudio(container.GetOpenProgressAudio());
    }

    protected virtual void OnContainerOpenEnd(IInventoryComponent container, bool complete)
    {
      this.StopOpenAudio();
      if (complete)
      {
        if (container.OpenState.Value == ContainerOpenStateEnum.Closed && (UnityEngine.Object) this.overrideUnlockCompleteAudio != (UnityEngine.Object) null)
          this.StartOpenAudio(this.overrideUnlockCompleteAudio);
        else
          this.StartOpenAudio(container.GetOpenCompleteAudio());
        if (container.GetInstrument() != 0)
          this.DamageInstrument(container);
      }
      else
        this.StartOpenAudio(container.GetOpenCancelAudio());
      this.UpdateContainerStates();
    }

    protected void PlayAudio(AudioClip audio)
    {
      if (!((UnityEngine.Object) audio != (UnityEngine.Object) null))
        return;
      SoundUtility.PlayAudioClip2D(audio, this.mixer, 1f, 0.0f, context: this.gameObject.GetFullName());
    }

    protected void StartOpenAudio(AudioClip audio)
    {
      if (!((UnityEngine.Object) audio != (UnityEngine.Object) null))
        return;
      this.currentOpenedAudioState = SoundUtility.PlayAudioClip2D(audio, this.mixer, 1f, 0.0f, context: this.gameObject.GetFullName());
    }

    protected void StopOpenAudio()
    {
      if (this.currentOpenedAudioState == null)
        return;
      this.currentOpenedAudioState.Complete = true;
      if ((UnityEngine.Object) this.currentOpenedAudioState.AudioSource != (UnityEngine.Object) null)
        this.currentOpenedAudioState.AudioSource.Stop();
      this.currentOpenedAudioState = (AudioState) null;
    }

    protected InventoryContainerUI CreateContainerView(
      IStorageComponent storage,
      IInventoryComponent container,
      UIControl anchor)
    {
      InventoryCellStyle style;
      switch (container.GetKind())
      {
        case ContainerCellKind.MultipleCellToOneStorable:
          style = this.styleMultipleCellOneStorable;
          break;
        case ContainerCellKind.OneCellToOneStorable:
          style = this.styleOneCellOneStorable;
          break;
        default:
          style = new InventoryCellStyle();
          break;
      }
      return this.CreateContainerView(storage, container, anchor, style);
    }

    protected InventoryContainerUI CreateContainerView(
      IStorageComponent storage,
      IInventoryComponent container,
      UIControl anchor,
      InventoryCellStyle style)
    {
      InventoryContainerUI key;
      if (container.GetGrid() is IInventoryGridLimited)
      {
        key = (InventoryContainerUI) LimitedInventoryContainerUI.Instantiate(container, style, this.inventoryContainerLimitedPrefab);
      }
      else
      {
        if (!(container.GetGrid() is IInventoryGridInfinited))
          throw new Exception();
        key = (InventoryContainerUI) InfinitedInventoryContainerUI.Instantiate(container, style, this.inventoryContainerInfinitedPrefab);
      }
      key.ImageBackground.sprite = container.GetImageBackground();
      key.ImageBackground.gameObject.SetActive(this.GetItemInContainer(container, container.GetStorage()) == null && (UnityEngine.Object) key.ImageBackground.sprite != (UnityEngine.Object) null);
      key.transform.SetParent(anchor.transform, false);
      this.containers.Add(key, storage);
      this.containerViews.Add(container, key);
      key.OpenBegin += new Action<InventoryContainerUI>(this.OpenBegin);
      key.OpenEnd += new Action<InventoryContainerUI, bool>(this.OpenEnd);
      foreach (IStorableComponent storable in storage.Items)
      {
        if (storable.Container == container)
          this.AddItemToView(storable);
      }
      return key;
    }

    protected IStorableComponent GetItemInContainer(
      IInventoryComponent container,
      IStorageComponent storage)
    {
      foreach (IStorableComponent itemInContainer in storage.Items)
      {
        if (itemInContainer.Container == container)
          return itemInContainer;
      }
      return (IStorableComponent) null;
    }

    private void ResizeActorContainersWindow()
    {
      List<InventoryContainerUI> containers = new List<InventoryContainerUI>();
      foreach (KeyValuePair<InventoryContainerUI, IStorageComponent> container in this.containers)
      {
        if (container.Value == this.Actor && container.Key.InventoryContainer.Enabled.Value && container.Key.InventoryContainer.GetGroup() == InventoryGroup.Backpack)
          containers.Add(container.Key);
      }
      this.ResizeContainersWindow(this.actorContainerWindow, containers);
    }

    protected void ResizeContainersWindow(
      ContainerResizableWindow window,
      List<InventoryContainerUI> containers)
    {
      if (!((UnityEngine.Object) window != (UnityEngine.Object) null))
        return;
      window.Resize(containers);
    }

    protected bool IsDebugMode() => InstanceByRequest<EngineApplication>.Instance.IsDebug;

    public virtual IEntity GetUseTarget() => this.Actor?.Owner;

    private void CreateItems()
    {
      for (int index1 = 0; index1 < this.actors.Count; ++index1)
      {
        IStorageComponent actor = this.actors[index1];
        if (actor != null && !actor.IsDisposed)
        {
          actor.OnAddItemEvent += new Action<IStorableComponent, IInventoryComponent>(this.Actor_OnChangeItemEvent);
          actor.OnRemoveItemEvent += new Action<IStorableComponent, IInventoryComponent>(this.Actor_OnChangeItemEvent);
          actor.OnChangeItemEvent += new Action<IStorableComponent, IInventoryComponent>(this.Actor_OnChangeItemEvent);
          foreach (IInventoryComponent container in actor.Containers)
          {
            if (this.ValidateContainer(container, actor))
            {
              InventoryCellStyle style;
              switch (container.GetKind())
              {
                case ContainerCellKind.MultipleCellToOneStorable:
                  style = this.styleMultipleCellOneStorable;
                  break;
                case ContainerCellKind.OneCellToOneStorable:
                  style = this.styleOneCellOneStorable;
                  break;
                default:
                  style = new InventoryCellStyle();
                  break;
              }
              bool flag = false;
              SlotAnchor slotAnchor = new SlotAnchor();
              if (container.GetSlotKind() != SlotKind.None && this.ValidateComputeActor(actor))
              {
                for (int index2 = 0; index2 < this.slotAnchor.Count; ++index2)
                {
                  if (container.GetLimitations().Contains<StorableGroup>(this.slotAnchor[index2].Group) && container.Enabled.Value)
                  {
                    flag = true;
                    slotAnchor = this.slotAnchor[index2];
                    break;
                  }
                }
                if (!flag)
                  continue;
              }
              InventoryContainerUI key;
              if (container.GetGrid() is IInventoryGridLimited)
              {
                key = (InventoryContainerUI) LimitedInventoryContainerUI.Instantiate(container, style, this.inventoryContainerLimitedPrefab);
              }
              else
              {
                if (!(container.GetGrid() is IInventoryGridInfinited))
                  throw new Exception("Grid Type not found : " + (container.GetGrid() != null ? container.GetGrid().GetType().ToString() : "null"));
                key = (InventoryContainerUI) InfinitedInventoryContainerUI.Instantiate(container, style, this.inventoryContainerInfinitedPrefab);
              }
              if ((UnityEngine.Object) key.ImageBackground != (UnityEngine.Object) null)
              {
                key.ImageBackground.sprite = container.GetImageBackground();
                key.ImageBackground.gameObject.SetActive((UnityEngine.Object) key.ImageBackground.sprite != (UnityEngine.Object) null);
              }
              if (flag)
                key.transform.SetParent(slotAnchor.UIBehaviour.transform, false);
              else if (index1 < this.actorAnchors.Count)
                key.transform.SetParent(this.actorAnchors[index1].transform, false);
              this.containers.Add(key, actor);
              key.OpenBegin += new Action<InventoryContainerUI>(this.OpenBegin);
              key.OpenEnd += new Action<InventoryContainerUI, bool>(this.OpenEnd);
            }
          }
          foreach (IStorableComponent storable in actor.Items)
            this.AddItemToView(storable);
        }
      }
    }

    private void ClearItems()
    {
      for (int index = 0; index < this.actors.Count; ++index)
      {
        IStorageComponent actor = this.actors[index];
        if (actor != null && !actor.IsDisposed)
        {
          actor.OnAddItemEvent -= new Action<IStorableComponent, IInventoryComponent>(this.Actor_OnChangeItemEvent);
          actor.OnRemoveItemEvent -= new Action<IStorableComponent, IInventoryComponent>(this.Actor_OnChangeItemEvent);
          actor.OnChangeItemEvent -= new Action<IStorableComponent, IInventoryComponent>(this.Actor_OnChangeItemEvent);
        }
      }
      foreach (KeyValuePair<InventoryContainerUI, IStorageComponent> container in this.containers)
      {
        container.Key.OpenBegin -= new Action<InventoryContainerUI>(this.OpenBegin);
        container.Key.OpenEnd -= new Action<InventoryContainerUI, bool>(this.OpenEnd);
        UnityEngine.Object.Destroy((UnityEngine.Object) container.Key.gameObject);
      }
      this.containers.Clear();
      this.containerViews.Clear();
      foreach (KeyValuePair<IStorableComponent, StorableUI> keyValuePair in this.storables.ToList<KeyValuePair<IStorableComponent, StorableUI>>())
        this.RemoveItemFromView(keyValuePair.Key);
      this.storables.Clear();
    }

    protected virtual void Actor_OnChangeItemEvent(
      IStorableComponent storable,
      IInventoryComponent inventory)
    {
      CoroutineService.Instance.WaitFrame((Action) (() =>
      {
        this.ClearItems();
        this.CreateItems();
        this.OnInvalidate();
        this.Drag();
        this.AdditionalAfterChangeAction();
      }));
    }

    protected virtual void AdditionalAfterChangeAction()
    {
      if (!InputService.Instance.JoystickUsed || this.drag.IsEnabled)
        return;
      this.SelectFirstStorableInContainer((List<StorableUI>) null);
    }

    protected virtual void AddActionsToInfoWindow(InfoWindowNew window, IStorableComponent storable)
    {
      bool joystickUsed = InputService.Instance.JoystickUsed;
      window.AddActionTooltip(joystickUsed ? GameActionType.Context : GameActionType.Submit, "{StorableTooltip.Drag}");
      window.AddActionTooltip(joystickUsed ? GameActionType.Submit : GameActionType.Context, "{StorableTooltip.ContextMenu}");
      if (!StorableComponentUtility.IsSplittable(storable))
        return;
      window.AddActionTooltip(GameActionType.Split, "{StorableTooltip.Split}");
    }

    public void OnEntityEvent(IEntity sender, EntityEvents kind)
    {
      if (kind != EntityEvents.DisposeEvent)
        return;
      this.StorableDisposeEvent(sender);
    }

    public virtual bool HaveToFindSelected() => true;

    protected StorableUI selectedStorable
    {
      get
      {
        if ((UnityEngine.Object) this._selectedStorable == (UnityEngine.Object) null && InputService.Instance.JoystickUsed && this.HaveToFindSelected())
          this.FindCurrentCellInstance();
        return this._selectedStorable;
      }
      set
      {
        if ((UnityEngine.Object) this._selectedStorable != (UnityEngine.Object) null)
          this._selectedStorable.SetSelected(false);
        this._selectedStorable = value;
        if ((UnityEngine.Object) this.selectionFrame == (UnityEngine.Object) null)
          return;
        this.selectionFrame.gameObject.SetActive(false);
        if (!((UnityEngine.Object) this._selectedStorable != (UnityEngine.Object) null))
          return;
        this._selectedStorable.SetSelected(true);
        this.SaveCurrentSelectedInstance(this.selectedStorable);
      }
    }

    protected void SetSelectedFrame()
    {
      if ((UnityEngine.Object) this.selectionFrame == (UnityEngine.Object) null)
        return;
      this.selectionFrame.gameObject.SetActive(InputService.Instance.JoystickUsed && (UnityEngine.Object) this._selectedStorable != (UnityEngine.Object) null && !this.drag.IsEnabled);
      if (!((UnityEngine.Object) this._selectedStorable != (UnityEngine.Object) null))
        return;
      InventoryCellStyle style = this._selectedStorable.Style;
      Vector2 innerSize = InventoryUtility.CalculateInnerSize((IInventoryGridBase) ((StorableComponent) this._selectedStorable.Internal).Placeholder.Grid, style);
      this.selectionFrame.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, innerSize.x + style.BackgroundImageOffset.x * 2f);
      this.selectionFrame.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, innerSize.y + style.BackgroundImageOffset.x * 2f);
      this.selectionFrame.rectTransform.position = this._selectedStorable.Image.rectTransform.position;
    }

    protected virtual void Subscribe()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      service.AddListener(GameActionType.Split, new GameActionHandle(this.SplitListener));
      service.AddListener(GameActionType.Submit, new GameActionHandle(this.ContextListener));
      service.AddListener(GameActionType.Context, new GameActionHandle(this.DragListener));
    }

    protected void SubscribeNavigation()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.AddListener(GameActionType.LStickUp, new GameActionHandle(this.NavigationListener));
      service.AddListener(GameActionType.LStickDown, new GameActionHandle(this.NavigationListener));
      service.AddListener(GameActionType.LStickRight, new GameActionHandle(this.NavigationListener));
      service.AddListener(GameActionType.LStickLeft, new GameActionHandle(this.NavigationListener));
      service.AddListener(GameActionType.DPadUp, new GameActionHandle(this.NavigationListener));
      service.AddListener(GameActionType.DPadDown, new GameActionHandle(this.NavigationListener));
      service.AddListener(GameActionType.DPadRight, new GameActionHandle(this.NavigationListener));
      service.AddListener(GameActionType.DPadLeft, new GameActionHandle(this.NavigationListener));
      this.UnsubscribeConsoleDragNavigation();
    }

    protected void UnsubscribeConsoleDragNavigation()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.LStickUp, new GameActionHandle(this.ConsoleDragNavigationListener));
      service.RemoveListener(GameActionType.LStickDown, new GameActionHandle(this.ConsoleDragNavigationListener));
      service.RemoveListener(GameActionType.LStickRight, new GameActionHandle(this.ConsoleDragNavigationListener));
      service.RemoveListener(GameActionType.LStickLeft, new GameActionHandle(this.ConsoleDragNavigationListener));
      service.RemoveListener(GameActionType.Submit, new GameActionHandle(this.DragListener));
      if (this.scrollCoroutine == null)
        return;
      this.StopCoroutine(this.scrollCoroutine);
    }

    protected void SubscribeConsoleDragNavigation()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.AddListener(GameActionType.LStickUp, new GameActionHandle(this.ConsoleDragNavigationListener), true);
      service.AddListener(GameActionType.LStickDown, new GameActionHandle(this.ConsoleDragNavigationListener), true);
      service.AddListener(GameActionType.LStickRight, new GameActionHandle(this.ConsoleDragNavigationListener), true);
      service.AddListener(GameActionType.LStickLeft, new GameActionHandle(this.ConsoleDragNavigationListener), true);
      service.AddListener(GameActionType.Submit, new GameActionHandle(this.DragListener), true);
    }

    private bool ConsoleDragNavigationListener(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      BaseInventoryWindow<T>.Navigation navigation = BaseInventoryWindow<T>.Navigation.None;
      switch (type)
      {
        case GameActionType.LStickUp:
          navigation = BaseInventoryWindow<T>.Navigation.CellUp;
          break;
        case GameActionType.LStickDown:
          navigation = BaseInventoryWindow<T>.Navigation.CellDown;
          break;
        case GameActionType.LStickLeft:
          navigation = BaseInventoryWindow<T>.Navigation.CellLeft;
          break;
        case GameActionType.LStickRight:
          navigation = BaseInventoryWindow<T>.Navigation.CellRight;
          break;
      }
      if (down)
      {
        if (this.scrollCoroutine != null)
          this.StopCoroutine(this.scrollCoroutine);
        this.scrollCoroutine = this.StartCoroutine(this.ScrollCoroutine(new BaseInventoryWindow<T>.NavigationScrollHandle(this.OnMoveItem), navigation));
        this.coroutineNavigation = navigation;
        return true;
      }
      if (this.scrollCoroutine != null && navigation == this.coroutineNavigation)
      {
        this.StopCoroutine(this.scrollCoroutine);
        this.scrollCoroutine = (Coroutine) null;
      }
      return false;
    }

    protected void UnsubscribeNavigation()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.LStickUp, new GameActionHandle(this.NavigationListener));
      service.RemoveListener(GameActionType.LStickDown, new GameActionHandle(this.NavigationListener));
      service.RemoveListener(GameActionType.LStickRight, new GameActionHandle(this.NavigationListener));
      service.RemoveListener(GameActionType.LStickLeft, new GameActionHandle(this.NavigationListener));
      service.RemoveListener(GameActionType.DPadUp, new GameActionHandle(this.NavigationListener));
      service.RemoveListener(GameActionType.DPadDown, new GameActionHandle(this.NavigationListener));
      service.RemoveListener(GameActionType.DPadRight, new GameActionHandle(this.NavigationListener));
      service.RemoveListener(GameActionType.DPadLeft, new GameActionHandle(this.NavigationListener));
      if (this.scrollCoroutine == null)
        return;
      this.StopCoroutine(this.scrollCoroutine);
    }

    protected virtual void Unsubscribe()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      service.RemoveListener(GameActionType.Split, new GameActionHandle(this.SplitListener));
      service.RemoveListener(GameActionType.Submit, new GameActionHandle(this.ContextListener));
      service.RemoveListener(GameActionType.Context, new GameActionHandle(this.DragListener));
    }

    protected bool DragListener(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed || !down)
        return false;
      this.currentAcceleration = 0.0f;
      if (type != GameActionType.Context && type != GameActionType.Submit)
        return false;
      if (!this.drag.IsEnabled && type != GameActionType.Submit)
      {
        if ((UnityEngine.Object) this.selectedContainer != (UnityEngine.Object) null)
          return true;
        if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
          this.SaveCurrentSelectedInstance(this.selectedStorable);
        else
          this.FindCurrentCellInstance();
        if ((UnityEngine.Object) this.selectedStorable == (UnityEngine.Object) null)
          return false;
        this.selectedStorable.SetSelected(false);
        if (this.DragBegin(this.selectedStorable.Internal))
        {
          this.UnsubscribeNavigation();
          this.SubscribeConsoleDragNavigation();
          this.Unsubscribe();
          ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Context, new GameActionHandle(this.DragListener));
          this.isConsoleDragging = true;
        }
        this.HideContextMenu();
        this.HideInfoWindow();
        return true;
      }
      bool isSuccess;
      this.DragEnd(this.intersect, out isSuccess);
      if (isSuccess)
      {
        this.Subscribe();
        this.SubscribeNavigation();
        this.isConsoleDragging = false;
        this.SaveCurrentSelectedInstance(this.selectedStorable);
        if ((UnityEngine.Object) this._selectedStorable != (UnityEngine.Object) null)
          this.currentInventory = this.selectedStorable.GetComponentInParent<ContainerResizableWindow>();
      }
      return true;
    }

    private void ConsoleDraggingUpdate()
    {
    }

    private IEnumerator ScrollCoroutine(
      BaseInventoryWindow<T>.NavigationScrollHandle handle,
      BaseInventoryWindow<T>.Navigation navigation)
    {
      while (true)
      {
        if (handle != null)
          handle(navigation);
        yield return (object) new WaitForSeconds(0.2f);
      }
    }

    private bool NavigationListener(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      BaseInventoryWindow<T>.Navigation navigation = BaseInventoryWindow<T>.Navigation.None;
      switch (type)
      {
        case GameActionType.LStickUp:
          navigation = BaseInventoryWindow<T>.Navigation.CellUp;
          break;
        case GameActionType.LStickDown:
          navigation = BaseInventoryWindow<T>.Navigation.CellDown;
          break;
        case GameActionType.LStickLeft:
          navigation = BaseInventoryWindow<T>.Navigation.CellLeft;
          break;
        case GameActionType.LStickRight:
          navigation = BaseInventoryWindow<T>.Navigation.CellRight;
          break;
        case GameActionType.DPadUp:
          navigation = BaseInventoryWindow<T>.Navigation.ContainerUp;
          break;
        case GameActionType.DPadDown:
          navigation = BaseInventoryWindow<T>.Navigation.ContainerDown;
          break;
        case GameActionType.DPadLeft:
          navigation = BaseInventoryWindow<T>.Navigation.ContainerLeft;
          break;
        case GameActionType.DPadRight:
          navigation = BaseInventoryWindow<T>.Navigation.ContainerRight;
          break;
      }
      if (down)
      {
        if (this.scrollCoroutine != null)
          this.StopCoroutine(this.scrollCoroutine);
        this.scrollCoroutine = this.StartCoroutine(this.ScrollCoroutine(new BaseInventoryWindow<T>.NavigationScrollHandle(this.OnNavigate), navigation));
        this.coroutineNavigation = navigation;
        return true;
      }
      if (this.scrollCoroutine != null && navigation == this.coroutineNavigation)
      {
        this.StopCoroutine(this.scrollCoroutine);
        this.scrollCoroutine = (Coroutine) null;
      }
      return false;
    }

    protected virtual void FillSelectableList(List<GameObject> list, bool block, bool addSelected = false)
    {
      list.Clear();
      foreach (KeyValuePair<IStorableComponent, StorableUI> storable in this.storables)
      {
        IInventoryComponent container = storable.Key.Container;
        if (addSelected || !((UnityEngine.Object) storable.Value == (UnityEngine.Object) this.selectedStorable))
        {
          Vector3 position = storable.Value.transform.position;
          if ((double) position.x >= 0.0 && (double) position.y >= 0.0 && (double) position.x <= (double) Screen.width && (double) position.y <= (double) Screen.height && (!block || container == null || !((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null) || container != this.selectedStorable.Internal.Container) && this.AdditionalConditionOfSelectableList(storable.Value))
            list.Add(storable.Value.gameObject);
        }
      }
    }

    protected virtual bool AdditionalConditionOfSelectableList(StorableUI storable) => true;

    protected virtual Vector2 CurentNavigationPosition()
    {
      RectTransform transform = (UnityEngine.Object) this._selectedStorable != (UnityEngine.Object) null ? this._selectedStorable.transform as RectTransform : (RectTransform) null;
      return (Vector2) ((UnityEngine.Object) transform != (UnityEngine.Object) null ? transform.TransformPoint((Vector3) transform.rect.center) : this.transform.position);
    }

    protected virtual void OnSelectObject(GameObject selected)
    {
      StorableUI component = selected?.GetComponent<StorableUI>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
        return;
      this.selectedStorable = component;
      if (InputService.Instance.JoystickUsed)
        this.ShowInfoWindow(this.selectedStorable.Internal);
      this.selectedStorable.SetSelected(true);
    }

    protected virtual void OnMoveItem(BaseInventoryWindow<T>.Navigation navigation)
    {
      Vector2 direction = Vector2.zero;
      if (!this.drag.IsEnabled)
        return;
      switch (navigation)
      {
        case BaseInventoryWindow<T>.Navigation.CellUp:
          direction = Vector2.up;
          break;
        case BaseInventoryWindow<T>.Navigation.CellDown:
          direction = Vector2.down;
          break;
        case BaseInventoryWindow<T>.Navigation.CellRight:
          direction = Vector2.right;
          break;
        case BaseInventoryWindow<T>.Navigation.CellLeft:
          direction = Vector2.left;
          break;
      }
      if (!(direction != Vector2.zero))
        return;
      if ((UnityEngine.Object) this.selectedStorable == (UnityEngine.Object) null)
        this.FindCurrentCellInstance();
      Vector3 position = this.selectedStorable.transform.position;
      StorableComponent storableComponent = this.selectedStorable.Internal as StorableComponent;
      int rows = storableComponent.Placeholder.Grid.Rows;
      int columns = storableComponent.Placeholder.Grid.Columns;
      List<GameObject> objects = new List<GameObject>();
      foreach (InventoryContainerUI key1 in this.containers.Keys)
      {
        InventoryContainerUI inventory = key1;
        if (inventory.InventoryContainer.Enabled.Value || inventory.InventoryContainer.GetStorage() != this.Actor)
        {
          IEnumerable<KeyValuePair<Cell, InventoryCellUI>> source = inventory.Cells.Where<KeyValuePair<Cell, InventoryCellUI>>((Func<KeyValuePair<Cell, InventoryCellUI>, bool>) (c =>
          {
            Cell key2 = c.Key;
            if (this.selectedStorable.transform.position == c.Value.transform.position || (double) c.Value.transform.position.x < 0.0 || (double) c.Value.transform.position.y < 0.0 || (double) c.Value.transform.position.x > (double) Screen.width || (double) c.Value.transform.position.y > (double) Screen.height)
              return false;
            if (inventory.InventoryContainer.GetGroup() == InventoryGroup.Clothes || inventory.InventoryContainer.GetGroup() == InventoryGroup.Weapons)
              return (double) Vector2.Dot((Vector2) (c.Value.transform.position - this.selectedStorable.transform.position).normalized, direction) >= 0.85000002384185791;
            return key2.Column + columns <= inventory.InventoryContainer.GetGrid().Columns && key2.Row + rows <= inventory.InventoryContainer.GetGrid().Rows && (inventory.InventoryContainer.GetGroup() == InventoryGroup.Backpack || inventory.InventoryContainer.GetGroup() == InventoryGroup.Loot) && (double) Vector2.Dot((Vector2) (c.Value.transform.position - this.selectedStorable.transform.position).normalized, direction) >= 0.85000002384185791;
          }));
          if (source != null)
            objects.AddRange((IEnumerable<GameObject>) source.Select<KeyValuePair<Cell, InventoryCellUI>, GameObject>((Func<KeyValuePair<Cell, InventoryCellUI>, GameObject>) (c => c.Value.gameObject)).ToList<GameObject>());
        }
      }
      GameObject gameObject = UISelectableHelper.Select((IEnumerable<GameObject>) objects, this.selectedStorable.transform.position, (Vector3) direction);
      if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
        position = gameObject.transform.position;
      this.selectedStorable.transform.position = position;
    }

    protected virtual InventoryContainerUI selectedContainer
    {
      get => (InventoryContainerUI) null;
      set
      {
      }
    }

    protected virtual void OnNavigate(BaseInventoryWindow<T>.Navigation navigation)
    {
      Vector2 dirrection = Vector2.zero;
      if ((UnityEngine.Object) this.selectedStorable == (UnityEngine.Object) null && (UnityEngine.Object) this.selectedContainer == (UnityEngine.Object) null)
      {
        this.GetFirstStorable();
      }
      else
      {
        this.FillSelectableList(this.selectableList, navigation >= BaseInventoryWindow<T>.Navigation.ContainerUp);
        Vector2 origin = this.CurentNavigationPosition();
        switch (navigation)
        {
          case BaseInventoryWindow<T>.Navigation.CellClosest:
            this.OnSelectObject(UISelectableHelper.SelectClosest((IEnumerable<GameObject>) this.selectableList, (Vector3) origin));
            break;
          case BaseInventoryWindow<T>.Navigation.CellUp:
          case BaseInventoryWindow<T>.Navigation.ContainerUp:
            dirrection = Vector2.up;
            break;
          case BaseInventoryWindow<T>.Navigation.CellDown:
          case BaseInventoryWindow<T>.Navigation.ContainerDown:
            dirrection = Vector2.down;
            break;
          case BaseInventoryWindow<T>.Navigation.CellRight:
          case BaseInventoryWindow<T>.Navigation.ContainerRight:
            dirrection = Vector2.right;
            break;
          case BaseInventoryWindow<T>.Navigation.CellLeft:
          case BaseInventoryWindow<T>.Navigation.ContainerLeft:
            dirrection = Vector2.left;
            break;
        }
        this.OnSelectObject(UISelectableHelper.Select((IEnumerable<GameObject>) this.selectableList, (Vector3) origin, (Vector3) dirrection));
      }
    }

    protected void FindCurrentCellInstance()
    {
      List<StorableUI> list1 = this.storables.Values.Where<StorableUI>((Func<StorableUI, bool>) (storable => (storable.Internal.Owner.TemplateId == this.lastSelectedTemplateId || this.lastSelectedTemplateId == new Guid()) && (double) Vector3.Distance(storable.transform.position, this.lastStorablePosition) <= 500.0 && this.ItemIsInteresting(storable.Internal))).ToList<StorableUI>();
      StorableUI storableUi = (StorableUI) null;
      if (list1 == null)
        return;
      List<StorableUI> list2 = list1.Where<StorableUI>((Func<StorableUI, bool>) (storable => storable.IsEnabled)).OrderBy<StorableUI, float>((Func<StorableUI, float>) (storable => Vector3.Distance(this.lastStorablePosition, storable.gameObject.transform.position))).ToList<StorableUI>();
      if (list2.Count > 0)
        storableUi = list2.First<StorableUI>();
      if ((UnityEngine.Object) storableUi == (UnityEngine.Object) null)
        return;
      if ((UnityEngine.Object) this._selectedStorable != (UnityEngine.Object) null)
      {
        if (this.shouldDeselectStorable || this._selectedStorable is StorableUITrade && ((StorableUITrade) this._selectedStorable).GetSelectedCount() == 0 || InputService.Instance.JoystickUsed)
        {
          this._selectedStorable.SetSelected(false);
          this.shouldDeselectStorable = false;
        }
        if (!(storableUi is StorableUITrade) && !storableUi.IsSelected())
          this.shouldDeselectStorable = true;
      }
      this.selectedStorable = storableUi;
      this.ShowInfoWindow(this._selectedStorable.Internal);
      this._selectedStorable.SetSelected(true);
    }

    private void OnNavigateContainers(BaseInventoryWindow<T>.Navigation navigation)
    {
      if ((UnityEngine.Object) this.currentInventory == (UnityEngine.Object) null)
        this.currentInventory = this._selectedStorable?.GetComponentInParent<ContainerResizableWindow>();
      this.currentContainer = this._selectedStorable?.GetComponentInParent<LimitedInventoryContainerUI>();
      if ((UnityEngine.Object) this.currentInventory == (UnityEngine.Object) null)
        this.currentInventory = new List<ContainerResizableWindow>((IEnumerable<ContainerResizableWindow>) this.GetComponentsInChildren<ContainerResizableWindow>())[0];
      List<LimitedInventoryContainerUI> source = new List<LimitedInventoryContainerUI>((IEnumerable<LimitedInventoryContainerUI>) this.currentInventory.GetComponentsInChildren<LimitedInventoryContainerUI>());
      for (int index = 0; index < source.Count; ++index)
      {
        if (source[index].GetComponentsInChildren<StorableUI>().Length < 1)
          source.Remove(source[index]);
      }
      if ((UnityEngine.Object) this.currentContainer == (UnityEngine.Object) null)
        this.currentContainer = source[0];
      source.Remove(this.currentContainer);
      if (source.Count <= 1)
        return;
      foreach (Component component in source)
        Debug.Log((object) Vector3.Distance(component.transform.position, this.currentContainer.transform.position));
      List<LimitedInventoryContainerUI> list = source.Where<LimitedInventoryContainerUI>((Func<LimitedInventoryContainerUI, bool>) (container => (double) Vector3.Distance(container.transform.position, this.currentContainer.transform.position) <= 800.0)).ToList<LimitedInventoryContainerUI>();
      switch (navigation)
      {
        case BaseInventoryWindow<T>.Navigation.ContainerUp:
          list = list.Where<LimitedInventoryContainerUI>((Func<LimitedInventoryContainerUI, bool>) (container => (double) container.transform.position.y > (double) this.currentContainer.transform.position.y)).ToList<LimitedInventoryContainerUI>();
          break;
        case BaseInventoryWindow<T>.Navigation.ContainerDown:
          list = list.Where<LimitedInventoryContainerUI>((Func<LimitedInventoryContainerUI, bool>) (container => (double) container.transform.position.y < (double) this.currentContainer.transform.position.y)).ToList<LimitedInventoryContainerUI>();
          break;
        case BaseInventoryWindow<T>.Navigation.ContainerRight:
          list = list.Where<LimitedInventoryContainerUI>((Func<LimitedInventoryContainerUI, bool>) (container => (double) container.transform.position.x > (double) this.currentContainer.transform.position.x)).ToList<LimitedInventoryContainerUI>();
          break;
        case BaseInventoryWindow<T>.Navigation.ContainerLeft:
          list = list.Where<LimitedInventoryContainerUI>((Func<LimitedInventoryContainerUI, bool>) (container => (double) container.transform.position.x < (double) this.currentContainer.transform.position.x)).ToList<LimitedInventoryContainerUI>();
          break;
      }
      if (list.Count <= 0)
        return;
      this.currentContainer = list.OrderBy<LimitedInventoryContainerUI, float>((Func<LimitedInventoryContainerUI, float>) (container => Vector3.Distance(this.currentContainer.transform.position, container.gameObject.transform.position))).First<LimitedInventoryContainerUI>();
      this.SelectFirstStorableInContainer(new List<StorableUI>((IEnumerable<StorableUI>) this.currentContainer.GetComponentsInChildren<StorableUI>()));
      this.SetSelectedFrame();
    }

    protected virtual void SelectFirstStorableInContainer(List<StorableUI> storables)
    {
      if (!this.CanShowInfoWindows)
        return;
      if (this.lastStorablePosition == Vector3.zero)
        this.lastStorablePosition = new Vector3(0.0f, this.actorContainerWindow.transform.position.y + this.actorContainerWindow.GetComponent<RectTransform>().sizeDelta.y);
      this.FillSelectableList(this.selectableList, false, true);
      this.OnSelectObject(UISelectableHelper.SelectClosest((IEnumerable<GameObject>) this.selectableList, this.lastStorablePosition));
    }

    protected bool ContextListener(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      if (down && (UnityEngine.Object) this.windowContextMenu != (UnityEngine.Object) null)
      {
        this.HideContextMenu();
        return down;
      }
      if (!down || !((UnityEngine.Object) this._selectedStorable != (UnityEngine.Object) null))
        return down;
      this.ShowContextMenu((StorableComponent) this._selectedStorable.Internal);
      return down;
    }

    protected bool SetStorableByComponent(IStorableComponent component)
    {
      StorableUI storableByComponent = this.GetStorableByComponent(component);
      if (!((UnityEngine.Object) storableByComponent != (UnityEngine.Object) null))
        return false;
      this.selectedStorable = storableByComponent;
      this.selectedStorable.SetSelected(true);
      this.ShowInfoWindow(component);
      return true;
    }

    protected StorableUI GetStorableByComponent(IStorableComponent component)
    {
      StorableUI storableUi;
      return component == null || !this.storables.TryGetValue(component, out storableUi) ? (StorableUI) null : storableUi;
    }

    protected enum Navigation
    {
      None = -2, // 0xFFFFFFFE
      CellClosest = -1, // 0xFFFFFFFF
      CellUp = 0,
      CellDown = 1,
      CellRight = 2,
      CellLeft = 3,
      ContainerUp = 4,
      ContainerDown = 5,
      ContainerRight = 6,
      ContainerLeft = 7,
    }

    private delegate void NavigationScrollHandle(BaseInventoryWindow<T>.Navigation navigation) where T : BaseInventoryWindow<T>;
  }
}
