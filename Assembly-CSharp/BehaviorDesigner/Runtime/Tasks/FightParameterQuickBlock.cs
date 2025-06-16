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
  [TaskCategory("Pathologic/Fight/Temp/Parameters")]
  [TaskDescription("Выставить время Stagger")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (FightParameterQuickBlock))]
  public class FightParameterQuickBlock : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("Вероятность (0-1) заблокировать удар")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat QuickBlockProbability = (SharedFloat) 0.5f;
    [Tooltip("Вероятность (0-1) увернуться движением (выбрасывается уже после того как решено блокировать)")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat DodgeProbability = (SharedFloat) 0.7f;
    private NPCEnemy owner;

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.owner == (UnityEngine.Object) null)
      {
        this.owner = this.GetComponent<NPCEnemy>();
        if ((UnityEngine.Object) this.owner == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (NPCEnemy).Name + " engine component"), (UnityEngine.Object) this.gameObject);
          return TaskStatus.Failure;
        }
      }
      this.owner.QuickBlockProbability = this.QuickBlockProbability.Value;
      this.owner.DodgeProbability = this.DodgeProbability.Value;
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "QuickBlockProbability", this.QuickBlockProbability);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "DodgeProbability", this.DodgeProbability);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.QuickBlockProbability = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "QuickBlockProbability", this.QuickBlockProbability);
      this.DodgeProbability = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "DodgeProbability", this.DodgeProbability);
    }
  }
}
