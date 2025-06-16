using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks;

[FactoryProxy(typeof(SendEvent))]
[TaskDescription("Sends an event to the behavior tree, returns success after sending the event.")]
[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=121")]
[TaskIcon("{SkinColor}SendEventIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class SendEvent : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	[Tooltip(
		"The GameObject of the behavior tree that should have the event sent to it. If null use the current behavior")]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[SerializeField]
	public SharedGameObject targetGameObject;

	[Tooltip("The event to send")] [DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedString eventName;

	[Tooltip("The group of the behavior tree that the event should be sent to")]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[SerializeField]
	public SharedInt group;

	[Tooltip("Optionally specify a first argument to send")]
	[SharedRequired]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy]
	[SerializeField]
	public SharedVariable argument1;

	[Tooltip("Optionally specify a second argument to send")]
	[SharedRequired]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy()]
	[SerializeField]
	public SharedVariable argument2;

	private BehaviorTree behaviorTree;

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "TargetGameObject", targetGameObject);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "EventName", eventName);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "Group", group);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "Argument1", argument1);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "Argument2", argument2);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		targetGameObject = BehaviorTreeDataReadUtility.ReadShared(reader, "TargetGameObject", targetGameObject);
		eventName = BehaviorTreeDataReadUtility.ReadShared(reader, "EventName", eventName);
		group = BehaviorTreeDataReadUtility.ReadShared(reader, "Group", group);
		argument1 = BehaviorTreeDataReadUtility.ReadShared(reader, "Argument1", argument1);
		argument2 = BehaviorTreeDataReadUtility.ReadShared(reader, "Argument2", argument2);
	}

	public override void OnStart() {
		var components = GetDefaultGameObject(targetGameObject.Value).GetComponents<BehaviorTree>();
		if (components.Length == 1)
			behaviorTree = components[0];
		else {
			if (components.Length <= 1 || !(behaviorTree == null))
				return;
			behaviorTree = components[0];
		}
	}

	public override TaskStatus OnUpdate() {
		if (behaviorTree == null)
			return TaskStatus.Success;
		if (argument1 == null || argument1.IsNone)
			behaviorTree.SendEvent(eventName.Value);
		else if (argument2 == null || argument2.IsNone)
			behaviorTree.SendEvent(eventName.Value, argument1.GetValue());
		else
			behaviorTree.SendEvent(eventName.Value, argument1.GetValue(), argument2.GetValue());
		return TaskStatus.Success;
	}

	public override void OnReset() {
		targetGameObject = null;
		eventName = "";
	}
}