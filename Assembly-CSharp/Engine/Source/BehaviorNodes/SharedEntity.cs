using System;
using BehaviorDesigner.Runtime;
using Cofe.Proxies;
using Cofe.Serializations.Converters;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;

namespace Engine.Source.BehaviorNodes;

[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof(SharedEntity))]
[Serializable]
public class SharedEntity : SharedVariable<string>, IStub, ISerializeDataWrite, ISerializeDataRead {
	public IEntity Entity {
		get {
			if (mValue == null)
				return null;
			var guid = DefaultConverter.ParseGuid(mValue);
			return ServiceLocator.GetService<ISimulation>().Get(guid);
		}
		set => mValue = value != null ? value.Id.ToString() : "";
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "IsShared", mIsShared);
		DefaultDataWriteUtility.Write(writer, "Name", mName);
		DefaultDataWriteUtility.Write(writer, "Value", mValue);
	}

	public void DataRead(IDataReader reader, Type type) {
		mIsShared = DefaultDataReadUtility.Read(reader, "IsShared", mIsShared);
		mName = DefaultDataReadUtility.Read(reader, "Name", mName);
		mValue = DefaultDataReadUtility.Read(reader, "Value", mValue);
	}
}