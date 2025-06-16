using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Engines.Controllers;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic;

[TaskDescription("Look at target OFF")]
[TaskCategory("Pathologic/IK")]
[TaskIcon("Pathologic_InstantIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(LookAtOff))]
public class LookAtOff : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	private IKController ikController;

	public override TaskStatus OnUpdate() {
		if (ikController == null) {
			ikController = gameObject.GetComponent<IKController>();
			if (ikController == null) {
				Debug.LogError(gameObject.name + ": doesn't contain " + typeof(IKController).Name + " unity component",
					gameObject);
				return TaskStatus.Failure;
			}
		}

		ikController.LookTarget = null;
		return TaskStatus.Success;
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