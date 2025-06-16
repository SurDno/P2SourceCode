using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Connections;
using Engine.Source.Services;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Effects;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class BlueprintEffect : IEffect {
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected string name = "";

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected UnityAsset<GameObject> blueprint;

	public string Name => name;

	[Inspected] public AbilityItem AbilityItem { get; set; }

	public IEntity Target { get; set; }

	public ParameterEffectQueueEnum Queue => queue;

	public void Cleanup() { }

	public bool Prepare(float currentRealTime, float currentGameTime) {
		return true;
	}

	public bool Compute(float currentRealTime, float currentGameTime) {
		BlueprintServiceUtility.StartAsync(blueprint, AbilityItem.AbilityController as IAbilityValueContainer, Target,
			null, null, false);
		return false;
	}
}