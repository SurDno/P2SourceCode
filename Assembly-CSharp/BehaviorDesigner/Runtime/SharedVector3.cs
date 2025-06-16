using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

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
      DefaultDataWriteUtility.Write(writer, "IsShared", this.mIsShared);
      DefaultDataWriteUtility.Write(writer, "Name", this.mName);
      BehaviorTreeDataWriteUtility.WriteUnity(writer, "Value", this.mValue);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.mIsShared = DefaultDataReadUtility.Read(reader, "IsShared", this.mIsShared);
      this.mName = DefaultDataReadUtility.Read(reader, "Name", this.mName);
      this.mValue = BehaviorTreeDataReadUtility.ReadUnity(reader, "Value", this.mValue);
    }

    public static implicit operator SharedVector3(Vector3 value)
    {
      SharedVector3 sharedVector3 = new SharedVector3();
      sharedVector3.mValue = value;
      return sharedVector3;
    }
  }
}
