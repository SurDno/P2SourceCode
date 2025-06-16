using System.Linq;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Effects;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class VisualFireEffect : IEffect {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected bool single = false;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected bool realTime = false;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected float duration = 0.0f;

	private float fadeOutTime = 2f;
	private float startTime;
	private RendererBurn effect;
	private IParameter<bool> burningParameter;

	public string Name => GetType().Name;

	[Inspected] public AbilityItem AbilityItem { get; set; }

	public IEntity Target { get; set; }

	public ParameterEffectQueueEnum Queue => queue;

	private bool TargetIgnored(IEntity target) {
		var component = target.GetComponent<ParametersComponent>();
		var byName1 = component?.GetByName<bool>(ParameterNameEnum.IsCombatIgnored);
		if (byName1 != null && byName1.Value)
			return true;
		var byName2 = component?.GetByName<float>(ParameterNameEnum.FireArmor);
		return byName2 != null && byName2.Value >= 1.0;
	}

	public bool Prepare(float currentRealTime, float currentGameTime) {
		if (TargetIgnored(Target))
			return true;
		var num = realTime ? currentRealTime : currentGameTime;
		if (single && !Name.IsNullOrEmpty() &&
		    Target.GetComponent<EffectsComponent>().Effects.FirstOrDefault(o => o.Name == Name) is VisualFireEffect
			    visualFireEffect) {
			visualFireEffect.startTime = num;
			return false;
		}

		startTime = num;
		var biggestRenderer = RendererUtility.GetBiggestRenderer(((IEntityView)Target).GameObject);
		var rendererBurn = ScriptableObjectInstance<ResourceFromCodeData>.Instance.RendererBurn;
		if (rendererBurn != null) {
			effect = UnityFactory.Instantiate<RendererBurn>(rendererBurn, "[Effects]");
			effect.BurningRenderer = biggestRenderer;
			effect.Strength = 1f;
		}

		burningParameter = Target.GetComponent<ParametersComponent>().GetByName<bool>(ParameterNameEnum.IsBurning);
		if (burningParameter != null)
			burningParameter.Value = true;
		return true;
	}

	public bool Compute(float currentRealTime, float currentGameTime) {
		if (TargetIgnored(Target))
			return false;
		var num1 = realTime ? currentRealTime : currentGameTime;
		var num2 = duration - (num1 - startTime);
		effect.Strength = num2 >= (double)fadeOutTime ? 1f : num2 / fadeOutTime;
		if (num1 - (double)startTime > duration) {
			if (burningParameter != null)
				burningParameter.Value = false;
			return false;
		}

		return burningParameter == null || burningParameter.Value;
	}

	public void Cleanup() {
		if (!(effect != null))
			return;
		UnityFactory.Destroy(effect);
		effect = null;
	}
}