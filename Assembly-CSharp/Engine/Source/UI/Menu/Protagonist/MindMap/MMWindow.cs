// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Protagonist.MindMap.MMWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Comparers;
using Engine.Common.MindMap;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings.External;
using Engine.Source.Utility;
using InputServices;
using Inspectors;
using Pingle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
namespace Engine.Source.UI.Menu.Protagonist.MindMap
{
  public class MMWindow : UIWindow, IMMWindow, IWindow, IPauseMenu
  {
    [SerializeField]
    private RectTransform contentRect;
    [SerializeField]
    private MMSideBar sideBar;
    [SerializeField]
    private MMInfoView infoView;
    [Space]
    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color successColor;
    [SerializeField]
    private Color failColor;
    [SerializeField]
    private Color knowledgeColor;
    [SerializeField]
    private Color isolatedFactColor;
    [SerializeField]
    private Color inactiveTint;
    [Space]
    [SerializeField]
    private Sprite linkImage;
    [SerializeField]
    private float linkThickness = 16f;
    [SerializeField]
    private Texture predictionIcon;
    [SerializeField]
    private Sprite activeGlowImage;
    [SerializeField]
    private Color activeGlowColor = Color.white;
    [SerializeField]
    private Material material;
    [Space]
    [SerializeField]
    private float missionRadius;
    [SerializeField]
    private float conclusionRadius;
    [SerializeField]
    private float normalRadius;
    [Space]
    [SerializeField]
    private float radiusToSpriteSize;
    [SerializeField]
    private GameObject newNodeIndicatorPrefab;
    [Inspected]
    private Dictionary<Guid, MMContent> contents = new Dictionary<Guid, MMContent>((IEqualityComparer<Guid>) GuidComparer.Instance);
    [Inspected]
    private List<MMPage> pages = new List<MMPage>();
    private MMPage globalPage;
    private CameraKindEnum lastCameraKind;
    private float baseScalePower = 0.0f;
    private float desiredScaleValue = 0.0f;
    private float scaleValue = 0.0f;
    private MMPageView openedPageView;
    private HashSet<IMMNode> mapNodes = new HashSet<IMMNode>();
    private int currentNodeIndex = 0;
    [SerializeField]
    private GameObject helpPanel;
    [SerializeField]
    private GameObject consoleCursor;
    [SerializeField]
    private ScrollRect scroll;
    private float xPrev;
    private float yPrev = 0.0f;
    private IMMNode currentNode = (IMMNode) null;
    private List<MMNodeView> nodes = new List<MMNodeView>();
    private const float CLOSEST_NODE_DETECTION_THRESHOLD = 2f;
    private MMWindow.NodeMoveDirection bufferedDirection = MMWindow.NodeMoveDirection.None;
    private MMNodeView previousNode = (MMNodeView) null;
    private MMNodeView hovered = (MMNodeView) null;
    private MMNodeView newHovered = (MMNodeView) null;
    private GraphicRaycaster rayCaster = (GraphicRaycaster) null;
    private PointerEventData pointerData = (PointerEventData) null;
    private const float MAX_ACCELERATION_RATE = 3f;
    private float currentAcceleration = 0.0f;
    private float accelerationSpeed = 3f;
    private Vector2 cursorMapPosition = Vector2.zero;
    private Vector2 maxNavigationValues = Vector2.zero;
    private Vector2 minNavigationValues = Vector2.zero;
    private bool analogNavigationCooldown = false;
    private const float ANALOG_NAVIGATION_TRESHOLD = 0.640000045f;
    private const float ANALOG_NAVIGATION_COOL = 0.0400000028f;
    private const float NAVIGATION_THRESHOLD = 150f;
    private bool isNodesListComplete = false;

    public IEnumerable<IMMPage> Pages => (IEnumerable<IMMPage>) this.pages;

    public MMPage OpenedPage { get; private set; }

    public RectTransform ContentRect => this.contentRect;

    public float LinkThickness => this.linkThickness;

    public Material Material => this.material;

    public Sprite LinkImage => this.linkImage;

    public float RadiusToSpriteSize => this.radiusToSpriteSize;

    public Sprite ActiveGlowImage => this.activeGlowImage;

