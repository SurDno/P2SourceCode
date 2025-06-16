using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/Fight/Temp/Movement")]
  [TaskDescription("Айдлиться в боевой стойке")]
  [TaskIcon("Pathologic_InstantIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (FightIdleInstant))]
  public class FightIdleInstant : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("Надо ли прицеливаться из оружия.")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool Aim = (SharedBool) false;
    private NpcState npcState;

    public override void OnStart()
    {
      if ((UnityEngine.Object) this.npcState == (UnityEngine.Object) null)
      {
        this.npcState = this.gameObject.GetComponent<NpcState>();
        if ((UnityEngine.Object) this.npcState == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component"), (UnityEngine.Object) this.gameObject);
          return;
        }
      }
      this.npcState.FightIdle(this.Aim.Value);
    }

    public override TaskStatus OnUpdate()
    {
      return (UnityEngine.Object) this.npcState == (UnityEngine.Object) null ? TaskStatus.Failure : TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "Aim", this.Aim);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.Aim = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "Aim", this.Aim);
    }
  }
}
