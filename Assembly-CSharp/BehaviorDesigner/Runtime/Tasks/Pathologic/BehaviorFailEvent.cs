using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Сообщение об ошибочном завершении задания редактору (дерево при этом не останавливается)")]
  [TaskCategory("Pathologic/Behavior")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (BehaviorFailEvent))]
  public class BehaviorFailEvent : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    public override TaskStatus OnUpdate()
    {
      IEntity entity = EntityUtility.GetEntity(gameObject);
      if (entity == null)
      {
        Debug.LogWarningFormat(gameObject.name + ": null target");
        return TaskStatus.Failure;
      }
      BehaviorComponent component = entity.GetComponent<BehaviorComponent>();
      if (component == null)
        Debug.LogWarningFormat(gameObject.name + ": has no " + typeof (BehaviorComponent).Name + " component");
      component.FireFailEvent();
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
    }
  }
}
