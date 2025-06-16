using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using UnityEngine;

[TaskDescription("Player lamp is on")]
[TaskCategory("Pathologic")]
[Factory]
[GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
[FactoryProxy(typeof (PlayerLampIsOn))]
public class PlayerLampIsOn : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
{
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [SerializeField]
  public bool LampOn = true;

  public override TaskStatus OnUpdate()
  {
    IEntity player = ServiceLocator.GetService<ISimulation>().Player;
    if (player == null)
      return TaskStatus.Failure;
    ParametersComponent component = player.GetComponent<ParametersComponent>();
    if (component == null)
      return TaskStatus.Failure;
    IParameter<bool> byName = component.GetByName<bool>(ParameterNameEnum.Flashlight);
    return byName == null ? TaskStatus.Failure : (this.LampOn == byName.Value ? TaskStatus.Success : TaskStatus.Failure);
  }

  public void DataWrite(IDataWriter writer)
  {
    DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
    DefaultDataWriteUtility.Write(writer, "Id", this.id);
    DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
    DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
    DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
    DefaultDataWriteUtility.Write(writer, "LampOn", this.LampOn);
  }

  public void DataRead(IDataReader reader, System.Type type)
  {
    this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
    this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
    this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
    this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
    this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
    this.LampOn = DefaultDataReadUtility.Read(reader, "LampOn", this.LampOn);
  }
}
