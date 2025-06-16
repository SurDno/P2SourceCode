using System;
using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using UnityEngine;

public class NPCFlamethrowerWeaponController : INPCWeaponController
{
  private Animator animator;
  private NPCWeaponService service;
  private float layersWeight;
  private int walkLayerIndex;
  private int attackLayerIndex;
  private int reactionLayerIndex;
  protected IEntity item;
  protected AnimatorState45 animatorState;
  private PivotSanitar pivotSanitar;

  public event Action<IEntity, ShotType, ReactionType> WeaponShootEvent;

  public void Initialise(NPCWeaponService service)
  {
    this.service = service;
    animator = service.gameObject.GetComponent<Pivot>().GetAnimator();
    pivotSanitar = service.gameObject.GetComponent<PivotSanitar>();
    IndoorChanged();
    animatorState = AnimatorState45.GetAnimatorState(animator);
    if (animator != null)
    {
      walkLayerIndex = animator.GetLayerIndex("Fight Flamethrower Walk Layer");
      attackLayerIndex = animator.GetLayerIndex("Fight Attack Layer");
      reactionLayerIndex = animator.GetLayerIndex("Fight Empty Reaction Layer");
    }
    SetLayers(0.0f);
  }

  public void IndoorChanged()
  {
    if (!(pivotSanitar != null) || !(service != null))
      return;
    pivotSanitar.IsIndoor = service.IsIndoor;
  }

  public void SetItem(IEntity item) => this.item = item;

  public void Activate() => SetLayers(1f);

  private void SetLayers(float weight)
  {
    if (walkLayerIndex != -1)
      service.AddNeededLayer(walkLayerIndex, weight);
    if (attackLayerIndex != -1)
      service.AddNeededLayer(attackLayerIndex, weight);
    if (reactionLayerIndex == -1)
      return;
    service.AddNeededLayer(reactionLayerIndex, weight * 0.5f);
  }

  public void FirstLaunch()
  {
  }

  public void Shutdown()
  {
    if (pivotSanitar != null)
      pivotSanitar.Flamethrower = false;
    SetLayers(0.0f);
  }

  public void ActivateImmediate()
  {
    layersWeight = 1f;
    SetLayers(1f, true);
  }

  public void ShutdownImmediate()
  {
    layersWeight = 0.0f;
    SetLayers(0.0f, true);
    if (!(pivotSanitar != null))
      return;
    pivotSanitar.Flamethrower = false;
  }

  public bool IsChangingWeapon() => false;

  public bool Validate(GameObject gameObject) => throw new NotImplementedException();

  public void Update()
  {
    layersWeight = Mathf.MoveTowards(layersWeight, 1f, Time.deltaTime * 5f);
  }

  public void UpdateSilent()
  {
    layersWeight = Mathf.MoveTowards(layersWeight, 0.0f, Time.deltaTime / 0.5f);
  }

  protected void SetLayers(float weight, bool immediate = false)
  {
    if (service == null)
      return;
    if (walkLayerIndex != -1)
      service.AddNeededLayer(walkLayerIndex, weight);
    if (attackLayerIndex != -1)
      service.AddNeededLayer(attackLayerIndex, weight);
    if (reactionLayerIndex != -1)
      service.AddNeededLayer(reactionLayerIndex, weight);
    if (!immediate)
      return;
    service.ForceUpdateLayers();
  }

  private void ApplyLayerWeights()
  {
    if (walkLayerIndex != -1)
      animator?.SetLayerWeight(walkLayerIndex, layersWeight);
    if (attackLayerIndex != -1)
      animator?.SetLayerWeight(attackLayerIndex, layersWeight);
    if (reactionLayerIndex == -1)
      return;
    animator?.SetLayerWeight(reactionLayerIndex, layersWeight * service.ReactionLayerWeight);
  }

  public void TriggerAction(WeaponActionEnum weaponAction)
  {
    animatorState.SetTrigger("Fight.Triggers/Attack");
    animator.SetInteger("Fight.AttackType", 0);
  }

  public void OnAnimatorEvent(string data)
  {
    if (!data.StartsWith("Flamethrower.Punch"))
      return;
    Action<IEntity, ShotType, ReactionType> weaponShootEvent = WeaponShootEvent;
    if (weaponShootEvent != null)
      weaponShootEvent(item, ShotType.Moderate, ReactionType.Front);
  }

  public void PunchReaction(ReactionType reactionType)
  {
  }
}
