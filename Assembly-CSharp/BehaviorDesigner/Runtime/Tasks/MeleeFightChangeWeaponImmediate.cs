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
  [TaskDescription("Поменять оружие мгновенно")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (MeleeFightChangeWeaponImmediate))]
  public class MeleeFightChangeWeaponImmediate : 
    Action,
    IStub,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public WeaponEnum Weapon = WeaponEnum.Hands;
    private NpcState npcState;
    private NPCWeaponService weaponService;

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.npcState == (UnityEngine.Object) null)
      {
        this.npcState = this.gameObject.GetComponentNonAlloc<NpcState>();
        if ((UnityEngine.Object) this.npcState == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component"), (UnityEngine.Object) this.gameObject);
          return TaskStatus.Failure;
        }
        this.weaponService = this.gameObject.GetComponent<NPCWeaponService>();
        if ((UnityEngine.Object) this.weaponService == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (NPCWeaponService).Name + " engine component"), (UnityEngine.Object) this.gameObject);
          return TaskStatus.Failure;
        }
      }
      if (this.npcState.Weapon != this.Weapon)
        this.npcState.Weapon = this.Weapon;
      this.weaponService.Weapon = this.Weapon;
      this.weaponService.SwitchWeaponOnImmediate();
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      DefaultDataWriteUtility.WriteEnum<WeaponEnum>(writer, "Weapon", this.Weapon);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.Weapon = DefaultDataReadUtility.ReadEnum<WeaponEnum>(reader, "Weapon");
    }
  }
}
