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
  [FactoryProxy(typeof (NamedVariable))]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [Serializable]
  public class NamedVariable : GenericVariable, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public string name = "";

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Type", this.type);
      BehaviorTreeDataWriteUtility.WriteShared<SharedVariable>(writer, "Value", this.value);
      DefaultDataWriteUtility.Write(writer, "Name", this.name);
    }

    public new void DataRead(IDataReader reader, System.Type type)
    {
      this.type = DefaultDataReadUtility.Read(reader, "Type", this.type);
      this.value = BehaviorTreeDataReadUtility.ReadShared<SharedVariable>(reader, "Value", this.value);
      this.name = DefaultDataReadUtility.Read(reader, "Name", this.name);
    }
  }
}
