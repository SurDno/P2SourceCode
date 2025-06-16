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
  [FactoryProxy(typeof (NamedVariable))]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [Serializable]
  public class NamedVariable : GenericVariable, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public string name = "";

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Type", type);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Value", value);
      DefaultDataWriteUtility.Write(writer, "Name", name);
    }

    public new void DataRead(IDataReader reader, Type type)
    {
      this.type = DefaultDataReadUtility.Read(reader, "Type", this.type);
      value = BehaviorTreeDataReadUtility.ReadShared(reader, "Value", value);
      name = DefaultDataReadUtility.Read(reader, "Name", name);
    }
  }
}
