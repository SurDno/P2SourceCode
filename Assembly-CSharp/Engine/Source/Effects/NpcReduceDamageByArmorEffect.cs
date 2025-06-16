using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Effects;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class NpcReduceDamageByArmorEffect : IEffect {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected bool enable = true;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected DurationTypeEnum durationType = DurationTypeEnum.None;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected bool realTime;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected float duration;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected float interval;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterNameEnum damageParameterName;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterNameEnum armorParameterName;

	private float lastTime;
	private float startTime;

	[Inspected] public AbilityItem AbilityItem { get; set; }

	public IEntity Target { get; set; }

	public string Name => GetType().Name;

	public ParameterEffectQueueEnum Queue => queue;

	public bool Prepare(float currentRealTime, float currentGameTime) {
		var num = realTime ? currentRealTime : currentGameTime;
		if (durationType == DurationTypeEnum.ByAbility)
			AbilityItem.AddDependEffect(this);
		startTime = num;
		lastTime = num;
		return true;
	}

	public bool Compute(float currentRealTime, float currentGameTime) {
		var num = realTime ? currentRealTime : currentGameTime;
		if ((durationType == DurationTypeEnum.ByDuration && num - (double)startTime > duration) ||
		    (durationType == DurationTypeEnum.ByAbility && (AbilityItem == null || !AbilityItem.Active)))
			return false;
		if (interval == 0.0) {
			lastTime = num;
			ComputeEffect();
		} else
			while (num - (double)this.lastTime >= interval) {
				var lastTime = this.lastTime;
				this.lastTime += interval;
				if (lastTime == (double)this.lastTime) {
					Debug.LogError("Error compute effects, effect name : " + Name + " , target : " + Target.GetInfo());
					break;
				}

				ComputeEffect();
			}

		return durationType != DurationTypeEnum.None && durationType != DurationTypeEnum.Once;
	}

	public void Cleanup() { }

	private void ComputeEffect() {
		if (!enable)
			return;
		var component = Target?.GetComponent<ParametersComponent>();
		var byName1 = component?.GetByName<float>(damageParameterName);
		if (byName1 == null)
			return;
		var byName2 = component?.GetByName<float>(armorParameterName);
		if (byName2 == null)
			return;
		byName1.Value -= byName1.Value * byName2.Value;
	}
}