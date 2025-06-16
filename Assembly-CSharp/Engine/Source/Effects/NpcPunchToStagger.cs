using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Abilities.Controllers;
using Engine.Source.Commons.Effects;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Effects;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class NpcPunchToStagger : IEffect {
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected string name = "";

	[DataReadProxy(Name = "punchType")]
	[DataWriteProxy(Name = "punchType")]
	[CopyableProxy]
	[Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected PunchTypeEnum punchEnum;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;

	public string Name => name;

	[Inspected] public AbilityItem AbilityItem { get; set; }

	public IEntity Target { get; set; }

	public ParameterEffectQueueEnum Queue => queue;

	public void Cleanup() { }

	public bool Prepare(float currentRealTime, float currentGameTime) {
		var component1 = ((IEntityView)AbilityItem.Self).GameObject.GetComponent<EnemyBase>();
		var component2 = ((IEntityView)Target).GameObject.GetComponent<EnemyBase>();
		if (!(AbilityItem.AbilityController is CloseCombatAbilityController abilityController)) {
			Debug.LogError(typeof(NpcPunchEffect).Name + " requires " + typeof(CloseCombatAbilityController).Name);
			return false;
		}

		component2?.PunchToStagger(punchEnum, abilityController.ReactionType, abilityController.WeaponKind, component1);
		return true;
	}

	public bool Compute(float currentRealTime, float currentGameTime) {
		return false;
	}
}