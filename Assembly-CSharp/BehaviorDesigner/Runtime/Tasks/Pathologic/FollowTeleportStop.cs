using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Stop teleporting behind player")]
  [TaskCategory("Pathologic/Movement")]
  [TaskIcon("Pathologic_LongIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (FollowTeleportStop))]
  public class FollowTeleportStop : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    public override TaskStatus OnUpdate()
    {
      IEntity owner = Owner.GetComponent<EngineGameObject>().Owner;
      if (owner == null)
      {
        Debug.LogWarning(gameObject.name + " has no entity");
        return TaskStatus.Failure;
      }
      MessangerComponent component = (MessangerComponent) owner.GetComponent<IMessangerComponent>();
      if (component == null)
      {
        Debug.LogWarning(gameObject.name + " has no " + typeof (IMessangerComponent).Name);
        return TaskStatus.Failure;
      }
      component.StopTeleporting();
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
