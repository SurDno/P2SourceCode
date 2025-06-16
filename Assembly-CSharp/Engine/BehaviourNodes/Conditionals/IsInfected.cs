using System;
using System.Reflection;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace Engine.BehaviourNodes.Conditionals;

[TaskDescription("Is NPC infected? (use null Target to check self)")]
[TaskCategory("Pathologic")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(IsInfected))]
public class IsInfected : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedTransform Target;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	public SharedFloat Threshold = 0.0f;

	public override TaskStatus OnUpdate() {
		var entity = !(Target.Value == null)
			? EntityUtility.GetEntity(Target.Value.gameObject)
			: EntityUtility.GetEntity(gameObject);
		if (entity == null) {
			Debug.LogWarning(
				gameObject.name + " : entity not found, method : " + GetType().Name + ":" +
				MethodBase.GetCurrentMethod().Name, gameObject);
			return TaskStatus.Failure;
		}

		var component = entity.GetComponent<ParametersComponent>();
		if (component != null) {
			var byName = component.GetByName<float>(ParameterNameEnum.Infection);
			if (byName != null)
				return byName.Value > (double)Threshold.Value ? TaskStatus.Success : TaskStatus.Failure;
		}

		return TaskStatus.Failure;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "Threshold", Threshold);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
		Threshold = BehaviorTreeDataReadUtility.ReadShared(reader, "Threshold", Threshold);
	}
}