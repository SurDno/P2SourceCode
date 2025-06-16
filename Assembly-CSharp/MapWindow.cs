using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Regions;
using Engine.Common.MindMap;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.MindMap;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Impl.UI.Menu;
using Engine.Impl.UI.Menu.Protagonist.MindMap;
using Engine.Source.Audio;
using Engine.Source.Commons;
using Engine.Source.Components.BoundCharacters;
using Engine.Source.Components.Maps;
using Engine.Source.Services;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings.External;
using Engine.Source.UI;
using Engine.Source.UI.Menu.Protagonist.Map;
using Engine.Source.UI.Menu.Protagonist.MindMap;
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
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapWindow : UIWindow, IMapWindow, IWindow, IPauseMenu
{
  [SerializeField]
  private RectTransform contentRect;
  [SerializeField]
  private RectTransform regionAnchorRect;
  [SerializeField]
  private RectTransform shadowsAnchorRect;
  [SerializeField]
  private RectTransform baseAnchorRect;
  [SerializeField]
  private RectTransform normalsAnchorRect;
  [SerializeField]
  private RectTransform overlayAnchorRect;
  [SerializeField]
  private RectTransform playerAnchorRect;
  [SerializeField]
  private RectTransform referenceRect;
  [SerializeField]
  private GameObject imagePrefab;
  [SerializeField]
  private GameObject overlayImagePrefab;
  [SerializeField]
  private GameObject mindMapImagePrefab;
  [SerializeField]
  private AudioClip windowOpenSound;
  [SerializeField]
  private AudioMixerGroup mixer;
  [Space]
  [SerializeField]
  private Vector2 worldLeftBottom;
  [SerializeField]
  private Vector2 worldRightTop;
  [Space]
  [SerializeField]
  private float regionResolution = 1f;
  [SerializeField]
  private float mainResolution = 4f;
  [SerializeField]
  private float normalResolution = 4f;
  [SerializeField]
  private float shadowResolution = 2f;
  [SerializeField]
  private float nodeSize = 32f;
  [Space]
  [SerializeField]
  private Material regionMaterial;
  [SerializeField]
  private Material buildingMaterial;
  [SerializeField]
  private Material normalMaterial;
  [SerializeField]
  private Material shadowMaterial;
  [SerializeField]
  private Material mindMapNodeMaterial;
  [Space]
  [SerializeField]
  private Color undiscoveredBuildingColor = Color.white;
  [SerializeField]
  private Color buildingColor = Color.white;
  [SerializeField]
  private Color shopBuildingColor = Color.white;
  [SerializeField]
  private Color specialBuildingColor = Color.white;
  [SerializeField]
  private Color npcAtRiskColor = Color.white;
  [SerializeField]
  private Color npcDiseasedColor = Color.white;
  [SerializeField]
  private Color npcDeadColor = Color.white;
  [SerializeField]
  private Color selectionColor = Color.white;
  [SerializeField]
  private RegionColor[] regionColors;
  [SerializeField]
  private Color regionSelectionFarColor = Color.white;
  [SerializeField]
  private Color regionSelectionNearColor = Color.white;
  [Space]
  [SerializeField]
  private MapItemInfoWindow infoWindow;
  [SerializeField]
  private MapRegionInfoWindow regionInfoWindow;
  [Space]
  [SerializeField]
  private ProgressView bull;
  [SerializeField]
  private Image vignette;
  [SerializeField]
  private Vector2 vignetteOpacityRange = new Vector2(0.0f, 1f);
  [Space]
  [SerializeField]
  [Range(0.0f, 1f)]
  private float onPlayerScalePower = 0.5f;
  [SerializeField]
  [Range(0.0f, 1f)]
  private float onNodeScalePower = 1f;
  [SerializeField]
  private MapNodeInfoView infoView;
  [Space]
  [SerializeField]
  private MapFastTravelPointInfoView fastTravelInfoView;
  [SerializeField]
  private Texture fastTravelIcon;
  [Space]
  [SerializeField]
  private HideableView hasCustomMarkerView;
  [SerializeField]
  private HideableView customMarkerEnabledView;
  private MapService mapService;
  [Inspected]
  private float desiredScroll = 0.0f;
  private float scrollVelocity = 0.0f;
  [Inspected]
  private float scroll = 0.0f;
  [Inspected]
  private List<MapItemView> regionObjects = new List<MapItemView>();
  [Inspected]
  private List<GameObject> baseObjects = new List<GameObject>();
  [Inspected]
  private List<RectTransform> overlayObjects = new List<RectTransform>();
  private Vector2 startingPosition = Vector2.zero;
  private float startingScroll = 0.0f;
  private CameraKindEnum lastCameraKind;
  private float xPrev;
  private float yPrev = 0.0f;
  [SerializeField]
  private GameObject consoleCursor;
  [SerializeField]
  private GameObject controlPanel;
  private GameObject playerMarker;
  private GameObject customMarker;
  private List<GameObject> nodes = new List<GameObject>();
  private Vector3 cursorCenter = new Vector3();
  private Vector2 cursorMapPosition = new Vector2();
  private IMMNode bufferedNode;
  private MonoBehaviour hovered = (MonoBehaviour) null;
  private MonoBehaviour newHovered = (MonoBehaviour) null;
  private MonoBehaviour hoveredRegion = (MonoBehaviour) null;
  private MonoBehaviour newHoveredRegion = (MonoBehaviour) null;
  private GraphicRaycaster rayCaster = (GraphicRaycaster) null;
  private PointerEventData pointerData = (PointerEventData) null;
  private const float MAX_ACCELERATION_RATE = 3f;
  private float currentAcceleration = 0.0f;
  private float accelerationSpeed = 3f;
  private const float NAVIGATION_THRESHOLD = 150f;
  private const float MIN_SCROLL_FOR_CURSOR_MOVE = 0.2f;
  private Vector3 maxAllowedPos = Vector3.zero;
  private Vector3 ConsoleToContentDif = Vector3.zero;
  private Vector3 contentEmulation = Vector3.zero;
  private bool analogNavigationCooldown = false;
  private const float ANALOG_NAVIGATION_TRESHOLD = 0.640000045f;
  private const float ANALOG_NAVIGATION_COOL = 0.0400000028f;
  private float cursorReturnSpeed = 5f;
  private float journeyLength = 0.0f;
  private float startTime = 0.0f;

  public float Scroll
  {
    get => this.scroll;
    private set
    {
      if ((double) this.scroll == (double) value)
        return;
      this.scroll = value;
      this.ApplyScale();
    }
  }

  private RectTransform AnchorByKind(ImageKind kind)
  {
    switch (kind)
    {
      case ImageKind.Region:
        return this.regionAnchorRect;
      case ImageKind.Shadow:
      case ImageKind.OutlineAtRisk:
      case ImageKind.OutlineDiseased:
        return this.shadowsAnchorRect;
      case ImageKind.Normal:
        return this.normalsAnchorRect;
      case ImageKind.Overlay:
        return this.overlayAnchorRect;
      case ImageKind.Player:
        return this.playerAnchorRect;
      default:
        return this.baseAnchorRect;
    }
  }

  private void ApplyScale()
  {
    float num1 = Mathf.Max(0.0f, this.scroll);
    float num2 = Mathf.Pow(4f, num1);
    this.contentRect.localScale = new Vector3(num2, num2, num2);
    if (this.overlayObjects != null)
    {
      float num3 = (float) (1.0 / ((double) num2 * (double) this.referenceRect.localScale.x));
      Vector3 vector3 = new Vector3(num3, num3, num3);
      for (int index = 0; index < this.overlayObjects.Count; ++index)
        this.overlayObjects[index].localScale = vector3;
    }
    if (this.regionObjects != null)
    {
      float num4 = num1;
      for (int index = 0; index < this.regionObjects.Count; ++index)
        this.regionObjects[index].OnZoomChange(num4);
    }
    if ((UnityEngine.Object) this.vignette != (UnityEngine.Object) null)
      this.vignette.color = new Color(1f, 1f, 1f, Mathf.Lerp(this.vignetteOpacityRange.y, this.vignetteOpacityRange.x, num1));
    if (!((UnityEngine.Object) this.bull != (UnityEngine.Object) null))
      return;
    this.bull.Progress = Mathf.Clamp01(-this.scroll);
  }

  private void Clear()
  {
    this.regionInfoWindow.Hide();
    if (this.regionObjects != null)
    {
      for (int index = 0; index < this.regionObjects.Count; ++index)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.regionObjects[index].gameObject);
      this.regionObjects.Clear();
      for (int index = 0; index < this.baseObjects.Count; ++index)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.baseObjects[index]);
      this.baseObjects.Clear();
      for (int index = 0; index < this.overlayObjects.Count; ++index)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.overlayObjects[index].gameObject);
      this.overlayObjects.Clear();
    }
    this.HideMapNodeInfo();
    this.HideFastTravelPointInfo();
  }

  private void ColorByKind(
    ImageKind kind,
    out Color baseColor,
    out Color activeColor,
    int disease = 0)
  {
    switch (kind)
    {
      case ImageKind.UndiscoveredBuilding:
        baseColor = this.undiscoveredBuildingColor;
        break;
      case ImageKind.Building:
        baseColor = this.buildingColor;
        break;
      case ImageKind.ShopBuilding:
        baseColor = this.shopBuildingColor;
        break;
      case ImageKind.SpecialBuilding:
        baseColor = this.specialBuildingColor;
        break;
      case ImageKind.SpecialBuildingDead:
        baseColor = this.npcDeadColor;
        break;
      case ImageKind.OutlineAtRisk:
        baseColor = this.npcAtRiskColor;
        break;
      case ImageKind.OutlineDiseased:
        baseColor = this.npcDiseasedColor;
        break;
      default:
        baseColor = Color.white;
        break;
    }
    if (kind == ImageKind.Region)
    {
      baseColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
      activeColor = baseColor;
      for (int index = 0; index < this.regionColors.Length; ++index)
      {
        if (this.regionColors[index].Level == disease)
        {
          baseColor = this.regionColors[index].FarColor;
          activeColor = this.regionColors[index].NearColor;
          break;
        }
      }
    }
    else
      activeColor = baseColor * this.selectionColor * 2f;
  }

  private void CreateImage(
    Rect rectangle,
    Vector2 worldPos,
    float rotation,
    ImageKind kind,
    Sprite sprite,
    float resolution,
    bool alphaRaycast,
    IMapItem item)
  {
    GameObject instance;
    RectTransform rectTransform;
    Image image;
    Vector2 normalizedPoint;
    this.CreateImage(rectangle, worldPos, rotation, sprite.rect.size * resolution, kind, sprite, out instance, out rectTransform, out image, out normalizedPoint);
    if (this.mapService.FastTravelOrigin == null && this.mapService.FocusedItem == null && this.mapService.FocusedNode == null && kind == ImageKind.Player)
    {
      this.startingPosition = (Vector2) this.contentRect.InverseTransformPoint(this.baseAnchorRect.TransformPoint((Vector3) normalizedPoint));
      this.startingScroll = this.onPlayerScalePower;
    }
    image.sprite = sprite;
    image.material = this.MaterialByKind(kind);
    Color baseColor;
    Color activeColor;
    if (kind == ImageKind.Region)
      this.ColorByKind(kind, out baseColor, out activeColor, item == null || !item.Discovered ? 0 : item.Disease);
    else
      this.ColorByKind(kind, out baseColor, out activeColor);
    image.color = baseColor;
    MapItemView mapItemView = (MapItemView) null;
    if (item != null)
    {
      image.alphaHitTestMinimumThreshold = alphaRaycast ? 0.25f : 0.0f;
      mapItemView = instance.AddComponent<MapItemView>();
      if (kind == ImageKind.Region)
        mapItemView.Initialize(this, item, item.Resource.HoverSprite, baseColor, this.regionSelectionFarColor, activeColor, this.regionSelectionNearColor);
      else
        mapItemView.Initialize(this, item, baseColor, activeColor);
    }
    else
      image.raycastTarget = false;
    if (kind == ImageKind.Overlay || kind == ImageKind.Player)
      this.overlayObjects.Add(rectTransform);
    else if (kind == ImageKind.Region)
      this.regionObjects.Add(mapItemView);
    else
      this.baseObjects.Add(instance);
  }

  private void CreateMindMapNodeImage(
    Rect rectangle,
    Vector2 worldPos,
    float size,
    MMTooltip tooltip)
  {
    MMPlaceholder placeholder = tooltip.node?.Content?.Placeholder as MMPlaceholder;
    MapTooltipResource tooltipResource = tooltip.tooltipResource as MapTooltipResource;
    if (placeholder == null && tooltipResource == null)
      return;
    ImageKind kind = ImageKind.Overlay;
    Texture sprite = placeholder == null ? tooltipResource.Image.Value : placeholder.Image.Value;
    if ((UnityEngine.Object) sprite == (UnityEngine.Object) null)
    {
      Debug.LogError((object) ("Sprite not found , holder : " + placeholder.GetInfo() + " , tooltip : " + tooltipResource.GetInfo()));
    }
    else
    {
      Vector2 size1 = new Vector2(size, size);
      GameObject instance;
      RectTransform rectTransform;
      RawImage image;
      Vector2 normalizedPoint;
      this.CreateMindMapImage(rectangle, worldPos, -this.referenceRect.localEulerAngles.z, size1, kind, sprite, out instance, out rectTransform, out image, out normalizedPoint);
      if (this.mapService.FocusedNode != null && this.mapService.FocusedNode == tooltip.node)
      {
        this.startingPosition = (Vector2) this.contentRect.InverseTransformPoint(this.baseAnchorRect.TransformPoint((Vector3) normalizedPoint));
        this.startingScroll = this.onNodeScalePower;
      }
      image.material = this.mindMapNodeMaterial;
      Color baseColor;
      this.ColorByKind(kind, out baseColor, out Color _);
      image.color = baseColor;
      if (tooltip.node != null || tooltip.tooltipText != LocalizedText.Empty)
      {
        image.raycastTarget = true;
        MapNodeView mapNodeView = instance.AddComponent<MapNodeView>();
        mapNodeView.SetMapView(this);
        mapNodeView.SetNode(tooltip);
      }
      else
        image.raycastTarget = false;
      this.overlayObjects.Add(rectTransform);
    }
  }

  private void CreateFastTravelImage(Rect rectangle, Vector2 worldPos, float size, IMapItem item)
  {
    ImageKind kind = ImageKind.Overlay;
    Vector2 size1 = new Vector2(size, size);
    GameObject instance;
    RectTransform rectTransform;
    RawImage image;
    this.CreateMindMapImage(rectangle, worldPos, -this.referenceRect.localEulerAngles.z, size1, kind, this.fastTravelIcon, out instance, out rectTransform, out image, out Vector2 _);
    image.material = this.mindMapNodeMaterial;
    Color baseColor;
    this.ColorByKind(kind, out baseColor, out Color _);
    image.color = baseColor;
    MapFastTravelPointView fastTravelPointView = instance.AddComponent<MapFastTravelPointView>();
    fastTravelPointView.MapView = this;
    fastTravelPointView.Item = item;
    this.overlayObjects.Add(rectTransform);
  }

  public void ShowMapNodeInfo(MapNodeView node, MMTooltip tooltip)
  {
    LocalizationService service = ServiceLocator.GetService<LocalizationService>();
    if (tooltip.node != null)
    {
      LocalizedText text = tooltip.node.Content != null ? tooltip.node.Content.Description : LocalizedText.Empty;
      bool mindMapInterface = ServiceLocator.GetService<InterfaceBlockingService>().BlockMindMapInterface;
      this.infoView.Show(node.GetComponent<RectTransform>(), service.GetText(text), !mindMapInterface);
    }
    else
      this.infoView.Show(node.GetComponent<RectTransform>(), service.GetText(tooltip.tooltipText), false);
  }

  public void HideMapNodeInfo() => this.infoView.Hide();

  public void CallMindMap(IMMNode node)
  {
    if (ServiceLocator.GetService<InterfaceBlockingService>().BlockMindMapInterface)
      return;
    ((MMWindow) ServiceLocator.GetService<UIService>().Swap<IMMWindow>()).GoToNode(node);
  }

  public void CallSelectedFastTravelPoint() => this.fastTravelInfoView?.Travel();

  private void CreateMindMapImage(
    Rect rectangle,
    Vector2 worldPos,
    float rotation,
    Vector2 size,
    ImageKind kind,
    Texture sprite,
    out GameObject instance,
    out RectTransform rectTransform,
    out RawImage image,
    out Vector2 normalizedPoint)
  {
    RectTransform parent = this.AnchorByKind(kind);
    instance = UnityEngine.Object.Instantiate<GameObject>(this.mindMapImagePrefab, (Transform) parent, false);
    instance.name = sprite.name;
    rectTransform = (RectTransform) instance.transform;
    image = instance.GetComponent<RawImage>();
    image.texture = sprite;
    rectTransform.anchorMin = Vector2.zero;
    rectTransform.anchorMax = Vector2.zero;
    normalizedPoint = new Vector2((worldPos.x - rectangle.x) / rectangle.width, (worldPos.y - rectangle.y) / rectangle.height);
    Vector2 sizeDelta = parent.sizeDelta;
    normalizedPoint.x *= sizeDelta.x;
    normalizedPoint.y *= sizeDelta.y;
    rectTransform.pivot = new Vector2(0.5f, 0.5f);
    rectTransform.anchoredPosition = normalizedPoint;
    rectTransform.sizeDelta = size;
    rectTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, rotation);
    instance.SetActive(true);
    if (!(sprite.name == "custom_marker"))
      return;
    this.customMarker = instance;
  }

  private void CreateImage(
    Rect rectangle,
    Vector2 worldPos,
    float rotation,
    Vector2 size,
    ImageKind kind,
    Sprite sprite,
    out GameObject instance,
    out RectTransform rectTransform,
    out Image image,
    out Vector2 normalizedPoint)
  {
    RectTransform parent = this.AnchorByKind(kind);
    instance = UnityEngine.Object.Instantiate<GameObject>(kind == ImageKind.Overlay ? this.overlayImagePrefab : this.imagePrefab, (Transform) parent, false);
    instance.name = sprite.name;
    rectTransform = (RectTransform) instance.transform;
    image = instance.GetComponent<Image>();
    image.sprite = sprite;
    rectTransform.anchorMin = Vector2.zero;
    rectTransform.anchorMax = Vector2.zero;
    normalizedPoint = new Vector2((worldPos.x - rectangle.x) / rectangle.width, (worldPos.y - rectangle.y) / rectangle.height);
    Vector2 sizeDelta = parent.sizeDelta;
    normalizedPoint.x *= sizeDelta.x;
    normalizedPoint.y *= sizeDelta.y;
    rectTransform.pivot = Rect.PointToNormalized(sprite.rect, sprite.pivot);
    rectTransform.anchoredPosition = normalizedPoint;
    rectTransform.sizeDelta = size;
    rectTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, rotation);
    instance.SetActive(true);
    if (kind == ImageKind.Player)
    {
      this.playerMarker = instance;
    }
    else
    {
      if (!(sprite.name == "custom_marker"))
        return;
      this.customMarker = instance;
    }
  }

  public void HideFastTravelPointInfo()
  {
    ServiceLocator.GetService<MapService>().Current = (IEntity) null;
    this.fastTravelInfoView?.Hide();
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.FastTravelListener));
  }

  public void HideInfo(MapItemView itemView)
  {
    ServiceLocator.GetService<MapService>().Current = (IEntity) null;
    if (itemView.IsRegion)
      return;
    this.infoWindow.Hide(itemView);
  }

  private static int ItemComparison(IMapItem x, IMapItem y)
  {
    return (x.Resource != null ? x.Resource.Kind : MapPlaceholderKind.Region).CompareTo((object) (MapPlaceholderKind) (y.Resource != null ? (int) y.Resource.Kind : 0));
  }

  private Material MaterialByKind(ImageKind kind)
  {
    switch (kind)
    {
      case ImageKind.Region:
        return this.regionMaterial;
      case ImageKind.Shadow:
        return this.shadowMaterial;
      case ImageKind.Building:
      case ImageKind.ShopBuilding:
      case ImageKind.SpecialBuilding:
        return this.buildingMaterial;
      case ImageKind.Normal:
        return this.normalMaterial;
      default:
        return (Material) null;
    }
  }

  private void Build2()
  {
    if ((UnityEngine.Object) this.baseAnchorRect == (UnityEngine.Object) null)
      return;
    Rect rect1 = new Rect();
    rect1.min = this.worldLeftBottom;
    rect1.max = this.worldRightTop;
    Rect rectangle = rect1;
    rect1 = new Rect();
    rect1.xMin = Math.Min(rectangle.xMin, rectangle.xMax);
    rect1.xMax = Math.Max(rectangle.xMin, rectangle.xMax);
    rect1.yMin = Math.Min(rectangle.yMin, rectangle.yMax);
    rect1.yMax = Math.Max(rectangle.yMin, rectangle.yMax);
    Rect rect2 = rect1;
    this.startingPosition = Vector2.zero;
    this.startingScroll = 0.0f;
    float num1 = this.baseAnchorRect.sizeDelta.x / rect2.width;
    float num2 = Vector2.Angle(rect2.min - rect2.center, rectangle.min - rectangle.center);
    List<MMTooltip> source = new List<MMTooltip>();
    this.mapService = ServiceLocator.GetService<MapService>();
    float size = this.mapService.FastTravelOrigin != null ? this.nodeSize * 0.5f : this.nodeSize;
    foreach (IMapItem mapItem1 in this.mapService.Items)
    {
      MapPlaceholder resource = mapItem1.Resource;
      Vector2 worldPosition = mapItem1.WorldPosition;
      if (resource != null)
      {
        if (resource.Kind == MapPlaceholderKind.Building || resource.Kind == MapPlaceholderKind.ShopBuilding || resource.Kind == MapPlaceholderKind.SpecialBuilding)
        {
          Sprite shadowSprite = resource.ShadowSprite;
          if ((UnityEngine.Object) shadowSprite != (UnityEngine.Object) null)
            this.CreateImage(rectangle, worldPosition, num2 - mapItem1.Rotation, ImageKind.Shadow, shadowSprite, num1 / this.shadowResolution, false, (IMapItem) null);
        }
        BoundHealthStateEnum boundHealthStateEnum = BoundHealthStateEnum.Normal;
        BoundCharacterComponent component = mapItem1.BoundCharacter?.GetComponent<BoundCharacterComponent>();
        if (component != null)
          boundHealthStateEnum = BoundCharacterUtility.PerceivedHealth(component);
        if (resource.Kind != MapPlaceholderKind.Region && mapItem1.Discovered)
        {
          ImageKind kind = ImageKind.None;
          switch (boundHealthStateEnum)
          {
            case BoundHealthStateEnum.Danger:
              kind = ImageKind.OutlineAtRisk;
              break;
            case BoundHealthStateEnum.Diseased:
              kind = ImageKind.OutlineDiseased;
              break;
          }
          if (kind != 0)
          {
            Sprite hoverSprite = resource.HoverSprite;
            if ((UnityEngine.Object) hoverSprite != (UnityEngine.Object) null)
              this.CreateImage(rectangle, worldPosition, num2 - mapItem1.Rotation, kind, hoverSprite, num1 / this.shadowResolution, false, (IMapItem) null);
          }
        }
        Sprite mainSprite = resource.MainSprite;
        if ((UnityEngine.Object) mainSprite != (UnityEngine.Object) null)
        {
          float resolution = 1f;
          switch (resource.Kind)
          {
            case MapPlaceholderKind.Region:
              resolution = num1 / this.regionResolution;
              break;
            case MapPlaceholderKind.Building:
            case MapPlaceholderKind.ShopBuilding:
            case MapPlaceholderKind.SpecialBuilding:
              resolution = num1 / this.mainResolution;
              break;
          }
          ImageKind kind = ImageKind.Overlay;
          IMapItem mapItem2 = resource.Kind == MapPlaceholderKind.Region || mapItem1.Title != LocalizedText.Empty ? mapItem1 : (IMapItem) null;
          switch (resource.Kind)
          {
            case MapPlaceholderKind.Region:
              kind = ImageKind.Region;
              break;
            case MapPlaceholderKind.Building:
              kind = mapItem1.Discovered ? ImageKind.Building : ImageKind.UndiscoveredBuilding;
              break;
            case MapPlaceholderKind.ShopBuilding:
              kind = mapItem1.Discovered ? ImageKind.ShopBuilding : ImageKind.UndiscoveredBuilding;
              break;
            case MapPlaceholderKind.SpecialBuilding:
              kind = mapItem1.Discovered ? (boundHealthStateEnum == BoundHealthStateEnum.Dead ? ImageKind.SpecialBuildingDead : ImageKind.SpecialBuilding) : ImageKind.UndiscoveredBuilding;
              break;
            case MapPlaceholderKind.Player:
              kind = ImageKind.Player;
              break;
          }
          bool flag = resource.Kind == MapPlaceholderKind.Marker;
          this.CreateImage(rectangle, worldPosition, flag ? 0.0f : num2 - mapItem1.Rotation, kind, mainSprite, resolution, resource.AlphaRaycast, mapItem2);
        }
        if (resource.Kind == MapPlaceholderKind.Building || resource.Kind == MapPlaceholderKind.ShopBuilding || resource.Kind == MapPlaceholderKind.SpecialBuilding)
        {
          Sprite normalSprite = resource.NormalSprite;
          if ((UnityEngine.Object) normalSprite != (UnityEngine.Object) null)
            this.CreateImage(rectangle, worldPosition, num2 - mapItem1.Rotation, ImageKind.Normal, normalSprite, num1 / this.normalResolution, false, (IMapItem) null);
        }
      }
      if (mapItem1.TooltipResource != null)
        source.Add(new MMTooltip(mapItem1.TooltipResource, mapItem1.TooltipText));
      foreach (IMMNode node in mapItem1.Nodes)
        source.Add(new MMTooltip(node));
      int num3 = source.Count<MMTooltip>();
      if (num3 == 1)
      {
        MMTooltip tooltip = source.First<MMTooltip>();
        this.CreateMindMapNodeImage(rectangle, worldPosition, size, tooltip);
      }
      else if (num3 > 1)
      {
        float num4 = (float) ((double) (ExternalSettingsInstance<ExternalGraphicsSettings>.Instance.MindMapIconSize * (float) num3) / 2.0 * 3.1415927410125732);
        float num5 = 6.28318548f / (float) num3;
        int num6 = 0;
        foreach (MMTooltip tooltip in source)
        {
          Vector2 vector2 = new Vector2(Mathf.Cos(num5 * (float) num6) * num4, Mathf.Sin(num5 * (float) num6) * num4);
          this.CreateMindMapNodeImage(rectangle, worldPosition + vector2, size, tooltip);
          ++num6;
        }
      }
      source.Clear();
      FastTravelPointEnum fastTravelPointEnum = this.mapService.FastTravelOrigin != null ? mapItem1.FastTravelPoint.Value : FastTravelPointEnum.None;
      if (fastTravelPointEnum != FastTravelPointEnum.None && fastTravelPointEnum != this.mapService.FastTravelOrigin.FastTravelPointIndex.Value)
        this.CreateFastTravelImage(rectangle, worldPosition, this.nodeSize, mapItem1);
      if (this.mapService.FocusedItem == mapItem1)
      {
        Vector2 normalized = Rect.PointToNormalized(rectangle, worldPosition);
        Vector2 sizeDelta = this.baseAnchorRect.sizeDelta;
        normalized.x *= sizeDelta.x;
        normalized.y *= sizeDelta.y;
        this.startingPosition = (Vector2) this.contentRect.InverseTransformPoint(this.baseAnchorRect.TransformPoint((Vector3) normalized));
        this.startingScroll = this.onNodeScalePower;
      }
    }
    this.mapService.FocusedItem = (IMapItem) null;
    this.mapService.FocusedNode = (IMMNode) null;
    IMapItemComponent component1 = this.mapService.CustomMarker?.GetComponent<IMapItemComponent>();
    this.customMarkerEnabledView.Visible = component1 != null && component1.IsEnabled;
  }

  protected override void OnEnable()
  {
    base.OnEnable();
    this.scroll = float.NaN;
    this.Build2();
    MapService service = ServiceLocator.GetService<MapService>();
    if (service.BullModeForced)
    {
      this.startingScroll = -1f;
      service.BullModeForced = false;
    }
    this.Scroll = this.startingScroll;
    this.scrollVelocity = 0.0f;
    this.desiredScroll = this.startingScroll;
    float num = Mathf.Pow(4f, Mathf.Max(0.0f, this.startingScroll));
    Vector2 vector2_1 = (this.contentRect.sizeDelta * num - this.contentRect.sizeDelta) * 0.5f;
    if ((double) vector2_1.x < 0.0)
      vector2_1.x = 0.0f;
    if ((double) vector2_1.y < 0.0)
      vector2_1.y = 0.0f;
    this.cursorMapPosition = this.startingPosition;
    Vector2 vector2_2 = -num * this.startingPosition;
    if ((double) vector2_2.x < -(double) vector2_1.x)
      vector2_2.x = -vector2_1.x;
    else if ((double) vector2_2.x > (double) vector2_1.x)
      vector2_2.x = vector2_1.x;
    if ((double) vector2_2.y < -(double) vector2_1.y)
      vector2_2.y = -vector2_1.y;
    else if ((double) vector2_2.y > (double) vector2_1.y)
      vector2_2.y = vector2_1.y;
    this.contentRect.anchoredPosition = vector2_2;
    this.lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
    ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
    CursorService.Instance.Free = CursorService.Instance.Visible = true;
    InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
    PlayerUtility.ShowPlayerHands(false);
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.GenericPlayerMenu, new GameActionHandle(((UIWindow) this).CancelListener), true);
    ServiceLocator.GetService<NotificationService>().RemoveNotify(NotificationEnum.Map);
    SoundUtility.PlayAudioClip2D(this.windowOpenSound, this.mixer, 1f, 0.0f, context: this.gameObject.GetFullName());
    this.hasCustomMarkerView.Visible = service.CustomMarker != null;
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickUp, new GameActionHandle(this.MoveOnNodes));
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickDown, new GameActionHandle(this.MoveOnNodes));
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickLeft, new GameActionHandle(this.MoveOnNodes));
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickRight, new GameActionHandle(this.MoveOnNodes));
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, new GameActionHandle(this.SwapToMMap));
    this.rayCaster = this.GetComponent<GraphicRaycaster>();
    this.pointerData = new PointerEventData(EventSystem.current)
    {
      position = (Vector2) this.consoleCursor.transform.position
    };
    List<MapNodeView> mapNodeViewList = new List<MapNodeView>((IEnumerable<MapNodeView>) this.contentRect.GetComponentsInChildren<MapNodeView>());
    this.cursorCenter = this.consoleCursor.transform.position;
    this.SelectNode();
  }

  protected override void OnDisable()
  {
    ServiceLocator.GetService<CameraService>().Kind = this.lastCameraKind;
    PlayerUtility.ShowPlayerHands(true);
    InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Map, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.GenericPlayerMenu, new GameActionHandle(((UIWindow) this).CancelListener));
    this.Clear();
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickUp, new GameActionHandle(this.MoveOnNodes));
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickDown, new GameActionHandle(this.MoveOnNodes));
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickLeft, new GameActionHandle(this.MoveOnNodes));
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickRight, new GameActionHandle(this.MoveOnNodes));
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.SwapToMMap));
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.FastTravelListener));
    this.consoleCursor.transform.position = this.cursorCenter;
    this.cursorMapPosition = (Vector2) this.contentRect.InverseTransformPoint(this.cursorCenter);
    base.OnDisable();
  }

  private bool FastTravelListener(GameActionType type, bool down)
  {
    if (!(type == GameActionType.Submit & down))
      return false;
    this.CallSelectedFastTravelPoint();
    return true;
  }

  private bool SwapToMMap(GameActionType type, bool down)
  {
    if (!InputService.Instance.JoystickUsed || !down || !((UnityEngine.Object) this.hovered != (UnityEngine.Object) null) || !(this.hovered is MapNodeView))
      return false;
    this.CallMindMap(((MapNodeView) this.hovered).GetNode());
    return true;
  }

  private bool MoveOnNodes(GameActionType type, bool down) => false;

  private void NavigateNode(Vector3 dir)
  {
    this.nodes = new List<GameObject>(((IEnumerable<MapNodeView>) this.contentRect.GetComponentsInChildren<MapNodeView>()).Select<MapNodeView, GameObject>((Func<MapNodeView, GameObject>) (n => n.gameObject)));
    this.nodes.Add(this.playerMarker);
    List<GameObject> collection = new List<GameObject>(((IEnumerable<MapFastTravelPointView>) this.contentRect.GetComponentsInChildren<MapFastTravelPointView>()).Select<MapFastTravelPointView, GameObject>((Func<MapFastTravelPointView, GameObject>) (node => node.gameObject)));
    if (collection.Count > 0)
      this.nodes.AddRange((IEnumerable<GameObject>) collection);
    if (this.customMarkerEnabledView.Visible)
      this.nodes.Add(this.customMarker);
    GameObject gameObject = UISelectableHelper.Select((IEnumerable<GameObject>) this.nodes, this.consoleCursor, dir);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      return;
    this.cursorMapPosition = (Vector2) this.contentRect.InverseTransformPoint(gameObject.gameObject.transform.position);
    this.consoleCursor.transform.position = this.contentRect.TransformPoint((Vector3) this.cursorMapPosition);
    this.pointerData = new PointerEventData(EventSystem.current)
    {
      position = (Vector2) this.consoleCursor.transform.position
    };
    this.SelectNode();
  }

  private void SelectNode()
  {
    if (this.pointerData == null)
      return;
    if ((UnityEngine.Object) this.bull != (UnityEngine.Object) null && (double) this.bull.Progress != 0.0)
    {
      ((IPointerExitHandler) this.newHovered)?.OnPointerExit(this.pointerData);
      ((IPointerExitHandler) this.newHoveredRegion)?.OnPointerExit(this.pointerData);
    }
    else
    {
      List<RaycastResult> raycastResultList = new List<RaycastResult>();
      this.rayCaster.Raycast(this.pointerData, raycastResultList);
      this.newHovered = (MonoBehaviour) null;
      this.newHoveredRegion = (MonoBehaviour) null;
      foreach (RaycastResult raycastResult in raycastResultList)
      {
        MonoBehaviour component = (MonoBehaviour) raycastResult.gameObject?.GetComponent<MapItemView>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null && ((MapItemView) component).IsRegion)
          this.newHoveredRegion = component;
        else if (!((UnityEngine.Object) this.newHovered != (UnityEngine.Object) null))
        {
          if ((UnityEngine.Object) component == (UnityEngine.Object) null)
            component = (MonoBehaviour) raycastResult.gameObject?.GetComponent<MapNodeView>();
          if ((UnityEngine.Object) component == (UnityEngine.Object) null)
            component = (MonoBehaviour) raycastResult.gameObject?.GetComponent<MapFastTravelPointView>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          {
            if (component is MapNodeView)
              this.bufferedNode = ((MapNodeView) component).GetNode();
            this.newHovered = component;
          }
        }
      }
      if ((UnityEngine.Object) this.newHovered != (UnityEngine.Object) this.hovered)
      {
        ((IPointerExitHandler) this.hovered)?.OnPointerExit(this.pointerData);
        ((IPointerEnterHandler) this.newHovered)?.OnPointerEnter(this.pointerData);
        this.hovered = this.newHovered;
      }
      if (!((UnityEngine.Object) this.newHoveredRegion != (UnityEngine.Object) this.hoveredRegion))
        return;
      ((IPointerExitHandler) this.hoveredRegion)?.OnPointerExit(this.pointerData);
      ((IPointerEnterHandler) this.newHoveredRegion)?.OnPointerEnter(this.pointerData);
      this.hoveredRegion = this.newHoveredRegion;
    }
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
    float scale = MapWindow.ScrollToScale(this.Scroll);
    Vector2 zero = Vector2.zero;
    float axis1 = InputService.Instance.GetAxis("LeftStickX");
    float num1 = (this.xPrev + axis1) * Time.deltaTime;
    this.xPrev = axis1;
    float axis2 = InputService.Instance.GetAxis("LeftStickY");
    float num2 = (this.yPrev + axis2) * Time.deltaTime;
    this.yPrev = axis2;
    if ((double) num1 != 0.0 || (double) num2 != 0.0)
    {
      this.currentAcceleration = 3f;
      if ((double) this.currentAcceleration >= 1.0)
      {
        num1 *= this.currentAcceleration;
        num2 *= this.currentAcceleration;
      }
      float num3 = num1 * (ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity / scale);
      float num4 = num2 * (ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity / scale);
      this.cursorMapPosition.x += num3;
      this.cursorMapPosition.y -= num4;
      this.cursorMapPosition.x = Mathf.Clamp(this.cursorMapPosition.x, (float) (-(double) this.contentRect.sizeDelta.x * 0.5), this.contentRect.sizeDelta.x * 0.5f);
      this.cursorMapPosition.y = Mathf.Clamp(this.cursorMapPosition.y, (float) (-(double) this.contentRect.sizeDelta.y * 0.5), this.contentRect.sizeDelta.y * 0.5f);
    }
    else
      this.currentAcceleration = 0.0f;
    RectTransform transform = (RectTransform) this.consoleCursor.transform;
    if ((double) transform.anchoredPosition.sqrMagnitude > 1.0)
    {
      Vector2 vector2_1 = this.contentRect.anchoredPosition - transform.anchoredPosition;
      Vector2 vector2_2 = new Vector2((float) ((double) this.contentRect.sizeDelta.x * ((double) scale - 1.0) * 0.5), (float) ((double) this.contentRect.sizeDelta.y * ((double) scale - 1.0) * 0.5));
      if ((double) vector2_2.x < 0.0)
        vector2_2.x = 0.0f;
      if ((double) vector2_2.y < 0.0)
        vector2_2.y = 0.0f;
      if ((double) vector2_1.x < -(double) vector2_2.x)
        vector2_1.x = -vector2_2.x;
      if ((double) vector2_1.x > (double) vector2_2.x)
        vector2_1.x = vector2_2.x;
      if ((double) vector2_1.y < -(double) vector2_2.y)
        vector2_1.y = -vector2_2.y;
      if ((double) vector2_1.y > (double) vector2_2.y)
        vector2_1.y = vector2_2.y;
      this.contentRect.anchoredPosition += (vector2_1 - this.contentRect.anchoredPosition) * Mathf.MoveTowards(0.0f, 1f, 5f * Time.deltaTime);
    }
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

  private bool IsOutOfArea(Vector3 maxPos, Vector3 anchorPos)
  {
    bool flag = false;
    if ((double) anchorPos.x < -(double) maxPos.x)
    {
      if ((double) anchorPos.y < -(double) maxPos.y)
        flag = true;
      else if ((double) anchorPos.y > (double) maxPos.y)
        flag = true;
    }
    else if ((double) anchorPos.x > (double) maxPos.x)
    {
      if ((double) anchorPos.y < -(double) maxPos.y)
        flag = true;
      else if ((double) anchorPos.y > (double) maxPos.y)
        flag = true;
    }
    else if ((double) anchorPos.y < -(double) maxPos.y)
    {
      if ((double) anchorPos.x < -(double) maxPos.x)
        flag = true;
      else if ((double) anchorPos.x > (double) maxPos.y)
        flag = true;
    }
    else if ((double) anchorPos.y > (double) maxPos.y)
    {
      if ((double) anchorPos.x < -(double) maxPos.x)
        flag = true;
      else if ((double) anchorPos.x > (double) maxPos.y)
        flag = true;
    }
    return flag;
  }

  protected override void OnJoystick(bool joystick)
  {
    base.OnJoystick(joystick);
    CursorService.Instance.Visible = !joystick;
    this.controlPanel.SetActive(joystick);
    this.consoleCursor.SetActive(joystick);
    EventSystem.current.SetSelectedGameObject((GameObject) null);
    if ((UnityEngine.Object) this.hovered != (UnityEngine.Object) null)
    {
      ((IPointerExitHandler) this.hovered)?.OnPointerExit(this.pointerData);
      this.hovered = (MonoBehaviour) null;
    }
    if ((UnityEngine.Object) this.newHovered != (UnityEngine.Object) null)
    {
      ((IPointerExitHandler) this.newHovered)?.OnPointerExit(this.pointerData);
      this.newHovered = (MonoBehaviour) null;
    }
    if (joystick)
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Map, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
      this.SelectNode();
    }
    else
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Map, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
  }

  public void ShowInfo(MapItemView itemView)
  {
    if (itemView.Item is IComponent component)
      ServiceLocator.GetService<MapService>().Current = component.Owner;
    if (itemView.IsRegion)
      this.regionInfoWindow.Show(itemView);
    else
      this.infoWindow.Show(itemView);
  }

  public void ShowFastTravelPointInfo(MapFastTravelPointView pointView)
  {
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, new GameActionHandle(this.FastTravelListener));
    MapService service = ServiceLocator.GetService<MapService>();
    if (pointView.Item is IComponent component)
      service.Current = component.Owner;
    this.fastTravelInfoView?.Show(service.FastTravelOrigin, pointView);
  }

  private void Update()
  {
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
    if ((double) num1 < 0.0)
      this.desiredScroll = (double) this.Scroll > 0.0 || !ServiceLocator.GetService<MapService>().BullModeAvailable ? Mathf.Clamp01(this.desiredScroll + num1) : -1f;
    else if ((double) num1 > 0.0)
      this.desiredScroll = (double) this.desiredScroll >= 0.0 ? Mathf.Clamp01(this.desiredScroll + num1) : 0.0f;
    if ((double) this.Scroll != (double) this.desiredScroll)
    {
      float scale1 = MapWindow.ScrollToScale(this.Scroll);
      this.Scroll = Mathf.MoveTowards(Mathf.SmoothDamp(this.Scroll, this.desiredScroll, ref this.scrollVelocity, 0.15f), this.desiredScroll, Time.deltaTime * 0.01f);
      float scale2 = MapWindow.ScrollToScale(this.Scroll);
      if (InputService.Instance.JoystickUsed ? !InputService.Instance.GetButton("A", false) : !Input.GetMouseButton(0))
      {
        float num2 = scale2 / scale1;
        Vector2 anchoredPosition = this.contentRect.anchoredPosition;
        Vector2 vector2_1 = (Vector2) this.contentRect.parent.InverseTransformPoint(InputService.Instance.JoystickUsed ? this.consoleCursor.transform.position : Input.mousePosition);
        Vector2 vector2_2 = (anchoredPosition - vector2_1) * num2;
        this.contentRect.anchoredPosition = vector2_1 + vector2_2;
      }
      if (!InputService.Instance.JoystickUsed)
      {
        Vector2 anchoredPosition = this.contentRect.anchoredPosition;
        Vector2 vector2 = new Vector2((float) ((1920.0 * (double) scale2 - 1920.0) * 0.5), (float) ((1080.0 * (double) scale2 - 886.0) * 0.5));
        if ((double) vector2.x < 0.0)
          vector2.x = 0.0f;
        if ((double) vector2.y < 0.0)
          vector2.y = 0.0f;
        if ((double) anchoredPosition.x < -(double) vector2.x)
          anchoredPosition.x = -vector2.x;
        if ((double) anchoredPosition.x > (double) vector2.x)
          anchoredPosition.x = vector2.x;
        if ((double) anchoredPosition.y < -(double) vector2.y)
          anchoredPosition.y = -vector2.y;
        if ((double) anchoredPosition.y > (double) vector2.y)
          anchoredPosition.y = vector2.y;
        this.contentRect.anchoredPosition = anchoredPosition;
      }
    }
    this.UpdateNavigation();
    if (!Input.GetMouseButtonDown(1) && !InputService.Instance.GetButtonDown("Y", false))
      return;
    this.OnCustomMarkerButton();
  }

  public override void Initialize()
  {
    this.RegisterLayer<IMapWindow>((IMapWindow) this);
    base.Initialize();
  }

  public override System.Type GetWindowType() => typeof (IMapWindow);

  public Vector3 GetWorldPosition(Vector3 mousePosition)
  {
    Vector2 sizeDelta = this.baseAnchorRect.sizeDelta;
    Rect rect = new Rect()
    {
      min = this.worldLeftBottom,
      max = this.worldRightTop
    };
    mousePosition = (Vector3) (Vector2) this.baseAnchorRect.InverseTransformPoint(mousePosition);
    mousePosition.x /= sizeDelta.x;
    mousePosition.y /= sizeDelta.y;
    mousePosition.x = mousePosition.x * rect.width + rect.x;
    mousePosition.y = mousePosition.y * rect.height + rect.y;
    return mousePosition;
  }

  public Vector2 GetScreenPosition(Vector3 worldPos)
  {
    Vector2 point = new Vector2(worldPos.x, worldPos.z);
    Vector2 sizeDelta = this.baseAnchorRect.sizeDelta;
    Vector2 normalized = Rect.PointToNormalized(new Rect()
    {
      min = this.worldLeftBottom,
      max = this.worldRightTop
    }, point);
    normalized.x *= sizeDelta.x;
    normalized.y *= sizeDelta.y;
    Vector2 screenPosition = (Vector2) this.baseAnchorRect.TransformPoint((Vector3) normalized);
    screenPosition.y = (float) Screen.height - screenPosition.y;
    return screenPosition;
  }

  private static float ScrollToScale(float scroll) => Mathf.Pow(4f, Mathf.Clamp01(scroll));

  private void OnCustomMarkerButton()
  {
    if (!this.hasCustomMarkerView.Visible)
      return;
    if (this.customMarkerEnabledView.Visible)
      this.RemoveCustomMarker();
    else
      this.CreateCustomMarker();
  }

  private void CreateCustomMarker()
  {
    IEntity customMarker = ServiceLocator.GetService<MapService>()?.CustomMarker;
    if (customMarker == null)
      return;
    MapCustomMarkerComponent component1 = customMarker.GetComponent<MapCustomMarkerComponent>();
    if (component1 != null)
      component1.Position = !InputService.Instance.JoystickUsed ? (Vector2) this.GetWorldPosition(Input.mousePosition) : (Vector2) this.GetWorldPosition(this.consoleCursor.transform.position);
    IMapItemComponent component2 = customMarker.GetComponent<IMapItemComponent>();
    if (component2 != null)
      component2.IsEnabled = true;
    this.OnCustomMarkerChange();
  }

  private void RemoveCustomMarker()
  {
    IMapItemComponent component = ServiceLocator.GetService<MapService>()?.CustomMarker?.GetComponent<IMapItemComponent>();
    if (component == null)
      return;
    component.IsEnabled = false;
    this.OnCustomMarkerChange();
  }

  private void OnCustomMarkerChange()
  {
    this.Clear();
    this.Build2();
    this.ApplyScale();
  }

  public override IEnumerator OnOpened()
  {
    SimplePlayerWindowSwapper.SetLastOpenedPlayerWindow<IMapWindow>((IWindow) this);
    return base.OnOpened();
  }

  public override bool IsWindowAvailable
  {
    get => !ServiceLocator.GetService<InterfaceBlockingService>().BlockMapInterface;
  }

  private enum NodeMoveDirection
  {
    Up,
    Down,
    Left,
    Right,
  }
}
