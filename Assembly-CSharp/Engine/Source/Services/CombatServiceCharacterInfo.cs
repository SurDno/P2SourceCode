using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Source.Services
{
  public class CombatServiceCharacterInfo : IChangeParameterListener
  {
    public EnemyBase Character;
    public bool WasLooted;
    public bool WasBeaten;
    public List<CombatServiceCharacterOrder> Orders = new List<CombatServiceCharacterOrder>();
    public Vector3 FightStartPosition;
    public Vector3 LastGotHitPosition;
    public List<CombatCry> HearedCries = new List<CombatCry>();
    private CombatServiceCharacterStateEnum state;
    private IEntity entity;
    private CombatServiceCharacterInfo currentEnemy;
    private List<CombatServiceCharacterInfo> personalAttackEnemies = new List<CombatServiceCharacterInfo>();
    private List<CombatServiceCharacterInfo> personalFearEnemies = new List<CombatServiceCharacterInfo>();
    private List<CombatServiceCharacterInfo> attackCharactersNearby = new List<CombatServiceCharacterInfo>();
    private List<CombatServiceCharacterInfo> fearCharactersNearby = new List<CombatServiceCharacterInfo>();
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
      get => this.state;
      set
      {
        if ((UnityEngine.Object) this.Character != (UnityEngine.Object) null)
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
          if (!this.IsPlayer)
            this.Character.IsFighting = flag;
          if (this.state != value && this.Character is NPCEnemy)
          {
            (this.Character as NPCEnemy).BlockStance = false;
            (this.Character as NPCEnemy).QuickBlockProbability = 0.0f;
            (this.Character as NPCEnemy).DodgeProbability = 0.0f;
          }
        }
        this.state = value;
        this.stateSetTime = Time.time;
      }
    }

    public bool IsCombatIgnored => this.Character.IsCombatIgnored;

    public bool IsImmortal => this.Character.IsImmortal;

    public CombatServiceCharacterInfo CurrentEnemy
    {
      get => this.currentEnemy;
      set
      {
        this.currentEnemy = value;
        this.Character.Enemy = this.currentEnemy?.Character;
        if (this.currentEnemy != null)
          return;
        this.Character.RetreatAngle = new float?();
        this.Character.RotationTarget = (Transform) null;
        this.Character.RotateByPath = false;
      }
    }

    public List<CombatServiceCharacterInfo> PersonalAttackEnemies => this.personalAttackEnemies;

    public List<CombatServiceCharacterInfo> PersonalFearEnemies => this.personalFearEnemies;

    public IEntity Entity
    {
      get
      {
        if (this.entity == null && (UnityEngine.Object) this.Character != (UnityEngine.Object) null)
          this.Entity = this.Character.Owner;
        return this.entity;
      }
      private set
      {
        this.entity = value;
        if (this.entity == null)
          return;
        this.Parameters = this.entity.GetComponent<ParametersComponent>();
        this.location = this.entity.GetComponent<LocationItemComponent>();
        if (this.location != null)
        {
          this.IsIndoors = this.location.IsIndoor;
          this.location.OnChangeLocation -= new Action<ILocationItemComponent, ILocationComponent>(this.ChangedLocation);
          this.location.OnChangeLocation += new Action<ILocationItemComponent, ILocationComponent>(this.ChangedLocation);
        }
        this.detector = this.entity.GetComponent<DetectorComponent>();
        if (this.detector != null)
          this.GetDetectedCharacters();
        this.npcController = this.entity.GetComponent<NpcControllerComponent>();
        this.playerController = this.entity.GetComponent<PlayerControllerComponent>();
        this.combatService.AddCharacterToDictionary(this.entity, this);
        ((IEntityView) this.entity).OnGameObjectChangedEvent -= new Action(this.OnGameObjectChanged);
        ((IEntityView) this.entity).OnGameObjectChangedEvent += new Action(this.OnGameObjectChanged);
      }
    }

    public bool IsIndoors
    {
      get => this.isIndoors;
      private set
      {
        if (this.isIndoors != value)
          this.combatService.ChangedLocation(this);
        this.isIndoors = value;
      }
    }

    public ParametersComponent Parameters
    {
      get
      {
        if (this.entity == null)
          this.Entity = this.Character.Owner;
        return this.parameters;
      }
      private set
      {
        this.parameters = value;
        if (this.parameters == null)
          return;
        this.fraction = this.parameters.GetByName<FractionEnum>(ParameterNameEnum.Fraction);
        if (this.fraction != null)
        {
          this.fraction.RemoveListener((IChangeParameterListener) this);
          this.fraction.AddListener((IChangeParameterListener) this);
        }
        this.deadParameter = this.parameters.GetByName<bool>(ParameterNameEnum.Dead);
        if (this.deadParameter != null)
        {
          this.deadParameter.RemoveListener((IChangeParameterListener) this);
          this.deadParameter.AddListener((IChangeParameterListener) this);
        }
        this.combatStyleParameter = this.parameters.GetByName<CombatStyleEnum>(ParameterNameEnum.CombatStyle);
        if (this.combatStyleParameter != null)
        {
          this.combatStyleParameter.RemoveListener((IChangeParameterListener) this);
          this.combatStyleParameter.AddListener((IChangeParameterListener) this);
        }
      }
    }

    public DetectorComponent Detector
    {
      get
      {
        if (this.entity == null)
          this.Entity = this.Character.Owner;
        return this.detector;
      }
    }

    public IParameter<bool> DeadParameter
    {
      get
      {
        if (this.entity == null)
          this.Entity = this.Character.Owner;
        return this.deadParameter;
      }
    }

    public bool IsDead
    {
      get
      {
        if (this.entity == null)
          this.Entity = this.Character.Owner;
        return this.deadParameter != null && this.deadParameter.Value;
      }
    }

    public bool CanFight
    {
      get
      {
        if (this.combatSettings == null)
          this.Entity = this.Character.Owner;
        return this.combatSettings == null || this.combatSettings.CanFight;
      }
    }

    public FractionEnum Fraction
    {
      get
      {
        if (this.entity == null || this.fraction == null)
          this.Entity = this.Character.Owner;
        return this.fraction == null ? FractionEnum.None : this.fraction.Value;
      }
    }

    public bool IsPlayer => this.isPlayer;

    public bool CanLoot
    {
      get => this.CombatSettings != null && (UnityEngine.Object) this.CombatSettings.LootAI != (UnityEngine.Object) null;
    }

    public NpcControllerComponent NpcController
    {
      get
      {
        if (this.IsPlayer)
          return (NpcControllerComponent) null;
        if (this.entity == null || this.npcController == null)
          this.Entity = this.Character.Owner;
        return this.npcController;
      }
    }

    public PlayerControllerComponent PlayerController
    {
      get
      {
        if (!this.IsPlayer)
          return (PlayerControllerComponent) null;
        if (this.entity == null || this.playerController == null)
          this.Entity = this.Character.Owner;
        return this.playerController;
      }
    }

    public IndividualCombatSettings CombatSettings
    {
      get
      {
        if (this.entity == null || this.combatSettings == null)
        {
          this.Entity = this.Character.Owner;
          CombatStyleEnum styleName = this.combatStyleParameter == null ? CombatStyleEnum.Default : this.combatStyleParameter.Value;
          this.combatSettings = ScriptableObjectInstance<FightSettingsData>.Instance.IndividualCombatSettings.Find((Predicate<IndividualCombatSettings>) (x => x.Name == styleName));
        }
        return this.combatSettings;
      }
    }

    public CombatServiceCharacterInfo(EnemyBase character, CombatService combatService)
    {
      this.Character = character;
      this.combatService = combatService;
      if (character is PlayerEnemy)
        this.isPlayer = true;
      this.visibleCharacters = new Dictionary<IEntity, CombatServiceCharacterInfo>();
      this.hearableCharacters = new Dictionary<IEntity, CombatServiceCharacterInfo>();
      this.Entity = character.Owner;
      this.personalAttackEnemies = new List<CombatServiceCharacterInfo>();
      this.personalFearEnemies = new List<CombatServiceCharacterInfo>();
      this.WasLooted = false;
      this.WasBeaten = false;
      this.stateSetTime = Time.time;
      this.needRecountEnemies = true;
    }

    public void ClearCombatInfo()
    {
    }

    public void Clear()
    {
      if (this.deadParameter != null)
        this.deadParameter.RemoveListener((IChangeParameterListener) this);
      if (this.combatStyleParameter != null)
        this.combatStyleParameter.RemoveListener((IChangeParameterListener) this);
      if (this.location != null)
        this.location.OnChangeLocation -= new Action<ILocationItemComponent, ILocationComponent>(this.ChangedLocation);
      if (this.detector != null)
      {
        this.detector.OnSee -= new Action<IDetectableComponent>(this.OnSee);
        this.detector.OnStopSee -= new Action<IDetectableComponent>(this.OnStopSee);
        this.detector.OnHear -= new Action<IDetectableComponent>(this.OnHear);
        this.detector.OnStopHear -= new Action<IDetectableComponent>(this.OnStopHear);
      }
      if (this.fraction != null)
        this.fraction.RemoveListener((IChangeParameterListener) this);
      if (this.entity == null)
        return;
      ((IEntityView) this.entity).OnGameObjectChangedEvent -= new Action(this.OnGameObjectChanged);
    }

    private void ChangedLocation(ILocationItemComponent locItem, ILocationComponent loc)
    {
      this.IsIndoors = locItem.IsIndoor;
    }

    private void GetDetectedCharacters()
    {
      if (this.detector == null)
        return;
      this.detector.OnSee -= new Action<IDetectableComponent>(this.OnSee);
      this.detector.OnSee += new Action<IDetectableComponent>(this.OnSee);
      this.detector.OnStopSee -= new Action<IDetectableComponent>(this.OnStopSee);
      this.detector.OnStopSee += new Action<IDetectableComponent>(this.OnStopSee);
      this.detector.OnHear -= new Action<IDetectableComponent>(this.OnHear);
      this.detector.OnHear += new Action<IDetectableComponent>(this.OnHear);
      this.detector.OnStopHear -= new Action<IDetectableComponent>(this.OnStopHear);
      this.detector.OnStopHear += new Action<IDetectableComponent>(this.OnStopHear);
      foreach (IDetectableComponent detectable in this.detector.Visible)
      {
        if (detectable != null && !detectable.IsDisposed)
          this.OnSee(detectable);
      }
      foreach (IDetectableComponent detectable in this.detector.Hearing)
      {
        if (detectable != null && !detectable.IsDisposed)
          this.OnHear(detectable);
      }
    }

    private void OnSee(IDetectableComponent detectable)
    {
      IEntity interestingEntity = this.GetDetectorInterestingEntity(detectable);
      if (interestingEntity != null && !this.visibleCharacters.ContainsKey(interestingEntity))
      {
        CombatServiceCharacterInfo character = this.GetCharacter(interestingEntity);
        this.visibleCharacters.Add(interestingEntity, character);
      }
      this.needRecountEnemies = true;
    }

    private void OnStopSee(IDetectableComponent detectable)
    {
      IEntity interestingEntity = this.GetDetectorInterestingEntity(detectable);
      if (interestingEntity != null && this.visibleCharacters.ContainsKey(interestingEntity))
        this.visibleCharacters.Remove(interestingEntity);
      this.needRecountEnemies = true;
    }

    private void OnHear(IDetectableComponent detectable)
    {
      IEntity interestingEntity = this.GetDetectorInterestingEntity(detectable);
      if (interestingEntity != null && !this.hearableCharacters.ContainsKey(interestingEntity))
      {
        CombatServiceCharacterInfo character = this.GetCharacter(interestingEntity);
        this.hearableCharacters.Add(interestingEntity, character);
      }
      this.needRecountEnemies = true;
    }

    private void OnStopHear(IDetectableComponent detectable)
    {
      IEntity interestingEntity = this.GetDetectorInterestingEntity(detectable);
      if (interestingEntity != null && this.hearableCharacters.ContainsKey(interestingEntity))
        this.hearableCharacters.Remove(interestingEntity);
      this.needRecountEnemies = true;
    }

    private void OnGameObjectChanged() => this.needRecountEnemies = true;

    private void RenewDetector()
    {
      if (this.visibleCharacters.ContainsValue((CombatServiceCharacterInfo) null))
      {
        foreach (IEntity entity in new List<IEntity>((IEnumerable<IEntity>) this.visibleCharacters.Keys))
        {
          if (this.visibleCharacters[entity] == null)
            this.visibleCharacters[entity] = this.GetCharacter(entity);
        }
      }
      if (!this.hearableCharacters.ContainsValue((CombatServiceCharacterInfo) null))
        return;
      foreach (IEntity entity in new List<IEntity>((IEnumerable<IEntity>) this.hearableCharacters.Keys))
      {
        if (this.hearableCharacters[entity] == null)
          this.hearableCharacters[entity] = this.GetCharacter(entity);
      }
    }

    private IEntity GetDetectorInterestingEntity(IDetectableComponent detectable)
    {
      IEntity owner = detectable.Owner;
      if (owner == null)
        return (IEntity) null;
      NpcControllerComponent component1 = owner.GetComponent<NpcControllerComponent>();
      PlayerControllerComponent component2 = owner.GetComponent<PlayerControllerComponent>();
      return component1 == null && component2 == null ? (IEntity) null : owner;
    }

    private CombatServiceCharacterInfo GetCharacter(IEntity entity)
    {
      return this.combatService.GetCharacterInfo(entity);
    }

    public bool CanSee(CombatServiceCharacterInfo character)
    {
      return this.visibleCharacters.ContainsValue(character);
    }

    public bool CanHear(CombatServiceCharacterInfo character)
    {
      return this.hearableCharacters.ContainsValue(character);
    }

    public void RecountEnemies()
    {
      this.RenewDetector();
      List<CombatServiceCharacterInfo> serviceCharacterInfoList = new List<CombatServiceCharacterInfo>();
      serviceCharacterInfoList.AddRange((IEnumerable<CombatServiceCharacterInfo>) this.hearableCharacters.Values);
      foreach (CombatServiceCharacterInfo serviceCharacterInfo in this.visibleCharacters.Values)
      {
        if (!serviceCharacterInfoList.Contains(serviceCharacterInfo))
          serviceCharacterInfoList.Add(serviceCharacterInfo);
      }
      List<FractionEnum> fraction1 = RelatedFractionUtility.GetFraction(this.entity, FractionRelationEnum.FearOnSee);
      List<FractionEnum> fraction2 = RelatedFractionUtility.GetFraction(this.entity, FractionRelationEnum.AttackOnSee);
      List<FractionEnum> fraction3 = RelatedFractionUtility.GetFraction(this.entity, FractionRelationEnum.AttackOnSeeInfected);
      this.fearCharactersNearby = new List<CombatServiceCharacterInfo>();
      this.attackCharactersNearby = new List<CombatServiceCharacterInfo>();
      if (this.IsCombatIgnored)
        return;
      foreach (CombatServiceCharacterInfo serviceCharacterInfo in serviceCharacterInfoList)
      {
        if (serviceCharacterInfo != null && !serviceCharacterInfo.IsDead && serviceCharacterInfo.IsIndoors == this.isIndoors && !serviceCharacterInfo.IsCombatIgnored)
        {
          if (!this.IsPlayer && this.PersonalFearEnemies != null && this.PersonalFearEnemies.Contains(serviceCharacterInfo) && !serviceCharacterInfo.WasBeaten)
            this.fearCharactersNearby.Add(serviceCharacterInfo);
          else if (!this.IsPlayer && this.PersonalAttackEnemies != null && this.PersonalAttackEnemies.Contains(serviceCharacterInfo))
          {
            this.attackCharactersNearby.Add(serviceCharacterInfo);
          }
          else
          {
            FractionEnum targetFraction = FractionsHelper.GetTargetFraction(serviceCharacterInfo.entity, this.entity);
            if (targetFraction != FractionEnum.None)
            {
              if (fraction1 != null && fraction1.Contains(targetFraction) && !serviceCharacterInfo.WasBeaten)
                this.fearCharactersNearby.Add(serviceCharacterInfo);
              else if (fraction2 != null && fraction2.Contains(targetFraction))
                this.attackCharactersNearby.Add(serviceCharacterInfo);
              else if (fraction3 != null && fraction3.Contains(targetFraction))
              {
                IParameter<float> byName = serviceCharacterInfo.Parameters.GetByName<float>(ParameterNameEnum.Infection);
                float num = this.fractionSettings == null ? 0.1f : this.fractionSettings.InfectionThreshold;
                if (byName != null && (double) byName.Value > (double) num)
                  this.attackCharactersNearby.Add(serviceCharacterInfo);
              }
            }
          }
        }
      }
    }

    public CombatServiceCharacterInfo FearEnemyNearby()
    {
      this.fearCharactersNearby.RemoveAll((Predicate<CombatServiceCharacterInfo>) (x => x.IsDead));
      return this.fearCharactersNearby.Count == 0 ? (CombatServiceCharacterInfo) null : this.fearCharactersNearby[0];
    }

    public CombatServiceCharacterInfo AttackEnemyNearby()
    {
      this.attackCharactersNearby.RemoveAll((Predicate<CombatServiceCharacterInfo>) (x => x.IsDead));
      return this.attackCharactersNearby.Count == 0 ? (CombatServiceCharacterInfo) null : this.attackCharactersNearby[0];
    }

    public float GetTimeFromLastOrder() => Time.time - this.stateSetTime;

    public void FireControllerCombatAction(CombatActionEnum action, IEntity target)
    {
      if (this.IsPlayer && this.PlayerController != null)
      {
        this.PlayerController.FireCombatAction(action, target);
      }
      else
      {
        if (this.NpcController == null)
          return;
        this.NpcController.FireCombatAction(action, target);
      }
    }

    public void Update()
    {
      if (!this.needRecountEnemies)
        return;
      this.RecountEnemies();
      this.needRecountEnemies = false;
    }

    public void OnParameterChanged(IParameter parameter)
    {
      if (parameter.Name == ParameterNameEnum.Fraction)
      {
        this.fractionSettings = ScriptableObjectInstance<FractionsSettingsData>.Instance.Fractions.Find((Predicate<FractionSettings>) (x => x.Name == ((IParameter<FractionEnum>) parameter).Value));
        this.needRecountEnemies = true;
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
        CombatStyleEnum styleName = this.combatStyleParameter == null ? CombatStyleEnum.Default : this.combatStyleParameter.Value;
        this.combatSettings = ScriptableObjectInstance<FightSettingsData>.Instance.IndividualCombatSettings.Find((Predicate<IndividualCombatSettings>) (x => x.Name == styleName));
      }
    }
  }
}
