using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
using UnityEngine;
using UnityEngine.Audio;
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
    private List<UIControl> actorAnchors = [];
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
    private List<SlotAnchor> slotAnchor = [];
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
    protected List<IStorageComponent> actors = [];
    protected Dictionary<InventoryContainerUI, IStorageComponent> containers = new();
    protected Dictionary<IInventoryComponent, InventoryContainerUI> containerViews = new();
    private CameraKindEnum lastCameraKind;
    [Inspected]
    protected Dictionary<IStorableComponent, StorableUI> storables = new();
    protected InfoWindowNew windowInfoNew;
    protected SplitGraphic windowSplit;
    protected ContextMenuWindowNew windowContextMenu;
    private AudioState currentOpenedAudioState;
    [Inspected]
    protected DragInventoryCell drag = new();
    [Inspected]
    protected Intersect intersect;
    protected bool splitEnabled;
    [SerializeField]
    protected GameObject controlPanel;
    [SerializeField]
    protected GameObject closeButtonObject;
    protected ControlTipsController tipsPannelController;
    protected bool CanShowInfoWindows = true;
    private bool ShowSimplifiedInfoWindows;
    private bool ShowActionTooltips = true;
    [SerializeField]
    protected Image selectionFrame;
    private StorableUI _selectedStorable;
    private bool isConsoleDragging;
    private Vector2 newStorableDraggingDelta = Vector2.zero;
    protected Vector3 lastStorablePosition;
    private Guid lastSelectedTemplateId;
    private float xPrev;
    private float yPrev = 0.0f;
    private const float MAX_ACCELERATION_RATE = 7.5f;
    private float currentAcceleration;
    private float accelerationSpeed = 3f;
    private Vector2 lastMoveDirecton;
    private Coroutine scrollCoroutine;
    private Navigation coroutineNavigation = Navigation.None;
    protected List<GameObject> selectableList = [];
    private const float CellDistanceThreshold = 500f;
    private bool shouldDeselectStorable;
    public ContainerResizableWindow currentInventory;
    public LimitedInventoryContainerUI currentContainer;
    private const float ContainerDistanceThreshold = 800f;

    [Inspected]
    public IStorageComponent Actor { get; set; }

    private void Awake()
    {
    }

    protected virtual void HideMainNavigationPanel()
    {
      tipsPannelController?.HidePannel("Main");
      closeButtonObject.GetSceneInstance()?.SetActive(false);
    }

    protected virtual void ShowMainNavigationPanel()
    {
      tipsPannelController?.ShowPannel("Main");
      closeButtonObject.GetSceneInstance()?.SetActive(true);
    }

    protected virtual void OnDragBegin()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      HideMainNavigationPanel();
      tipsPannelController?.ShowPannel("Move");
      service.AddListener(GameActionType.Cancel, DragCancel);
      service.RemoveListener(GameActionType.Cancel, CancelListener);
    }

    protected virtual void OnDragEnd()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      tipsPannelController?.HidePannel("Move");
      ShowMainNavigationPanel();
      service.RemoveListener(GameActionType.Cancel, DragCancel);
      service.AddListener(GameActionType.Cancel, CancelListener);
      splitEnabled = false;
    }

    private bool DragCancel(GameActionType type, bool down)
    {
      if (down)
        DragCancel();
      return down;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
      switch (eventData.button)
      {
        case PointerEventData.InputButton.Left:
          if (windowContextMenu != null)
          {
            HideContextMenu();
            break;
          }
          if (!drag.IsEnabled)
          {
            if (intersect.Storables == null)
            {
              Debug.LogError("intersect.Storables == null");
              break;
            }
            StorableComponent storable = intersect.Storables.FirstOrDefault();
            if (storable == null)
              break;
            if (StorableComponentUtility.IsSplittable(storable) && splitEnabled)
            {
              Split(storable);
              break;
            }
            if (ItemIsInteresting(storable))
              DragBegin(storable);
            break;
          }
          DragCheck(ref intersect);
          DragEnd(intersect);
          break;
        case PointerEventData.InputButton.Right:
          StorableComponent storable1 = drag.Storable;
          DragCancel();
          if (drag.Storable == null || !StorableComponentUtility.IsSplittable(drag.Storable))
            break;
          Split(storable1);
          break;
      }
    }

    protected virtual void Update()
    {
      if (isConsoleDragging)
        ConsoleDraggingUpdate();
      UpdateInternal();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
      intersect = !drag.IsEnabled ? GetIntersect(CursorService.Instance.Position) : GetIntersect(drag.Storable);
      switch (eventData.button)
      {
        case PointerEventData.InputButton.Right:
          if (drag.IsEnabled)
          {
            DragCancel();
            break;
          }
          if (windowContextMenu != null)
          {
            HideContextMenu();
            break;
          }
          StorableComponent storable = intersect.Storables.FirstOrDefault();
          if (storable != null)
          {
            bool flag = false;
            if (Actor.Items.Contains(storable))
              ShowContextMenu(storable);
            if (!flag)
              InteractItem(storable);
          }
          break;
        case PointerEventData.InputButton.Middle:
          if (!drag.IsEnabled)
            break;
          break;
      }
      eventData.Use();
    }

    private void ShowContextMenu(StorableComponent storable)
    {
      HideInfoWindow();
      HideContextMenu();
      HideMainNavigationPanel();
      windowContextMenu = ContextMenuWindowNew.Instantiate(InputService.Instance.JoystickUsed ? joystickContextMenuPrefab : contextMenuPrefab);
      windowContextMenu.Transform.SetParent(transform, false);
      windowContextMenu.Target = storable;
      if (storables.TryGetValue(storable, out StorableUI storableUi))
      {
        if (selectedStorable == null)
          selectedStorable = storableUi;
        if (selectedStorable != storableUi && !selectedStorable.IsSliderItem)
          selectedStorable = storableUi;
        if (InputService.Instance.JoystickUsed)
        {
          PositionWindow(windowContextMenu, storable);
        }
        else
        {
          float y;
          switch (storable.Container.GetKind())
          {
            case ContainerCellKind.MultipleCellToOneStorable:
              y = (storableUi.Image.rectTransform.localPosition.y + styleMultipleCellOneStorable.Size.y / 4f) * storableUi.Image.rectTransform.lossyScale.y;
              break;
            case ContainerCellKind.OneCellToOneStorable:
              y = 0.0f;
              break;
            default:
              throw new Exception();
          }
          windowContextMenu.Transform.position = storableUi.Image.transform.position - new Vector3(0.0f, y, 0.0f);
        }
      }
      windowContextMenu.OnButtonInvestigate += ShowInvestigationWindow;
      windowContextMenu.OnButtonDrop += OnButtonDrop;
      windowContextMenu.OnButtonWear += OnButtonDress;
      windowContextMenu.OnButtonUse += OnButtonUse;
      windowContextMenu.OnButtonPourOut += OnButtonPourOut;
      windowContextMenu.OnButtonSplit += OnButtonSplit;
      windowContextMenu.OnClose += HideContextMenu;
      Unsubscribe();
      UnsubscribeNavigation();
    }

    protected void HideContextMenu()
    {
      if (!(windowContextMenu != null))
        return;
      windowContextMenu.OnButtonInvestigate -= ShowInvestigationWindow;
      windowContextMenu.OnButtonDrop -= OnButtonDrop;
      windowContextMenu.OnButtonWear -= OnButtonDress;
      windowContextMenu.OnButtonUse -= OnButtonUse;
      windowContextMenu.OnButtonSplit -= OnButtonSplit;
      windowContextMenu.OnClose -= HideContextMenu;
      Destroy(windowContextMenu.gameObject);
      windowContextMenu = null;
      SubscribeNavigation();
      Subscribe();
      if (storables.Count > 0 && selectedStorable != null)
        ShowInfoWindow(selectedStorable.Internal);
      else
        selectedStorable = null;
      ShowMainNavigationPanel();
    }

    private void ShowInvestigationWindow(IStorableComponent storable)
    {
      HideInfoWindow();
      HideContextMenu();
      IInvestigationWindow investigationWindow = ServiceLocator.GetService<UIService>().Get<IInvestigationWindow>();
      investigationWindow.Actor = Actor;
      investigationWindow.Target = storable;
      ServiceLocator.GetService<UIService>().Push<IInvestigationWindow>();
    }

    private void UICancelClickHandler() => ServiceLocator.GetService<UIService>().Pop();

    private void StorableDisposeEvent(IEntity sender)
    {
      StorableComponent component = sender.GetComponent<StorableComponent>();
      RemoveItemFromView(component);
      if (!drag.IsEnabled || component != drag.Storable)
        return;
      drag.Reset();
    }

    protected void RemoveItemFromView(IStorableComponent storable)
    {
      if (storable == null)
        return;
      if (storables.TryGetValue(storable, out StorableUI storableUi))
      {
        storables.Remove(storable);
        Destroy(storableUi.gameObject);
      }
      if (storable.IsDisposed)
        return;
      ((Entity) storable.Owner).RemoveListener(this);
    }

    private void RemoveOpenResources(IInventoryComponent container)
    {
      if (container.GetOpenResources() == null)
        return;
      foreach (InventoryContainerOpenResource openResource in container.GetOpenResources())
      {
        if (StorageUtility.GetItemAmount(Actor.Items, openResource.ResourceType.Value) >= openResource.Amount)
        {
          RemoveItemsAmount(Actor, openResource.ResourceType.Value, openResource.Amount);
          break;
        }
      }
    }

    protected void RemoveItemsAmount(IStorageComponent storage, IEntity removingItem, int amount)
    {
      Guid itemId = StorageUtility.GetItemId(removingItem);
      int num = amount;
      List<KeyValuePair<IStorableComponent, int>> keyValuePairList = [];
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
      List<IStorableComponent> storableComponentList = [];
      foreach (KeyValuePair<IStorableComponent, int> keyValuePair in keyValuePairList)
      {
        KeyValuePair<IStorableComponent, int> k = keyValuePair;
        IStorableComponent storableComponent = storage.Items.First(x => x.Equals(k.Key));
        storableComponent.Count = k.Value;
        if (storableComponent.Count == 0)
          storableComponentList.Add(storableComponent);
      }
      foreach (IComponent component in storableComponentList)
        component.Owner.Dispose();
    }

    protected virtual void Clear()
    {
      ClearItems();
      drag.Reset();
      HideInfoWindow();
      if (windowInfoNew != null)
      {
        Destroy(windowInfoNew.gameObject);
        windowInfoNew = null;
      }
      if (windowSplit != null)
      {
        windowSplit.Dispose();
        windowSplit = null;
      }
      HideContextMenu();
      if (splitContainer != null && splitContainer.Owner != null)
        splitContainer.Owner.Dispose();
      splitContainer = null;
    }

    protected override void OnEnable()
    {
      tipsPannelController = controlPanel.GetComponent<ControlTipsController>();
      base.OnEnable();
      LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
      if (selectionFrame != null)
        selectionFrame.gameObject.SetActive(false);
      lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
      ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      buttonCancel?.onClick.AddListener(UICancelClickHandler);
      SubscribeNavigation();
      Subscribe();
    }

    protected override void OnDisable()
    {
      PlayerUtility.ShowPlayerHands(true);
      if (drag.IsEnabled)
        DragCancel();
      Clear();
      Actor?.Owner?.GetComponent<AttackerPlayerComponent>()?.ApplyInventoryChange();
      ServiceLocator.GetService<CameraService>().Kind = lastCameraKind;
      buttonCancel?.onClick.RemoveListener(UICancelClickHandler);
      UnsubscribeNavigation();
      UnsubscribeConsoleDragNavigation();
      Unsubscribe();
      isConsoleDragging = false;
      newStorableDraggingDelta = Vector2.zero;
      SetInfoWindowShowMode();
      SaveCurrentSelectedInstance(null);
      selectedStorable = null;
      base.OnDisable();
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      if (drag.IsEnabled)
        DragCancel();
      controlPanel.gameObject.SetActive(joystick);
      if (!(selectedStorable != null))
        return;
      selectedStorable.SetSelected(joystick);
      if (windowContextMenu != null && windowContextMenu.IsEnabled)
      {
        HideContextMenu();
        ShowContextMenu((StorableComponent) selectedStorable.Internal);
      }
      if (InputService.Instance.JoystickUsed && !selectedStorable.IsSliderItem && (windowContextMenu == null || windowContextMenu != null && !windowContextMenu.IsEnabled))
      {
        HideInfoWindow();
        ShowInfoWindow(selectedStorable.Internal);
      }
      if (windowInfoNew != null && windowInfoNew.IsEnabled)
        PositionWindow(windowInfoNew, selectedStorable.Internal);
    }

    protected virtual void GetFirstStorable()
    {
      SelectFirstStorableInContainer(storables.Values.ToList());
      if (_selectedStorable == null && storables.Count > 0)
        selectedStorable = GetComponentInChildren<StorableUI>();
      if (!(_selectedStorable != null))
        return;
      selectedStorable.SetSelected(true);
      ShowInfoWindow(_selectedStorable.Internal);
    }

    protected virtual bool WithPrice() => false;

    protected void Build2()
    {
      PlayerUtility.ShowPlayerHands(false);
      Clear();
      HideInfoWindow();
      if (windowInfoNew == null && inventoryInfoPrefabNew != null)
      {
        windowInfoNew = InfoWindowNew.Instantiate(WithPrice(), inventoryInfoPrefabNew);
        windowInfoNew.Transform.SetParent(transform, false);
        windowInfoNew.IsEnabled = false;
      }
      if (windowSplit == null)
      {
        windowSplit = SplitGraphic.Instantiate(inventorySplitPrefab);
        if (windowSplit != null)
        {
          windowSplit.Transform.SetParent(transform, false);
          windowSplit.IsEnabled = false;
          windowSplit.Disable_Event += SplitDisableHandler;
        }
      }
      CreateItems();
      OnInvalidate();
    }

    protected void OpenBegin(IInventoryComponent container) => OnContainerOpenBegin(container);

    private void OpenBegin(InventoryContainerUI uiContainer)
    {
      OpenBegin(uiContainer.InventoryContainer);
    }

    protected void OpenEnd(IInventoryComponent container, bool complete, bool keepClosedOnUnlock)
    {
      if (complete)
      {
        if (container.OpenState.Value == ContainerOpenStateEnum.Locked)
          RemoveOpenResources(container);
        int num = !keepClosedOnUnlock ? 0 : (container.OpenState.Value == ContainerOpenStateEnum.Locked ? 1 : 0);
        container.OpenState.Value = num == 0 ? ContainerOpenStateEnum.Open : ContainerOpenStateEnum.Closed;
      }
      OnContainerOpenEnd(container, complete);
      OnInvalidate();
    }

    protected virtual void OpenEnd(IInventoryComponent container, bool complete)
    {
      OpenEnd(container, complete, false);
    }

    protected void OpenEnd(
      InventoryContainerUI uiContainer,
      bool complete,
      bool keepClosedOnUnlock)
    {
      OpenEnd(uiContainer.InventoryContainer, complete, keepClosedOnUnlock);
    }

    protected virtual void OpenEnd(InventoryContainerUI uiContainer, bool complete)
    {
      OpenEnd(uiContainer, complete, false);
    }

    protected virtual void UpdateContainerStates()
    {
      foreach (KeyValuePair<InventoryContainerUI, IStorageComponent> container in containers)
        UpdateContainerStates(container.Key);
      UpdateCoins();
      ResizeActorContainersWindow();
    }

    private void UpdateContainerStates(InventoryContainerUI uiContainer)
    {
      IInventoryComponent inventoryContainer = uiContainer.InventoryContainer;
      if (inventoryContainer == null)
        return;
      bool b = CanOpenContainer(inventoryContainer);
      if (uiContainer.ImageForeground != null)
      {
        uiContainer.ImageForeground.sprite = inventoryContainer.OpenState.Value == ContainerOpenStateEnum.Open ? null : inventoryContainer.GetImageForeground();
        uiContainer.ImageForeground.gameObject.SetActive(uiContainer.ImageForeground.sprite != null);
      }
      if (uiContainer.ImageIcon != null)
      {
        if (!inventoryContainer.Available.Value)
        {
          Sprite imageNotAvailable = inventoryContainer.GetImageNotAvailable();
          uiContainer.ImageIcon.sprite = imageNotAvailable ?? nonAvailableIcon;
        }
        else if (inventoryContainer.OpenState.Value == ContainerOpenStateEnum.Locked && inventoryContainer.GetInstrument() != 0)
        {
          uiContainer.ImageIcon.sprite = inventoryContainer.GetImageInstrument();
          uiContainer.SetIconEnabled(b);
        }
        else
          uiContainer.ImageIcon.sprite = null;
        uiContainer.ImageIcon.gameObject.SetActive(uiContainer.ImageIcon.sprite != null);
      }
      if (uiContainer.ImageDisease != null)
        uiContainer.ImageDisease.gameObject.SetActive(inventoryContainer.OpenState.Value != ContainerOpenStateEnum.Open && inventoryContainer.Disease.Value > 0.0);
      if (uiContainer.ImageLock != null)
      {
        if (inventoryContainer.OpenState.Value == ContainerOpenStateEnum.Locked)
        {
          uiContainer.ImageLock.sprite = inventoryContainer.GetImageLock();
          uiContainer.SetLockEnabled(b);
        }
        else
          uiContainer.ImageLock.sprite = null;
        uiContainer.ImageLock.gameObject.SetActive(uiContainer.ImageLock.sprite != null);
      }
      if (uiContainer.Button != null)
      {
        bool flag = ((!inventoryContainer.Available.Value ? 0 : (inventoryContainer.OpenState.Value != ContainerOpenStateEnum.Open ? 1 : 0)) & (b ? 1 : 0)) != 0;
        if (flag)
        {
          int num = inventoryContainer.OpenState.Value != ContainerOpenStateEnum.Locked ? 0 : (overrideUnlockTime >= 0.0 ? 1 : 0);
          uiContainer.Button.HoldTime = num == 0 ? inventoryContainer.GetOpenTime() : overrideUnlockTime;
        }
        uiContainer.Button.gameObject.SetActive(flag);
      }
      IStorableComponent itemInContainer = GetItemInContainer(inventoryContainer, inventoryContainer.GetStorage());
      if (itemInContainer == null || drag.IsEnabled && drag.Storable == itemInContainer)
        uiContainer.ImageBackground.gameObject.SetActive(uiContainer.ImageBackground.sprite != null);
      else
        uiContainer.ImageBackground.gameObject.SetActive(false);
    }

    protected virtual void UpdateCoins()
    {
      if (moneyText == null)
        return;
      int num = 0;
      foreach (IStorableComponent storableComponent in Actor.Items)
      {
        if (storableComponent.Groups.Contains(StorableGroup.Money))
          num += storableComponent.Count;
      }
      moneyText.text = num.ToString();
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
      bool flag = ItemsContainsOpenResources(Actor.Items, container.GetOpenResources());
      return (container.GetInstrument() == StorableGroup.None || GetInstrument(container.GetInstrument()) != null) & flag;
    }

    protected virtual void DamageInstrument(IInventoryComponent container)
    {
      IEntity instrument = GetInstrument(container.GetInstrument());
      if (instrument == null)
        return;
      DamageInstrument(instrument, container);
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
      foreach (IStorableComponent storableComponent in Actor.Items)
      {
        if (storableComponent.Groups.Contains(instrumentGroup))
        {
          IEntity owner = storableComponent.Owner;
          ParametersComponent component = owner.GetComponent<ParametersComponent>();
          if (component == null)
            return owner;
          IParameter<float> byName = component.GetByName<float>(ParameterNameEnum.Durability);
          if (byName == null)
            return owner;
          if (byName.Value > 0.0 | brokenEnabled)
            source.Add(owner, byName);
        }
      }
      if (source.Count <= 0)
        return null;
      IEntity key = source.First().Key;
      if (brokenEnabled)
        return key;
      IParameter<float> parameter = source.First().Value;
      foreach (KeyValuePair<IEntity, IParameter<float>> keyValuePair in source)
      {
        if (keyValuePair.Value.Value < (double) parameter.Value)
        {
          key = keyValuePair.Key;
          parameter = keyValuePair.Value;
        }
      }
      return key;
    }

    protected virtual bool ValidateComputeActor(IStorageComponent actor) => actor == Actor;

    protected virtual bool ValidateContainer(
      IInventoryComponent container,
      IStorageComponent storage)
    {
      if (container.GetStorage() == Actor)
      {
        if (!container.Enabled.Value)
          return false;
        IEnumerable<StorableGroup> limitations = container.GetLimitations();
        if (limitations.Contains(StorableGroup.Money) || limitations.Contains(StorableGroup.Key) || limitations.Contains(StorableGroup.Weapons_Hands))
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
      if (!storables.ContainsKey(storable))
      {
        AddItemToView(storable);
      }
      else
      {
        StorableUI storable1 = storables[storable];
        InventoryContainerUI container1 = GetContainer(storage, container);
        if (container1 == null)
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
              style = styleMultipleCellOneStorable;
              spriteByStyle = InventoryUtility.GetSpriteByStyle(storable.Placeholder, styleMultipleCellOneStorable.imageStyle);
              break;
            case ContainerCellKind.OneCellToOneStorable:
              style = styleOneCellOneStorable;
              spriteByStyle = InventoryUtility.GetSpriteByStyle(storable.Placeholder, styleOneCellOneStorable.imageStyle);
              break;
            default:
              throw new Exception();
          }
          storable1.Style = style;
          storable1.Image.sprite = spriteByStyle;
          storable1.transform.SetParent(container1.Storables.transform, false);
          Vector2 storablePosition = InventoryUtility.CalculateStorablePosition(storable.Cell, style);
          storable1.Transform.localPosition = storablePosition;
        }
      }
    }

    private void OnButtonDrop(IStorableComponent storable)
    {
      ServiceLocator.GetService<DropBagService>().DropBag(storable, Actor.Owner);
      RemoveItemFromView(storable);
      PlayAudio(dropItemAudio);
      HideContextMenu();
      HideInfoWindow();
      UpdateContainerStates();
      OnNavigate(Navigation.CellClosest);
    }

    private InventoryContainerUI GetContainer(
      IStorageComponent storage,
      IInventoryComponent container)
    {
      foreach (KeyValuePair<InventoryContainerUI, IStorageComponent> container1 in containers)
      {
        if (container1.Value == storage && container1.Key.InventoryContainer == container)
          return container1.Key;
      }
      return null;
    }

    private void RegisterItemInView(IStorableComponent storable, InventoryCellSizeEnum size)
    {
      if (storable == null || storable.IsDisposed)
        return;
      StorableUI storableUi = StorableUI.Instantiate(storable, inventoryStorablePrefab, size);
      storableUi.Style = styleMultipleCellOneStorable;
      storables[storable] = storableUi;
      ((Entity) storable.Owner).AddListener(this);
    }

    protected bool AddItemToView(IStorableComponent storable)
    {
      InventoryContainerUI container = GetContainer(storable.Storage, storable.Container);
      if (container == null)
        return false;
      InventoryCellStyle inventoryCellStyle;
      switch (container.InventoryContainer.GetKind())
      {
        case ContainerCellKind.MultipleCellToOneStorable:
          inventoryCellStyle = styleMultipleCellOneStorable;
          break;
        case ContainerCellKind.OneCellToOneStorable:
          inventoryCellStyle = styleOneCellOneStorable;
          break;
        default:
          inventoryCellStyle = new InventoryCellStyle();
          break;
      }
      RegisterItemInView(storable, inventoryCellStyle.imageStyle);
      if (!storables.ContainsKey(storable))
        return false;
      storables[storable].Style = inventoryCellStyle;
      PlaceTo(storable.Storage, storable.Container, (StorableComponent) storable, ((StorableComponent) storable).Cell);
      return true;
    }

    protected virtual bool DragBegin(IStorableComponent storable)
    {
      if (drag.IsEnabled || storable == null || storable.IsDisposed || !storables.ContainsKey(storable) || storable.Container != null && containerViews.ContainsKey(storable.Container) && !containerViews[storable.Container].ClickEnabled)
        return false;
      StorableComponentUtility.PlayTakeSound(storable);
      if (!StorableComponentUtility.IsDraggable(storable))
      {
        MoveItem(storable, Actor);
        return false;
      }
      OnDragBegin();
      drag.Reset();
      drag.IsEnabled = true;
      CursorService.Instance.Visible = false;
      windowSplit.Actor = null;
      if (storable.Container != null)
      {
        drag.Storage = storable.Storage;
        drag.Container = storable.Container;
        drag.Storable = (StorableComponent) storable;
        drag.Cell = ((StorableComponent) storable).Cell;
      }
      else
      {
        drag.Storage = null;
        drag.Container = null;
        drag.Storable = (StorableComponent) storable;
        drag.Cell = null;
      }
      StorableUI storable1 = storables[storable];
      drag.MouseOffset = InventoryUtility.GetCenter(storable1);
      storable1.Dragging = true;
      Vector3 vector3 = Vector3.zero;
      if (selectedStorable != null)
        vector3 = selectedStorable.transform.position;
      storable1.transform.SetParent(dragAnchor.transform, false);
      if (selectedStorable != null)
        storable1.transform.position = vector3;
      if (storable.Container == null)
        return true;
      intersect = StorageUtility.GetIntersect(storable.Storage, storable.Container, (StorableComponent) storable, ((StorableComponent) storable).Cell);
      foreach (CellInfo cell1 in intersect.Cells)
      {
        InventoryContainerUI container = GetContainer(drag.Storage, drag.Container);
        if (!(container == null))
        {
          foreach (KeyValuePair<Cell, InventoryCellUI> cell2 in container.Cells)
          {
            if (cell1.Cell.Column == cell2.Key.Column && cell1.Cell.Row == cell2.Key.Row)
            {
              cell2.Value.State = CellState.Current;
              drag.BaseCells.Add(cell2.Value);
            }
          }
        }
      }
      UpdateContainerStates();
      return true;
    }

    protected virtual void Drag()
    {
      if (!drag.IsEnabled)
        return;
      if (!storables.ContainsKey(drag.Storable))
      {
        Debug.LogError("!storables.ContainsKey(drag.Storable), Такого быть недолжно, воспроизвести, разобраться в чем дело и пофиксить нормально");
      }
      else
      {
        StorableUI storable = storables[drag.Storable];
        storable.Dragging = true;
        drag.MouseOffset = InventoryUtility.GetCenter(storable);
        if (storable.Transform.parent != dragAnchor.transform.transform)
        {
          if (selectedStorable == null)
            selectedStorable = storable;
          Vector3 position = selectedStorable.transform.position;
          storable.Transform.SetParent(dragAnchor.transform.transform, false);
          storable.transform.position = position;
        }
        drag.MouseOffset = InventoryUtility.GetCenter(storable);
        if (InputService.Instance.JoystickUsed)
          return;
        storable.Transform.position = CursorService.Instance.Position + drag.MouseOffset;
      }
    }

    private void HoverShadow(Intersect intersect)
    {
      if (!drag.IsEnabled)
        return;
      if (!storables.ContainsKey(drag.Storable))
      {
        Debug.LogError("!storables.ContainsKey(drag.Storable), Такого быть недолжно, воспроизвести, разобраться в чем дело и пофиксить нормально");
      }
      else
      {
        if (!storables[drag.Storable].Transform.hasChanged)
          return;
        foreach (InventoryCellUI actionCell in drag.ActionCells)
        {
          if (actionCell != null)
            actionCell.State = CellState.Default;
        }
        drag.ActionCells.Clear();
        foreach (InventoryCellUI baseCell in drag.BaseCells)
          baseCell.State = CellState.Current;
        if (!intersect.IsIntersected)
          return;
        InventoryContainerUI container = GetContainer(intersect.Storage, intersect.Container);
        foreach (CellInfo cell1 in intersect.Cells)
        {
          InventoryCellUI inventoryCellUi = null;
          foreach (KeyValuePair<Cell, InventoryCellUI> cell2 in container.Cells)
          {
            if (cell2.Key.Column == cell1.Cell.Column && cell2.Key.Row == cell1.Cell.Row)
            {
              inventoryCellUi = cell2.Value;
              break;
            }
          }
          if (!(inventoryCellUi == null))
          {
            drag.ActionCells.Add(inventoryCellUi);
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
                      StorableComponent storableComponent = intersect.Storables.FirstOrDefault();
                      if (storableComponent == null)
                        throw new Exception();
                      if (drag.Storable.Owner.TemplateId == storableComponent.Owner.TemplateId && storableComponent.Count < storableComponent.Max)
                      {
                        inventoryCellUi.State = CellState.Stack;
                        break;
                      }
                      int num1 = container.InventoryContainer.GetKind() == ContainerCellKind.OneCellToOneStorable ? 1 : (intersect.Cells.Count == ((InventoryGridLimited) drag.Storable.Placeholder.Grid).Cells.Count ? 1 : 0);
                      inventoryCellUi.State = num1 == 0 ? CellState.NotFull : CellState.Swap;
                      break;
                    default:
                      int num2 = container.InventoryContainer.GetKind() == ContainerCellKind.OneCellToOneStorable ? 1 : (intersect.Cells.Count == ((InventoryGridLimited) drag.Storable.Placeholder.Grid).Cells.Count ? 1 : 0);
                      inventoryCellUi.State = num2 == 0 ? CellState.NotFull : CellState.Partial;
                      break;
                  }
                  break;
                case CellState.Stack:
                  inventoryCellUi.State = intersect.Storables.Count <= 1 ? cell1.State : CellState.Occupied;
                  break;
                default:
                  int num3 = container.InventoryContainer.GetKind() == ContainerCellKind.OneCellToOneStorable ? 1 : (intersect.Cells.Count == ((InventoryGridLimited) drag.Storable.Placeholder.Grid).Cells.Count ? 1 : 0);
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
      DragEnd(intersect, out bool isSuccess);
      if (!isSuccess)
        return;
      Subscribe();
      SubscribeNavigation();
      isConsoleDragging = false;
      SaveCurrentSelectedInstance(selectedStorable);
      if (_selectedStorable != null)
        currentInventory = selectedStorable.GetComponentInParent<ContainerResizableWindow>();
    }

    protected virtual void DragEnd(Intersect intersect, out bool isSuccess)
    {
      isSuccess = false;
      if (!drag.IsEnabled || !intersect.IsIntersected || intersect.Container != null && containerViews.ContainsKey(intersect.Container) && !containerViews[intersect.Container].ClickEnabled)
        return;
      if (!storables.ContainsKey(drag.Storable))
      {
        Debug.LogError("!storables.ContainsKey(drag.Storable), Такого быть недолжно, воспроизвести, разобраться в чем дело и пофиксить нормально");
      }
      else
      {
        StorableUI storable = storables[drag.Storable];
        storable.Dragging = false;
        foreach (CellInfo cell in intersect.Cells)
        {
          if (cell.State == CellState.Disabled)
            return;
        }
        StorableComponentUtility.PlayPutSound(drag.Storable);
        switch (intersect.Storables.Count)
        {
          case 0:
            if (!intersect.IsAllowed)
            {
              drag.IsEnabled = true;
              return;
            }
            if (!(drag.Storage == null ? intersect.Storage.AddItem(intersect.Storable, intersect.Container) : ((StorageComponent) drag.Storage).MoveItem(drag.Storable, intersect.Storage, intersect.Container, intersect.Cell.To())))
              throw new Exception();
            PlaceTo(intersect.Storage, intersect.Container, intersect.Storable, intersect.Cell.To());
            isSuccess = true;
            OnDragEnd();
            drag.Reset();
            break;
          case 1:
            StorableComponent storableComponent1 = intersect.Storables.FirstOrDefault();
            if (drag.Storable.Owner.TemplateId == storableComponent1.Owner.TemplateId && drag.Storable.Count + storableComponent1.Count <= storableComponent1.Max)
            {
              storableComponent1.Count += drag.Storable.Count;
              drag.Reset();
              if (intersect.Storable != null && !intersect.Storable.IsDisposed)
                intersect.Storable.Owner.Dispose();
              isSuccess = true;
              OnDragEnd();
              storable = storables[storableComponent1];
              break;
            }
            if (intersect.Container.GetKind() != ContainerCellKind.OneCellToOneStorable && ((InventoryGridLimited) drag.Storable.Placeholder.Grid).Cells.Count != intersect.Cells.Count)
              return;
            StorableComponent storableComponent2 = intersect.Storables.First();
            if (drag.Storable.Owner.TemplateId == storableComponent2.Owner.TemplateId && drag.Storable.Max > 1)
            {
              int a = storableComponent2.Max - storableComponent2.Count;
              if (a > 0)
              {
                int num = Mathf.Min(a, drag.Storable.Count);
                storableComponent2.Count += num;
                drag.Storable.Count -= num;
                break;
              }
            }
            storableComponent1.Container = null;
            storableComponent1.Cell = null;
            storableComponent1.Storage = null;
            if (!(drag.Storage == null ? intersect.Storage.AddItem(intersect.Storable, intersect.Container) : ((StorageComponent) drag.Storage).MoveItem(drag.Storable, intersect.Storage, intersect.Container, intersect.Cell.To())))
              return;
            PlaceTo(intersect.Storage, intersect.Container, intersect.Storable, intersect.Cell.To());
            InventoryCellStyle inventoryCellStyle;
            switch (intersect.Container.GetKind())
            {
              case ContainerCellKind.MultipleCellToOneStorable:
                inventoryCellStyle = styleMultipleCellOneStorable;
                break;
              case ContainerCellKind.OneCellToOneStorable:
                inventoryCellStyle = styleOneCellOneStorable;
                break;
              default:
                inventoryCellStyle = new InventoryCellStyle();
                break;
            }
            if (!storables.TryGetValue(storableComponent1, out storable))
            {
              Debug.LogError("Item not found, storable : " + storableComponent1.Owner.GetInfo() + " , count : " + storables.Count);
              return;
            }
            Vector3 lossyScale = storable.Transform.lossyScale;
            Vector2 vector2 = inventoryCellStyle.Size * 0.5f;
            vector2.Scale(lossyScale);
            storable.Transform.position = CursorService.Instance.Position - vector2;
            ClearShadow();
            drag.Reset();
            storableComponent1.Container = intersect.Container;
            storableComponent1.Cell = intersect.Cell.To();
            storableComponent1.Storage = intersect.Storage;
            if (DragBegin(storableComponent1))
            {
              drag.Storage = intersect.Storage;
            }
            else
            {
              isSuccess = true;
              drag.Reset();
              OnDragEnd();
            }
            HideContextMenu();
            HideInfoWindow();
            break;
        }
        if (!drag.IsEnabled)
        {
          windowSplit.Actor = null;
          ClearShadow();
          CursorService.Instance.Visible = true;
        }
        else
          CursorService.Instance.Visible = false;
        selectedStorable = InputService.Instance.JoystickUsed ? storable : null;
        UpdateContainerStates();
      }
    }

    protected void ClearShadow()
    {
      foreach (InventoryCellUI actionCell in drag.ActionCells)
        actionCell.State = CellState.Default;
      drag.ActionCells.Clear();
      foreach (InventoryCellUI baseCell in drag.BaseCells)
        baseCell.State = CellState.Default;
      drag.BaseCells.Clear();
    }

    protected virtual void DragCancel()
    {
      if (!drag.IsEnabled)
        return;
      OnDragEnd();
      if (drag.Storage == splitContainer)
      {
        if (storables.TryGetValue(drag.Storable, out StorableUI storableUi))
        {
          storableUi.Dragging = false;
          storables.Remove(drag.Storable);
          Destroy(storableUi.gameObject);
          Debug.LogError("-");
        }
        else
          Debug.LogError("View not found : " + drag.Storable.Owner.GetInfo());
        MoveItem(drag.Storable, Actor);
      }
      else
      {
        drag.Storage = drag.Storable.Storage;
        if (!StorageUtility.GetIntersect(drag.Storage, drag.Container, drag.Storable, drag.Cell).IsAllowed)
        {
          if (StorageUtility.GetIntersect(drag.Storage, null, drag.Storable, null).IsAllowed)
            MoveItem(drag.Storable, drag.Storage, drag.Container);
          else if (drag.Storable != null)
          {
            ServiceLocator.GetService<DropBagService>().DropBag(drag.Storable, Actor.Owner);
            RemoveItemFromView(drag.Storable);
            PlayAudio(dropItemAudio);
          }
        }
        else
          PlaceTo(drag.Storage, drag.Container, drag.Storable, drag.Cell);
      }
      StorableComponentUtility.PlayPutSound(drag.Storable);
      if (drag.Storable != null && storables.TryGetValue(drag.Storable, out StorableUI storableUi1))
      {
        storableUi1.Dragging = false;
        storableUi1.SetSelected(false);
      }
      drag.Reset();
      Subscribe();
      SubscribeNavigation();
      isConsoleDragging = false;
      SaveCurrentSelectedInstance(selectedStorable);
      CursorService.Instance.Visible = true;
      windowSplit.Actor = null;
      if (selectedStorable != null)
        OnSelectObject(selectedStorable.gameObject);
      ClearShadow();
      OnInvalidate();
    }

    protected void SaveCurrentSelectedInstance(StorableUI selectedStorable)
    {
      if (selectedStorable != null)
      {
        lastStorablePosition = selectedStorable.transform.position;
        try
        {
          lastSelectedTemplateId = selectedStorable.Internal.Owner.TemplateId;
        }
        catch
        {
          lastSelectedTemplateId = new Guid();
        }
      }
      else
      {
        float num = 0.0f;
        if (currentInventory != null)
          num = currentInventory.GetComponent<RectTransform>().sizeDelta.y;
        lastStorablePosition = new Vector3(0.0f, currentContainer != null ? currentContainer.transform.position.y + num / 2f : 0.0f);
        lastSelectedTemplateId = new Guid();
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
          inventoryCellStyle = styleMultipleCellOneStorable;
          break;
        case ContainerCellKind.OneCellToOneStorable:
          inventoryCellStyle = styleOneCellOneStorable;
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
      return new Pair<int, int> {
        Item1 = (int) Math.Round(position.x / (size.x + (double) offset.x)),
        Item2 = (int) Math.Round(position.y / (size.y + (double) offset.y))
      };
    }

    private Cell CellIntersect(Vector2 position, InventoryContainerUI container)
    {
      foreach (KeyValuePair<Cell, InventoryCellUI> cell in container.Cells)
      {
        Vector3 lossyScale = cell.Value.Transform.lossyScale;
        Rect scaledCoordinates = InventoryUtility.GetScaledCoordinates(cell.Value.Transform);
        Vector2 position1 = cell.Value.Transform.position;
        scaledCoordinates.position += position1;
        if (scaledCoordinates.Contains(position, true))
          return cell.Key;
      }
      return null;
    }

    private InventoryContainerUI ContainerIntersect(Vector2 position)
    {
      foreach (KeyValuePair<InventoryContainerUI, IStorageComponent> container in containers)
      {
        Rect scaledCoordinates1 = InventoryUtility.GetScaledCoordinates(container.Key.Transform);
        Vector2 position1 = container.Key.Transform.position;
        scaledCoordinates1.position += position1;
        bool flag = true;
        if (container.Key is ComplexInventoryContainerUI)
        {
          ComplexInventoryContainerUI key = container.Key as ComplexInventoryContainerUI;
          Rect scaledCoordinates2 = InventoryUtility.GetScaledCoordinates(key.Mask);
          Vector2 position2 = key.Mask.position;
          scaledCoordinates2.position += position2;
          flag = scaledCoordinates2.Contains(position, true);
        }
        if (scaledCoordinates1.Contains(position, true) & flag)
          return container.Key;
      }
      return null;
    }

    private InventoryContainerUI ContainerIntersect(Rect rectangle)
    {
      foreach (KeyValuePair<InventoryContainerUI, IStorageComponent> container in containers)
      {
        if (container.Key.InventoryContainer.GetKind() == ContainerCellKind.OneCellToOneStorable)
        {
          Rect scaledCoordinates = InventoryUtility.GetScaledCoordinates(container.Key.Transform);
          Vector2 position = container.Key.Transform.position;
          scaledCoordinates.position += position;
          if (scaledCoordinates.Contains(rectangle.center))
            return container.Key;
        }
      }
      foreach (KeyValuePair<InventoryContainerUI, IStorageComponent> container in containers)
      {
        Rect scaledCoordinates = InventoryUtility.GetScaledCoordinates(container.Key.Transform);
        Vector2 position = container.Key.Transform.position;
        scaledCoordinates.position += position;
        if (scaledCoordinates.Overlaps(rectangle, true))
          return container.Key;
      }
      return null;
    }

    private Intersect GetIntersect(IStorableComponent storable)
    {
      if (!storables.ContainsKey(storable))
        return new Intersect();
      StorableUI storable1 = storables[storable];
      Vector3 lossyScale1 = storable1.Transform.lossyScale;
      InventoryContainerUI key = ContainerIntersect(new Rect {
        position = storable1.PivotedPosition,
        size = Vector2.Scale(storable1.Transform.rotation * storable1.Transform.sizeDelta, lossyScale1)
      });
      if (key == null || !containers.ContainsKey(key))
        return new Intersect();
      if (!key.InventoryContainer.Available.Value || key.InventoryContainer.OpenState.Value != ContainerOpenStateEnum.Open)
        return new Intersect();
      InventoryCellStyle inventoryCellStyle;
      switch (key.InventoryContainer.GetKind())
      {
        case ContainerCellKind.MultipleCellToOneStorable:
          inventoryCellStyle = styleMultipleCellOneStorable;
          break;
        case ContainerCellKind.OneCellToOneStorable:
          inventoryCellStyle = styleOneCellOneStorable;
          break;
        default:
          inventoryCellStyle = new InventoryCellStyle();
          break;
      }
      Vector3 lossyScale2 = key.Transform.lossyScale;
      Vector2 vector2_1 = inventoryCellStyle.Size * 0.5f;
      Vector2 offset = inventoryCellStyle.Offset;
      vector2_1.Scale(lossyScale2);
      offset.Scale(lossyScale2);
      Rect scaledCoordinates = InventoryUtility.GetScaledCoordinates(key.Transform);
      Vector2 position = scaledCoordinates.position;
      Vector2 size = scaledCoordinates.size;
      Vector2 vector2_2 = storable1.Transform.rotation * vector2_1;
      Vector2 vector2_3 = (Vector2) storable1.Transform.position + vector2_2;
      Vector2 vector2_4 = (Vector2) key.Content.Transform.position + position;
      Pair<int, int> grid = PositionToGrid(key.InventoryContainer, vector2_3 - vector2_4, key.Transform.lossyScale);
      Cell cell = ProxyFactory.Create<Cell>();
      cell.Column = grid.Item1;
      cell.Row = grid.Item2;
      return StorageUtility.GetIntersect(containers[key], key.InventoryContainer, (StorableComponent) storable, cell);
    }

    private Intersect GetIntersect(Vector2 position)
    {
      InventoryContainerUI key = ContainerIntersect(position);
      if (key == null || !containers.ContainsKey(key))
        return new Intersect();
      if (key is ComplexInventoryContainerUI)
      {
        ComplexInventoryContainerUI container = key as ComplexInventoryContainerUI;
        Cell cell = CellIntersect(position, container);
        if (cell == null)
          return new Intersect();
        IStorableComponent cellItem = container.GetCellItem(cell);
        return cellItem != null ? StorageUtility.GetIntersect(cellItem.Storage, cellItem.Container, null, cell) : new Intersect();
      }
      if (key.InventoryContainer.OpenState.Value != ContainerOpenStateEnum.Open)
        ShowClosedContainerInfo(key.InventoryContainer);
      if (!key.InventoryContainer.Available.Value || key.InventoryContainer.OpenState.Value != ContainerOpenStateEnum.Open)
        return new Intersect();
      Rect scaledCoordinates = InventoryUtility.GetScaledCoordinates(key.Transform);
      Vector2 position1 = scaledCoordinates.position;
      Vector2 size = scaledCoordinates.size;
      Vector3 lossyScale = key.Transform.lossyScale;
      Pair<int, int> grid = PositionToGrid(key.InventoryContainer, position - ((Vector2) key.Content.Transform.position + position1), lossyScale);
      Cell cell1 = ProxyFactory.Create<Cell>();
      cell1.Column = grid.Item1;
      cell1.Row = grid.Item2;
      return StorageUtility.GetIntersect(containers[key], key.InventoryContainer, null, cell1);
    }

    private void UpdateInternal()
    {
      if (!InputService.Instance.JoystickUsed)
        HideClosedContainerInfo();
      Drag();
      intersect = !drag.IsEnabled ? GetIntersect(CursorService.Instance.Position) : GetIntersect(drag.Storable);
      if (drag.IsEnabled)
      {
        if (intersect.Container != null)
        {
          StorableUI storable = storables[drag.Storable];
          if (intersect.Container.GetKind() == ContainerCellKind.OneCellToOneStorable)
          {
            storable.Style = styleOneCellOneStorable;
            storable.Image.sprite = InventoryUtility.GetSpriteByStyle(drag.Storable.Placeholder, styleOneCellOneStorable.imageStyle);
          }
          else
          {
            storable.Style = styleMultipleCellOneStorable;
            storable.Image.sprite = InventoryUtility.GetSpriteByStyle(drag.Storable.Placeholder, styleMultipleCellOneStorable.imageStyle);
          }
        }
        DragCheck(ref intersect);
        HoverShadow(intersect);
      }
      if (!showInfo)
        return;
      IStorableComponent storable1 = !InputService.Instance.JoystickUsed || !(selectedStorable != null) ? intersect.Storables.FirstOrDefault() : selectedStorable.Internal;
      if (windowContextMenu == null && !drag.IsEnabled && intersect.IsIntersected && storable1 != null && (windowSplit == null || !windowSplit.IsEnabled))
        ShowInfoWindow(storable1);
      else if (!InputService.Instance.JoystickUsed && windowContextMenu == null)
        HideInfoWindow();
    }

    protected virtual void HideInfoWindow()
    {
      if (!(windowInfoNew != null))
        return;
      windowInfoNew.IsEnabled = false;
      windowInfoNew.Target = null;
      windowInfoNew.ClearActionTooltips();
    }

    protected virtual void ShowClosedContainerInfo(IInventoryComponent container)
    {
    }

    protected virtual void HideClosedContainerInfo()
    {
    }

    public void SetInfoWindowShowMode(bool isSimplified = false, bool showActionTooltips = true)
    {
      ShowSimplifiedInfoWindows = isSimplified;
      ShowActionTooltips = showActionTooltips;
    }

    protected virtual void ShowInfoWindow(IStorableComponent storable)
    {
      if (!CanShowInfoWindows || windowInfoNew == null || drag != null && drag.IsEnabled)
        return;
      bool flag = false;
      if (windowInfoNew.IsEnabled)
      {
        if (windowInfoNew.Target != storable)
          HideInfoWindow();
        else
          flag = true;
      }
      if (!flag)
      {
        windowInfoNew.Target = storable;
        AddActionsToInfoWindow(windowInfoNew, storable);
        windowInfoNew.IsEnabled = true;
        StorableUI storableByComponent = GetStorableByComponent(storable);
        if (storableByComponent != null && !storableByComponent.IsSliderItem)
          selectedStorable = storableByComponent;
      }
      windowInfoNew.ShowSimpliedWindow(ShowSimplifiedInfoWindows, ShowActionTooltips);
      PositionWindow(windowInfoNew, storable);
    }

    protected virtual float HintsBottomBorder => actorContainerWindow == null ? 40f * transform.lossyScale.y : GetComponent<RectTransform>().position.y - 450f * actorContainerWindow.transform.lossyScale.y;

    protected virtual void PositionWindow(UIControl window, IStorableComponent storable)
    {
      if (!storables.TryGetValue(storable, out StorableUI storableUi))
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
            if (storableUi.Image.transform.position.x + (double) num3 + component.rect.width * (double) x1 > Screen.width - (double) num1)
            {
              double x2 = storableUi.Image.rectTransform.localPosition.x;
              rect1 = storableUi.Image.rectTransform.rect;
              double width1 = rect1.width;
              double num5 = x2 - width1;
              rect1 = component.rect;
              double width2 = rect1.width;
              num3 = (float) (num5 - width2) * x1 - num2;
            }
            rect1 = storableUi.Image.rectTransform.rect;
            num4 = rect1.height / 2f * x1;
            double num6 = storableUi.Image.transform.position.y + (double) num4;
            rect1 = component.rect;
            double num7 = rect1.height * (double) x1;
            if (num6 - num7 < num1)
            {
              rect1 = storableUi.Image.rectTransform.rect;
              double num8 = rect1.height / 2.0;
              rect1 = component.rect;
              double height1 = rect1.height;
              double num9 = num8 + height1;
              rect1 = storableUi.Image.rectTransform.rect;
              double height2 = rect1.height;
              num4 = (float) (num9 - height2) * x1;
            }
            break;
          case ContainerCellKind.OneCellToOneStorable:
            num3 = storableUi.Image.rectTransform.rect.width / 2f * x1 + num2;
            if (storableUi.Image.transform.position.x + (double) num3 + component.rect.width * (double) x1 > Screen.width - (double) num1)
              num3 = ((float) (-(double) storableUi.Image.rectTransform.rect.width / 2.0) - component.rect.width) * x1 - num2;
            num4 = storableUi.Image.rectTransform.rect.height / 2f * x1;
            double num10 = storableUi.Image.transform.position.y + (double) num4;
            Rect rect2 = component.rect;
            double num11 = rect2.height * (double) x1;
            if (num10 - num11 < num1)
            {
              rect2 = component.rect;
              double height3 = rect2.height;
              rect2 = storableUi.Image.rectTransform.rect;
              double height4 = rect2.height;
              num4 = (float) ((height3 - height4) * x1 / 2.0);
            }
            break;
          default:
            throw new Exception();
        }
      }
      else
      {
        ContainerResizableWindow componentInParent = storableUi.GetComponentInParent<ContainerResizableWindow>();
        if (componentInParent == null)
          return;
        RectTransform containerRect = componentInParent.GetContainerRect();
        float hintsBottomBorder = HintsBottomBorder;
        Rect rect3 = storableUi.Image.rectTransform.rect;
        num4 = rect3.height / 2f * x1;
        double num12 = storableUi.Image.transform.position.y + (double) num4;
        rect3 = component.rect;
        double num13 = rect3.height * (double) x1;
        if (num12 - num13 < hintsBottomBorder)
        {
          rect3 = component.rect;
          double height = rect3.height;
          rect3 = storableUi.Image.rectTransform.rect;
          double num14 = rect3.height / 2.0;
          num4 = (float) (height - num14) * x1;
        }
        if (componentInParent == actorContainerWindow)
        {
          double x3 = containerRect.position.x;
          rect3 = component.rect;
          double num15 = rect3.width * (double) x1;
          num3 = (float) (x3 - num15 - storableUi.Image.transform.position.x - num1 / 2.0);
        }
        else
        {
          double x4 = containerRect.position.x;
          rect3 = containerRect.rect;
          double num16 = rect3.width * (double) x1;
          num3 = (float) (x4 + num16) + num1 - storableUi.Image.transform.position.x;
        }
      }
      window.Transform.position = new Vector3(storableUi.Image.transform.position.x + num3, storableUi.Image.transform.position.y + num4);
    }

    private void OnButtonUse(IStorableComponent storable)
    {
      if (!StorableComponentUtility.IsUsable(storable) || !Use(storable))
        return;
      HideContextMenu();
      HideInfoWindow();
    }

    private bool Use(IStorableComponent storable)
    {
      StorableComponentUtility.PlayUseSound(storable);
      IEntity template = (IEntity) storable.Owner.Template;
      StorableComponentUtility.Use(storable);
      ServiceLocator.GetService<LogicEventService>().FireEntityEvent(nameof (Use), template);
      OnInvalidate();
      return true;
    }

    private void OnButtonPourOut(IStorableComponent storable)
    {
      if (!StorableComponentUtility.IsBottled(storable) || !PourOut(storable))
        return;
      HideContextMenu();
      HideInfoWindow();
    }

    private bool PourOut(IStorableComponent storable)
    {
      StorableComponentUtility.PlayPourOutSound(storable);
      IEntity template = (IEntity) storable.Owner.Template;
      --storable.Count;
      if (storable.Count <= 0)
        storable.Owner.Dispose();
      ServiceLocator.GetService<LogicEventService>().FireEntityEvent(nameof (PourOut), template);
      OnInvalidate();
      return true;
    }

    private void OnButtonDress(IStorableComponent storable)
    {
      if (!StorableComponentUtility.IsWearable(storable) || !Dress(storable))
        return;
      HideContextMenu();
      HideInfoWindow();
    }

    protected bool Dress(IStorableComponent storable)
    {
      if (storable == null || storable.IsDisposed)
        return false;
      IInventoryComponent container1 = null;
      foreach (IInventoryComponent container2 in Actor.Containers.Where(o => o.GetGroup() == InventoryGroup.Clothes || o.GetGroup() == InventoryGroup.Weapons))
      {
        foreach (StorableGroup group in storable.Groups)
        {
          if (container2.GetLimitations().Contains(group))
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
      Intersect intersect1 = StorageUtility.GetIntersect(Actor, container1, (StorableComponent) storable, null);
      if (intersect1.IsAllowed)
      {
        if (((StorageComponent) storable.Storage).MoveItem(storable, intersect1.Storage, intersect1.Container, intersect1.Cell.To()))
          PlaceTo(intersect1.Storage, intersect1.Container, intersect1.Storable, intersect1.Cell.To());
      }
      else
      {
        IStorableComponent itemInContainer = GetItemInContainer(container1, Actor);
        if (itemInContainer != null)
        {
          Actor.RemoveItem(itemInContainer);
          Intersect intersect2 = StorageUtility.GetIntersect(Actor, container1, (StorableComponent) storable, null);
          if (intersect2.IsAllowed && ((StorageComponent) storable.Storage).MoveItem(storable, intersect2.Storage, intersect2.Container, intersect2.Cell.To()))
            PlaceTo(intersect2.Storage, intersect2.Container, intersect2.Storable, intersect2.Cell.To());
          if (StorageUtility.GetIntersect(Actor, null, (StorableComponent) itemInContainer, null).IsAllowed)
            Actor.AddItem(itemInContainer, null);
          else
            ServiceLocator.GetService<DropBagService>().DropBag(itemInContainer, Actor.Owner);
        }
      }
      return true;
    }

    private void OnButtonSplit(IStorableComponent storable)
    {
      if (!Split(storable))
        return;
      HideContextMenu();
      HideInfoWindow();
      Unsubscribe();
      UnsubscribeNavigation();
    }

    private bool Split(IStorableComponent storable)
    {
      if (!StorableComponentUtility.IsSplittable(storable))
        return false;
      HideInfoWindow();
      HideContextMenu();
      HideMainNavigationPanel();
      windowSplit.Actor = storable;
      windowSplit.IsEnabled = true;
      return true;
    }

    protected virtual void InteractItem(IStorableComponent storable)
    {
    }

    protected virtual void SplitDisableHandler(BaseGraphic base2)
    {
      FindCurrentCellInstance();
      SplitGraphic splitGraphic = (SplitGraphic) base2;
      if (splitGraphic.IsCanceled || splitGraphic.Target == null || splitGraphic.Target.IsDisposed)
      {
        Subscribe();
        SubscribeConsoleDragNavigation();
        splitEnabled = false;
        if (InputService.Instance.JoystickUsed)
          ShowContextMenu((StorableComponent) selectedStorable.Internal);
        ShowMainNavigationPanel();
      }
      else
      {
        IStorableComponent storableComponent = splitGraphic.Actor != splitGraphic.Target ? splitGraphic.Target : splitGraphic.Actor;
        if (splitContainer == null)
          CreateSplitContainer();
        splitContainer.AddItem(storableComponent, splitContainer.Containers.FirstOrDefault());
        RegisterItemInView(storableComponent, styleOneCellOneStorable.imageStyle);
        if (InputService.Instance.JoystickUsed)
        {
          storables[storableComponent].Transform.position = selectedStorable.Transform.position;
          selectedStorable = storables[storableComponent];
          ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Context, DragListener);
          DragListener(GameActionType.Context, true);
        }
        else
        {
          storables[storableComponent].Transform.position = CursorService.Instance.Position;
          DragBegin(storableComponent);
        }
        if (drag.Storable.Container != null)
          return;
        drag.Storage = splitContainer;
        drag.Storable.Storage = splitContainer;
        drag.Storable.Container = splitContainer.Containers.FirstOrDefault();
      }
    }

    protected void CreateSplitContainer()
    {
      IEntity template = splitContainerTemplate.Value;
      IEntity entity = ServiceLocator.GetService<IFactory>().Instantiate(template);
      splitContainer = entity.GetComponent<StorageComponent>();
      ServiceLocator.GetService<ISimulation>().Add(entity, ServiceLocator.GetService<ISimulation>().Others);
      actors.Add(splitContainer);
    }

    private bool SplitListener(GameActionType type, bool down)
    {
      if (drag.IsEnabled)
        return false;
      if (down)
      {
        if (!IsEnabled)
          return false;
        splitEnabled = true;
        IStorableComponent storable = null;
        if (InputService.Instance.JoystickUsed)
        {
          if (!(selectedStorable != null))
            return false;
          storable = selectedStorable.Internal;
        }
        else
        {
          intersect = GetIntersect(CursorService.Instance.Position);
          if (intersect.Storables != null)
            storable = intersect.Storables.FirstOrDefault();
          if (storable == null)
            return false;
        }
        if (Split(storable))
        {
          Unsubscribe();
          UnsubscribeNavigation();
        }
        return true;
      }
      Subscribe();
      SubscribeNavigation();
      splitEnabled = false;
      return false;
    }

    protected void MoveAllItems(IStorageComponent fromStorage, IStorageComponent toStorage)
    {
      foreach (IStorableComponent storableComponent in fromStorage.Items.ToList())
        MoveItem(storableComponent, toStorage, invalidate: false);
      OnInvalidate();
    }

    protected void MoveItem(
      IStorableComponent item,
      IStorageComponent toStorage,
      IInventoryComponent toContainer = null,
      bool invalidate = true)
    {
      IInventoryComponent container = null;
      if (toContainer != null && toContainer.GetStorage() == toStorage)
        container = toContainer;
      Intersect intersect = StorageUtility.GetIntersect(toStorage, container, (StorableComponent) item, null);
      if (item.IsDisposed)
      {
        if (!invalidate)
          return;
        OnInvalidate();
      }
      else
      {
        if (!intersect.IsAllowed)
        {
          ServiceLocator.GetService<DropBagService>().DropBag(item, Actor.Owner);
          RemoveItemFromView(item);
          PlayAudio(dropItemAudio);
        }
        else
        {
          if (!((StorageComponent) item.Storage).MoveItem(item, intersect.Storage, intersect.Container, intersect.Cell.To()))
            throw new Exception();
          StorableComponent storableComponent = intersect.Storables.FirstOrDefault();
          if (storableComponent == null || storableComponent.IsDisposed)
            PlaceTo(intersect.Storage, intersect.Container, intersect.Storable, intersect.Cell.To());
        }
        if (!invalidate)
          return;
        OnInvalidate();
      }
    }

    [Inspected]
    protected virtual void OnInvalidate() => UpdateContainerStates();

    private void OnContainerOpenBegin(IInventoryComponent container)
    {
      StartOpenAudio(container.GetOpenStartAudio());
      if (container.OpenState.Value == ContainerOpenStateEnum.Locked && overrideUnlockProgressAudio != null)
        StartOpenAudio(overrideUnlockProgressAudio);
      else
        StartOpenAudio(container.GetOpenProgressAudio());
    }

    protected virtual void OnContainerOpenEnd(IInventoryComponent container, bool complete)
    {
      StopOpenAudio();
      if (complete)
      {
        if (container.OpenState.Value == ContainerOpenStateEnum.Closed && overrideUnlockCompleteAudio != null)
          StartOpenAudio(overrideUnlockCompleteAudio);
        else
          StartOpenAudio(container.GetOpenCompleteAudio());
        if (container.GetInstrument() != 0)
          DamageInstrument(container);
      }
      else
        StartOpenAudio(container.GetOpenCancelAudio());
      UpdateContainerStates();
    }

    protected void PlayAudio(AudioClip audio)
    {
      if (!(audio != null))
        return;
      SoundUtility.PlayAudioClip2D(audio, mixer, 1f, 0.0f, context: gameObject.GetFullName());
    }

    protected void StartOpenAudio(AudioClip audio)
    {
      if (!(audio != null))
        return;
      currentOpenedAudioState = SoundUtility.PlayAudioClip2D(audio, mixer, 1f, 0.0f, context: gameObject.GetFullName());
    }

    protected void StopOpenAudio()
    {
      if (currentOpenedAudioState == null)
        return;
      currentOpenedAudioState.Complete = true;
      if (currentOpenedAudioState.AudioSource != null)
        currentOpenedAudioState.AudioSource.Stop();
      currentOpenedAudioState = null;
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
          style = styleMultipleCellOneStorable;
          break;
        case ContainerCellKind.OneCellToOneStorable:
          style = styleOneCellOneStorable;
          break;
        default:
          style = new InventoryCellStyle();
          break;
      }
      return CreateContainerView(storage, container, anchor, style);
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
        key = LimitedInventoryContainerUI.Instantiate(container, style, inventoryContainerLimitedPrefab);
      }
      else
      {
        if (!(container.GetGrid() is IInventoryGridInfinited))
          throw new Exception();
        key = InfinitedInventoryContainerUI.Instantiate(container, style, inventoryContainerInfinitedPrefab);
      }
      key.ImageBackground.sprite = container.GetImageBackground();
      key.ImageBackground.gameObject.SetActive(GetItemInContainer(container, container.GetStorage()) == null && key.ImageBackground.sprite != null);
      key.transform.SetParent(anchor.transform, false);
      containers.Add(key, storage);
      containerViews.Add(container, key);
      key.OpenBegin += OpenBegin;
      key.OpenEnd += OpenEnd;
      foreach (IStorableComponent storable in storage.Items)
      {
        if (storable.Container == container)
          AddItemToView(storable);
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
      return null;
    }

    private void ResizeActorContainersWindow()
    {
      List<InventoryContainerUI> containers = [];
      foreach (KeyValuePair<InventoryContainerUI, IStorageComponent> container in this.containers)
      {
        if (container.Value == Actor && container.Key.InventoryContainer.Enabled.Value && container.Key.InventoryContainer.GetGroup() == InventoryGroup.Backpack)
          containers.Add(container.Key);
      }
      ResizeContainersWindow(actorContainerWindow, containers);
    }

    protected void ResizeContainersWindow(
      ContainerResizableWindow window,
      List<InventoryContainerUI> containers)
    {
      if (!(window != null))
        return;
      window.Resize(containers);
    }

    protected bool IsDebugMode() => InstanceByRequest<EngineApplication>.Instance.IsDebug;

    public virtual IEntity GetUseTarget() => Actor?.Owner;

    private void CreateItems()
    {
      for (int index1 = 0; index1 < actors.Count; ++index1)
      {
        IStorageComponent actor = actors[index1];
        if (actor != null && !actor.IsDisposed)
        {
          actor.OnAddItemEvent += Actor_OnChangeItemEvent;
          actor.OnRemoveItemEvent += Actor_OnChangeItemEvent;
          actor.OnChangeItemEvent += Actor_OnChangeItemEvent;
          foreach (IInventoryComponent container in actor.Containers)
          {
            if (ValidateContainer(container, actor))
            {
              InventoryCellStyle style;
              switch (container.GetKind())
              {
                case ContainerCellKind.MultipleCellToOneStorable:
                  style = styleMultipleCellOneStorable;
                  break;
                case ContainerCellKind.OneCellToOneStorable:
                  style = styleOneCellOneStorable;
                  break;
                default:
                  style = new InventoryCellStyle();
                  break;
              }
              bool flag = false;
              SlotAnchor slotAnchor = new SlotAnchor();
              if (container.GetSlotKind() != SlotKind.None && ValidateComputeActor(actor))
              {
                for (int index2 = 0; index2 < this.slotAnchor.Count; ++index2)
                {
                  if (container.GetLimitations().Contains(this.slotAnchor[index2].Group) && container.Enabled.Value)
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
                key = LimitedInventoryContainerUI.Instantiate(container, style, inventoryContainerLimitedPrefab);
              }
              else
              {
                if (!(container.GetGrid() is IInventoryGridInfinited))
                  throw new Exception("Grid Type not found : " + (container.GetGrid() != null ? container.GetGrid().GetType().ToString() : "null"));
                key = InfinitedInventoryContainerUI.Instantiate(container, style, inventoryContainerInfinitedPrefab);
              }
              if (key.ImageBackground != null)
              {
                key.ImageBackground.sprite = container.GetImageBackground();
                key.ImageBackground.gameObject.SetActive(key.ImageBackground.sprite != null);
              }
              if (flag)
                key.transform.SetParent(slotAnchor.UIBehaviour.transform, false);
              else if (index1 < actorAnchors.Count)
                key.transform.SetParent(actorAnchors[index1].transform, false);
              containers.Add(key, actor);
              key.OpenBegin += OpenBegin;
              key.OpenEnd += OpenEnd;
            }
          }
          foreach (IStorableComponent storable in actor.Items)
            AddItemToView(storable);
        }
      }
    }

    private void ClearItems()
    {
      for (int index = 0; index < actors.Count; ++index)
      {
        IStorageComponent actor = actors[index];
        if (actor != null && !actor.IsDisposed)
        {
          actor.OnAddItemEvent -= Actor_OnChangeItemEvent;
          actor.OnRemoveItemEvent -= Actor_OnChangeItemEvent;
          actor.OnChangeItemEvent -= Actor_OnChangeItemEvent;
        }
      }
      foreach (KeyValuePair<InventoryContainerUI, IStorageComponent> container in containers)
      {
        container.Key.OpenBegin -= OpenBegin;
        container.Key.OpenEnd -= OpenEnd;
        Destroy(container.Key.gameObject);
      }
      containers.Clear();
      containerViews.Clear();
      foreach (KeyValuePair<IStorableComponent, StorableUI> keyValuePair in storables.ToList())
        RemoveItemFromView(keyValuePair.Key);
      storables.Clear();
    }

    protected virtual void Actor_OnChangeItemEvent(
      IStorableComponent storable,
      IInventoryComponent inventory)
    {
      CoroutineService.Instance.WaitFrame((Action) (() =>
      {
        ClearItems();
        CreateItems();
        OnInvalidate();
        Drag();
        AdditionalAfterChangeAction();
      }));
    }

    protected virtual void AdditionalAfterChangeAction()
    {
      if (!InputService.Instance.JoystickUsed || drag.IsEnabled)
        return;
      SelectFirstStorableInContainer(null);
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
      StorableDisposeEvent(sender);
    }

    public virtual bool HaveToFindSelected() => true;

    protected StorableUI selectedStorable
    {
      get
      {
        if (_selectedStorable == null && InputService.Instance.JoystickUsed && HaveToFindSelected())
          FindCurrentCellInstance();
        return _selectedStorable;
      }
      set
      {
        if (_selectedStorable != null)
          _selectedStorable.SetSelected(false);
        _selectedStorable = value;
        if (selectionFrame == null)
          return;
        selectionFrame.gameObject.SetActive(false);
        if (!(_selectedStorable != null))
          return;
        _selectedStorable.SetSelected(true);
        SaveCurrentSelectedInstance(selectedStorable);
      }
    }

    protected void SetSelectedFrame()
    {
      if (selectionFrame == null)
        return;
      selectionFrame.gameObject.SetActive(InputService.Instance.JoystickUsed && _selectedStorable != null && !drag.IsEnabled);
      if (!(_selectedStorable != null))
        return;
      InventoryCellStyle style = _selectedStorable.Style;
      Vector2 innerSize = InventoryUtility.CalculateInnerSize(((StorableComponent) _selectedStorable.Internal).Placeholder.Grid, style);
      selectionFrame.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, innerSize.x + style.BackgroundImageOffset.x * 2f);
      selectionFrame.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, innerSize.y + style.BackgroundImageOffset.x * 2f);
      selectionFrame.rectTransform.position = _selectedStorable.Image.rectTransform.position;
    }

    protected virtual void Subscribe()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.AddListener(GameActionType.Cancel,CancelListener);
      service.AddListener(GameActionType.Split, SplitListener);
      service.AddListener(GameActionType.Submit, ContextListener);
      service.AddListener(GameActionType.Context, DragListener);
    }

    protected void SubscribeNavigation()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.AddListener(GameActionType.LStickUp, NavigationListener);
      service.AddListener(GameActionType.LStickDown, NavigationListener);
      service.AddListener(GameActionType.LStickRight, NavigationListener);
      service.AddListener(GameActionType.LStickLeft, NavigationListener);
      service.AddListener(GameActionType.DPadUp, NavigationListener);
      service.AddListener(GameActionType.DPadDown, NavigationListener);
      service.AddListener(GameActionType.DPadRight, NavigationListener);
      service.AddListener(GameActionType.DPadLeft, NavigationListener);
      UnsubscribeConsoleDragNavigation();
    }

    protected void UnsubscribeConsoleDragNavigation()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.LStickUp, ConsoleDragNavigationListener);
      service.RemoveListener(GameActionType.LStickDown, ConsoleDragNavigationListener);
      service.RemoveListener(GameActionType.LStickRight, ConsoleDragNavigationListener);
      service.RemoveListener(GameActionType.LStickLeft, ConsoleDragNavigationListener);
      service.RemoveListener(GameActionType.Submit, DragListener);
      if (scrollCoroutine == null)
        return;
      StopCoroutine(scrollCoroutine);
    }

    protected void SubscribeConsoleDragNavigation()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.AddListener(GameActionType.LStickUp, ConsoleDragNavigationListener, true);
      service.AddListener(GameActionType.LStickDown, ConsoleDragNavigationListener, true);
      service.AddListener(GameActionType.LStickRight, ConsoleDragNavigationListener, true);
      service.AddListener(GameActionType.LStickLeft, ConsoleDragNavigationListener, true);
      service.AddListener(GameActionType.Submit, DragListener, true);
    }

    private bool ConsoleDragNavigationListener(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      Navigation navigation = Navigation.None;
      switch (type)
      {
        case GameActionType.LStickUp:
          navigation = Navigation.CellUp;
          break;
        case GameActionType.LStickDown:
          navigation = Navigation.CellDown;
          break;
        case GameActionType.LStickLeft:
          navigation = Navigation.CellLeft;
          break;
        case GameActionType.LStickRight:
          navigation = Navigation.CellRight;
          break;
      }
      if (down)
      {
        if (scrollCoroutine != null)
          StopCoroutine(scrollCoroutine);
        scrollCoroutine = StartCoroutine(ScrollCoroutine(OnMoveItem, navigation));
        coroutineNavigation = navigation;
        return true;
      }
      if (scrollCoroutine != null && navigation == coroutineNavigation)
      {
        StopCoroutine(scrollCoroutine);
        scrollCoroutine = null;
      }
      return false;
    }

    protected void UnsubscribeNavigation()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.LStickUp, NavigationListener);
      service.RemoveListener(GameActionType.LStickDown, NavigationListener);
      service.RemoveListener(GameActionType.LStickRight, NavigationListener);
      service.RemoveListener(GameActionType.LStickLeft, NavigationListener);
      service.RemoveListener(GameActionType.DPadUp, NavigationListener);
      service.RemoveListener(GameActionType.DPadDown, NavigationListener);
      service.RemoveListener(GameActionType.DPadRight, NavigationListener);
      service.RemoveListener(GameActionType.DPadLeft, NavigationListener);
      if (scrollCoroutine == null)
        return;
      StopCoroutine(scrollCoroutine);
    }

    protected virtual void Unsubscribe()
    {
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      service.RemoveListener(GameActionType.Cancel, CancelListener);
      service.RemoveListener(GameActionType.Split, SplitListener);
      service.RemoveListener(GameActionType.Submit, ContextListener);
      service.RemoveListener(GameActionType.Context, DragListener);
    }

    protected bool DragListener(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed || !down)
        return false;
      currentAcceleration = 0.0f;
      if (type != GameActionType.Context && type != GameActionType.Submit)
        return false;
      if (!drag.IsEnabled && type != GameActionType.Submit)
      {
        if (selectedContainer != null)
          return true;
        if (selectedStorable != null)
          SaveCurrentSelectedInstance(selectedStorable);
        else
          FindCurrentCellInstance();
        if (selectedStorable == null)
          return false;
        selectedStorable.SetSelected(false);
        if (DragBegin(selectedStorable.Internal))
        {
          UnsubscribeNavigation();
          SubscribeConsoleDragNavigation();
          Unsubscribe();
          ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Context, DragListener);
          isConsoleDragging = true;
        }
        HideContextMenu();
        HideInfoWindow();
        return true;
      }

      DragEnd(intersect, out bool isSuccess);
      if (isSuccess)
      {
        Subscribe();
        SubscribeNavigation();
        isConsoleDragging = false;
        SaveCurrentSelectedInstance(selectedStorable);
        if (_selectedStorable != null)
          currentInventory = selectedStorable.GetComponentInParent<ContainerResizableWindow>();
      }
      return true;
    }

    private void ConsoleDraggingUpdate()
    {
    }

    private IEnumerator ScrollCoroutine(
      NavigationScrollHandle handle,
      Navigation navigation)
    {
      while (true)
      {
        if (handle != null)
          handle(navigation);
        yield return new WaitForSeconds(0.2f);
      }
    }

    private bool NavigationListener(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      Navigation navigation = Navigation.None;
      switch (type)
      {
        case GameActionType.LStickUp:
          navigation = Navigation.CellUp;
          break;
        case GameActionType.LStickDown:
          navigation = Navigation.CellDown;
          break;
        case GameActionType.LStickLeft:
          navigation = Navigation.CellLeft;
          break;
        case GameActionType.LStickRight:
          navigation = Navigation.CellRight;
          break;
        case GameActionType.DPadUp:
          navigation = Navigation.ContainerUp;
          break;
        case GameActionType.DPadDown:
          navigation = Navigation.ContainerDown;
          break;
        case GameActionType.DPadLeft:
          navigation = Navigation.ContainerLeft;
          break;
        case GameActionType.DPadRight:
          navigation = Navigation.ContainerRight;
          break;
      }
      if (down)
      {
        if (scrollCoroutine != null)
          StopCoroutine(scrollCoroutine);
        scrollCoroutine = StartCoroutine(ScrollCoroutine(OnNavigate, navigation));
        coroutineNavigation = navigation;
        return true;
      }
      if (scrollCoroutine != null && navigation == coroutineNavigation)
      {
        StopCoroutine(scrollCoroutine);
        scrollCoroutine = null;
      }
      return false;
    }

    protected virtual void FillSelectableList(List<GameObject> list, bool block, bool addSelected = false)
    {
      list.Clear();
      foreach (KeyValuePair<IStorableComponent, StorableUI> storable in storables)
      {
        IInventoryComponent container = storable.Key.Container;
        if (addSelected || !(storable.Value == selectedStorable))
        {
          Vector3 position = storable.Value.transform.position;
          if (position.x >= 0.0 && position.y >= 0.0 && position.x <= (double) Screen.width && position.y <= (double) Screen.height && (!block || container == null || !(selectedStorable != null) || container != selectedStorable.Internal.Container) && AdditionalConditionOfSelectableList(storable.Value))
            list.Add(storable.Value.gameObject);
        }
      }
    }

    protected virtual bool AdditionalConditionOfSelectableList(StorableUI storable) => true;

    protected virtual Vector2 CurentNavigationPosition()
    {
      RectTransform transform = _selectedStorable != null ? _selectedStorable.transform as RectTransform : null;
      return transform != null ? transform.TransformPoint(transform.rect.center) : this.transform.position;
    }

    protected virtual void OnSelectObject(GameObject selected)
    {
      StorableUI component = selected?.GetComponent<StorableUI>();
      if (!(component != null))
        return;
      selectedStorable = component;
      if (InputService.Instance.JoystickUsed)
        ShowInfoWindow(selectedStorable.Internal);
      selectedStorable.SetSelected(true);
    }

    protected virtual void OnMoveItem(Navigation navigation)
    {
      Vector2 direction = Vector2.zero;
      if (!drag.IsEnabled)
        return;
      switch (navigation)
      {
        case Navigation.CellUp:
          direction = Vector2.up;
          break;
        case Navigation.CellDown:
          direction = Vector2.down;
          break;
        case Navigation.CellRight:
          direction = Vector2.right;
          break;
        case Navigation.CellLeft:
          direction = Vector2.left;
          break;
      }
      if (!(direction != Vector2.zero))
        return;
      if (selectedStorable == null)
        FindCurrentCellInstance();
      Vector3 position = selectedStorable.transform.position;
      StorableComponent storableComponent = selectedStorable.Internal as StorableComponent;
      int rows = storableComponent.Placeholder.Grid.Rows;
      int columns = storableComponent.Placeholder.Grid.Columns;
      List<GameObject> objects = [];
      foreach (InventoryContainerUI key1 in containers.Keys)
      {
        InventoryContainerUI inventory = key1;
        if (inventory.InventoryContainer.Enabled.Value || inventory.InventoryContainer.GetStorage() != Actor)
        {
          IEnumerable<KeyValuePair<Cell, InventoryCellUI>> source = inventory.Cells.Where(c =>
          {
            Cell key2 = c.Key;
            if (selectedStorable.transform.position == c.Value.transform.position || c.Value.transform.position.x < 0.0 || c.Value.transform.position.y < 0.0 || c.Value.transform.position.x > (double) Screen.width || c.Value.transform.position.y > (double) Screen.height)
              return false;
            if (inventory.InventoryContainer.GetGroup() == InventoryGroup.Clothes || inventory.InventoryContainer.GetGroup() == InventoryGroup.Weapons)
              return Vector2.Dot((c.Value.transform.position - selectedStorable.transform.position).normalized, direction) >= 0.85000002384185791;
            return key2.Column + columns <= inventory.InventoryContainer.GetGrid().Columns && key2.Row + rows <= inventory.InventoryContainer.GetGrid().Rows && (inventory.InventoryContainer.GetGroup() == InventoryGroup.Backpack || inventory.InventoryContainer.GetGroup() == InventoryGroup.Loot) && Vector2.Dot((c.Value.transform.position - selectedStorable.transform.position).normalized, direction) >= 0.85000002384185791;
          });
          if (source != null)
            objects.AddRange(source.Select(c => c.Value.gameObject).ToList());
        }
      }
      GameObject gameObject = UISelectableHelper.Select(objects, selectedStorable.transform.position, direction);
      if (gameObject != null)
        position = gameObject.transform.position;
      selectedStorable.transform.position = position;
    }

    protected virtual InventoryContainerUI selectedContainer
    {
      get => null;
      set
      {
      }
    }

    protected virtual void OnNavigate(Navigation navigation)
    {
      Vector2 dirrection = Vector2.zero;
      if (selectedStorable == null && selectedContainer == null)
      {
        GetFirstStorable();
      }
      else
      {
        FillSelectableList(selectableList, navigation >= Navigation.ContainerUp);
        Vector2 origin = CurentNavigationPosition();
        switch (navigation)
        {
          case Navigation.CellClosest:
            OnSelectObject(UISelectableHelper.SelectClosest(selectableList, origin));
            break;
          case Navigation.CellUp:
          case Navigation.ContainerUp:
            dirrection = Vector2.up;
            break;
          case Navigation.CellDown:
          case Navigation.ContainerDown:
            dirrection = Vector2.down;
            break;
          case Navigation.CellRight:
          case Navigation.ContainerRight:
            dirrection = Vector2.right;
            break;
          case Navigation.CellLeft:
          case Navigation.ContainerLeft:
            dirrection = Vector2.left;
            break;
        }
        OnSelectObject(UISelectableHelper.Select(selectableList, origin, dirrection));
      }
    }

    protected void FindCurrentCellInstance()
    {
      List<StorableUI> list1 = storables.Values.Where(storable => (storable.Internal.Owner.TemplateId == lastSelectedTemplateId || lastSelectedTemplateId == new Guid()) && Vector3.Distance(storable.transform.position, lastStorablePosition) <= 500.0 && ItemIsInteresting(storable.Internal)).ToList();
      StorableUI storableUi = null;
      if (list1 == null)
        return;
      List<StorableUI> list2 = list1.Where(storable => storable.IsEnabled).OrderBy(storable => Vector3.Distance(lastStorablePosition, storable.gameObject.transform.position)).ToList();
      if (list2.Count > 0)
        storableUi = list2.First();
      if (storableUi == null)
        return;
      if (_selectedStorable != null)
      {
        if (shouldDeselectStorable || _selectedStorable is StorableUITrade && ((StorableUITrade) _selectedStorable).GetSelectedCount() == 0 || InputService.Instance.JoystickUsed)
        {
          _selectedStorable.SetSelected(false);
          shouldDeselectStorable = false;
        }
        if (!(storableUi is StorableUITrade) && !storableUi.IsSelected())
          shouldDeselectStorable = true;
      }
      selectedStorable = storableUi;
      ShowInfoWindow(_selectedStorable.Internal);
      _selectedStorable.SetSelected(true);
    }

    private void OnNavigateContainers(Navigation navigation)
    {
      if (currentInventory == null)
        currentInventory = _selectedStorable?.GetComponentInParent<ContainerResizableWindow>();
      currentContainer = _selectedStorable?.GetComponentInParent<LimitedInventoryContainerUI>();
      if (currentInventory == null)
        currentInventory = new List<ContainerResizableWindow>(GetComponentsInChildren<ContainerResizableWindow>())[0];
      List<LimitedInventoryContainerUI> source =
        [..currentInventory.GetComponentsInChildren<LimitedInventoryContainerUI>()];
      for (int index = 0; index < source.Count; ++index)
      {
        if (source[index].GetComponentsInChildren<StorableUI>().Length < 1)
          source.Remove(source[index]);
      }
      if (currentContainer == null)
        currentContainer = source[0];
      source.Remove(currentContainer);
      if (source.Count <= 1)
        return;
      foreach (Component component in source)
        Debug.Log(Vector3.Distance(component.transform.position, currentContainer.transform.position));
      List<LimitedInventoryContainerUI> list = source.Where(container => Vector3.Distance(container.transform.position, currentContainer.transform.position) <= 800.0).ToList();
      switch (navigation)
      {
        case Navigation.ContainerUp:
          list = list.Where(container => container.transform.position.y > (double) currentContainer.transform.position.y).ToList();
          break;
        case Navigation.ContainerDown:
          list = list.Where(container => container.transform.position.y < (double) currentContainer.transform.position.y).ToList();
          break;
        case Navigation.ContainerRight:
          list = list.Where(container => container.transform.position.x > (double) currentContainer.transform.position.x).ToList();
          break;
        case Navigation.ContainerLeft:
          list = list.Where(container => container.transform.position.x < (double) currentContainer.transform.position.x).ToList();
          break;
      }
      if (list.Count <= 0)
        return;
      currentContainer = list.OrderBy(container => Vector3.Distance(currentContainer.transform.position, container.gameObject.transform.position)).First();
      SelectFirstStorableInContainer([..currentContainer.GetComponentsInChildren<StorableUI>()]);
      SetSelectedFrame();
    }

    protected virtual void SelectFirstStorableInContainer(List<StorableUI> storables)
    {
      if (!CanShowInfoWindows)
        return;
      if (lastStorablePosition == Vector3.zero)
        lastStorablePosition = new Vector3(0.0f, actorContainerWindow.transform.position.y + actorContainerWindow.GetComponent<RectTransform>().sizeDelta.y);
      FillSelectableList(selectableList, false, true);
      OnSelectObject(UISelectableHelper.SelectClosest(selectableList, lastStorablePosition));
    }

    protected bool ContextListener(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      if (down && windowContextMenu != null)
      {
        HideContextMenu();
        return down;
      }
      if (!down || !(_selectedStorable != null))
        return down;
      ShowContextMenu((StorableComponent) _selectedStorable.Internal);
      return down;
    }

    protected bool SetStorableByComponent(IStorableComponent component)
    {
      StorableUI storableByComponent = GetStorableByComponent(component);
      if (!(storableByComponent != null))
        return false;
      selectedStorable = storableByComponent;
      selectedStorable.SetSelected(true);
      ShowInfoWindow(component);
      return true;
    }

    protected StorableUI GetStorableByComponent(IStorableComponent component)
    {
      return component == null || !storables.TryGetValue(component, out StorableUI storableUi) ? null : storableUi;
    }

    protected enum Navigation
    {
      None = -2,
      CellClosest = -1,
      CellUp = 0,
      CellDown = 1,
      CellRight = 2,
      CellLeft = 3,
      ContainerUp = 4,
      ContainerDown = 5,
      ContainerRight = 6,
      ContainerLeft = 7,
    }

    private delegate void NavigationScrollHandle(Navigation navigation);
  }
}
