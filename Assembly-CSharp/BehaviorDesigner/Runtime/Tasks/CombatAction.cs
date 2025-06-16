// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.CombatAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Services;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/GroupBehaviour")]
  [TaskIcon("{SkinColor}SequenceIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (CombatAction))]
  public class CombatAction : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform Enemy;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool Watch;
    private CombatService combatService;
    private EnemyBase character;
    private bool inited = false;

    public override void OnStart()
    {
      if ((UnityEngine.Object) this.Enemy.Value == (UnityEngine.Object) null)
        return;
      this.combatService = ServiceLocator.GetService<CombatService>();
      this.character = this.Owner.GetComponent<EnemyBase>();
      if (this.combatService == null)
        return;
      this.combatService.EnterCombat(this.character, this.Enemy.Value.GetComponent<EnemyBase>(), this.Watch.Value);
      this.inited = true;
    }

    public override TaskStatus OnUpdate()
    {
      if (!this.inited)
        return TaskStatus.Success;
      if (this.combatService == null)
        return TaskStatus.Failure;
      return this.combatService.CharacterIsInCombat(this.character) ? TaskStatus.Running : TaskStatus.Success;
    }

    public override void OnEnd()
    {
      this.inited = false;
      if (this.combatService == null)
        return;
      this.combatService.ExitCombat(this.character);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "Enemy", this.Enemy);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "Watch", this.Watch);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.Enemy = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "Enemy", this.Enemy);
      this.Watch = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "Watch", this.Watch);
    }
  }
}
