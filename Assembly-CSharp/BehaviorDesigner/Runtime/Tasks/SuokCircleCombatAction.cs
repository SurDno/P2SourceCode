using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Services;

namespace BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Pathologic/GroupBehaviour/SuokCircle")]
[TaskIcon("{SkinColor}SequenceIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(SuokCircleCombatAction))]
public class SuokCircleCombatAction : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	private SuokCircleService suokService;

	public override void OnStart() {
		suokService = ServiceLocator.GetService<SuokCircleService>();
		if (suokService == null)
			return;
		suokService.RegisterCombatant(Owner.gameObject);
	}

	public override TaskStatus OnUpdate() {
		return TaskStatus.Running;
	}

	public override void OnEnd() {
		if (suokService == null)
			return;
		suokService.UnregisterCombatant(Owner.gameObject);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
	}
}