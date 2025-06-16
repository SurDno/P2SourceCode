using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Engines.Services;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskDescription("")]
  [TaskCategory("Pathologic/Fight")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (IsWeaponInEnemyHands))]
  public class IsWeaponInEnemyHands : Conditional, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    private WeaponEnum weapon = WeaponEnum.Hands;
    private EnemyBase owner;
    private NpcState npcState;

    public override void OnAwake()
    {
      owner = gameObject.GetComponent<EnemyBase>();
      if (!(owner != null) || !(owner.Enemy != null))
        return;
      npcState = owner.Enemy.gameObject.GetComponent<NpcState>();
    }

    public override TaskStatus OnUpdate()
    {
      if (owner == null || owner.Enemy == null)
        return TaskStatus.Failure;
      if (owner.Enemy is PlayerEnemy)
      {
        PlayerWeaponServiceNew component = owner.Enemy.gameObject.GetComponent<PlayerWeaponServiceNew>();
        if (component == null)
          return TaskStatus.Failure;
        switch (component.KindBase)
        {
          case WeaponKind.Hands:
            if (weapon == WeaponEnum.Hands)
              return TaskStatus.Success;
            break;
          case WeaponKind.Knife:
            if (weapon == WeaponEnum.Knife)
              return TaskStatus.Success;
            break;
          case WeaponKind.Revolver:
            if (weapon == WeaponEnum.Revolver)
              return TaskStatus.Success;
            break;
          case WeaponKind.Rifle:
            if (weapon == WeaponEnum.Rifle)
              return TaskStatus.Success;
            break;
          case WeaponKind.Scalpel:
            if (weapon == WeaponEnum.Scalpel)
              return TaskStatus.Success;
            break;
          case WeaponKind.Lockpick:
            if (weapon == WeaponEnum.Lockpick)
              return TaskStatus.Success;
            break;
          case WeaponKind.Shotgun:
            if (weapon == WeaponEnum.Shotgun)
              return TaskStatus.Success;
            break;
        }
        return TaskStatus.Failure;
      }
      npcState = owner.Enemy.gameObject.GetComponent<NpcState>();
      return npcState == null || npcState.Weapon != weapon ? TaskStatus.Failure : TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      DefaultDataWriteUtility.WriteEnum(writer, "Weapon", weapon);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      weapon = DefaultDataReadUtility.ReadEnum<WeaponEnum>(reader, "Weapon");
    }
  }
}
