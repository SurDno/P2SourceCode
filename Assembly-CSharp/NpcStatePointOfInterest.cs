using Engine.Behaviours.Components;
using Engine.Behaviours.Engines.Controllers;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common.Services;
using Engine.Impl.Tasks;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

public class NpcStatePointOfInterest : INpcState, INpcStateNeedSyncBack
{
  private NPCEnemy enemy;
  private NpcState npcState;
  private Pivot pivot;
  private EngineBehavior behavior;
  private POISetup poiSetup;
  private Animator animator;
  private Rigidbody rigidbody;
  private NavMeshAgent agent;
  private NPCWeaponService weaponService;
  private AnimatorState45 animatorState;
  private FightAnimatorBehavior.AnimatorState fightAnimatorState;
  private Vector3 _poiDeltaPos;
  private Vector3 _poiDeltaPosPassed;
  private float _poiDeltaAngle;
  private bool _poiDeltaFinished;
  private float _poiDeltaSyncDelay;
  private float _poiSyncPositionTimeLeft;
  private float _poiSyncRotationTimeLeft;
  private POIBase _poi;
  private POIService poiService;
  private bool frozenForDialog;
  private bool animationIsQuick;
  [Inspected]
  private GameObject poiTarget;
  [Inspected]
  private NpcStatePointOfInterest.StateEnum state;
  [Inspected]
  private float timeLeft;
  private Vector3 deltaPosition;
  private Vector3 poiStartPosition;
  [Inspected]
  private POIAnimationEnum animation;
  [Inspected]
  private int animationIndex;
  [Inspected]
  private int animationsCount;
  private bool initiallyKinematic;
  [Inspected]
  private bool inited;
  [Inspected]
  private bool failed;
  private bool dialogActivityChecked;
  private Vector3 enterPoint;
  private float timeToNextRandomAnimationSet = 0.0f;
  private float timeToNextRandomAnimationSetMax = 2f;
  private bool syncBackInited = false;
  private bool couldPlayReactionAnimation;
  private bool neededExtraExitPOI;
  private Vector3 poiBackPosition;

  public GameObject GameObject { get; private set; }

  private bool TryInit()
  {
    if (this.inited)
      return true;
    this.behavior = this.pivot.GetBehavior();
    this.poiSetup = this.GameObject.GetComponent<POISetup>();
    this.agent = this.pivot.GetAgent();
    this.rigidbody = this.pivot.GetRigidbody();
    this.weaponService = this.pivot.GetNpcWeaponService();
    this.animator = this.pivot.GetAnimator();
    this.enemy = this.pivot.GetNpcEnemy();
    if ((Object) this.animator == (Object) null)
    {
      Debug.LogError((object) ("Null animator " + this.GameObject.name), (Object) this.GameObject);
      Debug.LogError((object) ("Null animator " + this.GameObject.GetFullName()));
      this.failed = true;
      return false;
    }
    this.animatorState = AnimatorState45.GetAnimatorState(this.animator);
    this.fightAnimatorState = FightAnimatorBehavior.GetAnimatorState(this.animator);
    this.syncBackInited = false;
    if ((Object) this.poiSetup == (Object) null)
    {
      this.failed = true;
      return false;
    }
    this.failed = false;
    this.inited = true;
    return true;
  }

