using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Сообщение об ошибочном завершении задания редактору (дерево при этом не останавливается)")]
  [TaskCategory("Pathologic/Behavior")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (BehaviorCustomEvent))]
  public class BehaviorCustomEvent : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedString Message;

    public override TaskStatus OnUpdate()
    {
      IEntity entity = EntityUtility.GetEntity(this.gameObject);
      if (entity == null)
      {
        Debug.LogWarningFormat(this.gameObject.name + ": null target");
        return TaskStatus.Failure;
      }
      BehaviorComponent component = entity.GetComponent<BehaviorComponent>();
      if (component == null)
        Debug.LogWarningFormat(this.gameObject.name + ": has no " + typeof (BehaviorComponent).Name + " component");
      component.FireCustomEvent(this.Message.Value);
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedString>(writer, "Message", this.Message);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.Message = BehaviorTreeDataReadUtility.ReadShared<SharedString>(reader, "Message", this.Message);
    }
  }
}
