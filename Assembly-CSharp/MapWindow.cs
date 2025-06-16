using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
  private float desiredScroll;
  private float scrollVelocity;
  [Inspected]
  private float scroll;
  [Inspected]
  private List<MapItemView> regionObjects = new List<MapItemView>();
  [Inspected]
  private List<GameObject> baseObjects = new List<GameObject>();
  [Inspected]
  private List<RectTransform> overlayObjects = new List<RectTransform>();
  private Vector2 startingPosition = Vector2.zero;
  private float startingScroll;
  private CameraKindEnum lastCameraKind;
  private float xPrev;
  private float yPrev;
  [SerializeField]
  private GameObject consoleCursor;
  [SerializeField]
  private GameObject controlPanel;
  private GameObject playerMarker;
  private GameObject customMarker;
  private List<GameObject> nodes = new List<GameObject>();
  private Vector3 cursorCenter;
  private Vector2 cursorMapPosition;
  private IMMNode bufferedNode;
  private MonoBehaviour hovered;
  private MonoBehaviour newHovered;
  private MonoBehaviour hoveredRegion;
  private MonoBehaviour newHoveredRegion;
  private GraphicRaycaster rayCaster;
  private PointerEventData pointerData;
  private const float MAX_ACCELERATION_RATE = 3f;
  private float currentAcceleration;
  private float accelerationSpeed = 3f;
  private const float NAVIGATION_THRESHOLD = 150f;
  private const float MIN_SCROLL_FOR_CURSOR_MOVE = 0.2f;
  private Vector3 maxAllowedPos = Vector3.zero;
  private Vector3 ConsoleToContentDif = Vector3.zero;
  private Vector3 contentEmulation = Vector3.zero;
  private bool analogNavigationCooldown;
  private const float ANALOG_NAVIGATION_TRESHOLD = 0.640000045f;
  private const float ANALOG_NAVIGATION_COOL = 0.0400000028f;
  private float cursorReturnSpeed = 5f;
  private float journeyLength = 0.0f;
  private float startTime = 0.0f;

  public float Scroll
  {
    get => scroll;
    private set
    {
      if (scroll == (double) value)
        return;
      scroll = value;
      ApplyScale();
    }
  }

  private RectTransform AnchorByKind(ImageKind kind)
  {
    switch (kind)
    {
      case ImageKind.Region:
        return regionAnchorRect;
      case ImageKind.Shadow:
      case ImageKind.OutlineAtRisk:
      case ImageKind.OutlineDiseased:
        return shadowsAnchorRect;
      case ImageKind.Normal:
        return normalsAnchorRect;
      case ImageKind.Overlay:
        return overlayAnchorRect;
      case ImageKind.Player:
        return playerAnchorRect;
      default:
        return baseAnchorRect;
    }
  }

  private void ApplyScale()
  {
    float num1 = Mathf.Max(0.0f, scroll);
    float num2 = Mathf.Pow(4f, num1);
    contentRect.localScale = new Vector3(num2, num2, num2);
    if (overlayObjects != null)
    {
      float num3 = (float) (1.0 / (num2 * (double) referenceRect.localScale.x));
      Vector3 vector3 = new Vector3(num3, num3, num3);
      for (int index = 0; index < overlayObjects.Count; ++index)
        overlayObjects[index].localScale = vector3;
    }
    if (regionObjects != null)
    {
      float num4 = num1;
      for (int index = 0; index < regionObjects.Count; ++index)
        regionObjects[index].OnZoomChange(num4);
    }
    if (vignette != null)
      vignette.color = new Color(1f, 1f, 1f, Mathf.Lerp(vignetteOpacityRange.y, vignetteOpacityRange.x, num1));
    if (!(bull != null))
      return;
    bull.Progress = Mathf.Clamp01(-scroll);
  }

  private void Clear()
  {
    regionInfoWindow.Hide();
    if (regionObjects != null)
    {
      for (int index = 0; index < regionObjects.Count; ++index)
        Destroy(regionObjects[index].gameObject);
      regionObjects.Clear();
      for (int index = 0; index < baseObjects.Count; ++index)
        Destroy(baseObjects[index]);
      baseObjects.Clear();
      for (int index = 0; index < overlayObjects.Count; ++index)
        Destroy(overlayObjects[index].gameObject);
      overlayObjects.Clear();
    }
    HideMapNodeInfo();
    HideFastTravelPointInfo();
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
        baseColor = undiscoveredBuildingColor;
        break;
      case ImageKind.Building:
        baseColor = buildingColor;
        break;
      case ImageKind.ShopBuilding:
        baseColor = shopBuildingColor;
        break;
      case ImageKind.SpecialBuilding:
        baseColor = specialBuildingColor;
        break;
      case ImageKind.SpecialBuildingDead:
        baseColor = npcDeadColor;
        break;
      case ImageKind.OutlineAtRisk:
        baseColor = npcAtRiskColor;
        break;
      case ImageKind.OutlineDiseased:
        baseColor = npcDiseasedColor;
        break;
      default:
        baseColor = Color.white;
        break;
    }
    if (kind == ImageKind.Region)
    {
      baseColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
      activeColor = baseColor;
      for (int index = 0; index < regionColors.Length; ++index)
      {
        if (regionColors[index].Level == disease)
        {
          baseColor = regionColors[index].FarColor;
          activeColor = regionColors[index].NearColor;
          break;
        }
      }
    }
    else
      activeColor = baseColor * selectionColor * 2f;
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
    CreateImage(rectangle, worldPos, rotation, sprite.rect.size * resolution, kind, sprite, out instance, out rectTransform, out image, out normalizedPoint);
    if (mapService.FastTravelOrigin == null && mapService.FocusedItem == null && mapService.FocusedNode == null && kind == ImageKind.Player)
    {
      startingPosition = contentRect.InverseTransformPoint(baseAnchorRect.TransformPoint(normalizedPoint));
      startingScroll = onPlayerScalePower;
    }
    image.sprite = sprite;
    image.material = MaterialByKind(kind);
    Color baseColor;
    Color activeColor;
    if (kind == ImageKind.Region)
      ColorByKind(kind, out baseColor, out activeColor, item == null || !item.Discovered ? 0 : item.Disease);
    else
      ColorByKind(kind, out baseColor, out activeColor);
    image.color = baseColor;
    MapItemView mapItemView = null;
    if (item != null)
    {
      image.alphaHitTestMinimumThreshold = alphaRaycast ? 0.25f : 0.0f;
      mapItemView = instance.AddComponent<MapItemView>();
      if (kind == ImageKind.Region)
        mapItemView.Initialize(this, item, item.Resource.HoverSprite, baseColor, regionSelectionFarColor, activeColor, regionSelectionNearColor);
      else
        mapItemView.Initialize(this, item, baseColor, activeColor);
    }
    else
      image.raycastTarget = false;
    if (kind == ImageKind.Overlay || kind == ImageKind.Player)
      overlayObjects.Add(rectTransform);
    else if (kind == ImageKind.Region)
      regionObjects.Add(mapItemView);
    else
      baseObjects.Add(instance);
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
    if (sprite == null)
    {
      Debug.LogError("Sprite not found , holder : " + placeholder.GetInfo() + " , tooltip : " + tooltipResource.GetInfo());
    }
    else
    {
      Vector2 size1 = new Vector2(size, size);
      GameObject instance;
      RectTransform rectTransform;
      RawImage image;
      Vector2 normalizedPoint;
      CreateMindMapImage(rectangle, worldPos, -referenceRect.localEulerAngles.z, size1, kind, sprite, out instance, out rectTransform, out image, out normalizedPoint);
      if (mapService.FocusedNode != null && mapService.FocusedNode == tooltip.node)
      {
        startingPosition = contentRect.InverseTransformPoint(baseAnchorRect.TransformPoint(normalizedPoint));
        startingScroll = onNodeScalePower;
      }
      image.material = mindMapNodeMaterial;
      Color baseColor;
      ColorByKind(kind, out baseColor, out Color _);
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
      overlayObjects.Add(rectTransform);
    }
  }

  private void CreateFastTravelImage(Rect rectangle, Vector2 worldPos, float size, IMapItem item)
  {
    ImageKind kind = ImageKind.Overlay;
    Vector2 size1 = new Vector2(size, size);
    GameObject instance;
    RectTransform rectTransform;
    RawImage image;
    CreateMindMapImage(rectangle, worldPos, -referenceRect.localEulerAngles.z, size1, kind, fastTravelIcon, out instance, out rectTransform, out image, out Vector2 _);
    image.material = mindMapNodeMaterial;
    Color baseColor;
    ColorByKind(kind, out baseColor, out Color _);
    image.color = baseColor;
    MapFastTravelPointView fastTravelPointView = instance.AddComponent<MapFastTravelPointView>();
    fastTravelPointView.MapView = this;
    fastTravelPointView.Item = item;
    overlayObjects.Add(rectTransform);
  }

  public void ShowMapNodeInfo(MapNodeView node, MMTooltip tooltip)
  {
    LocalizationService service = ServiceLocator.GetService<LocalizationService>();
    if (tooltip.node != null)
    {
      LocalizedText text = tooltip.node.Content != null ? tooltip.node.Content.Description : LocalizedText.Empty;
      bool mindMapInterface = ServiceLocator.GetService<InterfaceBlockingService>().BlockMindMapInterface;
      infoView.Show(node.GetComponent<RectTransform>(), service.GetText(text), !mindMapInterface);
    }
    else
      infoView.Show(node.GetComponent<RectTransform>(), service.GetText(tooltip.tooltipText), false);
  }

  public void HideMapNodeInfo() => infoView.Hide();

  public void CallMindMap(IMMNode node)
  {
    if (ServiceLocator.GetService<InterfaceBlockingService>().BlockMindMapInterface)
      return;
    ((MMWindow) ServiceLocator.GetService<UIService>().Swap<IMMWindow>()).GoToNode(node);
  }

  public void CallSelectedFastTravelPoint() => fastTravelInfoView?.Travel();

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
    RectTransform parent = AnchorByKind(kind);
    instance = Instantiate(mindMapImagePrefab, parent, false);
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
    customMarker = instance;
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
    RectTransform parent = AnchorByKind(kind);
    instance = Instantiate(kind == ImageKind.Overlay ? overlayImagePrefab : imagePrefab, parent, false);
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
      playerMarker = instance;
    }
    else
    {
      if (!(sprite.name == "custom_marker"))
        return;
      customMarker = instance;
    }
  }

  public void HideFastTravelPointInfo()
  {
    ServiceLocator.GetService<MapService>().Current = null;
    fastTravelInfoView?.Hide();
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, FastTravelListener);
  }

  public void HideInfo(MapItemView itemView)
  {
    ServiceLocator.GetService<MapService>().Current = null;
    if (itemView.IsRegion)
      return;
    infoWindow.Hide(itemView);
  }

  private static int ItemComparison(IMapItem x, IMapItem y)
  {
    return (x.Resource != null ? x.Resource.Kind : MapPlaceholderKind.Region).CompareTo((MapPlaceholderKind) (y.Resource != null ? (int) y.Resource.Kind : 0));
  }

  private Material MaterialByKind(ImageKind kind)
  {
    switch (kind)
    {
      case ImageKind.Region:
        return regionMaterial;
      case ImageKind.Shadow:
        return shadowMaterial;
      case ImageKind.Building:
      case ImageKind.ShopBuilding:
      case ImageKind.SpecialBuilding:
        return buildingMaterial;
      case ImageKind.Normal:
        return normalMaterial;
      default:
        return null;
    }
  }

  private void Build2()
  {
    if (baseAnchorRect == null)
      return;
    Rect rect1 = new Rect();
    rect1.min = worldLeftBottom;
    rect1.max = worldRightTop;
    Rect rectangle = rect1;
    rect1 = new Rect();
    rect1.xMin = Math.Min(rectangle.xMin, rectangle.xMax);
    rect1.xMax = Math.Max(rectangle.xMin, rectangle.xMax);
    rect1.yMin = Math.Min(rectangle.yMin, rectangle.yMax);
    rect1.yMax = Math.Max(rectangle.yMin, rectangle.yMax);
    Rect rect2 = rect1;
    startingPosition = Vector2.zero;
    startingScroll = 0.0f;
    float num1 = baseAnchorRect.sizeDelta.x / rect2.width;
    float num2 = Vector2.Angle(rect2.min - rect2.center, rectangle.min - rectangle.center);
    List<MMTooltip> source = new List<MMTooltip>();
    mapService = ServiceLocator.GetService<MapService>();
    float size = mapService.FastTravelOrigin != null ? nodeSize * 0.5f : nodeSize;
    foreach (IMapItem mapItem1 in mapService.Items)
    {
      MapPlaceholder resource = mapItem1.Resource;
      Vector2 worldPosition = mapItem1.WorldPosition;
      if (resource != null)
      {
        if (resource.Kind == MapPlaceholderKind.Building || resource.Kind == MapPlaceholderKind.ShopBuilding || resource.Kind == MapPlaceholderKind.SpecialBuilding)
        {
          Sprite shadowSprite = resource.ShadowSprite;
          if (shadowSprite != null)
            CreateImage(rectangle, worldPosition, num2 - mapItem1.Rotation, ImageKind.Shadow, shadowSprite, num1 / shadowResolution, false, null);
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
            if (hoverSprite != null)
              CreateImage(rectangle, worldPosition, num2 - mapItem1.Rotation, kind, hoverSprite, num1 / shadowResolution, false, null);
          }
        }
        Sprite mainSprite = resource.MainSprite;
        if (mainSprite != null)
        {
          float resolution = 1f;
          switch (resource.Kind)
          {
            case MapPlaceholderKind.Region:
              resolution = num1 / regionResolution;
              break;
            case MapPlaceholderKind.Building:
            case MapPlaceholderKind.ShopBuilding:
            case MapPlaceholderKind.SpecialBuilding:
              resolution = num1 / mainResolution;
              break;
          }
          ImageKind kind = ImageKind.Overlay;
          IMapItem mapItem2 = resource.Kind == MapPlaceholderKind.Region || mapItem1.Title != LocalizedText.Empty ? mapItem1 : null;
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
          CreateImage(rectangle, worldPosition, flag ? 0.0f : num2 - mapItem1.Rotation, kind, mainSprite, resolution, resource.AlphaRaycast, mapItem2);
        }
        if (resource.Kind == MapPlaceholderKind.Building || resource.Kind == MapPlaceholderKind.ShopBuilding || resource.Kind == MapPlaceholderKind.SpecialBuilding)
        {
          Sprite normalSprite = resource.NormalSprite;
          if (normalSprite != null)
            CreateImage(rectangle, worldPosition, num2 - mapItem1.Rotation, ImageKind.Normal, normalSprite, num1 / normalResolution, false, null);
        }
      }
      if (mapItem1.TooltipResource != null)
        source.Add(new MMTooltip(mapItem1.TooltipResource, mapItem1.TooltipText));
      foreach (IMMNode node in mapItem1.Nodes)
        source.Add(new MMTooltip(node));
      int num3 = source.Count();
      if (num3 == 1)
      {
        MMTooltip tooltip = source.First();
        CreateMindMapNodeImage(rectangle, worldPosition, size, tooltip);
      }
      else if (num3 > 1)
      {
        float num4 = (float) (ExternalSettingsInstance<ExternalGraphicsSettings>.Instance.MindMapIconSize * num3 / 2.0 * 3.1415927410125732);
        float num5 = 6.28318548f / num3;
        int num6 = 0;
        foreach (MMTooltip tooltip in source)
        {
          Vector2 vector2 = new Vector2(Mathf.Cos(num5 * num6) * num4, Mathf.Sin(num5 * num6) * num4);
          CreateMindMapNodeImage(rectangle, worldPosition + vector2, size, tooltip);
          ++num6;
        }
      }
      source.Clear();
      FastTravelPointEnum fastTravelPointEnum = mapService.FastTravelOrigin != null ? mapItem1.FastTravelPoint.Value : FastTravelPointEnum.None;
      if (fastTravelPointEnum != FastTravelPointEnum.None && fastTravelPointEnum != mapService.FastTravelOrigin.FastTravelPointIndex.Value)
        CreateFastTravelImage(rectangle, worldPosition, nodeSize, mapItem1);
      if (mapService.FocusedItem == mapItem1)
      {
        Vector2 normalized = Rect.PointToNormalized(rectangle, worldPosition);
        Vector2 sizeDelta = baseAnchorRect.sizeDelta;
        normalized.x *= sizeDelta.x;
        normalized.y *= sizeDelta.y;
        startingPosition = contentRect.InverseTransformPoint(baseAnchorRect.TransformPoint(normalized));
        startingScroll = onNodeScalePower;
      }
    }
    mapService.FocusedItem = null;
    mapService.FocusedNode = null;
    IMapItemComponent component1 = mapService.CustomMarker?.GetComponent<IMapItemComponent>();
    customMarkerEnabledView.Visible = component1 != null && component1.IsEnabled;
  }

  protected override void OnEnable()
  {
    base.OnEnable();
    scroll = float.NaN;
    Build2();
    MapService service = ServiceLocator.GetService<MapService>();
    if (service.BullModeForced)
    {
      startingScroll = -1f;
      service.BullModeForced = false;
    }
    Scroll = startingScroll;
    scrollVelocity = 0.0f;
    desiredScroll = startingScroll;
    float num = Mathf.Pow(4f, Mathf.Max(0.0f, startingScroll));
    Vector2 vector2_1 = (contentRect.sizeDelta * num - contentRect.sizeDelta) * 0.5f;
    if (vector2_1.x < 0.0)
      vector2_1.x = 0.0f;
    if (vector2_1.y < 0.0)
      vector2_1.y = 0.0f;
    cursorMapPosition = startingPosition;
    Vector2 vector2_2 = -num * startingPosition;
    if (vector2_2.x < -(double) vector2_1.x)
      vector2_2.x = -vector2_1.x;
    else if (vector2_2.x > (double) vector2_1.x)
      vector2_2.x = vector2_1.x;
    if (vector2_2.y < -(double) vector2_1.y)
      vector2_2.y = -vector2_1.y;
    else if (vector2_2.y > (double) vector2_1.y)
      vector2_2.y = vector2_1.y;
    contentRect.anchoredPosition = vector2_2;
    lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
    ServiceLocator.GetService<CameraService>().Kind = CameraKindEnum.Unknown;
    CursorService.Instance.Free = CursorService.Instance.Visible = true;
    InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
    PlayerUtility.ShowPlayerHands(false);
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.GenericPlayerMenu, new GameActionHandle(((UIWindow) this).CancelListener), true);
    ServiceLocator.GetService<NotificationService>().RemoveNotify(NotificationEnum.Map);
    SoundUtility.PlayAudioClip2D(windowOpenSound, mixer, 1f, 0.0f, context: gameObject.GetFullName());
    hasCustomMarkerView.Visible = service.CustomMarker != null;
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickUp, MoveOnNodes);
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickDown, MoveOnNodes);
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickLeft, MoveOnNodes);
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickRight, MoveOnNodes);
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, SwapToMMap);
    rayCaster = GetComponent<GraphicRaycaster>();
    pointerData = new PointerEventData(EventSystem.current)
    {
      position = consoleCursor.transform.position
    };
    List<MapNodeView> mapNodeViewList = new List<MapNodeView>(contentRect.GetComponentsInChildren<MapNodeView>());
    cursorCenter = consoleCursor.transform.position;
    SelectNode();
  }

  protected override void OnDisable()
  {
    ServiceLocator.GetService<CameraService>().Kind = lastCameraKind;
    PlayerUtility.ShowPlayerHands(true);
    InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Map, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.GenericPlayerMenu, new GameActionHandle(((UIWindow) this).CancelListener));
    Clear();
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickUp, MoveOnNodes);
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickDown, MoveOnNodes);
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickLeft, MoveOnNodes);
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickRight, MoveOnNodes);
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, SwapToMMap);
    ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, FastTravelListener);
    consoleCursor.transform.position = cursorCenter;
    cursorMapPosition = contentRect.InverseTransformPoint(cursorCenter);
    base.OnDisable();
  }

  private bool FastTravelListener(GameActionType type, bool down)
  {
    if (!(type == GameActionType.Submit & down))
      return false;
    CallSelectedFastTravelPoint();
    return true;
  }

  private bool SwapToMMap(GameActionType type, bool down)
  {
    if (!InputService.Instance.JoystickUsed || !down || !(hovered != null) || !(hovered is MapNodeView))
      return false;
    CallMindMap(((MapNodeView) hovered).GetNode());
    return true;
  }

  private bool MoveOnNodes(GameActionType type, bool down) => false;

  private void NavigateNode(Vector3 dir)
  {
    nodes = new List<GameObject>(contentRect.GetComponentsInChildren<MapNodeView>().Select(n => n.gameObject));
    nodes.Add(playerMarker);
    List<GameObject> collection = new List<GameObject>(contentRect.GetComponentsInChildren<MapFastTravelPointView>().Select(node => node.gameObject));
    if (collection.Count > 0)
      nodes.AddRange(collection);
    if (customMarkerEnabledView.Visible)
      nodes.Add(customMarker);
    GameObject gameObject = UISelectableHelper.Select(nodes, consoleCursor, dir);
    if (gameObject == null)
      return;
    cursorMapPosition = contentRect.InverseTransformPoint(gameObject.gameObject.transform.position);
    consoleCursor.transform.position = contentRect.TransformPoint(cursorMapPosition);
    pointerData = new PointerEventData(EventSystem.current)
    {
      position = consoleCursor.transform.position
    };
    SelectNode();
  }

  private void SelectNode()
  {
    if (pointerData == null)
      return;
    if (bull != null && bull.Progress != 0.0)
    {
      ((IPointerExitHandler) newHovered)?.OnPointerExit(pointerData);
      ((IPointerExitHandler) newHoveredRegion)?.OnPointerExit(pointerData);
    }
    else
    {
      List<RaycastResult> raycastResultList = new List<RaycastResult>();
      rayCaster.Raycast(pointerData, raycastResultList);
      newHovered = null;
      newHoveredRegion = null;
      foreach (RaycastResult raycastResult in raycastResultList)
      {
        MonoBehaviour component = raycastResult.gameObject?.GetComponent<MapItemView>();
        if (component != null && ((MapItemView) component).IsRegion)
          newHoveredRegion = component;
        else if (!(newHovered != null))
        {
          if (component == null)
            component = raycastResult.gameObject?.GetComponent<MapNodeView>();
          if (component == null)
            component = raycastResult.gameObject?.GetComponent<MapFastTravelPointView>();
          if (component != null)
          {
            if (component is MapNodeView)
              bufferedNode = ((MapNodeView) component).GetNode();
            newHovered = component;
          }
        }
      }
      if (newHovered != hovered)
      {
        ((IPointerExitHandler) hovered)?.OnPointerExit(pointerData);
        ((IPointerEnterHandler) newHovered)?.OnPointerEnter(pointerData);
        hovered = newHovered;
      }
      if (!(newHoveredRegion != hoveredRegion))
        return;
      ((IPointerExitHandler) hoveredRegion)?.OnPointerExit(pointerData);
      ((IPointerEnterHandler) newHoveredRegion)?.OnPointerEnter(pointerData);
      hoveredRegion = newHoveredRegion;
    }
  }

  private void UpdateNavigation()
  {
    if (!InputService.Instance.JoystickUsed)
      return;
    Vector2 dir = new Vector2(InputService.Instance.GetAxis("RightStickX"), -InputService.Instance.GetAxis("RightStickY"));
    if (!analogNavigationCooldown && dir.sqrMagnitude > 0.64000004529953)
    {
      analogNavigationCooldown = true;
      dir.Normalize();
      NavigateNode(dir);
    }
    else if (analogNavigationCooldown && dir.sqrMagnitude < 0.64000004529953)
      analogNavigationCooldown = false;
    float scale = ScrollToScale(Scroll);
    Vector2 zero = Vector2.zero;
    float axis1 = InputService.Instance.GetAxis("LeftStickX");
    float num1 = (xPrev + axis1) * Time.deltaTime;
    xPrev = axis1;
    float axis2 = InputService.Instance.GetAxis("LeftStickY");
    float num2 = (yPrev + axis2) * Time.deltaTime;
    yPrev = axis2;
    if (num1 != 0.0 || num2 != 0.0)
    {
      currentAcceleration = 3f;
      if (currentAcceleration >= 1.0)
      {
        num1 *= currentAcceleration;
        num2 *= currentAcceleration;
      }
      float num3 = num1 * (ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity / scale);
      float num4 = num2 * (ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity / scale);
      cursorMapPosition.x += num3;
      cursorMapPosition.y -= num4;
      cursorMapPosition.x = Mathf.Clamp(cursorMapPosition.x, (float) (-(double) contentRect.sizeDelta.x * 0.5), contentRect.sizeDelta.x * 0.5f);
      cursorMapPosition.y = Mathf.Clamp(cursorMapPosition.y, (float) (-(double) contentRect.sizeDelta.y * 0.5), contentRect.sizeDelta.y * 0.5f);
    }
    else
      currentAcceleration = 0.0f;
    RectTransform transform = (RectTransform) consoleCursor.transform;
    if (transform.anchoredPosition.sqrMagnitude > 1.0)
    {
      Vector2 vector2_1 = contentRect.anchoredPosition - transform.anchoredPosition;
      Vector2 vector2_2 = new Vector2((float) (contentRect.sizeDelta.x * (scale - 1.0) * 0.5), (float) (contentRect.sizeDelta.y * (scale - 1.0) * 0.5));
      if (vector2_2.x < 0.0)
        vector2_2.x = 0.0f;
      if (vector2_2.y < 0.0)
        vector2_2.y = 0.0f;
      if (vector2_1.x < -(double) vector2_2.x)
        vector2_1.x = -vector2_2.x;
      if (vector2_1.x > (double) vector2_2.x)
        vector2_1.x = vector2_2.x;
      if (vector2_1.y < -(double) vector2_2.y)
        vector2_1.y = -vector2_2.y;
      if (vector2_1.y > (double) vector2_2.y)
        vector2_1.y = vector2_2.y;
      contentRect.anchoredPosition += (vector2_1 - contentRect.anchoredPosition) * Mathf.MoveTowards(0.0f, 1f, 5f * Time.deltaTime);
    }
  }

  private void LateUpdate()
  {
    if (!consoleCursor.activeSelf)
      return;
    Vector3 vector3 = contentRect.TransformPoint(cursorMapPosition);
    consoleCursor.transform.position = vector3;
    if (vector3.sqrMagnitude > 0.0)
    {
      pointerData = new PointerEventData(EventSystem.current)
      {
        position = consoleCursor.transform.position
      };
      SelectNode();
    }
  }

  private bool IsOutOfArea(Vector3 maxPos, Vector3 anchorPos)
  {
    bool flag = false;
    if (anchorPos.x < -(double) maxPos.x)
    {
      if (anchorPos.y < -(double) maxPos.y)
        flag = true;
      else if (anchorPos.y > (double) maxPos.y)
        flag = true;
    }
    else if (anchorPos.x > (double) maxPos.x)
    {
      if (anchorPos.y < -(double) maxPos.y)
        flag = true;
      else if (anchorPos.y > (double) maxPos.y)
        flag = true;
    }
    else if (anchorPos.y < -(double) maxPos.y)
    {
      if (anchorPos.x < -(double) maxPos.x)
        flag = true;
      else if (anchorPos.x > (double) maxPos.y)
        flag = true;
    }
    else if (anchorPos.y > (double) maxPos.y)
    {
      if (anchorPos.x < -(double) maxPos.x)
        flag = true;
      else if (anchorPos.x > (double) maxPos.y)
        flag = true;
    }
    return flag;
  }

  protected override void OnJoystick(bool joystick)
  {
    base.OnJoystick(joystick);
    CursorService.Instance.Visible = !joystick;
    controlPanel.SetActive(joystick);
    consoleCursor.SetActive(joystick);
    EventSystem.current.SetSelectedGameObject(null);
    if (hovered != null)
    {
      ((IPointerExitHandler) hovered)?.OnPointerExit(pointerData);
      hovered = null;
    }
    if (newHovered != null)
    {
      ((IPointerExitHandler) newHovered)?.OnPointerExit(pointerData);
      newHovered = null;
    }
    if (joystick)
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Map, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
      SelectNode();
    }
    else
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Map, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
  }

  public void ShowInfo(MapItemView itemView)
  {
    if (itemView.Item is IComponent component)
      ServiceLocator.GetService<MapService>().Current = component.Owner;
    if (itemView.IsRegion)
      regionInfoWindow.Show(itemView);
    else
      infoWindow.Show(itemView);
  }

  public void ShowFastTravelPointInfo(MapFastTravelPointView pointView)
  {
    ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, FastTravelListener);
    MapService service = ServiceLocator.GetService<MapService>();
    if (pointView.Item is IComponent component)
      service.Current = component.Owner;
    fastTravelInfoView?.Show(service.FastTravelOrigin, pointView);
  }

  private void Update()
  {
    float num1 = Input.GetAxis("MouseWheel") * ExternalSettingsInstance<ExternalInputSettings>.Instance.WheelMouseSensitivity;
    if (num1 == 0.0)
    {
      float axis1 = InputService.Instance.GetAxis("LT");
      float axis2 = InputService.Instance.GetAxis("RT");
      if (axis1 != 0.0)
        num1 = -axis1 * ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickScaleSensitivity;
      else if (axis2 != 0.0)
        num1 = axis2 * ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickScaleSensitivity;
    }
    if (num1 < 0.0)
      desiredScroll = Scroll > 0.0 || !ServiceLocator.GetService<MapService>().BullModeAvailable ? Mathf.Clamp01(desiredScroll + num1) : -1f;
    else if (num1 > 0.0)
      desiredScroll = desiredScroll >= 0.0 ? Mathf.Clamp01(desiredScroll + num1) : 0.0f;
    if (Scroll != (double) desiredScroll)
    {
      float scale1 = ScrollToScale(Scroll);
      Scroll = Mathf.MoveTowards(Mathf.SmoothDamp(Scroll, desiredScroll, ref scrollVelocity, 0.15f), desiredScroll, Time.deltaTime * 0.01f);
      float scale2 = ScrollToScale(Scroll);
      if (InputService.Instance.JoystickUsed ? !InputService.Instance.GetButton("A", false) : !Input.GetMouseButton(0))
      {
        float num2 = scale2 / scale1;
        Vector2 anchoredPosition = contentRect.anchoredPosition;
        Vector2 vector2_1 = contentRect.parent.InverseTransformPoint(InputService.Instance.JoystickUsed ? consoleCursor.transform.position : Input.mousePosition);
        Vector2 vector2_2 = (anchoredPosition - vector2_1) * num2;
        contentRect.anchoredPosition = vector2_1 + vector2_2;
      }
      if (!InputService.Instance.JoystickUsed)
      {
        Vector2 anchoredPosition = contentRect.anchoredPosition;
        Vector2 vector2 = new Vector2((float) ((1920.0 * scale2 - 1920.0) * 0.5), (float) ((1080.0 * scale2 - 886.0) * 0.5));
        if (vector2.x < 0.0)
          vector2.x = 0.0f;
        if (vector2.y < 0.0)
          vector2.y = 0.0f;
        if (anchoredPosition.x < -(double) vector2.x)
          anchoredPosition.x = -vector2.x;
        if (anchoredPosition.x > (double) vector2.x)
          anchoredPosition.x = vector2.x;
        if (anchoredPosition.y < -(double) vector2.y)
          anchoredPosition.y = -vector2.y;
        if (anchoredPosition.y > (double) vector2.y)
          anchoredPosition.y = vector2.y;
        contentRect.anchoredPosition = anchoredPosition;
      }
    }
    UpdateNavigation();
    if (!Input.GetMouseButtonDown(1) && !InputService.Instance.GetButtonDown("Y", false))
      return;
    OnCustomMarkerButton();
  }

  public override void Initialize()
  {
    RegisterLayer<IMapWindow>(this);
    base.Initialize();
  }

  public override Type GetWindowType() => typeof (IMapWindow);

  public Vector3 GetWorldPosition(Vector3 mousePosition)
  {
    Vector2 sizeDelta = baseAnchorRect.sizeDelta;
    Rect rect = new Rect {
      min = worldLeftBottom,
      max = worldRightTop
    };
    mousePosition = (Vector2) baseAnchorRect.InverseTransformPoint(mousePosition);
    mousePosition.x /= sizeDelta.x;
    mousePosition.y /= sizeDelta.y;
    mousePosition.x = mousePosition.x * rect.width + rect.x;
    mousePosition.y = mousePosition.y * rect.height + rect.y;
    return mousePosition;
  }

  public Vector2 GetScreenPosition(Vector3 worldPos)
  {
    Vector2 point = new Vector2(worldPos.x, worldPos.z);
    Vector2 sizeDelta = baseAnchorRect.sizeDelta;
    Vector2 normalized = Rect.PointToNormalized(new Rect {
      min = worldLeftBottom,
      max = worldRightTop
    }, point);
    normalized.x *= sizeDelta.x;
    normalized.y *= sizeDelta.y;
    Vector2 screenPosition = baseAnchorRect.TransformPoint(normalized);
    screenPosition.y = Screen.height - screenPosition.y;
    return screenPosition;
  }

  private static float ScrollToScale(float scroll) => Mathf.Pow(4f, Mathf.Clamp01(scroll));

  private void OnCustomMarkerButton()
  {
    if (!hasCustomMarkerView.Visible)
      return;
    if (customMarkerEnabledView.Visible)
      RemoveCustomMarker();
    else
      CreateCustomMarker();
  }

  private void CreateCustomMarker()
  {
    IEntity customMarker = ServiceLocator.GetService<MapService>()?.CustomMarker;
    if (customMarker == null)
      return;
    MapCustomMarkerComponent component1 = customMarker.GetComponent<MapCustomMarkerComponent>();
    if (component1 != null)
      component1.Position = !InputService.Instance.JoystickUsed ? GetWorldPosition(Input.mousePosition) : (Vector2) GetWorldPosition(consoleCursor.transform.position);
    IMapItemComponent component2 = customMarker.GetComponent<IMapItemComponent>();
    if (component2 != null)
      component2.IsEnabled = true;
    OnCustomMarkerChange();
  }

  private void RemoveCustomMarker()
  {
    IMapItemComponent component = ServiceLocator.GetService<MapService>()?.CustomMarker?.GetComponent<IMapItemComponent>();
    if (component == null)
      return;
    component.IsEnabled = false;
    OnCustomMarkerChange();
  }

  private void OnCustomMarkerChange()
  {
    Clear();
    Build2();
    ApplyScale();
  }

  public override IEnumerator OnOpened()
  {
    SimplePlayerWindowSwapper.SetLastOpenedPlayerWindow<IMapWindow>(this);
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
