using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using System;
using UnityEngine;

public class NPCFlamethrowerWeaponController : INPCWeaponController
{
  private Animator animator;
  private NPCWeaponService service;
  private float layersWeight = 0.0f;
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
    this.animator = service.gameObject.GetComponent<Pivot>().GetAnimator();
    this.pivotSanitar = service.gameObject.GetComponent<PivotSanitar>();
    this.IndoorChanged();
    this.animatorState = AnimatorState45.GetAnimatorState(this.animator);
    if ((UnityEngine.Object) this.animator != (UnityEngine.Object) null)
    {
      this.walkLayerIndex = this.animator.GetLayerIndex("Fight Flamethrower Walk Layer");
      this.attackLayerIndex = this.animator.GetLayerIndex("Fight Attack Layer");
      this.reactionLayerIndex = this.animator.GetLayerIndex("Fight Empty Reaction Layer");
    }
    this.SetLayers(0.0f);
  }

  public void IndoorChanged()
  {
    if (!((UnityEngine.Object) this.pivotSanitar != (UnityEngine.Object) null) || !((UnityEngine.Object) this.service != (UnityEngine.Object) null))
      return;
    this.pivotSanitar.IsIndoor = this.service.IsIndoor;
  }

  public void SetItem(IEntity item) => this.item = item;

  public void Activate() => this.SetLayers(1f);

  private void SetLayers(float weight)
  {
    if (this.walkLayerIndex != -1)
      this.service.AddNeededLayer(this.walkLayerIndex, weight);
    if (this.attackLayerIndex != -1)
      this.service.AddNeededLayer(this.attackLayerIndex, weight);
    if (this.reactionLayerIndex == -1)
      return;
    this.service.AddNeededLayer(this.reactionLayerIndex, weight * 0.5f);
  }

  public void FirstLaunch()
  {
  }

  public void Shutdown()
  {
    if ((UnityEngine.Object) this.pivotSanitar != (UnityEngine.Object) null)
      this.pivotSanitar.Flamethrower = false;
    this.SetLayers(0.0f);
  }

  public void ActivateImmediate()
  {
    this.layersWeight = 1f;
    this.SetLayers(1f, true);
  }

  public void ShutdownImmediate()
  {
    this.layersWeight = 0.0f;
    this.SetLayers(0.0f, true);
    if (!((UnityEngine.Object) this.pivotSanitar != (UnityEngine.Object) null))
      return;
    this.pivotSanitar.Flamethrower = false;
  }

  public bool IsChangingWeapon() => false;

  public bool Validate(GameObject gameObject) => throw new NotImplementedException();

  public void Update()
  {
    this.layersWeight = Mathf.MoveTowards(this.layersWeight, 1f, Time.deltaTime * 5f);
  }

  public void UpdateSilent()
  {
    this.layersWeight = Mathf.MoveTowards(this.layersWeight, 0.0f, Time.deltaTime / 0.5f);
  }

  protected void SetLayers(float weight, bool immediate = false)
  {
    if ((UnityEngine.Object) this.service == (UnityEngine.Object) null)
      return;
    if (this.walkLayerIndex != -1)
      this.service.AddNeededLayer(this.walkLayerIndex, weight);
    if (this.attackLayerIndex != -1)
      this.service.AddNeededLayer(this.attackLayerIndex, weight);
    if (this.reactionLayerIndex != -1)
      this.service.AddNeededLayer(this.reactionLayerIndex, weight);
    if (!immediate)
      return;
    this.service.ForceUpdateLayers();
  }

  private void ApplyLayerWeights()
  {
    if (this.walkLayerIndex != -1)
      this.animator?.SetLayerWeight(this.walkLayerIndex, this.layersWeight);
    if (this.attackLayerIndex != -1)
      this.animator?.SetLayerWeight(this.attackLayerIndex, this.layersWeight);
    if (this.reactionLayerIndex == -1)
      return;
    this.animator?.SetLayerWeight(this.reactionLayerIndex, this.layersWeight * this.service.ReactionLayerWeight);
  }

  public void TriggerAction(WeaponActionEnum weaponAction)
  {
    this.animatorState.SetTrigger("Fight.Triggers/Attack");
    this.animator.SetInteger("Fight.AttackType", 0);
  }

  public void OnAnimatorEvent(string data)
  {
    if (!data.StartsWith("Flamethrower.Punch"))
      return;
    Action<IEntity, ShotType, ReactionType> weaponShootEvent = this.WeaponShootEvent;
    if (weaponShootEvent != null)
      weaponShootEvent(this.item, ShotType.Moderate, ReactionType.Front);
  }

  public void PunchReaction(ReactionType reactionType)
  {
  }
}
