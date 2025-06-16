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
using Engine.Source.Services;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace Engine.BehaviourNodes.Conditionals
{
  [TaskDescription("Is there attack enemy nearby?")]
  [TaskCategory("Pathologic")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (IsAttackEnemyNearby))]
  public class IsAttackEnemyNearby : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedTransform Target;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedTransform Enemy;
    private CombatService combatService;

    public override void OnAwake()
    {
      combatService = ServiceLocator.GetService<CombatService>();
    }

    public override TaskStatus OnUpdate()
    {
      IEntity entity = !(Target.Value == null) ? EntityUtility.GetEntity(Target.Value.gameObject) : EntityUtility.GetEntity(gameObject);
      if (entity == null)
        return TaskStatus.Failure;
      CombatServiceCharacterInfo serviceCharacterInfo = combatService.GetCharacterInfo(entity)?.AttackEnemyNearby();
      bool flag = false;
      if (serviceCharacterInfo != null && serviceCharacterInfo.Character != null && serviceCharacterInfo.Character.transform != null)
      {
        Enemy.Value = serviceCharacterInfo?.Character?.transform;
        flag = true;
      }
      return flag ? TaskStatus.Success : TaskStatus.Failure;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Enemy", Enemy);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
      Enemy = BehaviorTreeDataReadUtility.ReadShared(reader, "Enemy", Enemy);
    }
  }
}
