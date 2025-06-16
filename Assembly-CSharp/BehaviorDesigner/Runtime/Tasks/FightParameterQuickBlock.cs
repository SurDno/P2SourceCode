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
  [FactoryProxy(typeof (FightParameterQuickBlock))]
  public class FightParameterQuickBlock : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("Вероятность (0-1) заблокировать удар")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat QuickBlockProbability = 0.5f;
    [Tooltip("Вероятность (0-1) увернуться движением (выбрасывается уже после того как решено блокировать)")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedFloat DodgeProbability = 0.7f;
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
      owner.QuickBlockProbability = QuickBlockProbability.Value;
      owner.DodgeProbability = DodgeProbability.Value;
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "QuickBlockProbability", QuickBlockProbability);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "DodgeProbability", DodgeProbability);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      QuickBlockProbability = BehaviorTreeDataReadUtility.ReadShared(reader, "QuickBlockProbability", QuickBlockProbability);
      DodgeProbability = BehaviorTreeDataReadUtility.ReadShared(reader, "DodgeProbability", DodgeProbability);
    }
  }
}
