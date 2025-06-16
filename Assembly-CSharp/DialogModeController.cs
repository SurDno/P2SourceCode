using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.CameraServices;
using UnityEngine;

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
    lastCameraKind = ServiceLocator.GetService<CameraService>().Kind;
    lastCameraTarget = ServiceLocator.GetService<CameraService>().Target;
    ServiceLocator.GetService<CameraService>().Kind = TargetCameraKind;
    ServiceLocator.GetService<CameraService>().Target = target;
  }

  public void DisableCameraKind()
  {
    ServiceLocator.GetService<CameraService>().Kind = lastCameraKind;
    ServiceLocator.GetService<CameraService>().Target = lastCameraTarget;
  }

  public void SetDialogMode(IEntity target, bool enabled)
  {
    GameObject gameObject = ((IEntityView) target)?.GameObject;
    if (gameObject == null)
      return;
    SetDialogLayerWeight(gameObject, enabled);
    SetKinematic(gameObject, enabled);
  }

  private void SetDialogLayerWeight(GameObject target, bool enabled)
  {
    Animator animator = target.GetComponent<Pivot>()?.GetAnimator();
    if (!(animator != null))
      return;
    animator.enabled = true;
    if (enabled)
    {
      lastCullingMode = animator.cullingMode;
      lastUpdateMode = animator.updateMode;
      animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
      animator.updateMode = AnimatorUpdateMode.Normal;
      animator.SetLayerWeight(animator.GetLayerIndex("Dialog Layer"), 1f);
    }
    else
    {
      animator.cullingMode = lastCullingMode;
      animator.updateMode = lastUpdateMode;
      animator.SetLayerWeight(animator.GetLayerIndex("Dialog Layer"), 0.0f);
    }
  }

  private void SetKinematic(GameObject target, bool kinematic)
  {
    Rigidbody component = target.GetComponent<Rigidbody>();
    if (!(component != null))
      return;
    if (kinematic)
    {
      wasKinematic = component.isKinematic;
      component.isKinematic = kinematic;
    }
    else
      component.isKinematic = wasKinematic;
  }
}
