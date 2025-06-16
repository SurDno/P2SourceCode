using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

[TaskDescription("Player is outdoor")]
[TaskCategory("Pathologic")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof (PlayerIsOutdoor))]
public class PlayerIsOutdoor : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
{
  [DataReadProxy]
  [DataWriteProxy]
  [CopyableProxy()]
  [SerializeField]
  public SharedBool Outdoor = true;

  public override TaskStatus OnUpdate()
  {
    IEntity player = ServiceLocator.GetService<ISimulation>().Player;
    if (player == null)
      return TaskStatus.Failure;
    LocationItemComponent component = player.GetComponent<LocationItemComponent>();
    return player == null || component.Location == null || (!Outdoor.Value || component.IsIndoor) && (Outdoor.Value || !component.IsIndoor) ? TaskStatus.Failure : TaskStatus.Success;
  }

  public void DataWrite(IDataWriter writer)
  {
    DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
    DefaultDataWriteUtility.Write(writer, "Id", id);
    DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
    DefaultDataWriteUtility.Write(writer, "Instant", instant);
    DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
    BehaviorTreeDataWriteUtility.WriteShared(writer, "Outdoor", Outdoor);
  }

  public void DataRead(IDataReader reader, Type type)
  {
    nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
    id = DefaultDataReadUtility.Read(reader, "Id", id);
    friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
    instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
    disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
    Outdoor = BehaviorTreeDataReadUtility.ReadShared(reader, "Outdoor", Outdoor);
  }
}
