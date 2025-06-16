using System;
using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using UnityEngine;

public class NPCEmptyWeaponController : INPCWeaponController
{
  private Animator animator;
  private NPCWeaponService service;
  private float layersWeight;
  private int reactionLayerIndex;

  public event Action<IEntity, ShotType, ReactionType> WeaponShootEvent;

  public void Initialise(NPCWeaponService service)
  {
    this.service = service;
    animator = service.gameObject.GetComponent<Pivot>().GetAnimator();
    if (!(animator != null))
      return;
    reactionLayerIndex = animator.GetLayerIndex("Fight Empty Reaction Layer");
  }

  public void IndoorChanged()
  {
  }

  private void SetLayers(float weight, bool immediate = false)
  {
    if (service == null)
      return;
    service.AddNeededLayer(reactionLayerIndex, weight);
    if (!immediate)
      return;
    service.ForceUpdateLayers();
  }

  public void Activate() => SetLayers(1f);

  public void Shutdown() => SetLayers(0.0f);

  public void ActivateImmediate()
  {
    layersWeight = 1f;
    SetLayers(1f);
  }

  public void ShutdownImmediate()
  {
    layersWeight = 0.0f;
    SetLayers(0.0f);
  }

  public bool IsChangingWeapon() => false;

  public bool Validate(GameObject gameObject) => throw new NotImplementedException();

  public void Update()
  {
    layersWeight = Mathf.MoveTowards(layersWeight, 1f, Time.deltaTime / 0.5f);
  }

  public void UpdateSilent()
  {
    layersWeight = Mathf.MoveTowards(layersWeight, 0.0f, Time.deltaTime / 0.5f);
  }

  private void ApplyLayerWeights()
  {
    if (!(service == null) && service.Weapon == WeaponEnum.Flamethrower)
      return;
    float weight = layersWeight * 0.5f;
    if (reactionLayerIndex != -1)
      animator?.SetLayerWeight(reactionLayerIndex, weight);
  }

  public void TriggerAction(WeaponActionEnum weaponAction)
  {
  }

  public void OnAnimatorEvent(string data)
  {
  }

  public void PunchReaction(ReactionType reactionType)
  {
  }
}
