using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Services;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Pathologic/GroupBehaviour")]
[TaskIcon("{SkinColor}SequenceIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(CombatAction))]
public class CombatAction : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedTransform Enemy;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	public SharedBool Watch;

	private CombatService combatService;
	private EnemyBase character;
	private bool inited;

	public override void OnStart() {
		if (Enemy.Value == null)
			return;
		combatService = ServiceLocator.GetService<CombatService>();
		character = Owner.GetComponent<EnemyBase>();
		if (combatService == null)
			return;
		combatService.EnterCombat(character, Enemy.Value.GetComponent<EnemyBase>(), Watch.Value);
		inited = true;
	}

	public override TaskStatus OnUpdate() {
		if (!inited)
			return TaskStatus.Success;
		if (combatService == null)
			return TaskStatus.Failure;
		return combatService.CharacterIsInCombat(character) ? TaskStatus.Running : TaskStatus.Success;
	}

	public override void OnEnd() {
		inited = false;
		if (combatService == null)
			return;
		combatService.ExitCombat(character);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "Enemy", Enemy);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "Watch", Watch);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		Enemy = BehaviorTreeDataReadUtility.ReadShared(reader, "Enemy", Enemy);
		Watch = BehaviorTreeDataReadUtility.ReadShared(reader, "Watch", Watch);
	}
}