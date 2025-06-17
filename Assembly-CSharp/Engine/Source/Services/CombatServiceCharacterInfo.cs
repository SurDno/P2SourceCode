using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using UnityEngine;

namespace Engine.Source.Services
{
  public class CombatServiceCharacterInfo : IChangeParameterListener
  {
    public EnemyBase Character;
    public bool WasLooted;
    public bool WasBeaten;
    public List<CombatServiceCharacterOrder> Orders = [];
    public Vector3 FightStartPosition;
    public Vector3 LastGotHitPosition;
    public List<CombatCry> HearedCries = [];
    private CombatServiceCharacterStateEnum state;
    private IEntity entity;
    private CombatServiceCharacterInfo currentEnemy;
    private List<CombatServiceCharacterInfo> personalAttackEnemies = [];
    private List<CombatServiceCharacterInfo> personalFearEnemies = [];
    private List<CombatServiceCharacterInfo> attackCharactersNearby = [];
    private List<CombatServiceCharacterInfo> fearCharactersNearby = [];
    private LocationItemComponent location;
    private NpcControllerComponent npcController;
    private PlayerControllerComponent playerController;
    private bool isIndoors;
    private DetectorComponent detector;
    private ParametersComponent parameters;
    private IParameter<bool> deadParameter;
    private IParameter<FractionEnum> fraction;
    private FractionSettings fractionSettings;
    private bool isPlayer;
    private IParameter<CombatStyleEnum> combatStyleParameter;
    private IndividualCombatSettings combatSettings;
    private CombatService combatService;
    private Dictionary<IEntity, CombatServiceCharacterInfo> visibleCharacters;
    private Dictionary<IEntity, CombatServiceCharacterInfo> hearableCharacters;
    private float stateSetTime;
    private bool needRecountEnemies;

    public CombatServiceCharacterStateEnum State
    {
      get => state;
      set
      {
        if (Character != null)
        {
          bool flag = false;
          if (value == CombatServiceCharacterStateEnum.Fight)
            flag = true;
          if (value == CombatServiceCharacterStateEnum.Loot)
            flag = true;
          if (value == CombatServiceCharacterStateEnum.Escape)
            flag = true;
          if (value == CombatServiceCharacterStateEnum.Surrender)
            flag = true;
          if (!IsPlayer)
            Character.IsFighting = flag;
          if (state != value && Character is NPCEnemy)
          {
            (Character as NPCEnemy).BlockStance = false;
            (Character as NPCEnemy).QuickBlockProbability = 0.0f;
            (Character as NPCEnemy).DodgeProbability = 0.0f;
          }
        }
        state = value;
        stateSetTime = Time.time;
      }
    }

    public bool IsCombatIgnored => Character.IsCombatIgnored;

    public bool IsImmortal => Character.IsImmortal;

    public CombatServiceCharacterInfo CurrentEnemy
    {
      get => currentEnemy;
      set
      {
        currentEnemy = value;
        Character.Enemy = currentEnemy?.Character;
        if (currentEnemy != null)
          return;
        Character.RetreatAngle = new float?();
        Character.RotationTarget = null;
        Character.RotateByPath = false;
      }
    }

    public List<CombatServiceCharacterInfo> PersonalAttackEnemies => personalAttackEnemies;

    public List<CombatServiceCharacterInfo> PersonalFearEnemies => personalFearEnemies;

    public IEntity Entity
    {
      get
      {
        if (entity == null && Character != null)
          Entity = Character.Owner;
        return entity;
      }
      private set
      {
        entity = value;
        if (entity == null)
          return;
        Parameters = entity.GetComponent<ParametersComponent>();
        location = entity.GetComponent<LocationItemComponent>();
        if (location != null)
        {
          IsIndoors = location.IsIndoor;
          location.OnChangeLocation -= ChangedLocation;
          location.OnChangeLocation += ChangedLocation;
        }
        detector = entity.GetComponent<DetectorComponent>();
        if (detector != null)
          GetDetectedCharacters();
        npcController = entity.GetComponent<NpcControllerComponent>();
        playerController = entity.GetComponent<PlayerControllerComponent>();
        combatService.AddCharacterToDictionary(entity, this);
        ((IEntityView) entity).OnGameObjectChangedEvent -= OnGameObjectChanged;
        ((IEntityView) entity).OnGameObjectChangedEvent += OnGameObjectChanged;
      }
    }

