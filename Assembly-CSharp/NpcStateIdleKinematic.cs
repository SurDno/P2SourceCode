using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Settings.External;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateIdleKinematic : INpcState
{
  private EngineBehavior behavior;
  private NavMeshAgent agent;
  private Rigidbody rigidbody;
  private Pivot pivot;
  private Animator animator;
  private NpcState npcState;
  private NPCWeaponService weaponService;
  private bool agentWasEnabled;
  private bool rigidbodyWasKinematic;
  private AnimatorUpdateMode initialAnimatorUpdateMode;
  private AnimatorCullingMode initialAnimatorCullingMode;
  private AnimatorState45 animatorState;
  private bool inited;
  private bool failed;
  private bool sayReplics;
  private float timeToNextReplic;

  public GameObject GameObject { get; private set; }

  [Inspected]
  public NpcStateStatusEnum Status => NpcStateStatusEnum.Running;

  private bool TryInit()
  {
    if (this.inited)
      return true;
    this.behavior = this.pivot.GetBehavior();
    this.rigidbody = this.pivot.GetRigidbody();
    this.weaponService = this.pivot.GetNpcWeaponService();
    this.agent = this.pivot.GetAgent();
    this.animator = this.pivot.GetAnimator();
    if ((Object) this.animator == (Object) null)
    {
      Debug.LogError((object) ("Null animator " + this.GameObject.name), (Object) this.GameObject);
      Debug.LogError((object) ("Null animator " + this.GameObject.GetFullName()));
      this.failed = true;
      return false;
    }
    this.animatorState = AnimatorState45.GetAnimatorState(this.animator);
    this.failed = false;
    this.inited = true;
    return true;
  }

  public NpcStateIdleKinematic(NpcState npcState, Pivot pivot)
  {
    this.npcState = npcState;
    this.pivot = pivot;
    this.GameObject = npcState.gameObject;
  }

  public void Activate(float primaryIdleProbability, bool makeObstacle = false)
  {
    if (!this.TryInit())
      return;
    MovementControllerUtility.SetRandomAnimation(this.animator, this.pivot.SecondaryIdleAnimationCount, this.pivot.SecondaryLowIdleAnimationCount);
    if ((Object) this.agent != (Object) null)
    {
      this.agentWasEnabled = this.agent.enabled;
      this.agent.enabled = false;
    }
    if ((Object) this.rigidbody != (Object) null)
    {
      this.rigidbodyWasKinematic = this.rigidbody.isKinematic;
      this.rigidbody.isKinematic = true;
    }
    this.animatorState.ControlMovableState = AnimatorState45.MovableState45.Idle;
    this.animatorState.PrimaryIdleProbability = primaryIdleProbability;
    this.initialAnimatorUpdateMode = this.animator.updateMode;
    this.animator.updateMode = AnimatorUpdateMode.Normal;
    this.initialAnimatorCullingMode = this.animator.cullingMode;
    this.animator.cullingMode = AnimatorCullingMode.CullCompletely;
    if (makeObstacle)
      this.CreateObstacle();
    if (this.npcState.Owner != null)
    {
      ParametersComponent component = this.npcState.Owner.GetComponent<ParametersComponent>();
      if (component != null)
      {
        IParameter<bool> byName = component.GetByName<bool>(ParameterNameEnum.SayReplicsInIdle);
        if (byName != null)
        {
          this.sayReplics = byName.Value;
          if (this.sayReplics)
            this.timeToNextReplic = Random.Range(ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMin, ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMax);
        }
      }
    }
    if (!((Object) this.weaponService != (Object) null))
      return;
    this.weaponService.Weapon = WeaponEnum.Unknown;
  }

  private void CreateObstacle()
  {
    this.agent.enabled = false;
    NavMeshObstacle obstacle = this.pivot.GetObstacle();
    if (!((Object) obstacle != (Object) null))
      return;
    obstacle.enabled = true;
    obstacle.carving = true;
    obstacle.radius = this.agent.radius;
  }

  public void Shutdown()
  {
    if (this.failed)
      return;
    if ((Object) this.agent != (Object) null)
      this.agent.enabled = this.agentWasEnabled;
    if ((bool) (Object) this.rigidbody)
      this.rigidbody.isKinematic = this.rigidbodyWasKinematic;
    this.animator.updateMode = this.initialAnimatorUpdateMode;
    this.animator.cullingMode = this.initialAnimatorCullingMode;
    NavMeshObstacle obstacle = this.pivot.GetObstacle();
    if ((Object) obstacle != (Object) null)
      obstacle.enabled = false;
    if (!((Object) this.weaponService != (Object) null))
      return;
    this.weaponService.Weapon = this.npcState.Weapon;
  }

  public void OnAnimatorMove()
  {
    if (this.failed)
      return;
    float num = this.animator.updateMode == AnimatorUpdateMode.AnimatePhysics ? Time.fixedDeltaTime : Time.deltaTime;
    this.GameObject.transform.position += this.animator.deltaPosition;
    this.GameObject.transform.rotation *= Quaternion.AngleAxis(57.29578f * this.animator.angularVelocity.y * num, Vector3.up);
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!this.failed)
      ;
  }

  public void Update()
  {
    if (this.failed)
      return;
    if (this.animatorState.ControlMovableState != AnimatorState45.MovableState45.Idle)
      this.animatorState.ControlMovableState = AnimatorState45.MovableState45.Idle;
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused || !this.sayReplics)
      return;
    this.timeToNextReplic -= Time.deltaTime;
    if ((double) this.timeToNextReplic <= 0.0)
    {
      this.timeToNextReplic = Random.Range(ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMin, ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMax);
      NPCStateHelper.SayIdleReplic(this.npcState.Owner);
    }
  }

  public void OnLodStateChanged(bool inLodState)
  {
    this.npcState.AnimatorEnabled = !inLodState;
    EffectsComponent component = this.npcState.Owner?.GetComponent<EffectsComponent>();
    if (component == null)
      return;
    component.Disabled = inLodState;
  }
}
