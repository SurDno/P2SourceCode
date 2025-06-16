using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Engine.Source.UI.Menu.Protagonist.MindMap;

public class MMWindow : UIWindow, IMMWindow, IWindow, IPauseMenu {
	[SerializeField] private RectTransform contentRect;
	[SerializeField] private MMSideBar sideBar;
	[SerializeField] private MMInfoView infoView;
	[Space] [SerializeField] private Color normalColor;
	[SerializeField] private Color successColor;
	[SerializeField] private Color failColor;
	[SerializeField] private Color knowledgeColor;
	[SerializeField] private Color isolatedFactColor;
	[SerializeField] private Color inactiveTint;
	[Space] [SerializeField] private Sprite linkImage;
	[SerializeField] private float linkThickness = 16f;
	[SerializeField] private Texture predictionIcon;
	[SerializeField] private Sprite activeGlowImage;
	[SerializeField] private Color activeGlowColor = Color.white;
	[SerializeField] private Material material;
	[Space] [SerializeField] private float missionRadius;
	[SerializeField] private float conclusionRadius;
	[SerializeField] private float normalRadius;
	[Space] [SerializeField] private float radiusToSpriteSize;
	[SerializeField] private GameObject newNodeIndicatorPrefab;
	[Inspected] private Dictionary<Guid, MMContent> contents = new(GuidComparer.Instance);
	[Inspected] private List<MMPage> pages = new();
	private MMPage globalPage;
	private CameraKindEnum lastCameraKind;
	private float baseScalePower;
	private float desiredScaleValue;
	private float scaleValue;
	private MMPageView openedPageView;
	private HashSet<IMMNode> mapNodes = new();
	private int currentNodeIndex = 0;
	[SerializeField] private GameObject helpPanel;
	[SerializeField] private GameObject consoleCursor;
	[SerializeField] private ScrollRect scroll;
	private float xPrev;
	private float yPrev;
	private IMMNode currentNode;
	private List<MMNodeView> nodes = new();
	private const float CLOSEST_NODE_DETECTION_THRESHOLD = 2f;
	private NodeMoveDirection bufferedDirection = NodeMoveDirection.None;
	private MMNodeView previousNode;
	private MMNodeView hovered;
	private MMNodeView newHovered;
	private GraphicRaycaster rayCaster;
	private PointerEventData pointerData;
	private const float MAX_ACCELERATION_RATE = 3f;
	private float currentAcceleration;
	private float accelerationSpeed = 3f;
	private Vector2 cursorMapPosition = Vector2.zero;
	private Vector2 maxNavigationValues = Vector2.zero;
	private Vector2 minNavigationValues = Vector2.zero;
	private bool analogNavigationCooldown;
	private const float ANALOG_NAVIGATION_TRESHOLD = 0.640000045f;
	private const float ANALOG_NAVIGATION_COOL = 0.0400000028f;
	private const float NAVIGATION_THRESHOLD = 150f;
	private bool isNodesListComplete;

	public IEnumerable<IMMPage> Pages => pages;

	public MMPage OpenedPage { get; private set; }

	public RectTransform ContentRect => contentRect;

	public float LinkThickness => linkThickness;

	public Material Material => material;

	public Sprite LinkImage => linkImage;

	public float RadiusToSpriteSize => radiusToSpriteSize;

	public Sprite ActiveGlowImage => activeGlowImage;

	public Texture PredictionIcon => predictionIcon;

	public GameObject NewNodeIndicatorPrefab => newNodeIndicatorPrefab;

	public Color ActiveGlowColor => activeGlowColor;

	public Color InactiveTint => inactiveTint;

	public IEntity Actor { get; set; }

	public MMPage GlobalPage {
		get => globalPage;
		set {
			if (value == globalPage)
				return;
			var flag = OpenedPage == globalPage;
			globalPage = value;
			if (flag)
				OpenPage(globalPage);
			else
				sideBar.UpdateButtons();
			Debug.Log("Mind Map : Global page set");
		}
	}

	public MMPage LastPage {
		get => pages.Count == 0 ? null : pages[pages.Count - 1];
		set { }
	}

	public int PageCount => pages.Count;

	private float BaseScalePower {
		get => baseScalePower;
		set {
			if (baseScalePower == (double)value)
				return;
			baseScalePower = value;
			ApplyScale();
		}
	}

