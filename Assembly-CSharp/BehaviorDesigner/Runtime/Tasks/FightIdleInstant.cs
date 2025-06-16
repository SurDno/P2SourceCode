using System;
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
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedBool Aim = false;
    private NpcState npcState;

    public override void OnStart()
    {
      if (npcState == null)
      {
        npcState = gameObject.GetComponent<NpcState>();
        if (npcState == null)
        {
          Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component", gameObject);
          return;
        }
      }
      npcState.FightIdle(Aim.Value);
    }

    public override TaskStatus OnUpdate()
    {
      return npcState == null ? TaskStatus.Failure : TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Aim", Aim);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      Aim = BehaviorTreeDataReadUtility.ReadShared(reader, "Aim", Aim);
    }
  }
}
