using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/Fight/Temp/Parameters")]
  [TaskDescription("Выставить время Stagger")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (FightParameterStaggerTime))]
  public class FightParameterStaggerTime : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedFloat StaggerTime = 5f;
    private NPCEnemy owner;

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) owner == (UnityEngine.Object) null)
      {
        owner = this.GetComponent<NPCEnemy>();
        if ((UnityEngine.Object) owner == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (gameObject.name + ": doesn't contain " + typeof (NPCEnemy).Name + " engine component"), (UnityEngine.Object) gameObject);
          return TaskStatus.Failure;
        }
      }
      owner.StaggerTime = StaggerTime.Value;
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "StaggerTime", StaggerTime);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      StaggerTime = BehaviorTreeDataReadUtility.ReadShared(reader, "StaggerTime", StaggerTime);
    }
  }
}
