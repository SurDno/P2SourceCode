using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Services;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/GroupBehaviour")]
  [TaskIcon("{SkinColor}SequenceIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (CombatLostSight))]
  public class CombatLostSight : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedTransform Enemy;

    public override void OnStart()
    {
      CombatService service = ServiceLocator.GetService<CombatService>();
      if (service == null || (UnityEngine.Object) Enemy.Value == (UnityEngine.Object) null || (UnityEngine.Object) Owner == (UnityEngine.Object) null)
        return;
      EnemyBase component1 = Enemy.Value.GetComponent<EnemyBase>();
      if ((UnityEngine.Object) component1 == (UnityEngine.Object) null)
        return;
      EnemyBase component2 = Owner.GetComponent<EnemyBase>();
      if ((UnityEngine.Object) component2 == (UnityEngine.Object) null)
        return;
      service.LostSight(component2, component1);
    }

    public override TaskStatus OnUpdate() => TaskStatus.Success;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Enemy", Enemy);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      Enemy = BehaviorTreeDataReadUtility.ReadShared(reader, "Enemy", Enemy);
    }
  }
}
