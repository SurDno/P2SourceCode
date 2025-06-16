using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic;

[TaskDescription("Has Setup point or crowd point GO")]
[TaskCategory("Pathologic/Movement")]
[TaskIcon("Pathologic_InstantIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(HasSetupPointOrCrowdPoint))]
public class HasSetupPointOrCrowdPoint :
	Conditional,
	IStub,
	ISerializeDataWrite,
	ISerializeDataRead {
	private bool success;

	public override void OnStart() {
		var owner = Owner.GetComponent<EngineGameObject>().Owner;
		if (owner == null)
			Debug.LogWarningFormat("{0} has no entity", gameObject.name);
		else
			success = HasSetupPoint(owner) || HasCrowdPoint(owner);
	}

	private bool HasSetupPoint(IEntity entity) {
		var component = entity.GetComponent<NavigationComponent>();
		if (component == null)
			return false;
		var setupPoint = component.SetupPoint;
		return setupPoint != null && ((IEntityView)setupPoint).GameObject != null;
	}

	private bool HasCrowdPoint(IEntity entity) {
		var component = entity.GetComponent<CrowdItemComponent>();
		if (component == null || component.Point == null)
			return false;
		var entityPoint = component.Point.EntityPoint;
		return entityPoint != null && ((IEntityView)entityPoint).GameObject != null;
	}

	public override TaskStatus OnUpdate() {
		return success ? TaskStatus.Success : TaskStatus.Failure;
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