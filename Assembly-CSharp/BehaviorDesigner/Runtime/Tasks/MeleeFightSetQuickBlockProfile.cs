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
  [TaskCategory("Pathologic/Fight/Melee")]
  [TaskDescription("Преследовать противника и атаковать по возможности")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (MeleeFightSetQuickBlockProfile))]
  public class MeleeFightSetQuickBlockProfile : 
    Action,
    IStub,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat followTime = 0.0f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    private QuickBlockDescription description;
    private NPCEnemy owner;

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) description == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) (typeof (MeleeFightFollow).Name + " has no " + typeof (FollowDescription).Name + " attached"), (UnityEngine.Object) gameObject);
        return TaskStatus.Failure;
      }
      if ((UnityEngine.Object) this.owner == (UnityEngine.Object) null)
      {
        this.owner = gameObject.GetComponentNonAlloc<NPCEnemy>();
        if ((UnityEngine.Object) this.owner == (UnityEngine.Object) null)
          return TaskStatus.Failure;
      }
      NPCEnemy owner = this.owner;
      owner.QuickBlockProbability = description.QuickBlockProbability;
      owner.DodgeProbability = description.DodgeProbability;
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "FollowTime", followTime);
      BehaviorTreeDataWriteUtility.WriteUnity<QuickBlockDescription>(writer, "Description", description);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      followTime = BehaviorTreeDataReadUtility.ReadShared(reader, "FollowTime", followTime);
      description = BehaviorTreeDataReadUtility.ReadUnity<QuickBlockDescription>(reader, "Description", description);
    }
  }
}
