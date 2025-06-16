using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using System;

namespace BehaviorDesigner.Runtime
{
  [FactoryProxy(typeof (SharedInt))]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [Serializable]
  public class SharedInt : SharedVariable<int>, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsShared", this.mIsShared);
      DefaultDataWriteUtility.Write(writer, "Name", this.mName);
      DefaultDataWriteUtility.Write(writer, "Value", this.mValue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.mIsShared = DefaultDataReadUtility.Read(reader, "IsShared", this.mIsShared);
      this.mName = DefaultDataReadUtility.Read(reader, "Name", this.mName);
      this.mValue = DefaultDataReadUtility.Read(reader, "Value", this.mValue);
    }

    public static implicit operator SharedInt(int value)
    {
      SharedInt sharedInt = new SharedInt();
      sharedInt.mValue = value;
      return sharedInt;
    }
  }
}
