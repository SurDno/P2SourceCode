using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Engine.Source.Difficulties;
using Inspectors;

namespace Engine.Source.Effects;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class ChangeItemParameterEffect : IEffect {
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
	protected ParameterNameEnum itemParameterName;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected float itemParameterChange = 0.0f;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected string difficultyMultiplierParameterName = "";

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
		if (num - (double)lastTime >= interval) {
			lastTime = num;
			ComputeEffect();
		}

		return durationType != DurationTypeEnum.None && durationType != DurationTypeEnum.Once;
	}

	public void Cleanup() { }

	private void ComputeEffect() {
		if (!enable)
			return;
		var byName = AbilityItem?.Item?.GetComponent<ParametersComponent>()?.GetByName<float>(itemParameterName);
		var num = string.IsNullOrEmpty(difficultyMultiplierParameterName)
			? 1f
			: InstanceByRequest<DifficultySettings>.Instance.GetValue(difficultyMultiplierParameterName);
		if (byName == null)
			return;
		byName.Value += itemParameterChange * num;
	}
}