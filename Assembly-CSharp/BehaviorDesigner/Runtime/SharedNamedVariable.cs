using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using System;

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
      DefaultDataWriteUtility.Write(writer, "IsShared", this.mIsShared);
      DefaultDataWriteUtility.Write(writer, "Name", this.mName);
      DefaultDataWriteUtility.WriteSerialize<NamedVariable>(writer, "Value", this.mValue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.mIsShared = DefaultDataReadUtility.Read(reader, "IsShared", this.mIsShared);
      this.mName = DefaultDataReadUtility.Read(reader, "Name", this.mName);
      this.mValue = DefaultDataReadUtility.ReadSerialize<NamedVariable>(reader, "Value");
    }

    public SharedNamedVariable() => this.mValue = new NamedVariable();

    public static implicit operator SharedNamedVariable(NamedVariable value)
    {
      SharedNamedVariable sharedNamedVariable = new SharedNamedVariable();
      sharedNamedVariable.mValue = value;
      return sharedNamedVariable;
    }
  }
}