  public NpcStatePointOfInterest(NpcState npcState, Pivot pivot)
  {
    this.GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public NpcStateStatusEnum Status
  {
    get
    {
      return this.state == NpcStatePointOfInterest.StateEnum.End ? NpcStateStatusEnum.Success : NpcStateStatusEnum.Running;
    }
  }

  public void Activate(
    float poiTime,
    POIBase poi,
    POIAnimationEnum animation,
    int animationIndex,
    int animationsCount)
  {
    if (!this.TryInit())
      return;
    this.poiStartPosition = this.GameObject.transform.position;
    this.animation = animation;
    this.animationIndex = animationIndex;
    this.animationsCount = animationsCount;
    if ((Object) this.enemy != (Object) null)
      this.couldPlayReactionAnimation = this.enemy.CanPlayReactionAnimation;
    this.neededExtraExitPOI = this.npcState.NeedExtraExitPOI;
    if ((bool) (Object) this.rigidbody)
    {
      this.initiallyKinematic = this.rigidbody.isKinematic;
      this.rigidbody.isKinematic = true;
    }
    this.animatorState.ControlMovableState = AnimatorState45.MovableState45.POI;
    this.animatorState.ControlPOIAnimationIndex = animationIndex;
    this.animatorState.ControlPOIMiddleAnimationsCount = animationsCount;
    this.animatorState.ControlPOIStartFromMiddle = false;
    this.animatorState.MovableStop = false;
    this.npcState.NeedExtraExitPOI = false;
    switch (animation)
    {
      case POIAnimationEnum.S_SitAtDesk:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_SitAtDesk;
        break;
      case POIAnimationEnum.S_SitOnBench:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_SitOnBench;
        if ((Object) this.enemy != (Object) null)
          this.enemy.CanPlayReactionAnimation = false;
        this.npcState.NeedExtraExitPOI = true;
        break;
      case POIAnimationEnum.S_LeanOnWall:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_LeanOnWall;
        break;
      case POIAnimationEnum.S_LeanOnTable:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_LeanOnTable;
        break;
      case POIAnimationEnum.S_SitNearWall:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_SitNearWall;
        if ((Object) this.enemy != (Object) null)
          this.enemy.CanPlayReactionAnimation = false;
        this.npcState.NeedExtraExitPOI = true;
        break;
      case POIAnimationEnum.S_LieOnBed:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_LieOnBed;
        break;
      case POIAnimationEnum.S_NearFire:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_NearFire;
        break;
      case POIAnimationEnum.Q_ViewPoster:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ViewPoster;
        break;
      case POIAnimationEnum.Q_LookOutOfTheWindow:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_LookOutOfTheWindow;
        break;
      case POIAnimationEnum.Q_LookUnder:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_LookUnder;
        break;
      case POIAnimationEnum.Q_LookIntoTheWindow:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_LookIntoTheWindow;
        break;
      case POIAnimationEnum.Q_ActionWithWall:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ActionWithWall;
        break;
      case POIAnimationEnum.Q_ActionWithTable:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ActionWithTable;
        break;
      case POIAnimationEnum.Q_ActionWithWardrobe:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ActionWithWardrobe;
        break;
      case POIAnimationEnum.Q_ActionWithShelves:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ActionWithShelves;
        break;
      case POIAnimationEnum.Q_ActionWithNightstand:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ActionWithNightstand;
        break;
      case POIAnimationEnum.Q_ActionOnFloor:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ActionOnFloor;
        break;
      case POIAnimationEnum.S_ActionOnFloor:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_ActionOnFloor;
        break;
      case POIAnimationEnum.Q_Idle:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_Idle;
        break;
      case POIAnimationEnum.Q_NearFire:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_NearFire;
        break;
      case POIAnimationEnum.S_Dialog:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_Dialog;
        break;
      case POIAnimationEnum.S_Loot:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_Loot;
        break;
      case POIAnimationEnum.Q_PlaygroundPlay:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_PlaygroundPlay;
        break;
      case POIAnimationEnum.S_PlaygroundSandbox:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_PlaygroundSandbox;
        break;
      case POIAnimationEnum.S_PlaygroundCooperative:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_PlaygroundCooperative;
        break;
      case POIAnimationEnum.Q_PlaygroundCooperative:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_PlaygroundCooperative;
        break;
      case POIAnimationEnum.S_SitAtDeskRight:
        this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_SitAtDeskRight;
        break;
    }
    this.timeLeft = !this.poiSetup.GetAnimationPlayOnce(animation, animationIndex) ? poiTime : 0.0f;
    this.poiService = ServiceLocator.GetService<POIService>();
    this.state = NpcStatePointOfInterest.StateEnum.Prepare;
    this._poi = poi;
    this.poiTarget = (GameObject) null;
    this.dialogActivityChecked = false;
    this.frozenForDialog = false;
    this.animationIsQuick = this.poiSetup.AnimationIsQuick(animation);
    if (this.poiSetup.GetNeedSynchronizeAnimation(animation, animationIndex))
      this.SynchronizeAnimation();
    this.SetRandomNextAnimation();
    if (!((Object) this.weaponService != (Object) null))
      return;
    this.weaponService.Weapon = WeaponEnum.Unknown;
  }

  public void ActivateLoot(float poiTime, GameObject target)
  {
    if (!this.TryInit())
      return;
    this._poi = (POIBase) null;
    this.poiTarget = target;
    this.animation = POIAnimationEnum.S_Loot;
    this.animationIndex = 0;
    this.animationsCount = 1;
    if ((bool) (Object) this.rigidbody)
    {
      this.initiallyKinematic = this.rigidbody.isKinematic;
      this.rigidbody.isKinematic = true;
    }
    if (this.animatorState != null)
    {
      this.animatorState.ControlMovableState = AnimatorState45.MovableState45.POI;
      this.animatorState.ControlPOIAnimationIndex = this.animationIndex;
      this.animatorState.ControlPOIMiddleAnimationsCount = this.animationsCount;
      this.animatorState.ControlPOIStartFromMiddle = false;
      this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_Loot;
    }
    this.timeLeft = !this.poiSetup.GetAnimationPlayOnce(this.animation, this.animationIndex) ? poiTime : 0.0f;
    this.state = NpcStatePointOfInterest.StateEnum.Prepare;
    this.poiStartPosition = this.GameObject.transform.position;
    this.SetRandomNextAnimation();
  }

  public void Shutdown()
  {
    if (this.failed)
      return;
    if (this.Status != NpcStateStatusEnum.Success)
      this.SetPOIDeltaParams(this.poiStartPosition - this.GameObject.transform.position, 0.0f, 0.3f, 0.0f);
    this.deltaPosition = Vector3.zero;
    if ((bool) (Object) this.rigidbody)
      this.rigidbody.isKinematic = this.initiallyKinematic;
    if (this.poiService != null)
      this.poiService.RemoveCharacterAsDialogTarget(this.GameObject);
    if ((Object) this.weaponService != (Object) null)
      this.weaponService.Weapon = this.npcState.Weapon;
    if ((Object) this.enemy != (Object) null)
      this.enemy.CanPlayReactionAnimation = this.couldPlayReactionAnimation;
    this.npcState.NeedExtraExitPOI = this.neededExtraExitPOI;
  }

  public void OnAnimatorMove()
  {
    if (this.failed)
      return;
    float deltaTime = this.animator.updateMode == AnimatorUpdateMode.AnimatePhysics ? Time.fixedDeltaTime : Time.deltaTime;
    if (this.animatorState.IsPOI)
    {
      this.agent.nextPosition = this.animator.rootPosition;
      this.GameObject.transform.position += this.animator.deltaPosition;
      this.GameObject.transform.rotation *= this.animator.deltaRotation;
      if (this._poiDeltaFinished)
        return;
      this.AddPOIDelta(deltaTime);
    }
    else
    {
      if (!this._poiDeltaFinished)
        this.AddPOIDelta(deltaTime);
      if (this.agent.isActiveAndEnabled && this.agent.isOnNavMesh)
      {
        Vector3 vector3 = this.GameObject.transform.position + this.animator.deltaPosition;
        vector3.y = Mathf.MoveTowards(vector3.y, this.agent.nextPosition.y, deltaTime * 0.1f);
        this.agent.nextPosition = vector3;
        this.GameObject.transform.position = this.agent.nextPosition;
      }
      this.GameObject.transform.rotation *= Quaternion.AngleAxis(57.29578f * this.animator.angularVelocity.y * deltaTime, Vector3.up);
    }
  }

  private void AddPOIDelta(float deltaTime)
  {
    if ((double) this._poiSyncRotationTimeLeft > 0.0)
    {
      this.GameObject.transform.rotation *= Quaternion.AngleAxis(this._poiDeltaAngle * Mathf.Min(this._poiSyncRotationTimeLeft, deltaTime), Vector3.up);
      this._poiSyncRotationTimeLeft -= deltaTime;
    }
    if ((double) this._poiDeltaSyncDelay > 0.0)
      this._poiDeltaSyncDelay -= deltaTime;
    if ((double) this._poiDeltaSyncDelay <= 0.0 && (double) this._poiSyncPositionTimeLeft > 0.0)
    {
      float num = Mathf.Min(this._poiSyncPositionTimeLeft, deltaTime);
      this.GameObject.transform.position += this._poiDeltaPos * num;
      this._poiSyncPositionTimeLeft -= num;
    }
    if ((double) this._poiSyncRotationTimeLeft > 0.0 || (double) this._poiSyncPositionTimeLeft > 0.0)
      return;
    this._poiDeltaFinished = true;
  }

  private void SynchronizeAnimation()
  {
    Vector3 closestTargetPosition;
    Quaternion closestTargetRotation;
    if ((Object) this._poi != (Object) null)
    {
      this._poi.GetClosestTargetPoint(this.animation, this.animationIndex, this.poiSetup, this.GameObject.transform.position, out closestTargetPosition, out closestTargetRotation);
      this.poiBackPosition = closestTargetPosition - this.poiSetup.GetAnimationOffset(this.animation, this.animationIndex);
    }
    else
    {
      closestTargetPosition = this.poiTarget.transform.position;
      closestTargetRotation = this.poiTarget.transform.rotation;
      this.poiBackPosition = closestTargetPosition;
    }
    Vector3 deltaPos = closestTargetPosition - this.GameObject.transform.position;
    Vector3 vector3 = this.GameObject.transform.InverseTransformVector(closestTargetRotation * Vector3.forward);
    float deltaAngle = Mathf.Atan2(vector3.x, vector3.z) * 57.29578f;
    float animationSyncDelay = this.poiSetup.GetAnimationSyncDelay(this.animation, this.animationIndex);
    this.deltaPosition = deltaPos;
    float syncTime = 1f;
    this.SetPOIDeltaParams(deltaPos, deltaAngle, syncTime, animationSyncDelay);
  }

  public void SetPOIDeltaParams(
    Vector3 deltaPos,
    float deltaAngle,
    float syncTime,
    float syncDelay)
  {
    this._poiDeltaPos = deltaPos / syncTime;
    this._poiDeltaAngle = deltaAngle / syncTime;
    this._poiDeltaPosPassed = Vector3.zero;
    this._poiDeltaSyncDelay = syncDelay;
    this._poiDeltaFinished = false;
    this._poiSyncPositionTimeLeft = syncTime;
    this._poiSyncRotationTimeLeft = syncTime;
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!this.failed)
      ;
  }

