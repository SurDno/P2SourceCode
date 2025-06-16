// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.TutorialSuokAttackRuinBlock
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/Fight/Tuorial Suok")]
  [TaskDescription("Атаковать только RuinBlock")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (TutorialSuokAttackRuinBlock))]
  public class TutorialSuokAttackRuinBlock : 
    FightBase,
    IStub,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    private float nextAttackTime;

    public override void OnStart()
    {
      base.OnStart();
      this.nextAttackTime = Time.time;
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.owner.gameObject == (UnityEngine.Object) null || !this.owner.gameObject.activeSelf || (UnityEngine.Object) this.owner?.Enemy == (UnityEngine.Object) null || this.owner.IsReacting)
        return TaskStatus.Failure;
      if ((double) Time.time < (double) this.nextAttackTime || (double) (this.owner.Enemy.transform.position - this.owner.transform.position).magnitude >= 2.0 || !(this.owner.Enemy is PlayerEnemy))
        return TaskStatus.Running;
      PlayerEnemy enemy = (PlayerEnemy) this.owner.Enemy;
      if (this.owner.IsPushing || this.owner.IsReacting || this.owner.IsAttacking)
        return TaskStatus.Running;
      this.nextAttackTime = Time.time + UnityEngine.Random.Range(4f, 6f);
      this.owner.TriggerAction(WeaponActionEnum.Uppercut);
      this.owner.RotationTarget = (Transform) null;
      this.owner.RotateByPath = false;
      return TaskStatus.Running;
    }

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
    }

    public new void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
    }
  }
}
