using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityNavMeshAgent;

[FactoryProxy(typeof(Stop))]
[TaskCategory("Basic/NavMeshAgent")]
[TaskDescription("Stop movement of this agent along its current path. Returns Success.")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class Stop : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy()]
	[SerializeField]
	public SharedGameObject targetGameObject;

	private NavMeshAgent navMeshAgent;
	private GameObject prevGameObject;

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "TargetGameObject", targetGameObject);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		targetGameObject = BehaviorTreeDataReadUtility.ReadShared(reader, "TargetGameObject", targetGameObject);
	}

	public override void OnStart() {
		var defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
		if (!(defaultGameObject != prevGameObject))
			return;
		navMeshAgent = defaultGameObject.GetComponent<NavMeshAgent>();
		prevGameObject = defaultGameObject;
	}

	public override TaskStatus OnUpdate() {
		if (navMeshAgent == null) {
			Debug.LogWarning("NavMeshAgent is null");
			return TaskStatus.Failure;
		}

		navMeshAgent.isStopped = true;
		return TaskStatus.Success;
	}

	public override void OnReset() {
		targetGameObject = null;
	}
}