	private float ScaleValue {
		get => scaleValue;
		set {
			if (scaleValue == (double)value)
				return;
			scaleValue = value;
			ApplyScale();
		}
	}

	public IEnumerable<IMMContent> Contents => contents.Values;

	private void ApplyScale() {
		var num = Mathf.Pow(2f, Mathf.Lerp(baseScalePower, 1.5f, scaleValue));
		ContentRect.localScale = new Vector3(num, num, num);
	}

	public void CallMap(IMMNode node) {
		if (ServiceLocator.GetService<InterfaceBlockingService>().BlockMapInterface)
			return;
		ServiceLocator.GetService<MapService>().FocusedNode = node;
		ServiceLocator.GetService<UIService>().Swap<IMapWindow>();
	}

	private void CollectMapNodes() {
		foreach (var mapItem in ServiceLocator.GetService<MapService>().Items)
			if (mapItem.Nodes != null)
				foreach (var node in mapItem.Nodes)
					mapNodes.Add(node);
	}

	public Color ColorByKind(MMContentKind kind) {
		switch (kind) {
			case MMContentKind.Success:
				return successColor;
			case MMContentKind.Fail:
				return failColor;
			case MMContentKind.Knowledge:
				return knowledgeColor;
			case MMContentKind.IsolatedFact:
				return isolatedFactColor;
			default:
				return normalColor;
		}
	}

	public bool MapContainsNode(IMMNode node) {
		return mapNodes.Contains(node);
	}

	public void CreateContent(
		Guid id,
		MMContentKind kind,
		IMMPlaceholder placeholder,
		LocalizedText description) {
		var content = ServiceLocator.GetService<IFactory>().Create<IMMContent>(id);
		content.Kind = kind;
		content.Placeholder = placeholder;
		content.Description = description;
		AddContent(content);
	}

	public void AddContent(IMMContent content) {
		contents.Add(content.Id, (MMContent)content);
	}

	public void RemoveContent(IMMContent content) {
		contents.Remove(content.Id);
	}

	private void CloseOpenedPage() {
		infoView.Hide();
		if (openedPageView != null) {
			Destroy(openedPageView.gameObject);
			openedPageView = null;
		}

		OpenedPage = null;
		nodes = new List<MMNodeView>();
		isNodesListComplete = false;
	}

	public void HideNodeInfo(MMNodeView nodeView) {
		infoView.Hide(nodeView);
	}

	public void AddPage(MMPage page) {
		var flag = OpenedPage != null && OpenedPage == LastPage;
		Debug.Log("Mind Map : Local page " + pages.Count + " added");
		pages.Add(page);
		if (flag)
			OpenPage(page);
		else
			sideBar.UpdateButtons();
	}

	public void ClearPages() {
		pages.Clear();
		if (OpenedPage != null && OpenedPage != globalPage)
			OpenPage(globalPage);
		else
			sideBar.UpdateButtons();
	}

	public void RemovePage(MMPage page) {
		var index = pages.IndexOf(page);
		if (index == -1)
			return;
		RemovePageAt(index);
	}

	public void RemovePageAt(int index) {
		var flag = OpenedPage == pages[index];
		pages.RemoveAt(index);
		if (flag)
			OpenPage(globalPage);
		else
			sideBar.UpdateButtons();
		Debug.Log("Mind Map : Local page " + index + " removed");
	}

	public MMPage GetPage(int index) {
		return pages[index];
	}

	public void SetPage(int index, MMPage page) {
		var flag = OpenedPage == pages[index];
		pages[index] = page;
		if (flag)
			OpenPage(page);
		else
			sideBar.UpdateButtons();
	}

