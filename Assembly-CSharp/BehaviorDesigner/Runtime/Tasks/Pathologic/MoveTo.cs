using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components.Utilities;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic;

[TaskDescription("Move To")]
[TaskCategory("Pathologic/Movement")]
[TaskIcon("Pathologic_LongIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(MoveTo))]
public class MoveTo : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedVector3 Target;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	public SharedBool FailOnPartialPath = true;

	protected EngineBehavior behavior;
	protected NpcState npcState;
	protected NavMeshAgent agent;
	protected bool inited;

	public override void OnStart() {
		if (!inited) {
			agent = gameObject.GetComponent<NavMeshAgent>();
			if (agent == null) {
				Debug.LogWarning(gameObject.name + ": doesn't contain NavMeshAgent unity component", gameObject);
				return;
			}

			behavior = gameObject.GetComponent<EngineBehavior>();
			if (behavior == null) {
				Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof(EngineBehavior).Name +
				                 " unity component");
				return;
			}

			npcState = gameObject.GetComponent<NpcState>();
			if (npcState == null) {
				Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof(NpcState).Name + " engine component");
				return;
			}

			inited = true;
		}

		var vector3 = Target.Value;
		if (false)
			return;
		npcState.Move(Target.Value, FailOnPartialPath.Value);
	}

	public override TaskStatus OnUpdate() {
		if (!inited)
			return TaskStatus.Failure;
		var vector3 = Target.Value;
		if (npcState.Status == NpcStateStatusEnum.Failed)
			return TaskStatus.Failure;
		return npcState.Status == NpcStateStatusEnum.Success ? TaskStatus.Success : TaskStatus.Running;
	}

	public override void OnDrawGizmos() {
		NavMeshUtility.DrawPath(agent);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "FailOnPartialPath", FailOnPartialPath);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
		FailOnPartialPath = BehaviorTreeDataReadUtility.ReadShared(reader, "FailOnPartialPath", FailOnPartialPath);
	}
}