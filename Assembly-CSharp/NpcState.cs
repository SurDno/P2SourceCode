// Decompiled with JetBrains decompiler
// Type: NpcState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime;
using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.MessangerStationary;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

#nullable disable
[DisallowMultipleComponent]
public class NpcState : MonoBehaviour, IEntityAttachable
{
  private const int skipFramesBeforeDisablingAnimator = 1;
  private Dictionary<NpcStateEnum, INpcState> states = new Dictionary<NpcStateEnum, INpcState>((IEqualityComparer<NpcStateEnum>) NpcStateEnumComparer.Instance);
  private Pivot pivot;
  private Animator animator;
  private WeaponEnum weapon;
  private const float defaultPrimaryIdleProbability = 0.7f;
  private NpcStateEnum currentNpcState;
  private string currentNpcStateName;
  private static readonly string[] stateNames = Enum.GetNames(typeof (NpcStateEnum));
  private bool animatorEnabled;
  private AnimatorCullingMode initialAnimatorcullingMode;
  private int frameCountFromStart;
  [Inspected]
  private bool inLodState;

  public event Action<WeaponEnum> WeaponChangeEvent;

  public bool RestartBehaviourAfterTeleport { get; set; }

  public bool DisableAnimationAndGeometryAtFarDistance { get; set; }

  public bool NeedExtraExitPOI { get; set; }

  [Inspected]
  public NpcStateEnum CurrentNpcState { get; private set; }

  [Inspected]
  public INpcState CurrentNpcStateInfo
  {
    get
    {
      INpcState npcState;
      return this.states.TryGetValue(this.CurrentNpcState, out npcState) ? npcState : (INpcState) null;
    }
  }

  [Inspected]
  public IEntity Owner { get; private set; }

  [Inspected]
  public bool AnimatorEnabled
  {
    get => this.animatorEnabled;
    set
    {
      this.animatorEnabled = value;
      if (this.frameCountFromStart <= 1 || !(bool) (UnityEngine.Object) this.animator)
        return;
      this.animator.enabled = this.animatorEnabled;
    }
  }

  [Inspected(Mode = ExecuteMode.EditAndRuntime, Mutable = true)]
  public WeaponEnum Weapon
  {
    get => this.weapon;
    set
    {
      this.weapon = value;
      Action<WeaponEnum> weaponChangeEvent = this.WeaponChangeEvent;
      if (weaponChangeEvent == null)
        return;
      weaponChangeEvent(this.weapon);
    }
  }

  public NpcStateStatusEnum Status => this.states[this.CurrentNpcState].Status;

  public void OnLodStateChanged(bool inLodState)
  {
    this.inLodState = inLodState;
    this.states[this.CurrentNpcState].OnLodStateChanged(inLodState);
  }

  private void CheckState(NpcStateEnum state)
  {
    if (this.states.ContainsKey(state))
      return;
    this.states[state] = NpcStateFactory.Create(state, this, this.pivot);
  }

