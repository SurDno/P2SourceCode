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
  [FactoryProxy(typeof (SharedGameObject))]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [Serializable]
  public class SharedGameObject : 
    SharedVariable<GameObject>,
    IStub,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsShared", this.mIsShared);
      DefaultDataWriteUtility.Write(writer, "Name", this.mName);
      BehaviorTreeDataWriteUtility.WriteUnity<GameObject>(writer, "Value", this.mValue);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.mIsShared = DefaultDataReadUtility.Read(reader, "IsShared", this.mIsShared);
      this.mName = DefaultDataReadUtility.Read(reader, "Name", this.mName);
      this.mValue = BehaviorTreeDataReadUtility.ReadUnity<GameObject>(reader, "Value", this.mValue);
    }

    public static implicit operator SharedGameObject(GameObject value)
    {
      SharedGameObject sharedGameObject = new SharedGameObject();
      sharedGameObject.mValue = value;
      return sharedGameObject;
    }
  }
}