	protected override void OnEnable() {
		base.OnEnable();
		lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
		ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
		InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
		PlayerUtility.ShowPlayerHands(false);
		CursorService.Instance.Free = CursorService.Instance.Visible = true;
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, CancelListener);
		ServiceLocator.GetService<GameActionService>()
			.AddListener(GameActionType.GenericPlayerMenu, CancelListener, true);
		CollectMapNodes();
		OpenPage(LastPage);
		ScaleValue = 0.0f;
		ServiceLocator.GetService<NotificationService>().RemoveNotify(NotificationEnum.MindMap);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickUp, MoveOnNodes);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickDown, MoveOnNodes);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickLeft, MoveOnNodes);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickRight, MoveOnNodes);
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, SwapToMap);
		rayCaster = GetComponent<GraphicRaycaster>();
		pointerData = new PointerEventData(EventSystem.current) {
			position = consoleCursor.transform.position
		};
		SelectNode();
	}

	private bool SwapToMap(GameActionType type, bool down) {
		if (!InputService.Instance.JoystickUsed || !down || !(newHovered != null))
			return false;
		CallMap(newHovered.Node);
		return true;
	}

	protected override void OnJoystick(bool joystick) {
		base.OnJoystick(joystick);
		scroll.movementType = joystick ? ScrollRect.MovementType.Unrestricted : ScrollRect.MovementType.Elastic;
		CursorService.Instance.Visible = !joystick;
		helpPanel.SetActive(joystick);
		consoleCursor.SetActive(joystick);
		if (hovered != null) {
			hovered?.OnPointerExit(pointerData);
			hovered = null;
		}

		if (newHovered != null) {
			newHovered?.OnPointerExit(pointerData);
			newHovered = null;
		}

		if (joystick) {
			ServiceLocator.GetService<GameActionService>()
				.RemoveListener(GameActionType.MindMap, WithoutJoystickCancelListener);
			SelectNode();
		} else
			ServiceLocator.GetService<GameActionService>()
				.AddListener(GameActionType.MindMap, WithoutJoystickCancelListener);
	}

	private bool MoveOnNodes(GameActionType type, bool down) {
		return false;
	}

	private void ParseNodes(NodeMoveDirection direction) {
		var transform = newHovered != null ? newHovered.transform : consoleCursor.transform;
		SetNodes(openedPageView);
		if (nodes.Count == 0)
			return;
		var dir = Vector3.up;
		switch (direction) {
			case NodeMoveDirection.Up:
				dir = Vector3.up;
				break;
			case NodeMoveDirection.Down:
				dir = Vector3.down;
				break;
			case NodeMoveDirection.Left:
				dir = Vector3.left;
				break;
			case NodeMoveDirection.Right:
				dir = Vector3.right;
				break;
		}

		NavigateNode(dir);
	}

	private void NavigateNode(Vector3 dir) {
		if (nodes.Count == 0)
			return;
		var component = UISelectableHelper.Select(nodes.Select(n => n.gameObject), consoleCursor, dir)
			?.GetComponent<MMNodeView>();
		if (component == null)
			return;
		cursorMapPosition = contentRect.InverseTransformPoint(component.gameObject.transform.position);
		consoleCursor.transform.position = contentRect.TransformPoint(cursorMapPosition);
		pointerData = new PointerEventData(EventSystem.current) {
			position = consoleCursor.transform.position
		};
		SelectNode();
	}

	private void UpdateNavigation() {
		if (!InputService.Instance.JoystickUsed)
			return;
		var dir = new Vector2(InputService.Instance.GetAxis("RightStickX"),
			-InputService.Instance.GetAxis("RightStickY"));
		if (!analogNavigationCooldown && dir.sqrMagnitude > 0.64000004529953) {
			analogNavigationCooldown = true;
			dir.Normalize();
			NavigateNode(dir);
		} else if (analogNavigationCooldown && dir.sqrMagnitude < 0.64000004529953)
			analogNavigationCooldown = false;

		var num1 = Mathf.Pow(2f, Mathf.Lerp(baseScalePower, 1.5f, scaleValue));
		var zero = Vector2.zero;
		var axis1 = InputService.Instance.GetAxis("LeftStickX");
		var num2 = (xPrev + axis1) * Time.deltaTime;
		xPrev = axis1;
		var axis2 = InputService.Instance.GetAxis("LeftStickY");
		var num3 = (yPrev + axis2) * Time.deltaTime;
		yPrev = axis2;
		if (num2 != 0.0 || num3 != 0.0) {
			currentAcceleration = 3f;
			if (currentAcceleration >= 1.0) {
				num2 *= currentAcceleration;
				num3 *= currentAcceleration;
			}

			var vector2 = cursorMapPosition -
			              new Vector2(
				              -(num2 * (ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity /
				                        num1)),
				              num3 * (ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity /
				                      num1));
			vector2.x = Mathf.Clamp(vector2.x, minNavigationValues.x, maxNavigationValues.x);
			vector2.y = Mathf.Clamp(vector2.y, minNavigationValues.y, maxNavigationValues.y);
			cursorMapPosition = vector2;
		} else if (currentAcceleration != 0.0)
			currentAcceleration = 0.0f;

		var transform = (RectTransform)consoleCursor.transform;
		if (transform.anchoredPosition.sqrMagnitude > 1.0)
			contentRect.anchoredPosition +=
				-transform.anchoredPosition * Mathf.MoveTowards(0.0f, 1f, 5f * Time.deltaTime);
	}

	private void LateUpdate() {
		if (!consoleCursor.activeSelf)
			return;
		var vector3 = contentRect.TransformPoint(cursorMapPosition);
		consoleCursor.transform.position = vector3;
		if (vector3.sqrMagnitude > 0.0) {
			pointerData = new PointerEventData(EventSystem.current) {
				position = consoleCursor.transform.position
			};
			SelectNode();
		}
	}

	private void SelectNode() {
		var source = new List<RaycastResult>();
		if (pointerData == null)
			return;
		rayCaster.Raycast(pointerData, source);
		if (source.Count == 1 && source.ElementAt(0).gameObject?.GetComponent<ScrollRect>() != null) {
			hovered?.OnPointerExit(pointerData);
			newHovered?.OnPointerExit(pointerData);
			hovered = null;
			newHovered = null;
		}

		foreach (var raycastResult in source) {
			var component = raycastResult.gameObject?.GetComponent<MMNodeView>();
			if (component != null) {
				newHovered = component;
				break;
			}
		}

		if (!(newHovered != hovered))
			return;
		hovered?.OnPointerExit(pointerData);
		previousNode = hovered;
		newHovered?.OnPointerEnter(pointerData);
		hovered = newHovered;
	}

	private void SetNodes(MMPageView pageView) {
		nodes = new List<MMNodeView>(pageView.GetComponentsInChildren<MMNodeView>());
		if (nodes.Count == 0)
			return;
		UpdateBoundVectors();
	}

	private void UpdateBoundVectors() {
		if (nodes.Count == 0)
			return;
		maxNavigationValues = new Vector2();
		minNavigationValues = new Vector2();
		for (var index = 0; index < nodes.Count; ++index) {
			Vector2 vector2 = contentRect.InverseTransformPoint(nodes[index].transform.position);
			minNavigationValues.x = Mathf.Min(minNavigationValues.x, vector2.x - 150f);
			minNavigationValues.y = Mathf.Min(minNavigationValues.y, vector2.y - 150f);
			maxNavigationValues.x = Mathf.Max(maxNavigationValues.x, vector2.x + 150f);
			maxNavigationValues.y = Mathf.Max(maxNavigationValues.y, vector2.y + 150f);
		}

		if (!isNodesListComplete)
			isNodesListComplete = true;
	}

	protected override void OnDisable() {
		ServiceLocator.GetService<CameraService>().Kind = lastCameraKind;
		PlayerUtility.ShowPlayerHands(true);
		InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, CancelListener);
		ServiceLocator.GetService<GameActionService>()
			.RemoveListener(GameActionType.MindMap, WithoutJoystickCancelListener);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.GenericPlayerMenu, CancelListener);
		CloseOpenedPage();
		mapNodes.Clear();
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickUp, MoveOnNodes);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickDown, MoveOnNodes);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickLeft, MoveOnNodes);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickRight, MoveOnNodes);
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, SwapToMap);
		base.OnDisable();
	}

	public void OpenPage(MMPage page, MMNode node = null) {
		CloseOpenedPage();
		contentRect.anchoredPosition = Vector2.zero;
		if (page == null)
			return;
		desiredScaleValue = 0.0f;
		OpenedPage = page;
		sideBar.UpdateButtons();
		openedPageView = MMPageView.Create(this, page, node);
		hovered = null;
		newHovered = null;
		maxNavigationValues = Vector2.zero;
		minNavigationValues = Vector2.zero;
		nodes = new List<MMNodeView>(openedPageView.GetComponentsInChildren<MMNodeView>());
		cursorMapPosition = Vector2.zero;
		UpdateBoundVectors();
	}

	public void FocusOnNode(RectTransform node) {
		ScaleValue = 0.5f;
		desiredScaleValue = 0.5f;
		Vector2 position = node.position;
		var vector2 = new Vector2(Screen.width / 2f, Screen.height / 2f);
		if (!InputService.Instance.JoystickUsed)
			ContentRect.anchoredPosition += vector2 - position;
		cursorMapPosition = ContentRect.InverseTransformPoint(node.position);
	}

	public float RadiusByKind(MMNodeKind kind) {
		switch (kind) {
			case MMNodeKind.Normal:
				return normalRadius;
			case MMNodeKind.Сonclusion:
				return conclusionRadius;
			default:
				return missionRadius;
		}
	}

	public void ShowNodeInfo(MMNodeView nodeView) {
		var blockMapInterface = ServiceLocator.GetService<InterfaceBlockingService>().BlockMapInterface;
		infoView.Show(nodeView, !blockMapInterface && mapNodes.Contains(nodeView.Node));
		currentNode = nodeView.Node;
	}

	private void Update() {
		if (!isNodesListComplete)
			SetNodes(openedPageView);
		UpdateNavigation();
		var num1 = Input.GetAxis("MouseWheel") *
		           ExternalSettingsInstance<ExternalInputSettings>.Instance.WheelMouseSensitivity;
		if (num1 == 0.0) {
			var axis1 = InputService.Instance.GetAxis("LT");
			var axis2 = InputService.Instance.GetAxis("RT");
			if (axis1 != 0.0)
				num1 = -axis1 * ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickScaleSensitivity;
			else if (axis2 != 0.0)
				num1 = axis2 * ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickScaleSensitivity;
		}

		desiredScaleValue = Mathf.Clamp(desiredScaleValue + num1, 0.0f, 1f);
		if (ScaleValue == (double)desiredScaleValue)
			return;
		var x1 = contentRect.localScale.x;
		ScaleValue = Mathf.MoveTowards(ScaleValue, desiredScaleValue, 5f * Time.deltaTime);
		var x2 = contentRect.localScale.x;
		if (InputService.Instance.JoystickUsed
			    ? !InputService.Instance.GetButton("A", false)
			    : !Input.GetMouseButton(0)) {
			var num2 = x2 / x1;
			Vector3 anchoredPosition = ContentRect.anchoredPosition;
			var vector3_1 = ContentRect.parent.InverseTransformPoint(InputService.Instance.JoystickUsed
				? consoleCursor.transform.position
				: Input.mousePosition);
			var vector3_2 = (anchoredPosition - vector3_1) * num2;
			ContentRect.anchoredPosition = vector3_1 + vector3_2;
		}
	}

	public override void Initialize() {
		RegisterLayer<IMMWindow>(this);
		base.Initialize();
	}

	public override Type GetWindowType() {
		return typeof(IMMWindow);
	}

	public IMMPlaceholder GetPlaceholderByNode(Guid id) {
		MMContent mmContent;
		contents.TryGetValue(id, out mmContent);
		return mmContent?.Placeholder;
	}

	public LocalizedText GetTextByNode(Guid id) {
		MMContent mmContent;
		contents.TryGetValue(id, out mmContent);
		return mmContent != null ? mmContent.Description : LocalizedText.Empty;
	}

	public void GoToNode(IMMNode node) {
		foreach (var page in pages)
			if (page.Nodes.Contains(node)) {
				OpenPage(page, (MMNode)node);
				return;
			}

		if (globalPage == null || !globalPage.Nodes.Contains(node))
			return;
		OpenPage(globalPage, (MMNode)node);
	}

	public void SetContentSize(Vector2 size) {
		contentRect.sizeDelta = size;
		var sizeDelta = ((RectTransform)contentRect.parent).sizeDelta;
		if (size.x <= 0.0 || size.y <= 0.0)
			BaseScalePower = 0.0f;
		else {
			var num = Mathf.Min(sizeDelta.x / size.x, sizeDelta.y / size.y);
			BaseScalePower = Mathf.Log(num < 1.0 ? num : 1f, 2f);
		}
	}

	public override IEnumerator OnOpened() {
		SimplePlayerWindowSwapper.SetLastOpenedPlayerWindow<IMMWindow>(this);
		return base.OnOpened();
	}

	public override bool IsWindowAvailable =>
		!ServiceLocator.GetService<InterfaceBlockingService>().BlockMindMapInterface;

	private enum NodeMoveDirection {
		None = -1,
		Up = 0,
		Down = 1,
		Left = 2,
		Right = 3
	}
}