    public bool IsIndoors
    {
      get => isIndoors;
      private set
      {
        if (isIndoors != value)
          combatService.ChangedLocation(this);
        isIndoors = value;
      }
    }

    public ParametersComponent Parameters
    {
      get
      {
        if (entity == null)
          Entity = Character.Owner;
        return parameters;
      }
      private set
      {
        parameters = value;
        if (parameters == null)
          return;
        fraction = parameters.GetByName<FractionEnum>(ParameterNameEnum.Fraction);
        if (fraction != null)
        {
          fraction.RemoveListener(this);
          fraction.AddListener(this);
        }
        deadParameter = parameters.GetByName<bool>(ParameterNameEnum.Dead);
        if (deadParameter != null)
        {
          deadParameter.RemoveListener(this);
          deadParameter.AddListener(this);
        }
        combatStyleParameter = parameters.GetByName<CombatStyleEnum>(ParameterNameEnum.CombatStyle);
        if (combatStyleParameter != null)
        {
          combatStyleParameter.RemoveListener(this);
          combatStyleParameter.AddListener(this);
        }
      }
    }

    public DetectorComponent Detector
    {
      get
      {
        if (entity == null)
          Entity = Character.Owner;
        return detector;
      }
    }

    public IParameter<bool> DeadParameter
    {
      get
      {
        if (entity == null)
          Entity = Character.Owner;
        return deadParameter;
      }
    }

    public bool IsDead
    {
      get
      {
        if (entity == null)
          Entity = Character.Owner;
        return deadParameter != null && deadParameter.Value;
      }
    }

    public bool CanFight
    {
      get
      {
        if (combatSettings == null)
          Entity = Character.Owner;
        return combatSettings == null || combatSettings.CanFight;
      }
    }

    public FractionEnum Fraction
    {
      get
      {
        if (entity == null || fraction == null)
          Entity = Character.Owner;
        return fraction == null ? FractionEnum.None : fraction.Value;
      }
    }

    public bool IsPlayer => isPlayer;

    public bool CanLoot => CombatSettings != null && CombatSettings.LootAI != null;

    public NpcControllerComponent NpcController
    {
      get
      {
        if (IsPlayer)
          return null;
        if (entity == null || npcController == null)
          Entity = Character.Owner;
        return npcController;
      }
    }

    public PlayerControllerComponent PlayerController
    {
      get
      {
        if (!IsPlayer)
          return null;
        if (entity == null || playerController == null)
          Entity = Character.Owner;
        return playerController;
      }
    }

    public IndividualCombatSettings CombatSettings
    {
      get
      {
        if (entity == null || combatSettings == null)
        {
          Entity = Character.Owner;
          CombatStyleEnum styleName = combatStyleParameter == null ? CombatStyleEnum.Default : combatStyleParameter.Value;
          combatSettings = ScriptableObjectInstance<FightSettingsData>.Instance.IndividualCombatSettings.Find(x => x.Name == styleName);
        }
        return combatSettings;
      }
    }

    public CombatServiceCharacterInfo(EnemyBase character, CombatService combatService)
    {
      Character = character;
      this.combatService = combatService;
      if (character is PlayerEnemy)
        isPlayer = true;
      visibleCharacters = new Dictionary<IEntity, CombatServiceCharacterInfo>();
      hearableCharacters = new Dictionary<IEntity, CombatServiceCharacterInfo>();
      Entity = character.Owner;
      personalAttackEnemies = [];
      personalFearEnemies = [];
      WasLooted = false;
      WasBeaten = false;
      stateSetTime = Time.time;
      needRecountEnemies = true;
    }

    public void ClearCombatInfo()
    {
    }

    public void Clear()
    {
      if (deadParameter != null)
        deadParameter.RemoveListener(this);
      if (combatStyleParameter != null)
        combatStyleParameter.RemoveListener(this);
      if (location != null)
        location.OnChangeLocation -= ChangedLocation;
      if (detector != null)
      {
        detector.OnSee -= OnSee;
        detector.OnStopSee -= OnStopSee;
        detector.OnHear -= OnHear;
        detector.OnStopHear -= OnStopHear;
      }
      if (fraction != null)
        fraction.RemoveListener(this);
      if (entity == null)
        return;
      ((IEntityView) entity).OnGameObjectChangedEvent -= OnGameObjectChanged;
    }

