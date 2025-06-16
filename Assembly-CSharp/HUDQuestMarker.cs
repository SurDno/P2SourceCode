using Engine.Common.MindMap;
using Engine.Impl.MindMap;
using Engine.Impl.UI.Controls;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;
using UnityEngine.UI;

public class HUDQuestMarker : MonoBehaviour
{
  [SerializeField]
  private ProgressView forwardDotView;
  [SerializeField]
  private ProgressView rightDotView;
  [SerializeField]
  private FloatView distanceView;
  [SerializeField]
  private RawImage image;
  [SerializeField]
  private RawImage shadowImage;
  [Inspected]
  private IMapItem mapItem;

  public IMapItem MapItem
  {
    get => mapItem;
    set
    {
      if (mapItem == value)
        return;
      mapItem = value;
      ApplyMapItem();
    }
  }

  private void OnEnable()
  {
    ApplyMapItem();
    ApplyPosition();
  }

  private void LateUpdate() => ApplyPosition();

  private void ApplyMapItem()
  {
    if (mapItem == null)
    {
      image.texture = null;
      shadowImage.texture = null;
    }
    else
    {
      Texture texture = null;
      foreach (IMMNode node in mapItem.Nodes)
      {
        if (node.Content?.Placeholder is MMPlaceholder placeholder)
        {
          texture = placeholder.Image.Value;
          break;
        }
      }
      if (texture == null)
        texture = mapItem.TooltipResource is MapTooltipResource tooltipResource ? tooltipResource.Image.Value : null;
      image.texture = texture;
      shadowImage.texture = texture;
    }
  }

  private void ApplyPosition()
  {
    if (mapItem == null)
      return;
    Transform cameraTransform = GameCamera.Instance?.CameraTransform;
    if (cameraTransform == null)
      return;
    Vector3 position = cameraTransform.position;
    Vector2 vector2 = mapItem.WorldPosition - new Vector2(position.x, position.z);
    float magnitude = vector2.magnitude;
    Vector2 lhs = vector2 / magnitude;
    Vector3 forward = cameraTransform.forward;
    Vector2 rhs1 = new Vector2(forward.x, forward.z);
    rhs1.Normalize();
    forwardDotView.Progress = Mathf.Clamp01(Vector2.Dot(lhs, rhs1));
    Vector2 rhs2 = new Vector2(rhs1.y, -rhs1.x);
    rightDotView.Progress = (float) ((Vector2.Dot(lhs, rhs2) + 1.0) / 2.0);
    distanceView.FloatValue = magnitude;
  }
}
