// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.MeleeFightSetEnemy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/Fight/Melee")]
  [TaskDescription("Назначить врага")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (MeleeFightSetEnemy))]
  public class MeleeFightSetEnemy : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedTransform EnemyTransform;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool UseCombatService;
    private Pivot pivot;
    private EnemyBase owner;
    private Animator animator;

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.pivot == (UnityEngine.Object) null)
      {
        this.pivot = this.gameObject.GetComponentNonAlloc<Pivot>();
        if ((UnityEngine.Object) this.pivot == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (Pivot).Name + " engine component"), (UnityEngine.Object) this.gameObject);
          return TaskStatus.Failure;
        }
        this.owner = (EnemyBase) this.pivot.GetNpcEnemy();
        if ((UnityEngine.Object) this.owner == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (EnemyBase).Name + " engine component"), (UnityEngine.Object) this.gameObject);
          return TaskStatus.Failure;
        }
        this.animator = this.pivot.GetAnimator();
        if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (Animator).Name + " engine component"), (UnityEngine.Object) this.gameObject);
          return TaskStatus.Failure;
        }
      }
      if (this.EnemyTransform == null || (UnityEngine.Object) this.EnemyTransform.Value == (UnityEngine.Object) null || (UnityEngine.Object) this.EnemyTransform.Value.GetComponentNonAlloc<EnemyBase>() == (UnityEngine.Object) null)
      {
        if (!this.UseCombatService.Value)
          this.owner.Enemy = (EnemyBase) null;
        this.owner.RotationTarget = (Transform) null;
        this.owner.RetreatAngle = new float?();
        this.owner.DesiredWalkSpeed = 0.0f;
        this.animator?.SetTrigger("Fight.Triggers/CancelWalk");
        this.animator?.SetTrigger("Fight.Triggers/CancelAttack");
        return TaskStatus.Success;
      }
      if (!this.UseCombatService.Value)
        this.owner.Enemy = this.EnemyTransform.Value.GetComponentNonAlloc<EnemyBase>();
      this.owner.RotationTarget = this.EnemyTransform.Value;
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedTransform>(writer, "EnemyTransform", this.EnemyTransform);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "UseCombatService", this.UseCombatService);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.EnemyTransform = BehaviorTreeDataReadUtility.ReadShared<SharedTransform>(reader, "EnemyTransform", this.EnemyTransform);
      this.UseCombatService = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "UseCombatService", this.UseCombatService);
    }
  }
}
