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
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat followTime = (SharedFloat) 0.0f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private QuickBlockDescription description;
    private NPCEnemy owner;

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.description == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) (typeof (MeleeFightFollow).Name + " has no " + typeof (FollowDescription).Name + " attached"), (UnityEngine.Object) this.gameObject);
        return TaskStatus.Failure;
      }
      if ((UnityEngine.Object) this.owner == (UnityEngine.Object) null)
      {
        this.owner = this.gameObject.GetComponentNonAlloc<NPCEnemy>();
        if ((UnityEngine.Object) this.owner == (UnityEngine.Object) null)
          return TaskStatus.Failure;
      }
      NPCEnemy owner = this.owner;
      owner.QuickBlockProbability = this.description.QuickBlockProbability;
      owner.DodgeProbability = this.description.DodgeProbability;
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "FollowTime", this.followTime);
      BehaviorTreeDataWriteUtility.WriteUnity<QuickBlockDescription>(writer, "Description", this.description);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.followTime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "FollowTime", this.followTime);
      this.description = BehaviorTreeDataReadUtility.ReadUnity<QuickBlockDescription>(reader, "Description", this.description);
    }
  }
}
