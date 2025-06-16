using System;
using System.Collections.Generic;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime
{
  [FactoryProxy(typeof (SharedTransformList))]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [Serializable]
  public class SharedTransformList : 
    SharedVariable<List<Transform>>,
    IStub,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsShared", mIsShared);
      DefaultDataWriteUtility.Write(writer, "Name", mName);
      BehaviorTreeDataWriteUtility.WriteUnityList(writer, "Value", mValue);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      mIsShared = DefaultDataReadUtility.Read(reader, "IsShared", mIsShared);
      mName = DefaultDataReadUtility.Read(reader, "Name", mName);
      mValue = BehaviorTreeDataReadUtility.ReadUnityList(reader, "Value", mValue);
    }

    public static implicit operator SharedTransformList(List<Transform> value)
    {
      SharedTransformList sharedTransformList = new SharedTransformList();
      sharedTransformList.mValue = value;
      return sharedTransformList;
    }
  }
}
