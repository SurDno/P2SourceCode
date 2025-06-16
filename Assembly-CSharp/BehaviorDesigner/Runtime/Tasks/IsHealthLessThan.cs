using System;
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

namespace BehaviorDesigner.Runtime.Tasks;

[TaskDescription("")]
[TaskCategory("Pathologic/Fight")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(IsHealthLessThan))]
public class IsHealthLessThan : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	public SharedFloat HealthThreshold = 0.0f;

	private IParameter<float> healthParameter;

	public override void OnAwake() {
		var owner = gameObject.GetComponent<EngineGameObject>().Owner;
		if (owner == null)
			return;
		var component = owner.GetComponent<ParametersComponent>();
		if (component == null)
			return;
		healthParameter = component.GetByName<float>(ParameterNameEnum.Health);
	}

	public override TaskStatus OnUpdate() {
		return healthParameter == null ? TaskStatus.Failure :
			healthParameter.Value <= (double)HealthThreshold.Value ? TaskStatus.Success : TaskStatus.Failure;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "HealthThreshold", HealthThreshold);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		HealthThreshold = BehaviorTreeDataReadUtility.ReadShared(reader, "HealthThreshold", HealthThreshold);
	}
}