    public Texture PredictionIcon => this.predictionIcon;

    public GameObject NewNodeIndicatorPrefab => this.newNodeIndicatorPrefab;

    public Color ActiveGlowColor => this.activeGlowColor;

    public Color InactiveTint => this.inactiveTint;

    public IEntity Actor { get; set; }

    public MMPage GlobalPage
    {
      get => this.globalPage;
      set
      {
        if (value == this.globalPage)
          return;
        bool flag = this.OpenedPage == this.globalPage;
        this.globalPage = value;
        if (flag)
          this.OpenPage(this.globalPage);
        else
          this.sideBar.UpdateButtons();
        Debug.Log((object) "Mind Map : Global page set");
      }
    }

    public MMPage LastPage
    {
      get => this.pages.Count == 0 ? (MMPage) null : this.pages[this.pages.Count - 1];
      set
      {
      }
    }

    public int PageCount => this.pages.Count;

    private float BaseScalePower
    {
      get => this.baseScalePower;
      set
      {
        if ((double) this.baseScalePower == (double) value)
          return;
        this.baseScalePower = value;
        this.ApplyScale();
      }
    }

    private float ScaleValue
    {
      get => this.scaleValue;
      set
      {
        if ((double) this.scaleValue == (double) value)
          return;
        this.scaleValue = value;
        this.ApplyScale();
      }
    }

    public IEnumerable<IMMContent> Contents => (IEnumerable<IMMContent>) this.contents.Values;

    private void ApplyScale()
    {
      float num = Mathf.Pow(2f, Mathf.Lerp(this.baseScalePower, 1.5f, this.scaleValue));
      this.ContentRect.localScale = new Vector3(num, num, num);
    }

    public void CallMap(IMMNode node)
    {
      if (ServiceLocator.GetService<InterfaceBlockingService>().BlockMapInterface)
        return;
      ServiceLocator.GetService<MapService>().FocusedNode = node;
      ServiceLocator.GetService<UIService>().Swap<IMapWindow>();
    }

    private void CollectMapNodes()
    {
      foreach (IMapItem mapItem in ServiceLocator.GetService<MapService>().Items)
      {
        if (mapItem.Nodes != null)
        {
          foreach (IMMNode node in mapItem.Nodes)
            this.mapNodes.Add(node);
        }
      }
    }

    public Color ColorByKind(MMContentKind kind)
    {
      switch (kind)
      {
        case MMContentKind.Success:
          return this.successColor;
        case MMContentKind.Fail:
          return this.failColor;
        case MMContentKind.Knowledge:
          return this.knowledgeColor;
        case MMContentKind.IsolatedFact:
          return this.isolatedFactColor;
        default:
          return this.normalColor;
      }
    }

    public bool MapContainsNode(IMMNode node) => this.mapNodes.Contains(node);

    public void CreateContent(
      Guid id,
      MMContentKind kind,
      IMMPlaceholder placeholder,
      LocalizedText description)
    {
      IMMContent content = ServiceLocator.GetService<IFactory>().Create<IMMContent>(id);
      content.Kind = kind;
      content.Placeholder = placeholder;
      content.Description = description;
      this.AddContent(content);
    }

    public void AddContent(IMMContent content)
    {
      this.contents.Add(content.Id, (MMContent) content);
    }

    public void RemoveContent(IMMContent content) => this.contents.Remove(content.Id);

