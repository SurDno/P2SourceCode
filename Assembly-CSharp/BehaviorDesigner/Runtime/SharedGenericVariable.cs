using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;

namespace BehaviorDesigner.Runtime;

[FactoryProxy(typeof(SharedGenericVariable))]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[Serializable]
public class SharedGenericVariable :
	SharedVariable<GenericVariable>,
	IStub,
	ISerializeDataWrite,
	ISerializeDataRead {
	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "IsShared", mIsShared);
		DefaultDataWriteUtility.Write(writer, "Name", mName);
		DefaultDataWriteUtility.WriteSerialize(writer, "Value", mValue);
	}

	public void DataRead(IDataReader reader, Type type) {
		mIsShared = DefaultDataReadUtility.Read(reader, "IsShared", mIsShared);
		mName = DefaultDataReadUtility.Read(reader, "Name", mName);
		mValue = DefaultDataReadUtility.ReadSerialize<GenericVariable>(reader, "Value");
	}

	public SharedGenericVariable() {
		mValue = new GenericVariable();
	}

	public static implicit operator SharedGenericVariable(GenericVariable value) {
		var sharedGenericVariable = new SharedGenericVariable();
		sharedGenericVariable.mValue = value;
		return sharedGenericVariable;
	}
}