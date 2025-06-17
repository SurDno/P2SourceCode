using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.MessangerStationary;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

[DisallowMultipleComponent]
public class NpcState : MonoBehaviour, IEntityAttachable
{
  private const int skipFramesBeforeDisablingAnimator = 1;
  private Dictionary<NpcStateEnum, INpcState> states = new(NpcStateEnumComparer.Instance);
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
      return states.TryGetValue(CurrentNpcState, out INpcState npcState) ? npcState : null;
    }
  }

  [Inspected]
  public IEntity Owner { get; private set; }

  [Inspected]
  public bool AnimatorEnabled
  {
    get => animatorEnabled;
    set
    {
      animatorEnabled = value;
      if (frameCountFromStart <= 1 || !(bool) (Object) animator)
        return;
      animator.enabled = animatorEnabled;
    }
  }

  [Inspected(Mode = ExecuteMode.EditAndRuntime, Mutable = true)]
  public WeaponEnum Weapon
  {
    get => weapon;
    set
    {
      weapon = value;
      Action<WeaponEnum> weaponChangeEvent = WeaponChangeEvent;
      if (weaponChangeEvent == null)
        return;
      weaponChangeEvent(weapon);
    }
  }

  public NpcStateStatusEnum Status => states[CurrentNpcState].Status;

  public void OnLodStateChanged(bool inLodState)
  {
    this.inLodState = inLodState;
    states[CurrentNpcState].OnLodStateChanged(inLodState);
  }

  private void CheckState(NpcStateEnum state)
  {
    if (states.ContainsKey(state))
      return;
    states[state] = NpcStateFactory.Create(state, this, pivot);
  }

  public void Idle(float primaryIdleProbability = 0.7f, bool makeObstacle = false)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.Idle;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateIdle).Activate(primaryIdleProbability, makeObstacle);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void IdleKinematic(float primaryIdleProbability = 0.7f, bool makeObstacle = false)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.IdleKinematic;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateIdleKinematic).Activate(primaryIdleProbability, makeObstacle);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void IdlePlagueCloud()
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.IdlePlagueCloud;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateIdlePlagueCloud).Activate();
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void InFire()
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.InFire;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateInFire).Activate();
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void Rotate(Transform target)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.Rotate;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateRotate).Activate(target);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void Rotate(Quaternion rotation)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.Rotate;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateRotate).Activate(rotation);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void Move(Vector3 destination, bool failOnPartialPath = false)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.Move;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateMove).Activate(destination, failOnPartialPath);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void MoveCloud(Vector3 destination)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.MoveCloud;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateMoveCloud).Activate(destination);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void MoveByPath(List<Vector3> path)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.MoveByPath;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateMoveByPath).Activate(path);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void MoveByPathCloud(List<Vector3> path)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.MoveByPathCloud;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateMoveByPathCloud).Activate(path);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void MoveRetreat(Transform target, float retreatDistance)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.MoveRetreat;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateMoveRetreat).Activate(target, retreatDistance);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void MoveFollow(Transform target, float followDistance)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.MoveFollow;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateMoveFollow).Activate(target, followDistance);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void MoveFollowTeleport(float followDistance, float trialTime, bool waitForTargetSeesMe)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.MoveFollowTeleport;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateMoveFollowTeleport).Activate(followDistance, trialTime, waitForTargetSeesMe);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void MoveFollowTeleportStationary(float trialTime, SpawnpointKindEnum spawnpointKindEnum)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.MoveFollowTeleportStationary;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateMoveFollowTeleportStationary).Activate(trialTime, spawnpointKindEnum);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void PointOfInterest(
    float poiTime,
    POIBase poi,
    POIAnimationEnum animation,
    int animationIndex,
    int animationsCount = 0)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.PointOfInterest;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStatePointOfInterest).Activate(poiTime, poi, animation, animationIndex, animationsCount);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void POIExtraExit(float time)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    INpcState state = states[CurrentNpcState];
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.POIExtraExit;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NPCStatePOIExtraExit).Activate(state, time);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void Loot(float poiTime, GameObject target)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.PointOfInterest;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStatePointOfInterest).ActivateLoot(poiTime, target);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void PresetIdle()
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.PresetIdle;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateIdlePreset).Activate();
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void PresetIdleTest(IdlePresetObject target)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.PresetIdleTest;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateIdlePresetTest).Activate(target);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void DialogNpc(GameObject targetCharacter, float time, bool speaking)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.DialogNpc;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateDialogNpc).Activate(targetCharacter, time, speaking);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void FightIdle(bool aim)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.FightIdle;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateFightIdle).Activate(aim);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void FightFollow(
    float stopDistance,
    float runDistance,
    bool aim,
    float timeAfterHitToRun = 7f)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.FightFollow;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateFightFollow).Activate(stopDistance, runDistance, aim, timeAfterHitToRun);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void FightFollowTarget(
    float stopDistance,
    float runDistance,
    float retreatDistance,
    Transform target,
    bool aim)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.FightFollowTarget;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateFightFollowTarget).Activate(stopDistance, runDistance, retreatDistance, target, aim);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void FightKeepDistance(float keepDistance, bool strafe, bool aim)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.FightKeepDistance;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateFightKeepDistance).Activate(keepDistance, strafe, aim);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void FightSurrender(float moveTime)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.FightSurrender;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateFightSurrender).Activate(moveTime);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void FightEscape(float escapeDistance)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.FightEscape;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateFightEscape).Activate(escapeDistance);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void FightSurrenderLoot(float lootTime)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.FightSurrenderLoot;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateFightSurrenderLoot).Activate(lootTime);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void Ragdoll(bool internalCollisions)
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    INpcState state = states[CurrentNpcState];
    CurrentNpcState = NpcStateEnum.Ragdoll;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateRagdoll).Activate(internalCollisions, state);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  public void RagdollRessurect()
  {
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    CurrentNpcState = NpcStateEnum.RagdollRessurect;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateRagdollRessurect).Activate();
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  private void Awake()
  {
    RestartBehaviourAfterTeleport = true;
    pivot = GetComponent<Pivot>();
    if ((bool) (Object) pivot)
    {
      AnimatorEventProxy animatorEventProxy = pivot.GetAnimatorEventProxy();
      if ((bool) (Object) animatorEventProxy)
      {
        animatorEventProxy.AnimatorMoveEvent += Proxy_AnimatorMoveEvent;
        animatorEventProxy.AnimatorEventEvent += Proxy_AnimatorEventEvent;
      }
      NavMeshAgent agent = pivot.GetAgent();
      if ((bool) (Object) agent)
        agent.enabled = false;
      animator = pivot.GetAnimator();
      if ((bool) (Object) animator)
      {
        animatorEnabled = animator.enabled;
        initialAnimatorcullingMode = animator.cullingMode;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
      }
    }
    CurrentNpcState = NpcStateEnum.Unknown;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateUnknown).Activate(null);
  }

  private void Proxy_AnimatorEventEvent(string obj)
  {
    states[CurrentNpcState].OnAnimatorEventEvent(obj);
  }

  private void Proxy_AnimatorMoveEvent() => states[CurrentNpcState].OnAnimatorMove();

  private void OnEnable()
  {
    ServiceLocator.GetService<LodService>().RegisterLod(this);
    foreach (BehaviorTree component in gameObject.GetComponents<BehaviorTree>())
      component.PauseWhenDisabled = false;
  }

  private void OnDisable()
  {
    ServiceLocator.GetService<LodService>().UnregisterLod(this);
    states[CurrentNpcState].Shutdown();
    INpcState state = states[CurrentNpcState];
    CurrentNpcState = NpcStateEnum.Unknown;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateUnknown).Activate(state);
  }

  void IEntityAttachable.Attach(IEntity owner)
  {
    Owner = owner;
    NavigationComponent component = Owner.GetComponent<NavigationComponent>();
    if (component == null)
      return;
    component.OnPreTeleport += NavigationComponent_OnPreTeleport;
    component.OnTeleport += NavigationComponent_OnTeleport;
  }

  void IEntityAttachable.Detach()
  {
    NavigationComponent component = Owner.GetComponent<NavigationComponent>();
    if (component != null)
    {
      component.OnPreTeleport -= NavigationComponent_OnPreTeleport;
      component.OnTeleport -= NavigationComponent_OnTeleport;
    }
    Owner = null;
  }

  private void Update()
  {
    if (Profiler.enabled)
      Profiler.BeginSample(stateNames[(int) CurrentNpcState]);
    if (frameCountFromStart == 1 && (bool) (Object) animator)
    {
      animator.enabled = animatorEnabled;
      animator.cullingMode = initialAnimatorcullingMode;
    }
    ++frameCountFromStart;
    states[CurrentNpcState].Update();
    if (!Profiler.enabled)
      return;
    Profiler.EndSample();
  }

  private void NavigationComponent_OnPreTeleport(INavigationComponent arg1, IEntity arg2)
  {
    if (RestartBehaviourAfterTeleport && enabled)
    {
      foreach (BehaviorTree component in gameObject.GetComponents<BehaviorTree>())
        component.OnDisable();
    }
    if (inLodState)
      states[CurrentNpcState].OnLodStateChanged(false);
    states[CurrentNpcState].Shutdown();
    INpcState state = states[CurrentNpcState];
    CurrentNpcState = NpcStateEnum.Unknown;
    CheckState(CurrentNpcState);
    (states[CurrentNpcState] as NpcStateUnknown).Activate(state);
    if (!inLodState)
      return;
    states[CurrentNpcState].OnLodStateChanged(true);
  }

  private void NavigationComponent_OnTeleport(INavigationComponent arg1, IEntity arg2)
  {
    if (!RestartBehaviourAfterTeleport || !enabled)
      return;
    foreach (BehaviorTree component in gameObject.GetComponents<BehaviorTree>())
      component.OnEnable();
  }
}
