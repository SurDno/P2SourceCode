// Decompiled with JetBrains decompiler
// Type: Engine.BehaviourNodes.Conditionals.IsFearEnemyNearby
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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

#nullable disable
namespace Engine.BehaviourNodes.Conditionals
{
  [TaskDescription("Is there fear enemy nearby?")]
  [TaskCategory("Pathologic")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (IsFearEnemyNearby))]
  public class IsFearEnemyNearby : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform Target;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform Enemy;
    private CombatService combatService;

    public override void OnAwake()
    {
      this.combatService = ServiceLocator.GetService<CombatService>();
    }

    public override TaskStatus OnUpdate()
    {
      IEntity entity = !((UnityEngine.Object) this.Target.Value == (UnityEngine.Object) null) ? EntityUtility.GetEntity(this.Target.Value.gameObject) : EntityUtility.GetEntity(this.gameObject);
      if (entity == null)
        return TaskStatus.Failure;
      CombatServiceCharacterInfo serviceCharacterInfo = this.combatService.GetCharacterInfo(entity)?.FearEnemyNearby();
      bool flag = false;
      if (serviceCharacterInfo != null && (UnityEngine.Object) serviceCharacterInfo.Character != (UnityEngine.Object) null && (UnityEngine.Object) serviceCharacterInfo.Character.transform != (UnityEngine.Object) null)
      {
        this.Enemy.Value = serviceCharacterInfo?.Character?.transform;
        flag = true;
      }
      return flag ? TaskStatus.Success : TaskStatus.Failure;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Target", this.Target);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Enemy", this.Enemy);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.Target = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Target", this.Target);
      this.Enemy = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Enemy", this.Enemy);
    }
  }
}
