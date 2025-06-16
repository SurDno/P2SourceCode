using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;

namespace BehaviorDesigner.Runtime
{
  [FactoryProxy(typeof (SharedString))]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [Serializable]
  public class SharedString : SharedVariable<string>, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsShared", mIsShared);
      DefaultDataWriteUtility.Write(writer, "Name", mName);
      DefaultDataWriteUtility.Write(writer, "Value", mValue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      mIsShared = DefaultDataReadUtility.Read(reader, "IsShared", mIsShared);
      mName = DefaultDataReadUtility.Read(reader, "Name", mName);
      mValue = DefaultDataReadUtility.Read(reader, "Value", mValue);
    }

    public static implicit operator SharedString(string value)
    {
      SharedString sharedString = new SharedString();
      sharedString.mValue = value;
      return sharedString;
    }
  }
}
