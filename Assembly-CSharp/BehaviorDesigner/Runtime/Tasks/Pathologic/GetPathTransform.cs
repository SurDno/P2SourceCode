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
using Scripts.Tools.Serializations.Converters;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic;

[TaskDescription("Get Spawnpoint")]
[TaskCategory("Pathologic/Movement")]
[TaskIcon("Pathologic_InstantIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(GetPathTransform))]
public class GetPathTransform : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	public SharedTransform Spawnpoint;

	public override void OnStart() {
		var owner = Owner.GetComponent<EngineGameObject>().Owner;
		if (owner == null)
			Debug.LogWarningFormat("{0} has no entity", gameObject.name);
		else {
			var component1 = owner.GetComponent<NavigationComponent>();
			if (component1 != null) {
				var setupPoint = component1.SetupPoint;
				if (setupPoint != null && ((IEntityView)setupPoint).GameObject != null) {
					var component2 = ((IEntityView)setupPoint).GameObject.GetComponent<PatrolPath>();
					var component3 = ((IEntityView)setupPoint).GameObject.GetComponent<PathPart>();
					if (component2 != null || component3 != null) {
						Spawnpoint.Value = ((IEntityView)setupPoint).GameObject.transform;
						return;
					}
				}
			}

			var component4 = owner.GetComponent<CrowdItemComponent>();
			if (component4 != null && component4.Point != null) {
				var entityPoint = component4.Point.EntityPoint;
				if (entityPoint == null)
					return;
				var gameObject = ((IEntityView)entityPoint).GameObject;
				if (gameObject == null)
					return;
				var component5 = gameObject.GetComponent<PatrolPath>();
				var component6 = gameObject.GetComponent<PathPart>();
				if (!(component5 != null) && !(component6 != null))
					return;
				Spawnpoint.Value = gameObject.transform;
			} else
				Debug.LogError(this.gameObject.name + " has no patrol path or path part!");
		}
	}

	public override TaskStatus OnUpdate() {
		return !(bool)(Object)Spawnpoint.Value ? TaskStatus.Failure : TaskStatus.Success;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		BehaviorTreeDataWriteUtility.WriteShared(writer, "Spawnpoint", Spawnpoint);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		Spawnpoint = BehaviorTreeDataReadUtility.ReadShared(reader, "Spawnpoint", Spawnpoint);
	}
}