  public void Idle(float primaryIdleProbability = 0.7f, bool makeObstacle = false)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.Idle;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateIdle).Activate(primaryIdleProbability, makeObstacle);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void IdleKinematic(float primaryIdleProbability = 0.7f, bool makeObstacle = false)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.IdleKinematic;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateIdleKinematic).Activate(primaryIdleProbability, makeObstacle);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void IdlePlagueCloud()
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.IdlePlagueCloud;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateIdlePlagueCloud).Activate();
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void InFire()
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.InFire;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateInFire).Activate();
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void Rotate(Transform target)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.Rotate;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateRotate).Activate(target);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void Rotate(Quaternion rotation)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.Rotate;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateRotate).Activate(rotation);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void Move(Vector3 destination, bool failOnPartialPath = false)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.Move;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateMove).Activate(destination, failOnPartialPath);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void MoveCloud(Vector3 destination)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.MoveCloud;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateMoveCloud).Activate(destination);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void MoveByPath(List<Vector3> path)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.MoveByPath;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateMoveByPath).Activate(path);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void MoveByPathCloud(List<Vector3> path)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.MoveByPathCloud;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateMoveByPathCloud).Activate(path);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void MoveRetreat(Transform target, float retreatDistance)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.MoveRetreat;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateMoveRetreat).Activate(target, retreatDistance);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void MoveFollow(Transform target, float followDistance)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.MoveFollow;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateMoveFollow).Activate(target, followDistance);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void MoveFollowTeleport(float followDistance, float trialTime, bool waitForTargetSeesMe)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.MoveFollowTeleport;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateMoveFollowTeleport).Activate(followDistance, trialTime, waitForTargetSeesMe);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void MoveFollowTeleportStationary(float trialTime, SpawnpointKindEnum spawnpointKindEnum)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.MoveFollowTeleportStationary;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateMoveFollowTeleportStationary).Activate(trialTime, spawnpointKindEnum);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void PointOfInterest(
    float poiTime,
    POIBase poi,
    POIAnimationEnum animation,
    int animationIndex,
    int animationsCount = 0)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.PointOfInterest;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStatePointOfInterest).Activate(poiTime, poi, animation, animationIndex, animationsCount);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void POIExtraExit(float time)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    INpcState state = this.states[this.CurrentNpcState];
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.POIExtraExit;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NPCStatePOIExtraExit).Activate(state, time);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void Loot(float poiTime, GameObject target)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.PointOfInterest;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStatePointOfInterest).ActivateLoot(poiTime, target);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void PresetIdle()
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.PresetIdle;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateIdlePreset).Activate();
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void PresetIdleTest(IdlePresetObject target)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.PresetIdleTest;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateIdlePresetTest).Activate(target);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void DialogNpc(GameObject targetCharacter, float time, bool speaking)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.DialogNpc;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateDialogNpc).Activate(targetCharacter, time, speaking);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void FightIdle(bool aim)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.FightIdle;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateFightIdle).Activate(aim);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void FightFollow(
    float stopDistance,
    float runDistance,
    bool aim,
    float timeAfterHitToRun = 7f)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.FightFollow;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateFightFollow).Activate(stopDistance, runDistance, aim, timeAfterHitToRun);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void FightFollowTarget(
    float stopDistance,
    float runDistance,
    float retreatDistance,
    Transform target,
    bool aim)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.FightFollowTarget;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateFightFollowTarget).Activate(stopDistance, runDistance, retreatDistance, target, aim);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void FightKeepDistance(float keepDistance, bool strafe, bool aim)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.FightKeepDistance;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateFightKeepDistance).Activate(keepDistance, strafe, aim);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void FightSurrender(float moveTime)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.FightSurrender;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateFightSurrender).Activate(moveTime);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void FightEscape(float escapeDistance)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.FightEscape;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateFightEscape).Activate(escapeDistance);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void FightSurrenderLoot(float lootTime)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.FightSurrenderLoot;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateFightSurrenderLoot).Activate(lootTime);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void Ragdoll(bool internalCollisions)
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    INpcState state = this.states[this.CurrentNpcState];
    this.CurrentNpcState = NpcStateEnum.Ragdoll;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateRagdoll).Activate(internalCollisions, state);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  public void RagdollRessurect()
  {
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    this.CurrentNpcState = NpcStateEnum.RagdollRessurect;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateRagdollRessurect).Activate();
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  private void Awake()
  {
    this.RestartBehaviourAfterTeleport = true;
    this.pivot = this.GetComponent<Pivot>();
    if ((bool) (UnityEngine.Object) this.pivot)
    {
      AnimatorEventProxy animatorEventProxy = this.pivot.GetAnimatorEventProxy();
      if ((bool) (UnityEngine.Object) animatorEventProxy)
      {
        animatorEventProxy.AnimatorMoveEvent += new Action(this.Proxy_AnimatorMoveEvent);
        animatorEventProxy.AnimatorEventEvent += new Action<string>(this.Proxy_AnimatorEventEvent);
      }
      NavMeshAgent agent = this.pivot.GetAgent();
      if ((bool) (UnityEngine.Object) agent)
        agent.enabled = false;
      this.animator = this.pivot.GetAnimator();
      if ((bool) (UnityEngine.Object) this.animator)
      {
        this.animatorEnabled = this.animator.enabled;
        this.initialAnimatorcullingMode = this.animator.cullingMode;
        this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
      }
    }
    this.CurrentNpcState = NpcStateEnum.Unknown;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateUnknown).Activate((INpcState) null);
  }

  private void Proxy_AnimatorEventEvent(string obj)
  {
    this.states[this.CurrentNpcState].OnAnimatorEventEvent(obj);
  }

  private void Proxy_AnimatorMoveEvent() => this.states[this.CurrentNpcState].OnAnimatorMove();

  private void OnEnable()
  {
    ServiceLocator.GetService<LodService>().RegisterLod(this);
    foreach (BehaviorTree component in this.gameObject.GetComponents<BehaviorTree>())
      component.PauseWhenDisabled = false;
  }

  private void OnDisable()
  {
    ServiceLocator.GetService<LodService>().UnregisterLod(this);
    this.states[this.CurrentNpcState].Shutdown();
    INpcState state = this.states[this.CurrentNpcState];
    this.CurrentNpcState = NpcStateEnum.Unknown;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateUnknown).Activate(state);
  }

  void IEntityAttachable.Attach(IEntity owner)
  {
    this.Owner = owner;
    NavigationComponent component = this.Owner.GetComponent<NavigationComponent>();
    if (component == null)
      return;
    component.OnPreTeleport += new Action<INavigationComponent, IEntity>(this.NavigationComponent_OnPreTeleport);
    component.OnTeleport += new Action<INavigationComponent, IEntity>(this.NavigationComponent_OnTeleport);
  }

  void IEntityAttachable.Detach()
  {
    NavigationComponent component = this.Owner.GetComponent<NavigationComponent>();
    if (component != null)
    {
      component.OnPreTeleport -= new Action<INavigationComponent, IEntity>(this.NavigationComponent_OnPreTeleport);
      component.OnTeleport -= new Action<INavigationComponent, IEntity>(this.NavigationComponent_OnTeleport);
    }
    this.Owner = (IEntity) null;
  }

  private void Update()
  {
    if (Profiler.enabled)
      Profiler.BeginSample(NpcState.stateNames[(int) this.CurrentNpcState]);
    if (this.frameCountFromStart == 1 && (bool) (UnityEngine.Object) this.animator)
    {
      this.animator.enabled = this.animatorEnabled;
      this.animator.cullingMode = this.initialAnimatorcullingMode;
    }
    ++this.frameCountFromStart;
    this.states[this.CurrentNpcState].Update();
    if (!Profiler.enabled)
      return;
    Profiler.EndSample();
  }

  private void NavigationComponent_OnPreTeleport(INavigationComponent arg1, IEntity arg2)
  {
    if (this.RestartBehaviourAfterTeleport && this.enabled)
    {
      foreach (BehaviorTree component in this.gameObject.GetComponents<BehaviorTree>())
        component.OnDisable();
    }
    if (this.inLodState)
      this.states[this.CurrentNpcState].OnLodStateChanged(false);
    this.states[this.CurrentNpcState].Shutdown();
    INpcState state = this.states[this.CurrentNpcState];
    this.CurrentNpcState = NpcStateEnum.Unknown;
    this.CheckState(this.CurrentNpcState);
    (this.states[this.CurrentNpcState] as NpcStateUnknown).Activate(state);
    if (!this.inLodState)
      return;
    this.states[this.CurrentNpcState].OnLodStateChanged(true);
  }

  private void NavigationComponent_OnTeleport(INavigationComponent arg1, IEntity arg2)
  {
    if (!this.RestartBehaviourAfterTeleport || !this.enabled)
      return;
    foreach (BehaviorTree component in this.gameObject.GetComponents<BehaviorTree>())
      component.OnEnable();
  }
}
