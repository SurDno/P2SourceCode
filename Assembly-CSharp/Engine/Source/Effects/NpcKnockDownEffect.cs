using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Abilities.Controllers;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Effects;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class NpcKnockDownEffect : IEffect {
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected string name = "";

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [Inspected] [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;

	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy()]
	[Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
	protected float holdTime = 5f;

	private IParameter<bool> movementBlockParameter;
	private float startTime;

	public string Name => name;

	[Inspected] public AbilityItem AbilityItem { get; set; }

	public IEntity Target { get; set; }

	public ParameterEffectQueueEnum Queue => queue;

	public void Cleanup() {
		if (movementBlockParameter == null)
			return;
		movementBlockParameter.Value = false;
	}

	public bool Prepare(float currentRealTime, float currentGameTime) {
		var component1 = ((IEntityView)AbilityItem.Self).GameObject.GetComponent<EnemyBase>();
		var component2 = ((IEntityView)Target).GameObject.GetComponent<EnemyBase>();
		if (!(AbilityItem.AbilityController is CloseCombatAbilityController)) {
			Debug.LogError(typeof(NpcKnockDownEffect).Name + " requires " + typeof(CloseCombatAbilityController).Name);
			return false;
		}

		component2?.KnockDown(component1);
		startTime = currentRealTime;
		var component3 = Target.GetComponent<ParametersComponent>();
		if (component3 != null) {
			movementBlockParameter = component3.GetByName<bool>(ParameterNameEnum.MovementControlBlock);
			if (movementBlockParameter != null)
				movementBlockParameter.Value = true;
		}

		return true;
	}

	public bool Compute(float currentRealTime, float currentGameTime) {
		return movementBlockParameter != null && currentRealTime - (double)startTime < holdTime;
	}
}