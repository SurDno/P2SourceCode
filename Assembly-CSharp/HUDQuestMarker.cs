// Decompiled with JetBrains decompiler
// Type: HUDQuestMarker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.MindMap;
using Engine.Impl.MindMap;
using Engine.Impl.UI.Controls;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
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
    get => this.mapItem;
    set
    {
      if (this.mapItem == value)
        return;
      this.mapItem = value;
      this.ApplyMapItem();
    }
  }

  private void OnEnable()
  {
    this.ApplyMapItem();
    this.ApplyPosition();
  }

  private void LateUpdate() => this.ApplyPosition();

  private void ApplyMapItem()
  {
    if (this.mapItem == null)
    {
      this.image.texture = (Texture) null;
      this.shadowImage.texture = (Texture) null;
    }
    else
    {
      Texture texture = (Texture) null;
      foreach (IMMNode node in this.mapItem.Nodes)
      {
        if (node.Content?.Placeholder is MMPlaceholder placeholder)
        {
          texture = placeholder.Image.Value;
          break;
        }
      }
      if ((Object) texture == (Object) null)
        texture = this.mapItem.TooltipResource is MapTooltipResource tooltipResource ? tooltipResource.Image.Value : (Texture) null;
      this.image.texture = texture;
      this.shadowImage.texture = texture;
    }
  }

  private void ApplyPosition()
  {
    if (this.mapItem == null)
      return;
    Transform cameraTransform = GameCamera.Instance?.CameraTransform;
    if ((Object) cameraTransform == (Object) null)
      return;
    Vector3 position = cameraTransform.position;
    Vector2 vector2 = this.mapItem.WorldPosition - new Vector2(position.x, position.z);
    float magnitude = vector2.magnitude;
    Vector2 lhs = vector2 / magnitude;
    Vector3 forward = cameraTransform.forward;
    Vector2 rhs1 = new Vector2(forward.x, forward.z);
    rhs1.Normalize();
    this.forwardDotView.Progress = Mathf.Clamp01(Vector2.Dot(lhs, rhs1));
    Vector2 rhs2 = new Vector2(rhs1.y, -rhs1.x);
    this.rightDotView.Progress = (float) (((double) Vector2.Dot(lhs, rhs2) + 1.0) / 2.0);
    this.distanceView.FloatValue = magnitude;
  }
}
