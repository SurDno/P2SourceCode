using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
  [FactoryProxy(typeof (SharedQuaternion))]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [Serializable]
  public class SharedQuaternion : 
    SharedVariable<Quaternion>,
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

    public static implicit operator SharedQuaternion(Quaternion value)
    {
      SharedQuaternion sharedQuaternion = new SharedQuaternion();
      sharedQuaternion.mValue = value;
      return sharedQuaternion;
    }
  }
}