    private void CloseOpenedPage()
    {
      this.infoView.Hide();
      if ((UnityEngine.Object) this.openedPageView != (UnityEngine.Object) null)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.openedPageView.gameObject);
        this.openedPageView = (MMPageView) null;
      }
      this.OpenedPage = (MMPage) null;
      this.nodes = new List<MMNodeView>();
      this.isNodesListComplete = false;
    }

    public void HideNodeInfo(MMNodeView nodeView) => this.infoView.Hide(nodeView);

    public void AddPage(MMPage page)
    {
      bool flag = this.OpenedPage != null && this.OpenedPage == this.LastPage;
      Debug.Log((object) ("Mind Map : Local page " + (object) this.pages.Count + " added"));
      this.pages.Add(page);
      if (flag)
        this.OpenPage(page);
      else
        this.sideBar.UpdateButtons();
    }

    public void ClearPages()
    {
      this.pages.Clear();
      if (this.OpenedPage != null && this.OpenedPage != this.globalPage)
        this.OpenPage(this.globalPage);
      else
        this.sideBar.UpdateButtons();
    }

    public void RemovePage(MMPage page)
    {
      int index = this.pages.IndexOf(page);
      if (index == -1)
        return;
      this.RemovePageAt(index);
    }

    public void RemovePageAt(int index)
    {
      bool flag = this.OpenedPage == this.pages[index];
      this.pages.RemoveAt(index);
      if (flag)
        this.OpenPage(this.globalPage);
      else
        this.sideBar.UpdateButtons();
      Debug.Log((object) ("Mind Map : Local page " + (object) index + " removed"));
    }

    public MMPage GetPage(int index) => this.pages[index];

    public void SetPage(int index, MMPage page)
    {
      bool flag = this.OpenedPage == this.pages[index];
      this.pages[index] = page;
      if (flag)
        this.OpenPage(page);
      else
        this.sideBar.UpdateButtons();
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
      ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
      InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
      PlayerUtility.ShowPlayerHands(false);
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.GenericPlayerMenu, new GameActionHandle(((UIWindow) this).CancelListener), true);
      this.CollectMapNodes();
      this.OpenPage(this.LastPage);
      this.ScaleValue = 0.0f;
      ServiceLocator.GetService<NotificationService>().RemoveNotify(NotificationEnum.MindMap);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickUp, new GameActionHandle(this.MoveOnNodes));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickDown, new GameActionHandle(this.MoveOnNodes));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickLeft, new GameActionHandle(this.MoveOnNodes));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickRight, new GameActionHandle(this.MoveOnNodes));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, new GameActionHandle(this.SwapToMap));
      this.rayCaster = this.GetComponent<GraphicRaycaster>();
      this.pointerData = new PointerEventData(EventSystem.current)
      {
        position = (Vector2) this.consoleCursor.transform.position
      };
      this.SelectNode();
    }

    private bool SwapToMap(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed || !down || !((UnityEngine.Object) this.newHovered != (UnityEngine.Object) null))
        return false;
      this.CallMap((IMMNode) this.newHovered.Node);
      return true;
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      this.scroll.movementType = joystick ? ScrollRect.MovementType.Unrestricted : ScrollRect.MovementType.Elastic;
      CursorService.Instance.Visible = !joystick;
      this.helpPanel.SetActive(joystick);
      this.consoleCursor.SetActive(joystick);
      if ((UnityEngine.Object) this.hovered != (UnityEngine.Object) null)
      {
        this.hovered?.OnPointerExit(this.pointerData);
        this.hovered = (MMNodeView) null;
      }
      if ((UnityEngine.Object) this.newHovered != (UnityEngine.Object) null)
      {
        this.newHovered?.OnPointerExit(this.pointerData);
        this.newHovered = (MMNodeView) null;
      }
      if (joystick)
      {
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.MindMap, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        this.SelectNode();
      }
      else
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.MindMap, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
    }

    private bool MoveOnNodes(GameActionType type, bool down) => false;

    private void ParseNodes(MMWindow.NodeMoveDirection direction)
    {
      Transform transform = (UnityEngine.Object) this.newHovered != (UnityEngine.Object) null ? this.newHovered.transform : this.consoleCursor.transform;
      this.SetNodes(this.openedPageView);
      if (this.nodes.Count == 0)
        return;
      Vector3 dir = Vector3.up;
      switch (direction)
      {
        case MMWindow.NodeMoveDirection.Up:
          dir = Vector3.up;
          break;
        case MMWindow.NodeMoveDirection.Down:
          dir = Vector3.down;
          break;
        case MMWindow.NodeMoveDirection.Left:
          dir = Vector3.left;
          break;
        case MMWindow.NodeMoveDirection.Right:
          dir = Vector3.right;
          break;
      }
      this.NavigateNode(dir);
    }

    private void NavigateNode(Vector3 dir)
    {
      if (this.nodes.Count == 0)
        return;
      MMNodeView component = UISelectableHelper.Select(this.nodes.Select<MMNodeView, GameObject>((Func<MMNodeView, GameObject>) (n => n.gameObject)), this.consoleCursor, dir)?.GetComponent<MMNodeView>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        return;
      this.cursorMapPosition = (Vector2) this.contentRect.InverseTransformPoint(component.gameObject.transform.position);
      this.consoleCursor.transform.position = this.contentRect.TransformPoint((Vector3) this.cursorMapPosition);
      this.pointerData = new PointerEventData(EventSystem.current)
      {
        position = (Vector2) this.consoleCursor.transform.position
      };
      this.SelectNode();
    }

    private void UpdateNavigation()
    {
      if (!InputService.Instance.JoystickUsed)
        return;
      Vector2 dir = new Vector2(InputService.Instance.GetAxis("RightStickX"), -InputService.Instance.GetAxis("RightStickY"));
      if (!this.analogNavigationCooldown && (double) dir.sqrMagnitude > 0.64000004529953)
      {
        this.analogNavigationCooldown = true;
        dir.Normalize();
        this.NavigateNode((Vector3) dir);
      }
      else if (this.analogNavigationCooldown && (double) dir.sqrMagnitude < 0.64000004529953)
        this.analogNavigationCooldown = false;
      float num1 = Mathf.Pow(2f, Mathf.Lerp(this.baseScalePower, 1.5f, this.scaleValue));
      Vector2 zero = Vector2.zero;
      float axis1 = InputService.Instance.GetAxis("LeftStickX");
      float num2 = (this.xPrev + axis1) * Time.deltaTime;
      this.xPrev = axis1;
      float axis2 = InputService.Instance.GetAxis("LeftStickY");
      float num3 = (this.yPrev + axis2) * Time.deltaTime;
      this.yPrev = axis2;
      if ((double) num2 != 0.0 || (double) num3 != 0.0)
      {
        this.currentAcceleration = 3f;
        if ((double) this.currentAcceleration >= 1.0)
        {
          num2 *= this.currentAcceleration;
          num3 *= this.currentAcceleration;
        }
        Vector2 vector2 = this.cursorMapPosition - new Vector2(-(num2 * (ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity / num1)), num3 * (ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity / num1));
        vector2.x = Mathf.Clamp(vector2.x, this.minNavigationValues.x, this.maxNavigationValues.x);
        vector2.y = Mathf.Clamp(vector2.y, this.minNavigationValues.y, this.maxNavigationValues.y);
        this.cursorMapPosition = vector2;
      }
      else if ((double) this.currentAcceleration != 0.0)
        this.currentAcceleration = 0.0f;
      RectTransform transform = (RectTransform) this.consoleCursor.transform;
      if ((double) transform.anchoredPosition.sqrMagnitude > 1.0)
        this.contentRect.anchoredPosition += -transform.anchoredPosition * Mathf.MoveTowards(0.0f, 1f, 5f * Time.deltaTime);
    }

    private void LateUpdate()
    {
      if (!this.consoleCursor.activeSelf)
        return;
      Vector3 vector3 = this.contentRect.TransformPoint((Vector3) this.cursorMapPosition);
      this.consoleCursor.transform.position = vector3;
      if ((double) vector3.sqrMagnitude > 0.0)
      {
        this.pointerData = new PointerEventData(EventSystem.current)
        {
          position = (Vector2) this.consoleCursor.transform.position
        };
        this.SelectNode();
      }
    }

    private void SelectNode()
    {
      List<RaycastResult> source = new List<RaycastResult>();
      if (this.pointerData == null)
        return;
      this.rayCaster.Raycast(this.pointerData, source);
      if (source.Count == 1 && (UnityEngine.Object) source.ElementAt<RaycastResult>(0).gameObject?.GetComponent<ScrollRect>() != (UnityEngine.Object) null)
      {
        this.hovered?.OnPointerExit(this.pointerData);
        this.newHovered?.OnPointerExit(this.pointerData);
        this.hovered = (MMNodeView) null;
        this.newHovered = (MMNodeView) null;
      }
      foreach (RaycastResult raycastResult in source)
      {
        MMNodeView component = raycastResult.gameObject?.GetComponent<MMNodeView>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        {
          this.newHovered = component;
          break;
        }
      }
      if (!((UnityEngine.Object) this.newHovered != (UnityEngine.Object) this.hovered))
        return;
      this.hovered?.OnPointerExit(this.pointerData);
      this.previousNode = this.hovered;
      this.newHovered?.OnPointerEnter(this.pointerData);
      this.hovered = this.newHovered;
    }

    private void SetNodes(MMPageView pageView)
    {
      this.nodes = new List<MMNodeView>((IEnumerable<MMNodeView>) pageView.GetComponentsInChildren<MMNodeView>());
      if (this.nodes.Count == 0)
        return;
      this.UpdateBoundVectors();
    }

    private void UpdateBoundVectors()
    {
      if (this.nodes.Count == 0)
        return;
      this.maxNavigationValues = new Vector2();
      this.minNavigationValues = new Vector2();
      for (int index = 0; index < this.nodes.Count; ++index)
      {
        Vector2 vector2 = (Vector2) this.contentRect.InverseTransformPoint(this.nodes[index].transform.position);
        this.minNavigationValues.x = Mathf.Min(this.minNavigationValues.x, vector2.x - 150f);
        this.minNavigationValues.y = Mathf.Min(this.minNavigationValues.y, vector2.y - 150f);
        this.maxNavigationValues.x = Mathf.Max(this.maxNavigationValues.x, vector2.x + 150f);
        this.maxNavigationValues.y = Mathf.Max(this.maxNavigationValues.y, vector2.y + 150f);
      }
      if (!this.isNodesListComplete)
        this.isNodesListComplete = true;
    }

    protected override void OnDisable()
    {
      ServiceLocator.GetService<CameraService>().Kind = this.lastCameraKind;
      PlayerUtility.ShowPlayerHands(true);
      InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.MindMap, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.GenericPlayerMenu, new GameActionHandle(((UIWindow) this).CancelListener));
      this.CloseOpenedPage();
      this.mapNodes.Clear();
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickUp, new GameActionHandle(this.MoveOnNodes));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickDown, new GameActionHandle(this.MoveOnNodes));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickLeft, new GameActionHandle(this.MoveOnNodes));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickRight, new GameActionHandle(this.MoveOnNodes));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.SwapToMap));
      base.OnDisable();
    }

    public void OpenPage(MMPage page, MMNode node = null)
    {
      this.CloseOpenedPage();
      this.contentRect.anchoredPosition = Vector2.zero;
      if (page == null)
        return;
      this.desiredScaleValue = 0.0f;
      this.OpenedPage = page;
      this.sideBar.UpdateButtons();
      this.openedPageView = MMPageView.Create(this, page, node);
      this.hovered = (MMNodeView) null;
      this.newHovered = (MMNodeView) null;
      this.maxNavigationValues = Vector2.zero;
      this.minNavigationValues = Vector2.zero;
      this.nodes = new List<MMNodeView>((IEnumerable<MMNodeView>) this.openedPageView.GetComponentsInChildren<MMNodeView>());
      this.cursorMapPosition = Vector2.zero;
      this.UpdateBoundVectors();
    }

    public void FocusOnNode(RectTransform node)
    {
      this.ScaleValue = 0.5f;
      this.desiredScaleValue = 0.5f;
      Vector2 position = (Vector2) node.position;
      Vector2 vector2 = new Vector2((float) Screen.width / 2f, (float) Screen.height / 2f);
      if (!InputService.Instance.JoystickUsed)
        this.ContentRect.anchoredPosition += vector2 - position;
      this.cursorMapPosition = (Vector2) this.ContentRect.InverseTransformPoint(node.position);
    }

    public float RadiusByKind(MMNodeKind kind)
    {
      switch (kind)
      {
        case MMNodeKind.Normal:
          return this.normalRadius;
        case MMNodeKind.Сonclusion:
          return this.conclusionRadius;
        default:
          return this.missionRadius;
      }
    }

    public void ShowNodeInfo(MMNodeView nodeView)
    {
      bool blockMapInterface = ServiceLocator.GetService<InterfaceBlockingService>().BlockMapInterface;
      this.infoView.Show(nodeView, !blockMapInterface && this.mapNodes.Contains((IMMNode) nodeView.Node));
      this.currentNode = (IMMNode) nodeView.Node;
    }

    private void Update()
    {
      if (!this.isNodesListComplete)
        this.SetNodes(this.openedPageView);
      this.UpdateNavigation();
      float num1 = Input.GetAxis("MouseWheel") * ExternalSettingsInstance<ExternalInputSettings>.Instance.WheelMouseSensitivity;
      if ((double) num1 == 0.0)
      {
        float axis1 = InputService.Instance.GetAxis("LT");
        float axis2 = InputService.Instance.GetAxis("RT");
        if ((double) axis1 != 0.0)
          num1 = -axis1 * ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickScaleSensitivity;
        else if ((double) axis2 != 0.0)
          num1 = axis2 * ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickScaleSensitivity;
      }
      this.desiredScaleValue = Mathf.Clamp(this.desiredScaleValue + num1, 0.0f, 1f);
      if ((double) this.ScaleValue == (double) this.desiredScaleValue)
        return;
      float x1 = this.contentRect.localScale.x;
      this.ScaleValue = Mathf.MoveTowards(this.ScaleValue, this.desiredScaleValue, 5f * Time.deltaTime);
      float x2 = this.contentRect.localScale.x;
      if (InputService.Instance.JoystickUsed ? !InputService.Instance.GetButton("A", false) : !Input.GetMouseButton(0))
      {
        float num2 = x2 / x1;
        Vector3 anchoredPosition = (Vector3) this.ContentRect.anchoredPosition;
        Vector3 vector3_1 = this.ContentRect.parent.InverseTransformPoint(InputService.Instance.JoystickUsed ? this.consoleCursor.transform.position : Input.mousePosition);
        Vector3 vector3_2 = (anchoredPosition - vector3_1) * num2;
        this.ContentRect.anchoredPosition = (Vector2) (vector3_1 + vector3_2);
      }
    }

    public override void Initialize()
    {
      this.RegisterLayer<IMMWindow>((IMMWindow) this);
      base.Initialize();
    }

    public override System.Type GetWindowType() => typeof (IMMWindow);

    public IMMPlaceholder GetPlaceholderByNode(Guid id)
    {
      MMContent mmContent;
      this.contents.TryGetValue(id, out mmContent);
      return mmContent?.Placeholder;
    }

    public LocalizedText GetTextByNode(Guid id)
    {
      MMContent mmContent;
      this.contents.TryGetValue(id, out mmContent);
      return mmContent != null ? mmContent.Description : LocalizedText.Empty;
    }

    public void GoToNode(IMMNode node)
    {
      foreach (MMPage page in this.pages)
      {
        if (page.Nodes.Contains<IMMNode>(node))
        {
          this.OpenPage(page, (MMNode) node);
          return;
        }
      }
      if (this.globalPage == null || !this.globalPage.Nodes.Contains<IMMNode>(node))
        return;
      this.OpenPage(this.globalPage, (MMNode) node);
    }

    public void SetContentSize(Vector2 size)
    {
      this.contentRect.sizeDelta = size;
      Vector2 sizeDelta = ((RectTransform) this.contentRect.parent).sizeDelta;
      if ((double) size.x <= 0.0 || (double) size.y <= 0.0)
      {
        this.BaseScalePower = 0.0f;
      }
      else
      {
        float num = Mathf.Min(sizeDelta.x / size.x, sizeDelta.y / size.y);
        this.BaseScalePower = Mathf.Log((double) num < 1.0 ? num : 1f, 2f);
      }
    }

    public override IEnumerator OnOpened()
    {
      SimplePlayerWindowSwapper.SetLastOpenedPlayerWindow<IMMWindow>((IWindow) this);
      return base.OnOpened();
    }

    public override bool IsWindowAvailable
    {
      get => !ServiceLocator.GetService<InterfaceBlockingService>().BlockMindMapInterface;
    }

    private enum NodeMoveDirection
    {
      None = -1, // 0xFFFFFFFF
      Up = 0,
      Down = 1,
      Left = 2,
      Right = 3,
    }
  }
}
