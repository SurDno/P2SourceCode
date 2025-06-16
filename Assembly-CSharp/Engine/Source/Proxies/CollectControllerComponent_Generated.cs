using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(CollectControllerComponent))]
public class CollectControllerComponent_Generated :
	CollectControllerComponent,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<CollectControllerComponent_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var componentGenerated = (CollectControllerComponent_Generated)target2;
		componentGenerated.itemEntity = itemEntity;
		componentGenerated.sendActionEvent = sendActionEvent;
	}

	public void DataWrite(IDataWriter writer) {
		UnityDataWriteUtility.Write(writer, "Storable", itemEntity);
		DefaultDataWriteUtility.Write(writer, "SendActionEvent", sendActionEvent);
	}

	public void DataRead(IDataReader reader, Type type) {
		itemEntity = UnityDataReadUtility.Read(reader, "Storable", itemEntity);
		sendActionEvent = DefaultDataReadUtility.Read(reader, "SendActionEvent", sendActionEvent);
	}
}