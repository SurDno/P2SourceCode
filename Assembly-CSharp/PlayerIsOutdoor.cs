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
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [SerializeField]
  public SharedBool Outdoor = (SharedBool) true;

  public override TaskStatus OnUpdate()
  {
    IEntity player = ServiceLocator.GetService<ISimulation>().Player;
    if (player == null)
      return TaskStatus.Failure;
    LocationItemComponent component = player.GetComponent<LocationItemComponent>();
    return player == null || component.Location == null || (!this.Outdoor.Value || component.IsIndoor) && (this.Outdoor.Value || !component.IsIndoor) ? TaskStatus.Failure : TaskStatus.Success;
  }

  public void DataWrite(IDataWriter writer)
  {
    DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
    DefaultDataWriteUtility.Write(writer, "Id", this.id);
    DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
    DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
    DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
    BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "Outdoor", this.Outdoor);
  }

  public void DataRead(IDataReader reader, System.Type type)
  {
    this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
    this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
    this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
    this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
    this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
    this.Outdoor = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "Outdoor", this.Outdoor);
  }
}
