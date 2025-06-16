using System.Linq;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Engine.Source.Effects.Engine;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Effects;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class ExpressionEffect : IEffect {
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected string name = "";

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected bool enable = true;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected bool single;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected DurationTypeEnum durationType = DurationTypeEnum.None;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected bool realTime;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected float duration;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected float interval;

	[DataReadProxy]
	[DataWriteProxy()]
	[CopyableProxy(MemberEnum.RuntimeOnlyCopy)]
	[Inspected]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected IEffectValueSetter expression;

	private float lastTime;
	private float startTime;

	public string Name => name;

	[Inspected] public AbilityItem AbilityItem { get; set; }

	public IEntity Target { get; set; }

	public ParameterEffectQueueEnum Queue => queue;

	public bool Prepare(float currentRealTime, float currentGameTime) {
		var num = realTime ? currentRealTime : currentGameTime;
		if (single && !name.IsNullOrEmpty() &&
		    Target.GetComponent<EffectsComponent>().Effects.FirstOrDefault(o => o.Name == name) is ExpressionEffect
			    expressionEffect) {
			expressionEffect.startTime = num;
			return false;
		}

		if (durationType == DurationTypeEnum.ByAbility)
			AbilityItem.AddDependEffect(this);
		startTime = num;
		lastTime = num;
		if (durationType == DurationTypeEnum.Once)
			Target.GetComponent<EffectsComponent>().ForceComputeUpdate();
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
		if (!enable || expression == null)
			return;
		expression.Compute(this);
	}
}