    private void ChangedLocation(ILocationItemComponent locItem, ILocationComponent loc)
    {
      IsIndoors = locItem.IsIndoor;
    }

    private void GetDetectedCharacters()
    {
      if (detector == null)
        return;
      detector.OnSee -= OnSee;
      detector.OnSee += OnSee;
      detector.OnStopSee -= OnStopSee;
      detector.OnStopSee += OnStopSee;
      detector.OnHear -= OnHear;
      detector.OnHear += OnHear;
      detector.OnStopHear -= OnStopHear;
      detector.OnStopHear += OnStopHear;
      foreach (IDetectableComponent detectable in detector.Visible)
      {
        if (detectable != null && !detectable.IsDisposed)
          OnSee(detectable);
      }
      foreach (IDetectableComponent detectable in detector.Hearing)
      {
        if (detectable != null && !detectable.IsDisposed)
          OnHear(detectable);
      }
    }

    private void OnSee(IDetectableComponent detectable)
    {
      IEntity interestingEntity = GetDetectorInterestingEntity(detectable);
      if (interestingEntity != null && !visibleCharacters.ContainsKey(interestingEntity))
      {
        CombatServiceCharacterInfo character = GetCharacter(interestingEntity);
        visibleCharacters.Add(interestingEntity, character);
      }
      needRecountEnemies = true;
    }

    private void OnStopSee(IDetectableComponent detectable)
    {
      IEntity interestingEntity = GetDetectorInterestingEntity(detectable);
      if (interestingEntity != null && visibleCharacters.ContainsKey(interestingEntity))
        visibleCharacters.Remove(interestingEntity);
      needRecountEnemies = true;
    }

    private void OnHear(IDetectableComponent detectable)
    {
      IEntity interestingEntity = GetDetectorInterestingEntity(detectable);
      if (interestingEntity != null && !hearableCharacters.ContainsKey(interestingEntity))
      {
        CombatServiceCharacterInfo character = GetCharacter(interestingEntity);
        hearableCharacters.Add(interestingEntity, character);
      }
      needRecountEnemies = true;
    }

    private void OnStopHear(IDetectableComponent detectable)
    {
      IEntity interestingEntity = GetDetectorInterestingEntity(detectable);
      if (interestingEntity != null && hearableCharacters.ContainsKey(interestingEntity))
        hearableCharacters.Remove(interestingEntity);
      needRecountEnemies = true;
    }

    private void OnGameObjectChanged() => needRecountEnemies = true;

    private void RenewDetector()
    {
      if (visibleCharacters.ContainsValue(null))
      {
        foreach (IEntity entity in new List<IEntity>(visibleCharacters.Keys))
        {
          if (visibleCharacters[entity] == null)
            visibleCharacters[entity] = GetCharacter(entity);
        }
      }
      if (!hearableCharacters.ContainsValue(null))
        return;
      foreach (IEntity entity in new List<IEntity>(hearableCharacters.Keys))
      {
        if (hearableCharacters[entity] == null)
          hearableCharacters[entity] = GetCharacter(entity);
      }
    }

    private IEntity GetDetectorInterestingEntity(IDetectableComponent detectable)
    {
      IEntity owner = detectable.Owner;
      if (owner == null)
        return null;
      NpcControllerComponent component1 = owner.GetComponent<NpcControllerComponent>();
      PlayerControllerComponent component2 = owner.GetComponent<PlayerControllerComponent>();
      return component1 == null && component2 == null ? null : owner;
    }

    private CombatServiceCharacterInfo GetCharacter(IEntity entity)
    {
      return combatService.GetCharacterInfo(entity);
    }

    public bool CanSee(CombatServiceCharacterInfo character)
    {
      return visibleCharacters.ContainsValue(character);
    }

    public bool CanHear(CombatServiceCharacterInfo character)
    {
      return hearableCharacters.ContainsValue(character);
    }