  public void Update()
  {
    if (this.failed || InstanceByRequest<EngineApplication>.Instance.IsPaused || (Object) this.poiTarget == (Object) null && ((Object) this._poi == (Object) null || (Object) this._poi.gameObject == (Object) null))
      return;
    if (this.state == NpcStatePointOfInterest.StateEnum.Prepare)
    {
      if (!this.animatorState.IsPOI)
        return;
      this.state = NpcStatePointOfInterest.StateEnum.POI;
    }
    else if (this.state == NpcStatePointOfInterest.StateEnum.POI)
    {
      if (!this.dialogActivityChecked)
      {
        this.dialogActivityChecked = true;
        this.enterPoint = this.GameObject.transform.position;
        if ((Object) this._poi != (Object) null && this._poi.SupportsDialog && this.poiService != null && (double) this.timeLeft > 0.0)
          this.poiService.AddCharacterAsDialogTarget(this.GameObject, this);
      }
      if (!this.UpdateDuringPOI(this._poi))
        return;
      this.animatorState.MovableStop = true;
      this.state = !this.animatorState.IsPOIExit ? (!this.animationIsQuick ? NpcStatePointOfInterest.StateEnum.WaitForPOIExitStart : NpcStatePointOfInterest.StateEnum.WaitForPOIEnd) : NpcStatePointOfInterest.StateEnum.WaitForPOIEnd;
    }
    else if (this.state == NpcStatePointOfInterest.StateEnum.WaitForPOIExitStart)
    {
      if (this.animatorState.IsPOIExit || !this.animatorState.IsPOI)
      {
        this.state = NpcStatePointOfInterest.StateEnum.WaitForPOIEnd;
        this.SetSyncBack();
      }
    }
    else if (this.state == NpcStatePointOfInterest.StateEnum.WaitForPOIEnd && this.UpdateWaitForPOIEnd())
      this.state = NpcStatePointOfInterest.StateEnum.End;
    if (this.fightAnimatorState == null || !this.fightAnimatorState.IsReaction || this.syncBackInited)
      return;
    this.SetSyncBack();
    this.animatorState.ControlMovableState = AnimatorState45.MovableState45.Idle;
    if ((Object) this.weaponService != (Object) null)
      this.weaponService.Weapon = this.npcState.Weapon;
  }

