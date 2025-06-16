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
[FactoryProxy(typeof(HearedCombatCry))]
public class HearedCombatCry : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	public SharedTransform Target;

	private CombatService combatService;
	private CombatServiceCharacterInfo character;

	public override void OnStart() {
		combatService = ServiceLocator.GetService<CombatService>();
		character = combatService.GetCharacterInfo(gameObject.GetComponent<EnemyBase>());
	}

	public override TaskStatus OnUpdate() {
		if (character == null || character.Character == null || character.HearedCries.Count <= 0)
			return TaskStatus.Failure;
		if (character?.HearedCries[0]?.Character?.Character != null)
			Target.Value = character?.HearedCries[0]?.Character?.Character?.transform;
		return TaskStatus.Success;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
	}
}