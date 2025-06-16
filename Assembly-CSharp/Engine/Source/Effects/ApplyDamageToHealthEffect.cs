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
public class ApplyDamageToHealthEffect : IEffect {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected bool enable = true;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterNameEnum healthParameterName;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterNameEnum isCombatIgnoredParameterName;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterNameEnum immortalParameterName;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterNameEnum ballisticDamageParameterName;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterNameEnum fireDamageParameterName;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterNameEnum meleeDamageParameterName;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterNameEnum fallDamageParameterName;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterNameEnum fistsDamageParameterName;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected DurationTypeEnum durationType = DurationTypeEnum.None;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected bool realTime;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected float duration;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected float interval;

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

	public void ComputeEffect() {
		if (!enable)
			return;
		var component = Target?.GetComponent<ParametersComponent>();
		var byName1 = component?.GetByName<bool>(isCombatIgnoredParameterName);
		if (byName1 != null && byName1.Value)
			return;
		var byName2 = component?.GetByName<bool>(immortalParameterName);
		if (byName2 != null && byName2.Value)
			return;
		var byName3 = component?.GetByName<float>(healthParameterName);
		if (byName3 == null || byName3.Value < 0.0)
			return;
		var num1 = 0.0f;
		var byName4 = component?.GetByName<float>(ballisticDamageParameterName);
		if (byName4 != null)
			num1 = byName4.Value;
		var num2 = 0.0f;
		var byName5 = component?.GetByName<float>(fireDamageParameterName);
		if (byName5 != null)
			num2 = byName5.Value;
		var num3 = 0.0f;
		var byName6 = component?.GetByName<float>(meleeDamageParameterName);
		if (byName6 != null)
			num3 = byName6.Value;
		var num4 = 0.0f;
		var byName7 = component?.GetByName<float>(fallDamageParameterName);
		if (byName7 != null)
			num4 = byName7.Value;
		var num5 = 0.0f;
		var byName8 = component?.GetByName<float>(fistsDamageParameterName);
		if (byName8 != null)
			num5 = byName8.Value;
		byName3.Value -= num1 + num2 + num3 + num4 + num5;
	}

	public void Cleanup() { }
}