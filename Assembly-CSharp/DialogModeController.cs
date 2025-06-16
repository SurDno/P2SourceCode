// Decompiled with JetBrains decompiler
// Type: DialogModeController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.CameraServices;
using UnityEngine;

#nullable disable
public struct DialogModeController
{
  private CameraKindEnum lastCameraKind;
  private IEntity lastCameraTarget;
  private AnimatorUpdateMode lastUpdateMode;
  private AnimatorCullingMode lastCullingMode;
  private bool wasKinematic;

  public CameraKindEnum TargetCameraKind { get; set; }

  public void EnableCameraKind(IEntity target)
  {
    this.lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
    this.lastCameraTarget = ServiceLocator.GetService<CameraService>().Target;
    ServiceLocator.GetService<CameraService>().Kind = this.TargetCameraKind;
    ServiceLocator.GetService<CameraService>().Target = target;
  }

  public void DisableCameraKind()
  {
    ServiceLocator.GetService<CameraService>().Kind = this.lastCameraKind;
    ServiceLocator.GetService<CameraService>().Target = this.lastCameraTarget;
  }

  public void SetDialogMode(IEntity target, bool enabled)
  {
    GameObject gameObject = ((IEntityView) target)?.GameObject;
    if ((Object) gameObject == (Object) null)
      return;
    this.SetDialogLayerWeight(gameObject, enabled);
    this.SetKinematic(gameObject, enabled);
  }

  private void SetDialogLayerWeight(GameObject target, bool enabled)
  {
    Animator animator = target.GetComponent<Pivot>()?.GetAnimator();
    if (!((Object) animator != (Object) null))
      return;
    animator.enabled = true;
    if (enabled)
    {
      this.lastCullingMode = animator.cullingMode;
      this.lastUpdateMode = animator.updateMode;
      animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
      animator.updateMode = AnimatorUpdateMode.Normal;
      animator.SetLayerWeight(animator.GetLayerIndex("Dialog Layer"), 1f);
    }
    else
    {
      animator.cullingMode = this.lastCullingMode;
      animator.updateMode = this.lastUpdateMode;
      animator.SetLayerWeight(animator.GetLayerIndex("Dialog Layer"), 0.0f);
    }
  }

  private void SetKinematic(GameObject target, bool kinematic)
  {
    Rigidbody component = target.GetComponent<Rigidbody>();
    if (!((Object) component != (Object) null))
      return;
    if (kinematic)
    {
      this.wasKinematic = component.isKinematic;
      component.isKinematic = kinematic;
    }
    else
      component.isKinematic = this.wasKinematic;
  }
}
