using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Impl.Tasks;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Settings.External;
using Inspectors;
using System;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateIdlePreset : INpcState, INpcStateNeedSyncBack
{
  private NpcState npcState;
  private Pivot pivot;
  private NPCEnemy enemy;
  private EngineBehavior behavior;
  private POISetup poiSetup;
  protected Animator animator;
  protected Rigidbody rigidbody;
  private NavMeshAgent agent;
  private AnimatorState45 animatorState;
  private POIAnimationEnum animation;
  private int animationIndex;
  private int animationsCount;
  private bool initiallyKinematic;
  private bool initiallyNavmesh;
  private AnimatorUpdateMode initialAnimatorUpdateMode;
  protected IdlePresetObject preset;
  private bool inited;
  private bool failed;
  private NPCWeaponService weaponService;
  private bool combatIdle = false;
  private IEntity setupPoint;
  private float timeToNextRandomAnimationSet = 0.0f;
  private float timeToNextRandomAnimationSetMax = 2f;
  private bool positionsAreSet;
  private bool sayReplics;
  private float timeToNextReplic;
  private bool couldPlayReactionAnimation;
  private bool neededExtraExitPOI;

  [Inspected]
  public NpcStateStatusEnum Status => NpcStateStatusEnum.Running;

  public GameObject GameObject { get; private set; }

  protected bool TryInit()
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
    if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null)
    {
      Debug.LogError((object) ("Null animator " + this.GameObject.name), (UnityEngine.Object) this.GameObject);
      Debug.LogError((object) ("Null animator " + this.GameObject.GetFullName()));
      this.failed = true;
      return false;
    }
    this.animatorState = AnimatorState45.GetAnimatorState(this.animator);
    if ((UnityEngine.Object) this.poiSetup == (UnityEngine.Object) null)
    {
      Debug.LogError((object) ("No PoiSetup " + this.GameObject.name), (UnityEngine.Object) this.GameObject);
      this.failed = true;
      return false;
    }
    this.failed = false;
    this.inited = true;
    return true;
  }

  public NpcStateIdlePreset(NpcState npcState, Pivot pivot)
  {
    this.GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate()
  {
    if (!this.TryInit())
      return;
    if ((bool) (UnityEngine.Object) this.rigidbody)
    {
      this.initiallyKinematic = this.rigidbody.isKinematic;
      this.rigidbody.isKinematic = true;
    }
    if ((UnityEngine.Object) this.agent != (UnityEngine.Object) null)
    {
      this.initiallyNavmesh = this.agent.isActiveAndEnabled;
      this.agent.enabled = false;
    }
    this.setupPoint = this.npcState.Owner.GetComponent<NavigationComponent>().SetupPoint;
    if (this.setupPoint == null)
      return;
    if ((UnityEngine.Object) this.weaponService != (UnityEngine.Object) null)
      this.weaponService.Weapon = WeaponEnum.Unknown;
    if ((UnityEngine.Object) this.enemy != (UnityEngine.Object) null)
      this.couldPlayReactionAnimation = this.enemy.CanPlayReactionAnimation;
    this.neededExtraExitPOI = this.npcState.NeedExtraExitPOI;
    this.npcState.NeedExtraExitPOI = false;
    if ((UnityEngine.Object) ((IEntityView) this.setupPoint).GameObject == (UnityEngine.Object) null)
    {
      ((IEntityView) this.setupPoint).OnGameObjectChangedEvent -= new Action(this.OnSetupPointViewChanged);
      ((IEntityView) this.setupPoint).OnGameObjectChangedEvent += new Action(this.OnSetupPointViewChanged);
    }
    else
      this.OnSetupPointViewChanged();
    this.initialAnimatorUpdateMode = this.animator.updateMode;
    this.animator.updateMode = AnimatorUpdateMode.Normal;
    this.positionsAreSet = false;
    if (this.npcState.Owner == null)
      return;
    ParametersComponent component = this.npcState.Owner.GetComponent<ParametersComponent>();
    if (component != null)
    {
      IParameter<bool> byName = component.GetByName<bool>(ParameterNameEnum.SayReplicsInIdle);
      if (byName != null)
      {
        this.sayReplics = byName.Value;
        if (this.sayReplics)
          this.timeToNextReplic = UnityEngine.Random.Range(ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMin, ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMax);
      }
    }
  }

  private void CreateObstacle()
  {
    this.agent.enabled = false;
    NavMeshObstacle obstacle = this.pivot.GetObstacle();
    if (!((UnityEngine.Object) obstacle != (UnityEngine.Object) null))
      return;
    obstacle.enabled = true;
    obstacle.carving = true;
    obstacle.radius = this.agent.radius;
  }

  private void OnSetupPointViewChanged()
  {
    this.combatIdle = false;
    if (this.setupPoint == null || (UnityEngine.Object) ((IEntityView) this.setupPoint).GameObject == (UnityEngine.Object) null)
      return;
    this.preset = ((IEntityView) this.setupPoint).GameObject.GetComponent<IdlePresetObject>();
    this.SetIdle();
    ((IEntityView) this.setupPoint).OnGameObjectChangedEvent -= new Action(this.OnSetupPointViewChanged);
  }

  protected void SetIdle()
  {
    if ((UnityEngine.Object) this.preset == (UnityEngine.Object) null)
    {
      GameObject gameObject = ((IEntityView) this.setupPoint).GameObject;
      if (!((UnityEngine.Object) gameObject != (UnityEngine.Object) null))
        return;
      this.animatorState.ControlMovableState = AnimatorState45.MovableState45.Idle;
      this.GameObject.transform.position = gameObject.transform.position;
      this.GameObject.transform.rotation = gameObject.transform.rotation;
    }
    else
    {
      if (this.preset.PresetAnimation == IdlePresetEnum.StandAggressive || this.preset.PresetAnimation == IdlePresetEnum.StandSurrender)
      {
        this.combatIdle = true;
        this.animatorState.ResetTrigger("Fight.Triggers/CancelWalk");
        if ((UnityEngine.Object) this.weaponService != (UnityEngine.Object) null)
        {
          this.weaponService.Weapon = WeaponEnum.Hands;
          this.weaponService.SwitchWeaponOnImmediate();
        }
        this.animatorState.MovableSpeed = 0.0f;
        if (this.preset.PresetAnimation == IdlePresetEnum.StandSurrender)
          this.animatorState.SetTrigger("Fight.Triggers/TakeMyMoneyImmediate");
      }
      else
      {
        if ((UnityEngine.Object) this.weaponService != (UnityEngine.Object) null)
          this.weaponService.SwitchWeaponOffImmediate();
        POIAnimationEnum poiAnimationEnum = this.preset.GetPOIAnimationEnum();
        if (poiAnimationEnum == POIAnimationEnum.Unknown)
        {
          this.animatorState.ControlMovableState = AnimatorState45.MovableState45.Idle;
        }
        else
        {
          POIAnimationSetupBase animationSetup = this.poiSetup.GetAnimationSetup(poiAnimationEnum);
          if (animationSetup != null)
          {
            this.animationIndex = !this.preset.RandomAnimationIndex ? this.preset.AnimationIndex : UnityEngine.Random.Range(0, animationSetup.Elements.Count);
            this.animationsCount = 1;
            if (animationSetup.Elements.Count > this.animationIndex && animationSetup.Elements[this.animationIndex] is POIAnimationSetupElementSlow)
              this.animationsCount = (animationSetup.Elements[this.animationIndex] as POIAnimationSetupElementSlow).MiddleAnimationClips.Count;
            this.animatorState.ControlMovableState = AnimatorState45.MovableState45.IdlePreset;
            this.animatorState.ControlPOIAnimationIndex = this.animationIndex;
            this.animatorState.ControlPOIMiddleAnimationsCount = this.animationsCount;
            this.animatorState.ControlPOIStartFromMiddle = true;
            switch (poiAnimationEnum)
            {
              case POIAnimationEnum.S_SitAtDesk:
                this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_SitAtDesk;
                break;
              case POIAnimationEnum.S_SitOnBench:
                this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_SitOnBench;
                if ((UnityEngine.Object) this.enemy != (UnityEngine.Object) null)
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
                if ((UnityEngine.Object) this.enemy != (UnityEngine.Object) null)
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
              case POIAnimationEnum.S_SitAtDeskRight:
                this.animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_SitAtDeskRight;
                break;
            }
          }
        }
      }
      this.GameObject.transform.position = this.preset.transform.position;
      this.GameObject.transform.rotation = this.preset.transform.rotation;
      if (this.preset.MakeObstacle)
        this.CreateObstacle();
      this.positionsAreSet = false;
    }
  }

  public void OnAnimatorMove()
  {
    if (this.failed)
      return;
    if ((UnityEngine.Object) this.agent != (UnityEngine.Object) null)
      this.agent.nextPosition = this.animator.rootPosition;
    this.GameObject.transform.position = this.animator.rootPosition;
    this.GameObject.transform.rotation = this.animator.rootRotation;
  }

  public void Shutdown()
  {
    if (this.failed)
      return;
    if (this.combatIdle)
      this.animatorState.SetTrigger("Fight.Triggers/CancelWalk");
    if ((bool) (UnityEngine.Object) this.rigidbody)
      this.rigidbody.isKinematic = this.initiallyKinematic;
    if ((UnityEngine.Object) this.agent != (UnityEngine.Object) null)
      this.agent.enabled = this.initiallyNavmesh;
    if (this.setupPoint != null)
      ((IEntityView) this.setupPoint).OnGameObjectChangedEvent -= new Action(this.OnSetupPointViewChanged);
    NavMeshObstacle obstacle = this.pivot.GetObstacle();
    if ((UnityEngine.Object) obstacle != (UnityEngine.Object) null)
      obstacle.enabled = false;
    this.animator.updateMode = this.initialAnimatorUpdateMode;
    if ((UnityEngine.Object) this.weaponService != (UnityEngine.Object) null)
      this.weaponService.Weapon = this.npcState.Weapon;
    if ((UnityEngine.Object) this.enemy != (UnityEngine.Object) null)
      this.enemy.CanPlayReactionAnimation = this.couldPlayReactionAnimation;
    this.npcState.NeedExtraExitPOI = this.neededExtraExitPOI;
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
    this.timeToNextRandomAnimationSet -= Time.deltaTime;
    if ((double) this.timeToNextRandomAnimationSet <= 0.0)
    {
      this.timeToNextRandomAnimationSet = this.timeToNextRandomAnimationSetMax;
      this.SetRandomNextAnimation();
    }
    if (!this.positionsAreSet)
    {
      this.positionsAreSet = true;
      this.SetPositions();
    }
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused || !this.sayReplics)
      return;
    this.timeToNextReplic -= Time.deltaTime;
    if ((double) this.timeToNextReplic <= 0.0)
    {
      this.timeToNextReplic = UnityEngine.Random.Range(ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMin, ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMax);
      NPCStateHelper.SayIdleReplic(this.npcState.Owner);
    }
  }

  private void SetPositions()
  {
    if (!((UnityEngine.Object) this.preset != (UnityEngine.Object) null))
      return;
    if ((UnityEngine.Object) this.GameObject != (UnityEngine.Object) null)
    {
      this.GameObject.transform.position = this.preset.transform.position;
      this.GameObject.transform.rotation = this.preset.transform.rotation;
    }
    if ((UnityEngine.Object) this.animator != (UnityEngine.Object) null)
    {
      this.animator.rootPosition = this.preset.transform.position;
      this.animator.rootRotation = this.preset.transform.rotation;
    }
  }

  private void SetRandomNextAnimation()
  {
    this.animator.SetInteger("Movable.POI.AnimationIndex2", UnityEngine.Random.Range(0, this.animator.GetInteger("Movable.POI.MiddleAnimationsCount")));
  }

  public Vector3 GetSyncBackPosition() => this.GameObject.transform.position;

  public POIAnimationEnum GetPoiType() => this.animation;

  public void OnLodStateChanged(bool inLodState)
  {
    this.npcState.AnimatorEnabled = !inLodState;
    EffectsComponent component = this.npcState.Owner?.GetComponent<EffectsComponent>();
    if (component == null)
      return;
    component.Disabled = inLodState;
  }
}
