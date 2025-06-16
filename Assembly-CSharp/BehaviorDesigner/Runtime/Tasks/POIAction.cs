using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Services;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Pathologic/GroupBehaviour")]
[TaskIcon("{SkinColor}SequenceIcon.png")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(POIAction))]
public class POIAction : Action, IStub, ISerializeDataWrite, ISerializeDataRead {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy] [SerializeField]
	public bool SearchClosestPOI;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()] [SerializeField]
	public bool Crowd;

	private POIService poiService;

	public override void OnStart() {
		poiService = ServiceLocator.GetService<POIService>();
		if (poiService == null)
			return;
		poiService.RegisterCharacter(Owner.gameObject, SearchClosestPOI, Crowd);
	}

	public override TaskStatus OnUpdate() {
		return TaskStatus.Running;
	}

	public override void OnEnd() {
		if (poiService == null)
			return;
		poiService.UnregisterCharacter(Owner.gameObject);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
		DefaultDataWriteUtility.Write(writer, "Instant", instant);
		DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
		DefaultDataWriteUtility.Write(writer, "SearchClosestPOI", SearchClosestPOI);
		DefaultDataWriteUtility.Write(writer, "Crowd", Crowd);
	}

	public void DataRead(IDataReader reader, Type type) {
		nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
		instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
		disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
		SearchClosestPOI = DefaultDataReadUtility.Read(reader, "SearchClosestPOI", SearchClosestPOI);
		Crowd = DefaultDataReadUtility.Read(reader, "Crowd", Crowd);
	}
}