// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.PlayerControllerComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Locations;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Regions;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;
using Engine.Source.Components.Regions;
using Engine.Source.Inventory;
using Engine.Source.Reputations;
using Engine.Source.Services;
using Engine.Source.Services.Detectablies;
using Engine.Source.Settings.External;
using Inspectors;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace Engine.Source.Components
{
  [Factory(typeof (IPlayerControllerComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class PlayerControllerComponent : 
    EngineComponent,
    IPlayerControllerComponent,
    IComponent,
    IUpdatable,
    IEntityEventsListener
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<ReputationInfo> reputations = new List<ReputationInfo>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float thresholdNearRegionsPositive;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float thresholdNearRegionsNegative;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float coefficientNearRegionsPositive;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float coefficientNearRegionsNegative;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected float thresholdPlayerInfected;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected int thresholdRegionInfected;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<FractionEnum> dangerousFractions = new List<FractionEnum>();
    [Inspected]
    private HashSet<IEntity> visibles = new HashSet<IEntity>();
    [Inspected]
    private HashSet<IEntity> hearers = new HashSet<IEntity>();
    [Inspected]
    private NpcControllerComponent target;
    [FromThis]
    private ParametersComponent parameters;
    [FromThis]
    private DetectorComponent detector;
    [FromThis]
    private ILocationItemComponent locationItem;
    [FromThis]
    private NavigationComponent navigation;
    [FromLocator]
    private NotificationService notificationService;
    [Inspected]
    private HashSet<NpcControllerComponent> nears = new HashSet<NpcControllerComponent>();
    private IUpdater updater;
    private float lastSeenReputation = 0.0f;
    [Inspected]
    private HashSet<NpcControllerComponent> candidates = new HashSet<NpcControllerComponent>();

    public event Action<IInventoryComponent> OpenContainerEvent;

    public event Action<CombatActionEnum, IEntity> CombatActionEvent;

    [Inspected]
    public IParameterValue<float> Reputation { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [Inspected]
    public IParameterValue<bool> IsDead { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> IsImmortal { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<float> Health { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [Inspected]
    public IParameterValue<float> Hunger { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [Inspected]
    public IParameterValue<float> Thirst { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [Inspected]
    public IParameterValue<float> Fatigue { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [Inspected]
    public IParameterValue<float> Infection { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [Inspected]
    public IParameterValue<float> Immunity { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [Inspected]
    public IParameterValue<float> PreInfection { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [Inspected]
    public IParameterValue<bool> Sleep { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> CanTrade { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<FractionEnum> Fraction { get; } = (IParameterValue<FractionEnum>) new ParameterValue<FractionEnum>();

    [Inspected]
    public IParameterValue<bool> FundEnabled { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<bool> FundFinished { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public IParameterValue<float> FundPoints { get; } = (IParameterValue<float>) new ParameterValue<float>();

    [Inspected]
    public IParameterValue<bool> CanReceiveMail { get; } = (IParameterValue<bool>) new ParameterValue<bool>();

    [Inspected]
    public bool Danger { get; private set; }

    public IEnumerable<NpcControllerComponent> Nears
    {
      get => (IEnumerable<NpcControllerComponent>) this.nears;
    }

    public override void OnAdded()
    {
      base.OnAdded();
      this.Reputation.Set<float>(this.parameters.GetByName<float>(ParameterNameEnum.Reputation));
      this.Hunger.Set<float>(this.parameters.GetByName<float>(ParameterNameEnum.Hunger));
      this.Thirst.Set<float>(this.parameters.GetByName<float>(ParameterNameEnum.Thirst));
      this.Fatigue.Set<float>(this.parameters.GetByName<float>(ParameterNameEnum.Fatigue));
      this.PreInfection.Set<float>(this.parameters.GetByName<float>(ParameterNameEnum.PreInfection));
      this.Infection.Set<float>(this.parameters.GetByName<float>(ParameterNameEnum.Infection));
      this.Immunity.Set<float>(this.parameters.GetByName<float>(ParameterNameEnum.Immunity));
      this.IsDead.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.Dead));
      this.IsImmortal.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.Immortal));
      this.Health.Set<float>(this.parameters.GetByName<float>(ParameterNameEnum.Health));
      this.Sleep.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.Sleep));
      this.CanTrade.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.CanTrade));
      this.Fraction.Set<FractionEnum>(this.parameters.GetByName<FractionEnum>(ParameterNameEnum.Fraction));
      this.FundEnabled.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.FundEnabled));
      this.FundFinished.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.FundFinished));
      this.FundPoints.Set<float>(this.parameters.GetByName<float>(ParameterNameEnum.FundPoints));
      this.CanReceiveMail.Set<bool>(this.parameters.GetByName<bool>(ParameterNameEnum.CanReceiveMail));
      this.updater = InstanceByRequest<UpdateService>.Instance.PlayerUpdater;
      this.updater.AddUpdatable((IUpdatable) this);
      this.navigation.EnterRegionEvent += new RegionHandler(this.OnEnterRegionEvent);
      this.Reputation.ChangeValueEvent += new Action<float>(this.OnChangeReputationValueEvent);
    }

    public override void OnRemoved()
    {
      this.Reputation.ChangeValueEvent -= new Action<float>(this.OnChangeReputationValueEvent);
      this.navigation.EnterRegionEvent -= new RegionHandler(this.OnEnterRegionEvent);
      this.updater.RemoveUpdatable((IUpdatable) this);
      this.Reputation.Set<float>((IParameter<float>) null);
      this.Hunger.Set<float>((IParameter<float>) null);
      this.Thirst.Set<float>((IParameter<float>) null);
      this.Fatigue.Set<float>((IParameter<float>) null);
      this.PreInfection.Set<float>((IParameter<float>) null);
      this.Infection.Set<float>((IParameter<float>) null);
      this.Immunity.Set<float>((IParameter<float>) null);
      this.IsDead.Set<bool>((IParameter<bool>) null);
      this.IsImmortal.Set<bool>((IParameter<bool>) null);
      this.Health.Set<float>((IParameter<float>) null);
      this.Sleep.Set<bool>((IParameter<bool>) null);
      this.CanTrade.Set<bool>((IParameter<bool>) null);
      this.Fraction.Set<FractionEnum>((IParameter<FractionEnum>) null);
      this.FundEnabled.Set<bool>((IParameter<bool>) null);
      this.FundFinished.Set<bool>((IParameter<bool>) null);
      this.FundPoints.Set<float>((IParameter<float>) null);
      this.CanReceiveMail.Set<bool>((IParameter<bool>) null);
      this.SetTarget((NpcControllerComponent) null);
      base.OnRemoved();
    }

    public void AddVisible(IEntity entity)
    {
      this.visibles.Add(entity);
      this.ComputeInfected();
    }

    public void RemoveVisible(IEntity entity) => this.visibles.Remove(entity);

    public void AddHearing(IEntity entity) => this.hearers.Add(entity);

    public void RemoveHearing(IEntity entity) => this.hearers.Remove(entity);

    public void OnOpenContainer(IInventoryComponent container)
    {
      Action<IInventoryComponent> openContainerEvent = this.OpenContainerEvent;
      if (openContainerEvent != null)
        openContainerEvent(container);
      if (!this.CheckTheft(container.GetStorage()))
        return;
      this.ComputeAction(ActionEnum.BreakContainer);
    }

    public void OnGetLoot(IStorageComponent target)
    {
      INpcControllerComponent component1 = target.GetComponent<INpcControllerComponent>();
      if (component1 != null)
      {
        if (!component1.IsDead.Value)
          this.ComputeActionToSingleTarget(ActionEnum.TakeItemsFromSurrender, target.Owner);
        else
          this.ComputeAction(ActionEnum.LootDeadCharacter, false, target.Owner);
      }
      else
      {
        ParametersComponent component2 = target.GetComponent<ParametersComponent>();
        if (component2 != null)
        {
          IParameter<bool> byName = component2.GetByName<bool>(ParameterNameEnum.LootAsNPC);
          if (byName != null && byName.Value)
          {
            this.ComputeAction(ActionEnum.LootDeadCharacter, false, target.Owner);
            return;
          }
        }
        this.ComputeLootInanimate(target);
      }
    }

    private void ComputeLootInanimate(IStorageComponent target) => this.ComputeActionTheft(target);

    private bool CheckTheft(IStorageComponent target)
    {
      if (target == null || target.IsFree.Value)
        return false;
      LocationComponent parentComponent = LocationItemUtility.FindParentComponent<LocationComponent>(target.Owner);
      if (parentComponent == null)
      {
        Debug.LogError((object) "location == null, Разобраться");
        return false;
      }
      return ((LocationComponent) parentComponent.LogicLocation).LocationType == LocationType.Indoor;
    }

    private void ComputeActionTheft(IStorageComponent target)
    {
      if (!this.CheckTheft(target))
        return;
      this.ComputeAction(ActionEnum.Theft);
    }

    public void FireCombatAction(CombatActionEnum action, IEntity target)
    {
      if (this.CombatActionEvent == null)
        return;
      this.CombatActionEvent(action, target);
    }

    public void ComputeAction(ActionEnum action, float multiplicator = 1f)
    {
      this.ComputeReputation(action, (IEntity) null, multiplicator);
      this.ComputeEvents(action, false, (IEntity) null);
    }

    public void ComputeAction(ActionEnum action, bool hear, IEntity target)
    {
      this.ComputeReputation(action, target);
      this.ComputeEvents(action, hear, target);
    }

    public void ComputeActionToSingleTarget(ActionEnum action, IEntity target)
    {
      this.ComputeReputation(action, target);
      this.ComputeEventsToSingleTarget(action, target);
    }

    public bool IsCrime(ActionEnum action, IEntity target)
    {
      ReputationInfo delictInfo = this.GetDelictInfo(action, target);
      return delictInfo != null && (double) delictInfo.Visible < 0.0 && !this.TargetIsFree(target);
    }

    private void ComputeReputation(ActionEnum action, IEntity target, float multiplicator = 1f)
    {
      NavigationComponent component = this.Owner.GetComponent<NavigationComponent>();
      if (component == null)
        return;
      ReputationInfo delictInfo = this.GetDelictInfo(action, target);
      float num1 = 0.0f;
      if (delictInfo == null)
        return;
      if (!this.TargetIsFree(target))
        num1 = this.visibles.Count != 0 ? delictInfo.Visible : delictInfo.Invisible;
      float a = num1 * multiplicator;
      if (Mathf.Approximately(a, 0.0f) || !(component.Region is RegionComponent region) || region.RegionBehaviour != RegionBehaviourEnum.None)
        return;
      float num2 = Mathf.Clamp01(region.Reputation.Value + a);
      region.Reputation.Value = num2;
      if (delictInfo.AffectNearRegions)
      {
        foreach (IRegionComponent nearRegion in this.GetNearRegions((IRegionComponent) region))
        {
          float num3 = nearRegion.Reputation.Value;
          if (((double) a <= 0.0 || (double) num3 < (double) this.thresholdNearRegionsPositive) && ((double) a >= 0.0 || (double) num3 > (double) this.thresholdNearRegionsNegative))
          {
            float num4 = Mathf.Clamp01(Mathf.Clamp(num3 + a * ((double) a > 0.0 ? this.coefficientNearRegionsPositive : this.coefficientNearRegionsNegative), this.thresholdNearRegionsNegative, this.thresholdNearRegionsPositive));
            nearRegion.Reputation.Value = num4;
          }
        }
      }
    }

    private ReputationInfo GetDelictInfo(ActionEnum action, IEntity target)
    {
      ReputationInfo delictInfo = this.reputations.FirstOrDefault<ReputationInfo>((Func<ReputationInfo, bool>) (o =>
      {
        if (o.Action != action)
          return false;
        return target == null || o.Fractions.Count == 0;
      }));
      if (delictInfo == null && target != null)
      {
        ParametersComponent component = target.GetComponent<ParametersComponent>();
        if (component != null)
        {
          IParameter<FractionEnum> fractionParameters = component.GetByName<FractionEnum>(ParameterNameEnum.Fraction);
          if (fractionParameters != null)
            delictInfo = this.reputations.FirstOrDefault<ReputationInfo>((Func<ReputationInfo, bool>) (o => o.Action == action && o.Fractions.Contains(fractionParameters.Value)));
        }
      }
      return delictInfo;
    }

    private bool TargetIsFree(IEntity target)
    {
      if (target != null)
      {
        ParametersComponent component = target.GetComponent<ParametersComponent>();
        if (component != null)
        {
          IParameter<bool> byName = component.GetByName<bool>(ParameterNameEnum.IsFree);
          if (byName != null)
            return byName.Value;
        }
      }
      return false;
    }

    public IEnumerable<IRegionComponent> GetNearRegions(IRegionComponent currentRegion)
    {
      RegionMesh regionMesh = ((RegionComponent) currentRegion).RegionMesh;
      if ((UnityEngine.Object) regionMesh != (UnityEngine.Object) null)
      {
        RegionMesh[] regionMeshArray = regionMesh.NearRegions;
        for (int index = 0; index < regionMeshArray.Length; ++index)
        {
          RegionMesh nearRegion = regionMeshArray[index];
          if (!((UnityEngine.Object) nearRegion == (UnityEngine.Object) null))
          {
            RegionComponent near = RegionUtility.GetRegionByName(nearRegion.Region);
            if (near != null)
            {
              yield return (IRegionComponent) near;
              near = (RegionComponent) null;
              nearRegion = (RegionMesh) null;
            }
          }
        }
        regionMeshArray = (RegionMesh[]) null;
      }
    }

    private void ComputeEvents(ActionEnum action, bool hear, IEntity target)
    {
      List<NpcControllerComponent> controllerComponentList = new List<NpcControllerComponent>();
      foreach (IEntity visible in this.visibles)
      {
        NpcControllerComponent component = visible.GetComponent<NpcControllerComponent>();
        if (component != null && !controllerComponentList.Contains(component))
          controllerComponentList.Add(component);
      }
      if (hear)
      {
        foreach (IDetectableComponent component1 in this.detector.Hearing)
        {
          if (!component1.IsDisposed)
          {
            NpcControllerComponent component2 = component1.GetComponent<NpcControllerComponent>();
            if (component2 != null && !controllerComponentList.Contains(component2))
              controllerComponentList.Add(component2);
          }
        }
        foreach (IEntity hearer in this.hearers)
        {
          NpcControllerComponent component = hearer.GetComponent<NpcControllerComponent>();
          if (!controllerComponentList.Contains(component) && component != null && !controllerComponentList.Contains(component))
            controllerComponentList.Add(component);
        }
      }
      if (target != null)
      {
        NpcControllerComponent component = target.GetComponent<NpcControllerComponent>();
        if (component != null && !controllerComponentList.Contains(component))
          controllerComponentList.Add(component);
      }
      foreach (NpcControllerComponent controllerComponent in controllerComponentList)
        controllerComponent.FireAction(action);
    }

    private void ComputeEventsToSingleTarget(ActionEnum action, IEntity target)
    {
      target?.GetComponent<NpcControllerComponent>()?.FireAction(action);
    }

    public void ComputeHit(NpcControllerComponent target)
    {
      if (this.IsCombatIgnored(target))
        return;
      this.ComputeActionToSingleTarget(ActionEnum.HitNpc, target.Owner);
      this.ComputeAggression(target);
    }

    public void ComputeShoot(NpcControllerComponent target)
    {
      if (this.IsCombatIgnored(target))
        return;
      this.ComputeActionToSingleTarget(ActionEnum.ShootNpc, target.Owner);
      this.ComputeAggression(target);
    }

    private void ComputeAggression(NpcControllerComponent target)
    {
      ParametersComponent component = target.Owner.GetComponent<ParametersComponent>();
      if (component != null)
      {
        IParameter<bool> byName = component.GetByName<bool>(ParameterNameEnum.WasAttackedByPlayer);
        if (byName != null)
        {
          if (!byName.Value)
          {
            this.ComputeActionToSingleTarget(ActionEnum.FirstAttackNPC, target.Owner);
            if (target.Target != null)
              this.ComputeActionToSingleTarget(ActionEnum.SafeNpc, target.Target.Owner);
          }
          byName.Value = true;
        }
      }
      this.SetTarget(target);
      ServiceLocator.GetService<CombatService>()?.HitNpc(this.Owner, target.Owner);
      target.LastAttacker = this.Owner;
    }

    public void ComputeHitAnotherNPC(NpcControllerComponent target)
    {
      if (this.IsCombatIgnored(target))
        return;
      this.SetTarget(target);
      ReputationInfo delictInfo = this.GetDelictInfo(ActionEnum.FirstAttackNPC, target.Owner);
      if (delictInfo != null && (double) delictInfo.Visible < 0.0)
        this.ComputeEvents(ActionEnum.HitAnotherGoodNPC, true, target.Owner);
      else
        this.ComputeEvents(ActionEnum.HitAnotherNPC, true, target.Owner);
    }

    private bool IsCombatIgnored(NpcControllerComponent target)
    {
      if (target == null)
        return false;
      ParametersComponent component = target.Owner.GetComponent<ParametersComponent>();
      if (component == null)
        return false;
      IParameter<bool> byName = component.GetByName<bool>(ParameterNameEnum.IsCombatIgnored);
      return byName != null && byName.Value;
    }

    public void ComputeHealPain(IEntity target, float healed)
    {
      this.ComputeAction(ActionEnum.HealNpcPain, healed);
    }

    public void ComputeHealInfection(IEntity target, float healed)
    {
      this.ComputeAction(ActionEnum.HealNpcInfection, healed);
    }

    public void ComputeCureInfection(IEntity target)
    {
      this.ComputeActionToSingleTarget(ActionEnum.CureInfection, target);
    }

    public void ComputePicklock(IEntity target)
    {
      this.ComputeActionToSingleTarget(ActionEnum.BreakPicklock, target);
    }

    public void ComputeGiftNPC(IEntity target, float gift)
    {
      this.ComputeAction(ActionEnum.GiftNPC, gift);
    }

    public void ComputeRepairHydrant() => this.ComputeAction(ActionEnum.RepairHydrant);

    private void SetTarget(NpcControllerComponent target)
    {
      if (this.target == target)
        return;
      if (this.target != null)
      {
        ((Entity) this.target.Owner).RemoveListener((IEntityEventsListener) this);
        this.target.IsDead.ChangeValueEvent -= new Action<bool>(this.OnChangeDeadTarget);
        this.target = (NpcControllerComponent) null;
      }
      this.target = target;
      if (this.target == null)
        return;
      ((Entity) this.target.Owner).AddListener((IEntityEventsListener) this);
      this.target.IsDead.ChangeValueEvent += new Action<bool>(this.OnChangeDeadTarget);
    }

    private void OnChangeDeadTarget(bool value)
    {
      if (!value)
        return;
      this.ComputeDead(this.target);
      this.SetTarget((NpcControllerComponent) null);
    }

    private void OnDisposeEvent() => this.SetTarget((NpcControllerComponent) null);

    private void ComputeDead(NpcControllerComponent target)
    {
      if (target == null || target.LastAttacker != this.Owner)
        return;
      this.ComputeActionToSingleTarget(ActionEnum.MurderNpc, target.Owner);
    }

    private void ComputeInfected()
    {
      LocationItemComponent parentComponent = LocationItemUtility.FindParentComponent<LocationItemComponent>(this.Owner);
      if (parentComponent.IsIndoor || this.parameters == null)
        return;
      IParameter<float> byName = this.parameters.GetByName<float>(ParameterNameEnum.Infection);
      if (byName == null || (double) byName.Value < (double) this.thresholdPlayerInfected)
        return;
      RegionComponent component = parentComponent.LogicLocation.Owner.GetComponent<RegionComponent>();
      if (component == null || component.DiseaseLevel.Value > this.thresholdRegionInfected)
        return;
      this.ComputeAction(ActionEnum.SeeInfected);
    }

    public void ComputeUpdate()
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused || !this.Owner.IsEnabledInHierarchy)
        return;
      this.ComputeAway();
      this.ComputeDanger();
    }

    private void ComputeAway()
    {
      this.candidates.Clear();
      DetectorUtility.GetCandidats(ServiceLocator.GetService<DetectorService>().Detectablies, this.detector, this.locationItem, ExternalSettingsInstance<ExternalCommonSettings>.Instance.AwayDistance, (Action<DetectableCandidatInfo>) (target =>
      {
        NpcControllerComponent component = target.Detectable.Owner.GetComponent<NpcControllerComponent>();
        if (component == null || component.IsDisposed)
          return;
        this.candidates.Add(component);
      }));
      foreach (NpcControllerComponent candidate in this.candidates)
      {
        if (this.nears.Add(candidate))
          candidate.IsAway.Value = false;
      }
      foreach (NpcControllerComponent near in this.nears)
      {
        if (this.candidates.Add(near) && !near.IsDisposed)
          near.IsAway.Value = true;
      }
      foreach (NpcControllerComponent candidate in this.candidates)
      {
        if (candidate.IsDisposed || candidate.IsAway.Value)
          this.nears.Remove(candidate);
      }
    }

    private void ComputeDanger()
    {
      bool danger = false;
      DetectorUtility.GetCandidats(ServiceLocator.GetService<DetectorService>().Detectablies, this.detector, this.locationItem, ExternalSettingsInstance<ExternalCommonSettings>.Instance.DangerDistance, (Action<DetectableCandidatInfo>) (target =>
      {
        if (danger)
          return;
        NpcControllerComponent component1 = target.Detectable.Owner.GetComponent<NpcControllerComponent>();
        if (component1 == null || component1.IsDisposed)
          return;
        ParametersComponent component2 = target.Detectable.Owner.GetComponent<ParametersComponent>();
        if (component2 == null)
          return;
        IParameter<FractionEnum> byName = component2.GetByName<FractionEnum>(ParameterNameEnum.Fraction);
        if (byName == null || !this.dangerousFractions.Contains(byName.Value))
          return;
        danger = true;
      }));
      this.Danger = danger;
    }

    private void OnEnterRegionEvent(
      ref EventArgument<IEntity, IRegionComponent> eventArguments)
    {
      IRegionComponent region = this.navigation.Region;
      if (region == null)
        return;
      this.ComputeNotification(region);
      this.ComputeChangeLocation(region);
    }

    private void ComputeChangeLocation(IRegionComponent region)
    {
      if (!PlatformUtility.IsChangeLocationLoadingWindow(region))
        return;
      GameObject locationBlueprint = ScriptableObjectInstance<ResourceFromCodeData>.Instance.ChangeLocationBlueprint;
      if ((UnityEngine.Object) locationBlueprint != (UnityEngine.Object) null)
        BlueprintServiceUtility.Start(locationBlueprint, this.Owner, (Action) null, "ChangeLocationBlueprint");
    }

    private void ComputeNotification(IRegionComponent region)
    {
      if (!this.CanReceiveMail.Value || this.notificationService == null)
        return;
      IParameter<float> byName = region.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Reputation);
      if (byName != null)
        this.lastSeenReputation = byName.Value;
      this.notificationService.AddNotify(NotificationEnum.Region, new object[1]
      {
        (object) region
      });
    }

    private void OnChangeReputationValueEvent(float value)
    {
      if (!this.CanReceiveMail.Value || this.notificationService == null)
        return;
      IRegionComponent region = this.navigation.Region;
      if (region == null)
        return;
      float num = this.Reputation.Value;
      if ((double) num == (double) this.lastSeenReputation)
        return;
      this.notificationService.AddNotify(NotificationEnum.Reputation, new object[2]
      {
        (object) region,
        (object) this.lastSeenReputation
      });
      this.lastSeenReputation = num;
    }

    public void OnEntityEvent(IEntity sender, EntityEvents kind)
    {
      if (kind != EntityEvents.DisposeEvent)
        return;
      this.OnDisposeEvent();
    }
  }
}