    public void RecountEnemies()
    {
      RenewDetector();
      List<CombatServiceCharacterInfo> serviceCharacterInfoList = [];
      serviceCharacterInfoList.AddRange(hearableCharacters.Values);
      foreach (CombatServiceCharacterInfo serviceCharacterInfo in visibleCharacters.Values)
      {
        if (!serviceCharacterInfoList.Contains(serviceCharacterInfo))
          serviceCharacterInfoList.Add(serviceCharacterInfo);
      }
      List<FractionEnum> fraction1 = RelatedFractionUtility.GetFraction(entity, FractionRelationEnum.FearOnSee);
      List<FractionEnum> fraction2 = RelatedFractionUtility.GetFraction(entity, FractionRelationEnum.AttackOnSee);
      List<FractionEnum> fraction3 = RelatedFractionUtility.GetFraction(entity, FractionRelationEnum.AttackOnSeeInfected);
      fearCharactersNearby = [];
      attackCharactersNearby = [];
      if (IsCombatIgnored)
        return;
      foreach (CombatServiceCharacterInfo serviceCharacterInfo in serviceCharacterInfoList)
      {
        if (serviceCharacterInfo != null && !serviceCharacterInfo.IsDead && serviceCharacterInfo.IsIndoors == isIndoors && !serviceCharacterInfo.IsCombatIgnored)
        {
          if (!IsPlayer && PersonalFearEnemies != null && PersonalFearEnemies.Contains(serviceCharacterInfo) && !serviceCharacterInfo.WasBeaten)
            fearCharactersNearby.Add(serviceCharacterInfo);
          else if (!IsPlayer && PersonalAttackEnemies != null && PersonalAttackEnemies.Contains(serviceCharacterInfo))
          {
            attackCharactersNearby.Add(serviceCharacterInfo);
          }
          else
          {
            FractionEnum targetFraction = FractionsHelper.GetTargetFraction(serviceCharacterInfo.entity, entity);
            if (targetFraction != FractionEnum.None)
            {
              if (fraction1 != null && fraction1.Contains(targetFraction) && !serviceCharacterInfo.WasBeaten)
                fearCharactersNearby.Add(serviceCharacterInfo);
              else if (fraction2 != null && fraction2.Contains(targetFraction))
                attackCharactersNearby.Add(serviceCharacterInfo);
              else if (fraction3 != null && fraction3.Contains(targetFraction))
              {
                IParameter<float> byName = serviceCharacterInfo.Parameters.GetByName<float>(ParameterNameEnum.Infection);
                float num = fractionSettings == null ? 0.1f : fractionSettings.InfectionThreshold;
                if (byName != null && byName.Value > (double) num)
                  attackCharactersNearby.Add(serviceCharacterInfo);
              }
            }
          }
        }
      }
    }

    public CombatServiceCharacterInfo FearEnemyNearby()
    {
      fearCharactersNearby.RemoveAll(x => x.IsDead);
      return fearCharactersNearby.Count == 0 ? null : fearCharactersNearby[0];
    }

    public CombatServiceCharacterInfo AttackEnemyNearby()
    {
      attackCharactersNearby.RemoveAll(x => x.IsDead);
      return attackCharactersNearby.Count == 0 ? null : attackCharactersNearby[0];
    }

    public float GetTimeFromLastOrder() => Time.time - stateSetTime;

    public void FireControllerCombatAction(CombatActionEnum action, IEntity target)
    {
      if (IsPlayer && PlayerController != null)
      {
        PlayerController.FireCombatAction(action, target);
      }
      else
      {
        if (NpcController == null)
          return;
        NpcController.FireCombatAction(action, target);
      }
    }

    public void Update()
    {
      if (!needRecountEnemies)
        return;
      RecountEnemies();
      needRecountEnemies = false;
    }

    public void OnParameterChanged(IParameter parameter)
    {
      if (parameter.Name == ParameterNameEnum.Fraction)
      {
        fractionSettings = ScriptableObjectInstance<FractionsSettingsData>.Instance.Fractions.Find(x => x.Name == ((IParameter<FractionEnum>) parameter).Value);
        needRecountEnemies = true;
      }
      else if (parameter.Name == ParameterNameEnum.Dead)
      {
        CombatService service = ServiceLocator.GetService<CombatService>();
        if (service == null || !((IParameter<bool>) parameter).Value)
          return;
        service.Died(this);
      }
      else
      {
        if (parameter.Name != ParameterNameEnum.CombatStyle)
          return;
        CombatStyleEnum styleName = combatStyleParameter == null ? CombatStyleEnum.Default : combatStyleParameter.Value;
        combatSettings = ScriptableObjectInstance<FightSettingsData>.Instance.IndividualCombatSettings.Find(x => x.Name == styleName);
      }
    }
  }
}
