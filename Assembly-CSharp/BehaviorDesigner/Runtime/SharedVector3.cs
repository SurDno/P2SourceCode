using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime
{
  [FactoryProxy(typeof (SharedVector3))]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [Serializable]
  public class SharedVector3 : 
    SharedVariable<Vector3>,
    IStub,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsShared", mIsShared);
      DefaultDataWriteUtility.Write(writer, "Name", mName);
      BehaviorTreeDataWriteUtility.WriteUnity(writer, "Value", mValue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      mIsShared = DefaultDataReadUtility.Read(reader, "IsShared", mIsShared);
      mName = DefaultDataReadUtility.Read(reader, "Name", mName);
      mValue = BehaviorTreeDataReadUtility.ReadUnity(reader, "Value", mValue);
    }

    public static implicit operator SharedVector3(Vector3 value)
    {
      SharedVector3 sharedVector3 = new SharedVector3();
      sharedVector3.mValue = value;
      return sharedVector3;
    }
  }
}
