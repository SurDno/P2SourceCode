// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.FightBombAttack
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
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
  [TaskDescription("атака бомбами")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (FightBombAttack))]
  public class FightBombAttack : FightBase, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private SharedFloat followTime = (SharedFloat) 0.0f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private SharedFloat throwCooldown = (SharedFloat) 5f;
    private float attackCooldownTime;

    public override void OnStart()
    {
      base.OnStart();
      this.waitDuration = this.followTime.Value;
    }

    public override TaskStatus DoUpdate(float deltaTime)
    {
      if ((UnityEngine.Object) this.owner.gameObject == (UnityEngine.Object) null || !this.owner.gameObject.activeSelf || (UnityEngine.Object) this.owner?.Enemy == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      if (this.owner.IsReacting)
        return TaskStatus.Running;
      float magnitude = (this.owner.Enemy.transform.position - this.owner.transform.position).magnitude;
      if (!this.owner.IsPushing && !this.owner.IsPushing)
        this.attackCooldownTime -= deltaTime;
      if ((double) this.attackCooldownTime <= 0.0 && !this.owner.IsAttacking && !this.owner.IsPushing && !this.owner.IsReacting && !this.owner.IsQuickBlock)
      {
        float outAngle = 0.0f;
        if (this.CanThrowBomb(out outAngle))
        {
          this.owner.EnqueueProjectileThrow(outAngle, 15f);
          if ((double) outAngle * 57.295780181884766 > 25.0)
            this.owner.TriggerThrowBomb(2);
          else if ((double) outAngle * 57.295780181884766 > 15.0)
            this.owner.TriggerThrowBomb(1);
          else
            this.owner.TriggerThrowBomb(0);
          this.attackCooldownTime = this.throwCooldown.Value;
        }
      }
      return TaskStatus.Running;
    }

    private bool CanThrowBomb(out float outAngle)
    {
      float v = 15f;
      float h = 1.5f;
      Vector3 vector3 = this.owner.Enemy.transform.position - this.owner.transform.position;
      float angle1;
      float angle2;
      if (BomberHelper.CalcThrowAngles(v, vector3.magnitude, h, out angle1, out angle2))
      {
        Vector3 startPosition = this.owner.transform.position + Vector3.up * h;
        Vector3 normalized = vector3.normalized;
        float num = float.MinValue;
        if (BomberHelper.SphereCastParabola(angle1, v, h, startPosition, normalized))
          num = angle1;
        else if (BomberHelper.SphereCastParabola(angle2, v, h, startPosition, normalized))
          num = angle2;
        if ((double) num != -3.4028234663852886E+38)
        {
          outAngle = num;
          return true;
        }
      }
      outAngle = 0.0f;
      return false;
    }

    public new void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "FollowTime", this.followTime);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "ThrowCooldown", this.throwCooldown);
    }

    public new void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.followTime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "FollowTime", this.followTime);
      this.throwCooldown = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "ThrowCooldown", this.throwCooldown);
    }
  }
}
