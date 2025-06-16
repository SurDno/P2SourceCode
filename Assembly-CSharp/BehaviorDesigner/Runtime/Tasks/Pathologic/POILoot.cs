using System;
using System.Collections.Generic;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Inventory;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic;

[TaskDescription("POI Idle")]
[TaskCategory("Pathologic")]
[TaskIcon("Pathologic_IdleIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(POILoot))]
public class POILoot : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedFloat InPOITime;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public SharedTransform Target;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	private List<string> lootContainers;

	private NpcState npcState;

	public override void OnAwake() {
		npcState = gameObject.GetComponent<NpcState>();
		if (!(npcState == null))
			return;
		Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof(NpcState).Name + " engine component");
	}

	public override void OnStart() {
		if (npcState == null)
			return;
		npcState.Loot(InPOITime.Value, Target.Value.gameObject);
		TakeItems();
	}

	public override TaskStatus OnUpdate() {
		if (npcState.CurrentNpcState != NpcStateEnum.PointOfInterest)
			return TaskStatus.Failure;
		switch (npcState.Status) {
			case NpcStateStatusEnum.Success:
				return TaskStatus.Success;
			case NpcStateStatusEnum.Failed:
				return TaskStatus.Failure;
			default:
				return TaskStatus.Running;
		}
	}

	public override void OnEnd() { }

	private void TakeItems() {
		var component1 = Owner?.GetComponent<EngineGameObject>()?.Owner?.GetComponent<StorageComponent>();
		var component2 = Target.Value?.GetComponent<EngineGameObject>()?.Owner?.GetComponent<StorageComponent>();
		if (component1 == null || component2 == null)
			return;
		var all1 = new List<IStorableComponent>(component2.Items).FindAll(x =>
			lootContainers.Exists(y => y == x.Container.Owner.Template.Name));
		var all2 = new List<IInventoryComponent>(component1.Containers).FindAll(x =>
			lootContainers.Exists(y => y == x.Owner.Template.Name));
		foreach (var storableComponent in all1) {
			var storable = (StorableComponent)storableComponent;
			if (storable != null && storable.Owner != null && storable.Storage != null)
				foreach (var container in all2)
					if (component1 != null && container != null) {
						var intersect = StorageUtility.GetIntersect(component1, container, storable, null);
						if (intersect.IsAllowed) {
							((StorageComponent)storable.Storage).MoveItem(storableComponent, intersect.Storage,
								intersect.Container, intersect.Cell.To());
							break;
						}
					}
		}
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "InPOITime", InPOITime);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
		DefaultDataWriteUtility.WriteList(writer, "LootContainers", lootContainers);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		InPOITime = BehaviorTreeDataReadUtility.ReadShared(reader, "InPOITime", InPOITime);
		Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
		lootContainers = DefaultDataReadUtility.ReadList(reader, "LootContainers", lootContainers);
	}
}