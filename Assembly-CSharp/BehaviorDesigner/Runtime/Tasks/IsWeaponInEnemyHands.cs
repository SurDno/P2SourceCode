// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.IsWeaponInEnemyHands
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Engines.Services;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskDescription("")]
  [TaskCategory("Pathologic/Fight")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (IsWeaponInEnemyHands))]
  public class IsWeaponInEnemyHands : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private WeaponEnum weapon = WeaponEnum.Hands;
    private EnemyBase owner;
    private NpcState npcState;

    public override void OnAwake()
    {
      this.owner = this.gameObject.GetComponent<EnemyBase>();
      if (!((UnityEngine.Object) this.owner != (UnityEngine.Object) null) || !((UnityEngine.Object) this.owner.Enemy != (UnityEngine.Object) null))
        return;
      this.npcState = this.owner.Enemy.gameObject.GetComponent<NpcState>();
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.owner == (UnityEngine.Object) null || (UnityEngine.Object) this.owner.Enemy == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      if (this.owner.Enemy is PlayerEnemy)
      {
        PlayerWeaponServiceNew component = this.owner.Enemy.gameObject.GetComponent<PlayerWeaponServiceNew>();
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
          return TaskStatus.Failure;
        switch (component.KindBase)
        {
          case WeaponKind.Hands:
            if (this.weapon == WeaponEnum.Hands)
              return TaskStatus.Success;
            break;
          case WeaponKind.Knife:
            if (this.weapon == WeaponEnum.Knife)
              return TaskStatus.Success;
            break;
          case WeaponKind.Revolver:
            if (this.weapon == WeaponEnum.Revolver)
              return TaskStatus.Success;
            break;
          case WeaponKind.Rifle:
            if (this.weapon == WeaponEnum.Rifle)
              return TaskStatus.Success;
            break;
          case WeaponKind.Scalpel:
            if (this.weapon == WeaponEnum.Scalpel)
              return TaskStatus.Success;
            break;
          case WeaponKind.Lockpick:
            if (this.weapon == WeaponEnum.Lockpick)
              return TaskStatus.Success;
            break;
          case WeaponKind.Shotgun:
            if (this.weapon == WeaponEnum.Shotgun)
              return TaskStatus.Success;
            break;
        }
        return TaskStatus.Failure;
      }
      this.npcState = this.owner.Enemy.gameObject.GetComponent<NpcState>();
      return (UnityEngine.Object) this.npcState == (UnityEngine.Object) null || this.npcState.Weapon != this.weapon ? TaskStatus.Failure : TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      DefaultDataWriteUtility.WriteEnum<WeaponEnum>(writer, "Weapon", this.weapon);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.weapon = DefaultDataReadUtility.ReadEnum<WeaponEnum>(reader, "Weapon");
    }
  }
}
