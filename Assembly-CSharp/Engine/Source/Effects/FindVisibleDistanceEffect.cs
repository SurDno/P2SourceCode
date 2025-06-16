using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Services.Engine;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Engine.Source.Services;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Effects;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class FindVisibleDistanceEffect : IEffect {
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
	protected ParameterNameEnum VisibileDistanceParameterName;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterNameEnum FlashlightOnParameterName;

	private float lastTime;
	private float startTime;

	[Inspected] public AbilityItem AbilityItem { get; set; }

	public IEntity Target { get; set; }

	public string Name => GetType().Name;

	public ParameterEffectQueueEnum Queue => queue;

	public bool Prepare(float currentRealTime, float currentGameTime) {
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

	private float ComputeSkyLight() {
		var skyLight = 1f;
		var tod = ServiceLocator.GetService<EnvironmentService>().Tod;
		if (tod != null)
			skyLight = tod.LerpValue;
		return skyLight;
	}

	private void ComputeEffect() {
		var component = Target?.GetComponent<ParametersComponent>();
		var byName1 = component?.GetByName<float>(VisibileDistanceParameterName);
		if (byName1 == null)
			return;
		if (Target.GetComponent<LocationItemComponent>().IsIndoor)
			byName1.Value = byName1.MaxValue;
		else {
			var skyLight = ComputeSkyLight();
			var num1 = 0.0f;
			var byName2 = component?.GetByName<bool>(FlashlightOnParameterName);
			if (byName2 != null)
				num1 = byName2.Value ? 1f : 0.0f;
			var num2 = 0;
			var service = ServiceLocator.GetService<LightService>();
			if (service != null)
				num2 = service.PlayerIsLighted ? 1 : 0;
			var t = skyLight;
			if (t < (double)num1)
				t = num1;
			if (t < (double)num2)
				t = num2;
			var num3 = Mathf.Lerp(byName1.MinValue, byName1.MaxValue, t);
			byName1.Value = num3;
		}
	}

	public void Cleanup() { }
}