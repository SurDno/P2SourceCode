using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using System;
using UnityEngine;

public class NPCEmptyWeaponController : INPCWeaponController
{
  private Animator animator;
  private NPCWeaponService service;
  private float layersWeight = 0.0f;
  private int reactionLayerIndex;

  public event Action<IEntity, ShotType, ReactionType> WeaponShootEvent;

  public void Initialise(NPCWeaponService service)
  {
    this.service = service;
    this.animator = service.gameObject.GetComponent<Pivot>().GetAnimator();
    if (!((UnityEngine.Object) this.animator != (UnityEngine.Object) null))
      return;
    this.reactionLayerIndex = this.animator.GetLayerIndex("Fight Empty Reaction Layer");
  }

  public void IndoorChanged()
  {
  }

  private void SetLayers(float weight, bool immediate = false)
  {
    if ((UnityEngine.Object) this.service == (UnityEngine.Object) null)
      return;
    this.service.AddNeededLayer(this.reactionLayerIndex, weight);
    if (!immediate)
      return;
    this.service.ForceUpdateLayers();
  }

  public void Activate() => this.SetLayers(1f);

  public void Shutdown() => this.SetLayers(0.0f);

  public void ActivateImmediate()
  {
    this.layersWeight = 1f;
    this.SetLayers(1f);
  }

  public void ShutdownImmediate()
  {
    this.layersWeight = 0.0f;
    this.SetLayers(0.0f);
  }

  public bool IsChangingWeapon() => false;

  public bool Validate(GameObject gameObject) => throw new NotImplementedException();

  public void Update()
  {
    this.layersWeight = Mathf.MoveTowards(this.layersWeight, 1f, Time.deltaTime / 0.5f);
  }

  public void UpdateSilent()
  {
    this.layersWeight = Mathf.MoveTowards(this.layersWeight, 0.0f, Time.deltaTime / 0.5f);
  }

  private void ApplyLayerWeights()
  {
    if (!((UnityEngine.Object) this.service == (UnityEngine.Object) null) && this.service.Weapon == WeaponEnum.Flamethrower)
      return;
    float weight = this.layersWeight * 0.5f;
    if (this.reactionLayerIndex != -1)
      this.animator?.SetLayerWeight(this.reactionLayerIndex, weight);
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
