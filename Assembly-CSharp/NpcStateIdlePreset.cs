using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Impl.Tasks;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Settings.External;
using Inspectors;
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
  private bool combatIdle;
  private IEntity setupPoint;
  private float timeToNextRandomAnimationSet;
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
    if (inited)
      return true;
    behavior = pivot.GetBehavior();
    poiSetup = GameObject.GetComponent<POISetup>();
    agent = pivot.GetAgent();
    rigidbody = pivot.GetRigidbody();
    weaponService = pivot.GetNpcWeaponService();
    animator = pivot.GetAnimator();
    enemy = pivot.GetNpcEnemy();
    if (animator == null)
    {
      Debug.LogError("Null animator " + GameObject.name, GameObject);
      Debug.LogError("Null animator " + GameObject.GetFullName());
      failed = true;
      return false;
    }
    animatorState = AnimatorState45.GetAnimatorState(animator);
    if (poiSetup == null)
    {
      Debug.LogError("No PoiSetup " + GameObject.name, GameObject);
      failed = true;
      return false;
    }
    failed = false;
    inited = true;
    return true;
  }

  public NpcStateIdlePreset(NpcState npcState, Pivot pivot)
  {
    GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate()
  {
    if (!TryInit())
      return;
    if ((bool) (Object) rigidbody)
    {
      initiallyKinematic = rigidbody.isKinematic;
      rigidbody.isKinematic = true;
    }
    if (agent != null)
    {
      initiallyNavmesh = agent.isActiveAndEnabled;
      agent.enabled = false;
    }
    setupPoint = npcState.Owner.GetComponent<NavigationComponent>().SetupPoint;
    if (setupPoint == null)
      return;
    if (weaponService != null)
      weaponService.Weapon = WeaponEnum.Unknown;
    if (enemy != null)
      couldPlayReactionAnimation = enemy.CanPlayReactionAnimation;
    neededExtraExitPOI = npcState.NeedExtraExitPOI;
    npcState.NeedExtraExitPOI = false;
    if (((IEntityView) setupPoint).GameObject == null)
    {
      ((IEntityView) setupPoint).OnGameObjectChangedEvent -= OnSetupPointViewChanged;
      ((IEntityView) setupPoint).OnGameObjectChangedEvent += OnSetupPointViewChanged;
    }
    else
      OnSetupPointViewChanged();
    initialAnimatorUpdateMode = animator.updateMode;
    animator.updateMode = AnimatorUpdateMode.Normal;
    positionsAreSet = false;
    if (npcState.Owner == null)
      return;
    ParametersComponent component = npcState.Owner.GetComponent<ParametersComponent>();
    if (component != null)
    {
      IParameter<bool> byName = component.GetByName<bool>(ParameterNameEnum.SayReplicsInIdle);
      if (byName != null)
      {
        sayReplics = byName.Value;
        if (sayReplics)
          timeToNextReplic = Random.Range(ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMin, ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMax);
      }
    }
  }

  private void CreateObstacle()
  {
    agent.enabled = false;
    NavMeshObstacle obstacle = pivot.GetObstacle();
    if (!(obstacle != null))
      return;
    obstacle.enabled = true;
    obstacle.carving = true;
    obstacle.radius = agent.radius;
  }

  private void OnSetupPointViewChanged()
  {
    combatIdle = false;
    if (setupPoint == null || ((IEntityView) setupPoint).GameObject == null)
      return;
    preset = ((IEntityView) setupPoint).GameObject.GetComponent<IdlePresetObject>();
    SetIdle();
    ((IEntityView) setupPoint).OnGameObjectChangedEvent -= OnSetupPointViewChanged;
  }

  protected void SetIdle()
  {
    if (preset == null)
    {
      GameObject gameObject = ((IEntityView) setupPoint).GameObject;
      if (!(gameObject != null))
        return;
      animatorState.ControlMovableState = AnimatorState45.MovableState45.Idle;
      GameObject.transform.position = gameObject.transform.position;
      GameObject.transform.rotation = gameObject.transform.rotation;
    }
    else
    {
      if (preset.PresetAnimation == IdlePresetEnum.StandAggressive || preset.PresetAnimation == IdlePresetEnum.StandSurrender)
      {
        combatIdle = true;
        animatorState.ResetTrigger("Fight.Triggers/CancelWalk");
        if (weaponService != null)
        {
          weaponService.Weapon = WeaponEnum.Hands;
          weaponService.SwitchWeaponOnImmediate();
        }
        animatorState.MovableSpeed = 0.0f;
        if (preset.PresetAnimation == IdlePresetEnum.StandSurrender)
          animatorState.SetTrigger("Fight.Triggers/TakeMyMoneyImmediate");
      }
      else
      {
        if (weaponService != null)
          weaponService.SwitchWeaponOffImmediate();
        POIAnimationEnum poiAnimationEnum = preset.GetPOIAnimationEnum();
        if (poiAnimationEnum == POIAnimationEnum.Unknown)
        {
          animatorState.ControlMovableState = AnimatorState45.MovableState45.Idle;
        }
        else
        {
          POIAnimationSetupBase animationSetup = poiSetup.GetAnimationSetup(poiAnimationEnum);
          if (animationSetup != null)
          {
            animationIndex = !preset.RandomAnimationIndex ? preset.AnimationIndex : Random.Range(0, animationSetup.Elements.Count);
            animationsCount = 1;
            if (animationSetup.Elements.Count > animationIndex && animationSetup.Elements[animationIndex] is POIAnimationSetupElementSlow)
              animationsCount = (animationSetup.Elements[animationIndex] as POIAnimationSetupElementSlow).MiddleAnimationClips.Count;
            animatorState.ControlMovableState = AnimatorState45.MovableState45.IdlePreset;
            animatorState.ControlPOIAnimationIndex = animationIndex;
            animatorState.ControlPOIMiddleAnimationsCount = animationsCount;
            animatorState.ControlPOIStartFromMiddle = true;
            switch (poiAnimationEnum)
            {
              case POIAnimationEnum.S_SitAtDesk:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_SitAtDesk;
                break;
              case POIAnimationEnum.S_SitOnBench:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_SitOnBench;
                if (enemy != null)
                  enemy.CanPlayReactionAnimation = false;
                npcState.NeedExtraExitPOI = true;
                break;
              case POIAnimationEnum.S_LeanOnWall:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_LeanOnWall;
                break;
              case POIAnimationEnum.S_LeanOnTable:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_LeanOnTable;
                break;
              case POIAnimationEnum.S_SitNearWall:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_SitNearWall;
                if (enemy != null)
                  enemy.CanPlayReactionAnimation = false;
                npcState.NeedExtraExitPOI = true;
                break;
              case POIAnimationEnum.S_LieOnBed:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_LieOnBed;
                break;
              case POIAnimationEnum.S_NearFire:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_NearFire;
                break;
              case POIAnimationEnum.Q_ViewPoster:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ViewPoster;
                break;
              case POIAnimationEnum.Q_LookOutOfTheWindow:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_LookOutOfTheWindow;
                break;
              case POIAnimationEnum.Q_LookUnder:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_LookUnder;
                break;
              case POIAnimationEnum.Q_LookIntoTheWindow:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_LookIntoTheWindow;
                break;
              case POIAnimationEnum.Q_ActionWithWall:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ActionWithWall;
                break;
              case POIAnimationEnum.Q_ActionWithTable:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ActionWithTable;
                break;
              case POIAnimationEnum.Q_ActionWithWardrobe:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ActionWithWardrobe;
                break;
              case POIAnimationEnum.Q_ActionWithShelves:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ActionWithShelves;
                break;
              case POIAnimationEnum.Q_ActionWithNightstand:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ActionWithNightstand;
                break;
              case POIAnimationEnum.Q_ActionOnFloor:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_ActionOnFloor;
                break;
              case POIAnimationEnum.S_ActionOnFloor:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_ActionOnFloor;
                break;
              case POIAnimationEnum.Q_Idle:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_Idle;
                break;
              case POIAnimationEnum.Q_NearFire:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.Q_NearFire;
                break;
              case POIAnimationEnum.S_Dialog:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_Dialog;
                break;
              case POIAnimationEnum.S_SitAtDeskRight:
                animatorState.ControlPOIState = MecanimKinds.MovablePOIStateKind.S_SitAtDeskRight;
                break;
            }
          }
        }
      }
      GameObject.transform.position = preset.transform.position;
      GameObject.transform.rotation = preset.transform.rotation;
      if (preset.MakeObstacle)
        CreateObstacle();
      positionsAreSet = false;
    }
  }

  public void OnAnimatorMove()
  {
    if (failed)
      return;
    if (agent != null)
      agent.nextPosition = animator.rootPosition;
    GameObject.transform.position = animator.rootPosition;
    GameObject.transform.rotation = animator.rootRotation;
  }

  public void Shutdown()
  {
    if (failed)
      return;
    if (combatIdle)
      animatorState.SetTrigger("Fight.Triggers/CancelWalk");
    if ((bool) (Object) rigidbody)
      rigidbody.isKinematic = initiallyKinematic;
    if (agent != null)
      agent.enabled = initiallyNavmesh;
    if (setupPoint != null)
      ((IEntityView) setupPoint).OnGameObjectChangedEvent -= OnSetupPointViewChanged;
    NavMeshObstacle obstacle = pivot.GetObstacle();
    if (obstacle != null)
      obstacle.enabled = false;
    animator.updateMode = initialAnimatorUpdateMode;
    if (weaponService != null)
      weaponService.Weapon = npcState.Weapon;
    if (enemy != null)
      enemy.CanPlayReactionAnimation = couldPlayReactionAnimation;
    npcState.NeedExtraExitPOI = neededExtraExitPOI;
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!failed)
      ;
  }

  public void Update()
  {
    if (failed)
      return;
    timeToNextRandomAnimationSet -= Time.deltaTime;
    if (timeToNextRandomAnimationSet <= 0.0)
    {
      timeToNextRandomAnimationSet = timeToNextRandomAnimationSetMax;
      SetRandomNextAnimation();
    }
    if (!positionsAreSet)
    {
      positionsAreSet = true;
      SetPositions();
    }
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused || !sayReplics)
      return;
    timeToNextReplic -= Time.deltaTime;
    if (timeToNextReplic <= 0.0)
    {
      timeToNextReplic = Random.Range(ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMin, ExternalSettingsInstance<ExternalCommonSettings>.Instance.IdleReplicsFrequencyMax);
      NPCStateHelper.SayIdleReplic(npcState.Owner);
    }
  }

  private void SetPositions()
  {
    if (!(preset != null))
      return;
    if (GameObject != null)
    {
      GameObject.transform.position = preset.transform.position;
      GameObject.transform.rotation = preset.transform.rotation;
    }
    if (animator != null)
    {
      animator.rootPosition = preset.transform.position;
      animator.rootRotation = preset.transform.rotation;
    }
  }

  private void SetRandomNextAnimation()
  {
    animator.SetInteger("Movable.POI.AnimationIndex2", Random.Range(0, animator.GetInteger("Movable.POI.MiddleAnimationsCount")));
  }

  public Vector3 GetSyncBackPosition() => GameObject.transform.position;

  public POIAnimationEnum GetPoiType() => animation;

  public void OnLodStateChanged(bool inLodState)
  {
    npcState.AnimatorEnabled = !inLodState;
    EffectsComponent component = npcState.Owner?.GetComponent<EffectsComponent>();
    if (component == null)
      return;
    component.Disabled = inLodState;
  }
}
