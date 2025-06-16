using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.Services.Factories;
using Inspectors;

namespace Engine.Source.Commons.Abilities.Controllers;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class FlameAbilityController : IAbilityController, IUpdatable {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected bool realTime = false;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected double interval = 0.0;

	private IEntity owner;
	private AbilityItem abilityItem;
	private PivotSanitar sanitar;
	private double time;

	public void Initialise(AbilityItem abilityItem) {
		this.abilityItem = abilityItem;
		owner = abilityItem.Ability.Owner;
		((IEntityView)owner).OnGameObjectChangedEvent += OnGameObjectChangedEvent;
		OnGameObjectChangedEvent();
	}

	public void Shutdown() {
		((IEntityView)owner).OnGameObjectChangedEvent -= OnGameObjectChangedEvent;
	}

	private void OnGameObjectChangedEvent() {
		if (sanitar != null) {
			InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
			sanitar = null;
		}

		if (!((IEntityView)owner).IsAttached)
			return;
		sanitar = ((IEntityView)owner).GameObject.GetComponent<PivotSanitar>();
		if (sanitar != null) {
			time = 0.0;
			InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
		}
	}

	public void ComputeUpdate() {
		if (sanitar == null)
			return;
		if (!sanitar.Flamethrower)
			time = 0.0;
		else {
			var service = ServiceLocator.GetService<TimeService>();
			var num = realTime ? service.RealTime.TotalSeconds : service.AbsoluteGameTime.TotalSeconds;
			if (time + interval > num)
				return;
			time = num;
			abilityItem.Active = true;
			abilityItem.Active = false;
		}
	}
}