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
  [FactoryProxy(typeof (GenericVariable))]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [Serializable]
  public class GenericVariable : IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public string type = "SharedString";
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedVariable value;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Type", type);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Value", value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.type = DefaultDataReadUtility.Read(reader, "Type", this.type);
      value = BehaviorTreeDataReadUtility.ReadShared(reader, "Value", value);
    }

    public GenericVariable() => value = new SharedString();
  }
}
