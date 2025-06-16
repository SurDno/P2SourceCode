using System;
using System.Collections.Generic;
using System.Linq;
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
using Engine.Source.Settings.External;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Components;

[Factory(typeof(IPlayerControllerComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class PlayerControllerComponent :
	EngineComponent,
	IPlayerControllerComponent,
	IComponent,
	IUpdatable,
	IEntityEventsListener {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected List<ReputationInfo> reputations = new();

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected float thresholdNearRegionsPositive;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected float thresholdNearRegionsNegative;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected float coefficientNearRegionsPositive;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected float coefficientNearRegionsNegative;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected float thresholdPlayerInfected;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected int thresholdRegionInfected;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected List<FractionEnum> dangerousFractions = new();

	[Inspected] private HashSet<IEntity> visibles = new();
	[Inspected] private HashSet<IEntity> hearers = new();
	[Inspected] private NpcControllerComponent target;
	[FromThis] private ParametersComponent parameters;
	[FromThis] private DetectorComponent detector;
	[FromThis] private ILocationItemComponent locationItem;
	[FromThis] private NavigationComponent navigation;
	[FromLocator] private NotificationService notificationService;
	[Inspected] private HashSet<NpcControllerComponent> nears = new();
	private IUpdater updater;
	private float lastSeenReputation;
	[Inspected] private HashSet<NpcControllerComponent> candidates = new();

	public event Action<IInventoryComponent> OpenContainerEvent;

	public event Action<CombatActionEnum, IEntity> CombatActionEvent;

	[Inspected] public IParameterValue<float> Reputation { get; } = new ParameterValue<float>();

	[Inspected] public IParameterValue<bool> IsDead { get; } = new ParameterValue<bool>();

	[Inspected] public IParameterValue<bool> IsImmortal { get; } = new ParameterValue<bool>();

	[Inspected] public IParameterValue<float> Health { get; } = new ParameterValue<float>();

	[Inspected] public IParameterValue<float> Hunger { get; } = new ParameterValue<float>();

	[Inspected] public IParameterValue<float> Thirst { get; } = new ParameterValue<float>();

	[Inspected] public IParameterValue<float> Fatigue { get; } = new ParameterValue<float>();

	[Inspected] public IParameterValue<float> Infection { get; } = new ParameterValue<float>();

	[Inspected] public IParameterValue<float> Immunity { get; } = new ParameterValue<float>();

	[Inspected] public IParameterValue<float> PreInfection { get; } = new ParameterValue<float>();

	[Inspected] public IParameterValue<bool> Sleep { get; } = new ParameterValue<bool>();

	[Inspected] public IParameterValue<bool> CanTrade { get; } = new ParameterValue<bool>();

	[Inspected] public IParameterValue<FractionEnum> Fraction { get; } = new ParameterValue<FractionEnum>();

	[Inspected] public IParameterValue<bool> FundEnabled { get; } = new ParameterValue<bool>();

	[Inspected] public IParameterValue<bool> FundFinished { get; } = new ParameterValue<bool>();

	[Inspected] public IParameterValue<float> FundPoints { get; } = new ParameterValue<float>();

	[Inspected] public IParameterValue<bool> CanReceiveMail { get; } = new ParameterValue<bool>();

	[Inspected] public bool Danger { get; private set; }

	public IEnumerable<NpcControllerComponent> Nears => nears;

	public override void OnAdded() {
		base.OnAdded();
		Reputation.Set(parameters.GetByName<float>(ParameterNameEnum.Reputation));
		Hunger.Set(parameters.GetByName<float>(ParameterNameEnum.Hunger));
		Thirst.Set(parameters.GetByName<float>(ParameterNameEnum.Thirst));
		Fatigue.Set(parameters.GetByName<float>(ParameterNameEnum.Fatigue));
		PreInfection.Set(parameters.GetByName<float>(ParameterNameEnum.PreInfection));
		Infection.Set(parameters.GetByName<float>(ParameterNameEnum.Infection));
		Immunity.Set(parameters.GetByName<float>(ParameterNameEnum.Immunity));
		IsDead.Set(parameters.GetByName<bool>(ParameterNameEnum.Dead));
		IsImmortal.Set(parameters.GetByName<bool>(ParameterNameEnum.Immortal));
		Health.Set(parameters.GetByName<float>(ParameterNameEnum.Health));
		Sleep.Set(parameters.GetByName<bool>(ParameterNameEnum.Sleep));
		CanTrade.Set(parameters.GetByName<bool>(ParameterNameEnum.CanTrade));
		Fraction.Set(parameters.GetByName<FractionEnum>(ParameterNameEnum.Fraction));
		FundEnabled.Set(parameters.GetByName<bool>(ParameterNameEnum.FundEnabled));
		FundFinished.Set(parameters.GetByName<bool>(ParameterNameEnum.FundFinished));
		FundPoints.Set(parameters.GetByName<float>(ParameterNameEnum.FundPoints));
		CanReceiveMail.Set(parameters.GetByName<bool>(ParameterNameEnum.CanReceiveMail));
		updater = InstanceByRequest<UpdateService>.Instance.PlayerUpdater;
		updater.AddUpdatable(this);
		navigation.EnterRegionEvent += OnEnterRegionEvent;
		Reputation.ChangeValueEvent += OnChangeReputationValueEvent;
	}

	public override void OnRemoved() {
		Reputation.ChangeValueEvent -= OnChangeReputationValueEvent;
		navigation.EnterRegionEvent -= OnEnterRegionEvent;
		updater.RemoveUpdatable(this);
		Reputation.Set(null);
		Hunger.Set(null);
		Thirst.Set(null);
		Fatigue.Set(null);
		PreInfection.Set(null);
		Infection.Set(null);
		Immunity.Set(null);
		IsDead.Set(null);
		IsImmortal.Set(null);
		Health.Set(null);
		Sleep.Set(null);
		CanTrade.Set(null);
		Fraction.Set(null);
		FundEnabled.Set(null);
		FundFinished.Set(null);
		FundPoints.Set(null);
		CanReceiveMail.Set(null);
		SetTarget(null);
		base.OnRemoved();
	}

	public void AddVisible(IEntity entity) {
		visibles.Add(entity);
		ComputeInfected();
	}

	public void RemoveVisible(IEntity entity) {
		visibles.Remove(entity);
	}

	public void AddHearing(IEntity entity) {
		hearers.Add(entity);
	}

	public void RemoveHearing(IEntity entity) {
		hearers.Remove(entity);
	}

	public void OnOpenContainer(IInventoryComponent container) {
		var openContainerEvent = OpenContainerEvent;
		if (openContainerEvent != null)
			openContainerEvent(container);
		if (!CheckTheft(container.GetStorage()))
			return;
		ComputeAction(ActionEnum.BreakContainer);
	}

	public void OnGetLoot(IStorageComponent target) {
		var component1 = target.GetComponent<INpcControllerComponent>();
		if (component1 != null) {
			if (!component1.IsDead.Value)
				ComputeActionToSingleTarget(ActionEnum.TakeItemsFromSurrender, target.Owner);
			else
				ComputeAction(ActionEnum.LootDeadCharacter, false, target.Owner);
		} else {
			var component2 = target.GetComponent<ParametersComponent>();
			if (component2 != null) {
				var byName = component2.GetByName<bool>(ParameterNameEnum.LootAsNPC);
				if (byName != null && byName.Value) {
					ComputeAction(ActionEnum.LootDeadCharacter, false, target.Owner);
					return;
				}
			}

			ComputeLootInanimate(target);
		}
	}

	private void ComputeLootInanimate(IStorageComponent target) {
		ComputeActionTheft(target);
	}

	private bool CheckTheft(IStorageComponent target) {
		if (target == null || target.IsFree.Value)
			return false;
		var parentComponent = LocationItemUtility.FindParentComponent<LocationComponent>(target.Owner);
		if (parentComponent == null) {
			Debug.LogError("location == null, Разобраться");
			return false;
		}

		return ((LocationComponent)parentComponent.LogicLocation).LocationType == LocationType.Indoor;
	}

	private void ComputeActionTheft(IStorageComponent target) {
		if (!CheckTheft(target))
			return;
		ComputeAction(ActionEnum.Theft);
	}

	public void FireCombatAction(CombatActionEnum action, IEntity target) {
		if (CombatActionEvent == null)
			return;
		CombatActionEvent(action, target);
	}

	public void ComputeAction(ActionEnum action, float multiplicator = 1f) {
		ComputeReputation(action, null, multiplicator);
		ComputeEvents(action, false, null);
	}

	public void ComputeAction(ActionEnum action, bool hear, IEntity target) {
		ComputeReputation(action, target);
		ComputeEvents(action, hear, target);
	}

	public void ComputeActionToSingleTarget(ActionEnum action, IEntity target) {
		ComputeReputation(action, target);
		ComputeEventsToSingleTarget(action, target);
	}

	public bool IsCrime(ActionEnum action, IEntity target) {
		var delictInfo = GetDelictInfo(action, target);
		return delictInfo != null && delictInfo.Visible < 0.0 && !TargetIsFree(target);
	}

	private void ComputeReputation(ActionEnum action, IEntity target, float multiplicator = 1f) {
		var component = Owner.GetComponent<NavigationComponent>();
		if (component == null)
			return;
		var delictInfo = GetDelictInfo(action, target);
		var num1 = 0.0f;
		if (delictInfo == null)
			return;
		if (!TargetIsFree(target))
			num1 = visibles.Count != 0 ? delictInfo.Visible : delictInfo.Invisible;
		var a = num1 * multiplicator;
		if (Mathf.Approximately(a, 0.0f) || !(component.Region is RegionComponent region) ||
		    region.RegionBehaviour != RegionBehaviourEnum.None)
			return;
		var num2 = Mathf.Clamp01(region.Reputation.Value + a);
		region.Reputation.Value = num2;
		if (delictInfo.AffectNearRegions)
			foreach (var nearRegion in GetNearRegions(region)) {
				var num3 = nearRegion.Reputation.Value;
				if ((a <= 0.0 || num3 < (double)thresholdNearRegionsPositive) &&
				    (a >= 0.0 || num3 > (double)thresholdNearRegionsNegative)) {
					var num4 = Mathf.Clamp01(Mathf.Clamp(
						num3 + a * (a > 0.0 ? coefficientNearRegionsPositive : coefficientNearRegionsNegative),
						thresholdNearRegionsNegative, thresholdNearRegionsPositive));
					nearRegion.Reputation.Value = num4;
				}
			}
	}

	private ReputationInfo GetDelictInfo(ActionEnum action, IEntity target) {
		var delictInfo = reputations.FirstOrDefault(o => {
			if (o.Action != action)
				return false;
			return target == null || o.Fractions.Count == 0;
		});
		if (delictInfo == null && target != null) {
			var component = target.GetComponent<ParametersComponent>();
			if (component != null) {
				var fractionParameters = component.GetByName<FractionEnum>(ParameterNameEnum.Fraction);
				if (fractionParameters != null)
					delictInfo = reputations.FirstOrDefault(o =>
						o.Action == action && o.Fractions.Contains(fractionParameters.Value));
			}
		}

		return delictInfo;
	}

	private bool TargetIsFree(IEntity target) {
		if (target != null) {
			var component = target.GetComponent<ParametersComponent>();
			if (component != null) {
				var byName = component.GetByName<bool>(ParameterNameEnum.IsFree);
				if (byName != null)
					return byName.Value;
			}
		}

		return false;
	}

	public IEnumerable<IRegionComponent> GetNearRegions(IRegionComponent currentRegion) {
		var regionMesh = ((RegionComponent)currentRegion).RegionMesh;
		if (regionMesh != null) {
			var regionMeshArray = regionMesh.NearRegions;
			for (var index = 0; index < regionMeshArray.Length; ++index) {
				var nearRegion = regionMeshArray[index];
				if (!(nearRegion == null)) {
					var near = RegionUtility.GetRegionByName(nearRegion.Region);
					if (near != null) {
						yield return near;
						near = null;
						nearRegion = null;
					}
				}
			}

			regionMeshArray = null;
		}
	}

	private void ComputeEvents(ActionEnum action, bool hear, IEntity target) {
		var controllerComponentList = new List<NpcControllerComponent>();
		foreach (var visible in visibles) {
			var component = visible.GetComponent<NpcControllerComponent>();
			if (component != null && !controllerComponentList.Contains(component))
				controllerComponentList.Add(component);
		}

		if (hear) {
			foreach (var component1 in detector.Hearing)
				if (!component1.IsDisposed) {
					var component2 = component1.GetComponent<NpcControllerComponent>();
					if (component2 != null && !controllerComponentList.Contains(component2))
						controllerComponentList.Add(component2);
				}

			foreach (var hearer in hearers) {
				var component = hearer.GetComponent<NpcControllerComponent>();
				if (!controllerComponentList.Contains(component) && component != null &&
				    !controllerComponentList.Contains(component))
					controllerComponentList.Add(component);
			}
		}

		if (target != null) {
			var component = target.GetComponent<NpcControllerComponent>();
			if (component != null && !controllerComponentList.Contains(component))
				controllerComponentList.Add(component);
		}

		foreach (var controllerComponent in controllerComponentList)
			controllerComponent.FireAction(action);
	}

	private void ComputeEventsToSingleTarget(ActionEnum action, IEntity target) {
		target?.GetComponent<NpcControllerComponent>()?.FireAction(action);
	}

	public void ComputeHit(NpcControllerComponent target) {
		if (IsCombatIgnored(target))
			return;
		ComputeActionToSingleTarget(ActionEnum.HitNpc, target.Owner);
		ComputeAggression(target);
	}

	public void ComputeShoot(NpcControllerComponent target) {
		if (IsCombatIgnored(target))
			return;
		ComputeActionToSingleTarget(ActionEnum.ShootNpc, target.Owner);
		ComputeAggression(target);
	}

	private void ComputeAggression(NpcControllerComponent target) {
		var component = target.Owner.GetComponent<ParametersComponent>();
		if (component != null) {
			var byName = component.GetByName<bool>(ParameterNameEnum.WasAttackedByPlayer);
			if (byName != null) {
				if (!byName.Value) {
					ComputeActionToSingleTarget(ActionEnum.FirstAttackNPC, target.Owner);
					if (target.Target != null)
						ComputeActionToSingleTarget(ActionEnum.SafeNpc, target.Target.Owner);
				}

				byName.Value = true;
			}
		}

		SetTarget(target);
		ServiceLocator.GetService<CombatService>()?.HitNpc(Owner, target.Owner);
		target.LastAttacker = Owner;
	}

	public void ComputeHitAnotherNPC(NpcControllerComponent target) {
		if (IsCombatIgnored(target))
			return;
		SetTarget(target);
		var delictInfo = GetDelictInfo(ActionEnum.FirstAttackNPC, target.Owner);
		if (delictInfo != null && delictInfo.Visible < 0.0)
			ComputeEvents(ActionEnum.HitAnotherGoodNPC, true, target.Owner);
		else
			ComputeEvents(ActionEnum.HitAnotherNPC, true, target.Owner);
	}

	private bool IsCombatIgnored(NpcControllerComponent target) {
		if (target == null)
			return false;
		var component = target.Owner.GetComponent<ParametersComponent>();
		if (component == null)
			return false;
		var byName = component.GetByName<bool>(ParameterNameEnum.IsCombatIgnored);
		return byName != null && byName.Value;
	}

	public void ComputeHealPain(IEntity target, float healed) {
		ComputeAction(ActionEnum.HealNpcPain, healed);
	}

	public void ComputeHealInfection(IEntity target, float healed) {
		ComputeAction(ActionEnum.HealNpcInfection, healed);
	}

	public void ComputeCureInfection(IEntity target) {
		ComputeActionToSingleTarget(ActionEnum.CureInfection, target);
	}

	public void ComputePicklock(IEntity target) {
		ComputeActionToSingleTarget(ActionEnum.BreakPicklock, target);
	}

	public void ComputeGiftNPC(IEntity target, float gift) {
		ComputeAction(ActionEnum.GiftNPC, gift);
	}

	public void ComputeRepairHydrant() {
		ComputeAction(ActionEnum.RepairHydrant);
	}

	private void SetTarget(NpcControllerComponent target) {
		if (this.target == target)
			return;
		if (this.target != null) {
			((Entity)this.target.Owner).RemoveListener(this);
			this.target.IsDead.ChangeValueEvent -= OnChangeDeadTarget;
			this.target = null;
		}

		this.target = target;
		if (this.target == null)
			return;
		((Entity)this.target.Owner).AddListener(this);
		this.target.IsDead.ChangeValueEvent += OnChangeDeadTarget;
	}

	private void OnChangeDeadTarget(bool value) {
		if (!value)
			return;
		ComputeDead(target);
		SetTarget(null);
	}

	private void OnDisposeEvent() {
		SetTarget(null);
	}

	private void ComputeDead(NpcControllerComponent target) {
		if (target == null || target.LastAttacker != Owner)
			return;
		ComputeActionToSingleTarget(ActionEnum.MurderNpc, target.Owner);
	}

	private void ComputeInfected() {
		var parentComponent = LocationItemUtility.FindParentComponent<LocationItemComponent>(Owner);
		if (parentComponent.IsIndoor || parameters == null)
			return;
		var byName = parameters.GetByName<float>(ParameterNameEnum.Infection);
		if (byName == null || byName.Value < (double)thresholdPlayerInfected)
			return;
		var component = parentComponent.LogicLocation.Owner.GetComponent<RegionComponent>();
		if (component == null || component.DiseaseLevel.Value > thresholdRegionInfected)
			return;
		ComputeAction(ActionEnum.SeeInfected);
	}

	public void ComputeUpdate() {
		if (InstanceByRequest<EngineApplication>.Instance.IsPaused || !Owner.IsEnabledInHierarchy)
			return;
		ComputeAway();
		ComputeDanger();
	}

	private void ComputeAway() {
		candidates.Clear();
		DetectorUtility.GetCandidats(ServiceLocator.GetService<DetectorService>().Detectablies, detector, locationItem,
			ExternalSettingsInstance<ExternalCommonSettings>.Instance.AwayDistance, target => {
				var component = target.Detectable.Owner.GetComponent<NpcControllerComponent>();
				if (component == null || component.IsDisposed)
					return;
				candidates.Add(component);
			});
		foreach (var candidate in candidates)
			if (nears.Add(candidate))
				candidate.IsAway.Value = false;
		foreach (var near in nears)
			if (candidates.Add(near) && !near.IsDisposed)
				near.IsAway.Value = true;
		foreach (var candidate in candidates)
			if (candidate.IsDisposed || candidate.IsAway.Value)
				nears.Remove(candidate);
	}

	private void ComputeDanger() {
		var danger = false;
		DetectorUtility.GetCandidats(ServiceLocator.GetService<DetectorService>().Detectablies, detector, locationItem,
			ExternalSettingsInstance<ExternalCommonSettings>.Instance.DangerDistance, target => {
				if (danger)
					return;
				var component1 = target.Detectable.Owner.GetComponent<NpcControllerComponent>();
				if (component1 == null || component1.IsDisposed)
					return;
				var component2 = target.Detectable.Owner.GetComponent<ParametersComponent>();
				if (component2 == null)
					return;
				var byName = component2.GetByName<FractionEnum>(ParameterNameEnum.Fraction);
				if (byName == null || !dangerousFractions.Contains(byName.Value))
					return;
				danger = true;
			});
		Danger = danger;
	}

	private void OnEnterRegionEvent(
		ref EventArgument<IEntity, IRegionComponent> eventArguments) {
		var region = navigation.Region;
		if (region == null)
			return;
		ComputeNotification(region);
		ComputeChangeLocation(region);
	}

	private void ComputeChangeLocation(IRegionComponent region) {
		if (!PlatformUtility.IsChangeLocationLoadingWindow(region))
			return;
		var locationBlueprint = ScriptableObjectInstance<ResourceFromCodeData>.Instance.ChangeLocationBlueprint;
		if (locationBlueprint != null)
			BlueprintServiceUtility.Start(locationBlueprint, Owner, null, "ChangeLocationBlueprint");
	}

	private void ComputeNotification(IRegionComponent region) {
		if (!CanReceiveMail.Value || notificationService == null)
			return;
		var byName = region.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Reputation);
		if (byName != null)
			lastSeenReputation = byName.Value;
		notificationService.AddNotify(NotificationEnum.Region, region);
	}

	private void OnChangeReputationValueEvent(float value) {
		if (!CanReceiveMail.Value || notificationService == null)
			return;
		var region = navigation.Region;
		if (region == null)
			return;
		var num = Reputation.Value;
		if (num == (double)lastSeenReputation)
			return;
		notificationService.AddNotify(NotificationEnum.Reputation, region, lastSeenReputation);
		lastSeenReputation = num;
	}

	public void OnEntityEvent(IEntity sender, EntityEvents kind) {
		if (kind != EntityEvents.DisposeEvent)
			return;
		OnDisposeEvent();
	}
}