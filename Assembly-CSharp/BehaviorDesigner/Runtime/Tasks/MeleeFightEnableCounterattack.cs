using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
  [Obsolete]
  [TaskCategory("Pathologic/Fight/Melee")]
  [TaskDescription("включить/выключить возможность контратаки")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (MeleeFightEnableCounterattack))]
  public class MeleeFightEnableCounterattack : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedBool Counterattack;

    public override TaskStatus OnUpdate() => TaskStatus.Success;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Counterattack", Counterattack);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      Counterattack = BehaviorTreeDataReadUtility.ReadShared(reader, "Counterattack", Counterattack);
    }
  }
}
