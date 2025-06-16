using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/Fight/Melee")]
  [TaskDescription("Поменять оружие")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (MeleeFightChangeWeapon))]
  public class MeleeFightChangeWeapon : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public WeaponEnum Weapon = WeaponEnum.Hands;
    private NpcState npcState;

    public override TaskStatus OnUpdate()
    {
      if (npcState == null)
      {
        npcState = gameObject.GetComponentNonAlloc<NpcState>();
        if (npcState == null)
        {
          Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component");
          return TaskStatus.Failure;
        }
      }
      if (npcState.Weapon != Weapon)
        npcState.Weapon = Weapon;
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      DefaultDataWriteUtility.WriteEnum(writer, "Weapon", Weapon);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      Weapon = DefaultDataReadUtility.ReadEnum<WeaponEnum>(reader, "Weapon");
    }
  }
}
