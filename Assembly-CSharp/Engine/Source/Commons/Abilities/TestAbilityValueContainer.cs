using System.Collections.Generic;
using System.Linq;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Connections;
using Engine.Source.Effects.Values;
using Engine.Source.Services;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Commons.Abilities;

[Factory(typeof(TestAbilityValueContainer))]
[GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class TestAbilityValueContainer : EngineObject, IAbilityValueContainer {
	[DataReadProxy] [DataWriteProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)] [CopyableProxy]
	protected List<AbilityValueInfo> values = new();

	[DataReadProxy] [DataWriteProxy] [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)] [CopyableProxy()]
	protected UnityAsset<GameObject> blueprint;

	public IAbilityValue<T> GetAbilityValue<T>(AbilityValueNameEnum name) where T : struct {
		var abilityValueInfo = values.FirstOrDefault(o => o.Name == name);
		return abilityValueInfo != null ? abilityValueInfo.Value as IAbilityValue<T> : null;
	}

	[Inspected]
	private void CreateEffect() {
		BlueprintServiceUtility.Start(blueprint, this, null);
	}
}