  private bool UpdatDuringeStopping() => this.animatorState.IsPOI;

  private bool UpdateDuringPOI(POIBase poi)
  {
    this.timeToNextRandomAnimationSet -= Time.deltaTime;
    if ((double) this.timeToNextRandomAnimationSet <= 0.0)
    {
      this.timeToNextRandomAnimationSet = this.timeToNextRandomAnimationSetMax;
      this.SetRandomNextAnimation();
    }
    if (this.frozenForDialog)
      return false;
    this.timeLeft -= Time.deltaTime;
    return (double) this.timeLeft < 0.0;
  }

  private bool UpdateWaitForPOIEnd() => !this.animatorState.IsPOI;

  private void SetSyncBack()
  {
    this.syncBackInited = true;
    this.SetPOIDeltaParams(new Vector3(0.0f, -this.deltaPosition.y, 0.0f), 0.0f, 0.5f, 0.0f);
  }

  public void SetDialogFreeze(bool freeze) => this.frozenForDialog = freeze;

  public Vector3 GetEnterPoint() => this.enterPoint;

  public POIBase GetPOI() => this._poi;

  public void LookAt(GameObject target)
  {
    if ((Object) this.GameObject == (Object) null)
      return;
    if ((Object) target == (Object) null)
    {
      Debug.LogError((object) "target == null, Такого быть недолжно, воспроизвести, разобраться в чем дело и пофиксить нормально");
    }
    else
    {
      IKController component = this.GameObject.GetComponent<IKController>();
      if (!((Object) component != (Object) null))
        return;
      component.enabled = true;
      component.LookTarget = target.transform;
      component.LookEyeContactOnly = false;
      component.StopIfOutOfLimits = true;
    }
  }

  private void SetRandomNextAnimation()
  {
    this.animator.SetInteger("Movable.POI.AnimationIndex2", Random.Range(0, this.animator.GetInteger("Movable.POI.MiddleAnimationsCount")));
  }

  public Vector3 GetSyncBackPosition() => this.poiBackPosition;

  public POIAnimationEnum GetPoiType() => this.animation;

  public void OnLodStateChanged(bool enabled)
  {
  }

  private enum StateEnum
  {
    Prepare,
    POI,
    WaitForPOIExitStart,
    WaitForPOIEnd,
    End,
  }
}
