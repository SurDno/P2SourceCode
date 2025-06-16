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

namespace Engine.BehaviourNodes.Conditionals
{
  [TaskDescription("Is there fear enemy nearby?")]
  [TaskCategory("Pathologic")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (IsFearEnemyNearby))]
  public class IsFearEnemyNearby : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
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
      IEntity entity = !((UnityEngine.Object) Target.Value == (UnityEngine.Object) null) ? EntityUtility.GetEntity(Target.Value.gameObject) : EntityUtility.GetEntity(gameObject);
      if (entity == null)
        return TaskStatus.Failure;
      CombatServiceCharacterInfo serviceCharacterInfo = combatService.GetCharacterInfo(entity)?.FearEnemyNearby();
      bool flag = false;
      if (serviceCharacterInfo != null && (UnityEngine.Object) serviceCharacterInfo.Character != (UnityEngine.Object) null && (UnityEngine.Object) serviceCharacterInfo.Character.transform != (UnityEngine.Object) null)
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
