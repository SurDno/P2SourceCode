using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;

namespace BehaviorDesigner.Runtime
{
  [FactoryProxy(typeof (SharedNamedVariable))]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [Serializable]
  public class SharedNamedVariable : 
    SharedVariable<NamedVariable>,
    IStub,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsShared", mIsShared);
      DefaultDataWriteUtility.Write(writer, "Name", mName);
      DefaultDataWriteUtility.WriteSerialize(writer, "Value", mValue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      mIsShared = DefaultDataReadUtility.Read(reader, "IsShared", mIsShared);
      mName = DefaultDataReadUtility.Read(reader, "Name", mName);
      mValue = DefaultDataReadUtility.ReadSerialize<NamedVariable>(reader, "Value");
    }

    public SharedNamedVariable() => mValue = new NamedVariable();

    public static implicit operator SharedNamedVariable(NamedVariable value)
    {
      SharedNamedVariable sharedNamedVariable = new SharedNamedVariable();
      sharedNamedVariable.mValue = value;
      return sharedNamedVariable;
    }
  }
}
