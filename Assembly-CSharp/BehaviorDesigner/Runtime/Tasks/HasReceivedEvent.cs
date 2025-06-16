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

[FactoryProxy(typeof(HasReceivedEvent))]
[TaskDescription("Returns success as soon as the event specified by eventName has been received.")]
[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=123")]
[TaskIcon("{SkinColor}HasReceivedEventIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class HasReceivedEvent : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead {
	[Tooltip("The name of the event to receive")] [DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedString eventName = "";

	[Tooltip("Optionally store the first sent argument")]
	[SharedRequired]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy()]
	[SerializeField]
	public SharedVariable storedValue1;

	private bool eventReceived;
	private bool registered;

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "EventName", eventName);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "StoredValue1", storedValue1);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		eventName = BehaviorTreeDataReadUtility.ReadShared(reader, "EventName", eventName);
		storedValue1 = BehaviorTreeDataReadUtility.ReadShared(reader, "StoredValue1", storedValue1);
	}

	public override void OnStart() {
		if (registered)
			return;
		Owner.RegisterEvent(eventName.Value, ReceivedEvent);
		Owner.RegisterEvent(eventName.Value, new Action<object>(ReceivedEvent));
		Owner.RegisterEvent(eventName.Value, new Action<object, object>(ReceivedEvent));
		Owner.RegisterEvent(eventName.Value, new Action<object, object, object>(ReceivedEvent));
		registered = true;
	}

	public override TaskStatus OnUpdate() {
		return eventReceived ? TaskStatus.Success : TaskStatus.Failure;
	}

	public override void OnEnd() {
		if (eventReceived) {
			Owner.UnregisterEvent(eventName.Value, ReceivedEvent);
			Owner.UnregisterEvent(eventName.Value, new Action<object>(ReceivedEvent));
			Owner.UnregisterEvent(eventName.Value, new Action<object, object>(ReceivedEvent));
			Owner.UnregisterEvent(eventName.Value, new Action<object, object, object>(ReceivedEvent));
			registered = false;
		}

		eventReceived = false;
	}

	private void ReceivedEvent() {
		eventReceived = true;
	}

	private void ReceivedEvent(object arg1) {
		ReceivedEvent();
		if (storedValue1 == null || storedValue1.IsNone)
			return;
		storedValue1.SetValue(arg1);
	}

	private void ReceivedEvent(object arg1, object arg2) {
		ReceivedEvent();
		if (storedValue1 == null || storedValue1.IsNone)
			return;
		storedValue1.SetValue(arg1);
	}

	private void ReceivedEvent(object arg1, object arg2, object arg3) {
		ReceivedEvent();
		if (storedValue1 == null || storedValue1.IsNone)
			return;
		storedValue1.SetValue(arg1);
	}

	public override void OnBehaviorComplete() {
		Owner.UnregisterEvent(eventName.Value, ReceivedEvent);
		Owner.UnregisterEvent(eventName.Value, new Action<object>(ReceivedEvent));
		Owner.UnregisterEvent(eventName.Value, new Action<object, object>(ReceivedEvent));
		Owner.UnregisterEvent(eventName.Value, new Action<object, object, object>(ReceivedEvent));
		eventReceived = false;
		registered = false;
	}

	public override void OnReset() {
		eventName = "";
	}
}