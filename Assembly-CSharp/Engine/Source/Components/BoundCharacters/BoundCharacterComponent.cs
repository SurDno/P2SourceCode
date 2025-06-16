using System;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.BoundCharacters;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Parameters;
using Engine.Source.Components.Maps;
using Engine.Source.Connections;
using Engine.Source.Services;
using Inspectors;

namespace Engine.Source.Components.BoundCharacters;

[Factory(typeof(IBoundCharacterComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave |
               TypeEnum.StateLoad)]
public class BoundCharacterComponent :
	EngineComponent,
	IBoundCharacterComponent,
	IComponent,
	INeedSave {
	[CopyableProxy]
	[DataReadProxy]
	[DataWriteProxy]
	[StateSaveProxy]
	[StateLoadProxy]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected bool isEnabled = true;

	[StateSaveProxy] [StateLoadProxy] protected BoundCharacterGroup group;

	[StateSaveProxy] [StateLoadProxy()] [Inspected(Mutable = true)]
	protected Typed<IBoundCharacterPlaceholder> placeholder;

	protected IEntity homeRegion;
	private bool added;
	[FromThis] private ParametersComponent parameters;
	[FromLocator] private BoundCharactersService boundCharactersService;

	[StateSaveProxy]
	[StateLoadProxy]
	[Inspected(Mutable = true)]
	public int SortOrder { get; set; }

	[StateSaveProxy]
	[StateLoadProxy]
	[Inspected(Mutable = true)]
	public BoundCharacterGroup SeenGroup { get; set; }

	[StateSaveProxy]
	[StateLoadProxy]
	[Inspected]
	public LocalizedText Name { get; set; } = LocalizedText.Empty;

	[Inspected]
	public IParameterValue<BoundHealthStateEnum> BoundHealthState { get; } = new ParameterValue<BoundHealthStateEnum>();

	[Inspected] public IParameterValue<bool> HealingAttempted { get; } = new ParameterValue<bool>();

	[Inspected] public IParameterValue<bool> ImmuneBoosterAttempted { get; } = new ParameterValue<bool>();

	[Inspected] public IParameterValue<float> Immunity { get; } = new ParameterValue<float>();

	[Inspected] public IParameterValue<float> Infection { get; } = new ParameterValue<float>();

	[Inspected] public IParameterValue<float> RandomRoll { get; } = new ParameterValue<float>();

	[StateSaveProxy]
	[StateLoadProxy]
	[Inspected(Mutable = true)]
	public BoundHealthStateEnum SeenBoundHealthState { get; set; } = BoundHealthStateEnum.None;

	[StateSaveProxy(MemberEnum.CustomReference)]
	[StateLoadProxy(MemberEnum.CustomReference)]
	[Inspected]
	public IEntity HomeRegion {
		get => homeRegion;
		set {
			if (homeRegion == value)
				return;
			if (homeRegion != null) {
				var component = homeRegion.GetComponent<MapItemComponent>();
				if (component != null)
					component.DiscoveredChangeEvent -= NotifyIfNotSeen;
			}

			homeRegion = value;
			if (homeRegion != null) {
				var component = homeRegion.GetComponent<MapItemComponent>();
				if (component != null)
					component.DiscoveredChangeEvent += NotifyIfNotSeen;
			}

			NotifyIfNotSeen();
		}
	}

	[Inspected] public bool PreRollStateStored { get; set; }

	[Inspected] public BoundHealthStateEnum PreRollHealthState { get; private set; }

	[Inspected] public float PreRollStatValue { get; private set; }

	[Inspected] public bool PreRollMedicated { get; set; }

	[Inspected(Mutable = true)]
	public bool IsEnabled {
		get => isEnabled;
		set {
			if (isEnabled == value)
				return;
			isEnabled = value;
			OnChangeEnabled();
		}
	}

	[Inspected(Mutable = true)]
	public BoundCharacterGroup Group {
		get => group;
		set {
			if (group == value)
				return;
			group = value;
			NotifyIfNotSeen();
		}
	}

	public bool Discovered {
		get => true;
		set { }
	}

	public IBoundCharacterPlaceholder Resource {
		get => placeholder.Value;
		set => placeholder.Value = value;
	}

	public bool NeedSave => true;

	private void NotifyIfNotSeen() {
		if (!added || !InstanceByRequest<EngineApplication>.Instance.ViewEnabled || group == BoundCharacterGroup.None)
			return;
		if (group == SeenGroup) {
			var boundHealthStateEnum = BoundCharacterUtility.PerceivedHealth(this);
			if (SeenBoundHealthState == boundHealthStateEnum || boundHealthStateEnum == BoundHealthStateEnum.Normal)
				return;
		}

		ServiceLocator.GetService<NotificationService>()
			.AddNotify(NotificationEnum.BoundCharacters, Array.Empty<object>());
	}

	public override void OnAdded() {
		base.OnAdded();
		BoundHealthState.Set(parameters?.GetByName<BoundHealthStateEnum>(ParameterNameEnum.BoundHealthState));
		BoundHealthState.ChangeValueEvent += OnBoundHealthStateValueChange;
		HealingAttempted.Set(parameters?.GetByName<bool>(ParameterNameEnum.HealingAttempted));
		ImmuneBoosterAttempted.Set(parameters?.GetByName<bool>(ParameterNameEnum.ImmuneBoostAttempted));
		Immunity.Set(parameters?.GetByName<float>(ParameterNameEnum.Immunity));
		Infection.Set(parameters?.GetByName<float>(ParameterNameEnum.Infection));
		RandomRoll.Set(parameters?.GetByName<float>(ParameterNameEnum.RandomRoll));
		OnEnableChangedEvent();
	}

	private void OnBoundHealthStateValueChange(BoundHealthStateEnum value) {
		NotifyIfNotSeen();
	}

	private void OnEnableChangedEvent() {
		if (IsEnabled && !IsDisposed)
			AddToService();
		else
			RemoveFromService();
	}

	public override void OnRemoved() {
		RemoveFromService();
		parameters = null;
		BoundHealthState.ChangeValueEvent -= OnBoundHealthStateValueChange;
		BoundHealthState.Set(null);
		HealingAttempted.Set(null);
		ImmuneBoosterAttempted.Set(null);
		Immunity.Set(null);
		Infection.Set(null);
		RandomRoll.Set(null);
		base.OnRemoved();
	}

	private void AddToService() {
		if (added)
			return;
		added = true;
		boundCharactersService.AddKeyCharacter(this);
		NotifyIfNotSeen();
	}

	private void RemoveFromService() {
		if (!added)
			return;
		added = false;
		boundCharactersService.RemoveKeyCharacter(this);
	}

	public override void OnChangeEnabled() {
		base.OnChangeEnabled();
		OnEnableChangedEvent();
	}

	[OnLoaded]
	private void OnLoaded() {
		OnEnableChangedEvent();
	}

	[Inspected]
	public void StorePreRollState() {
		PreRollStateStored = true;
		PreRollHealthState = BoundHealthState.Value;
		if (PreRollHealthState == BoundHealthStateEnum.Danger) {
			PreRollStatValue = Immunity.Value;
			PreRollMedicated = ImmuneBoosterAttempted.Value;
		} else if (PreRollHealthState == BoundHealthStateEnum.Diseased) {
			PreRollStatValue = Infection.Value;
			PreRollMedicated = HealingAttempted.Value;
		} else
			PreRollMedicated = false;
	}
}