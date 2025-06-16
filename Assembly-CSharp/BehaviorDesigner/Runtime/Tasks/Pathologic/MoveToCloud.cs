using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Move To Cloud")]
  [TaskCategory("Pathologic/Movement")]
  [TaskIcon("Pathologic_LongIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (MoveToCloud))]
  public class MoveToCloud : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedVector3 Target;
    protected EngineBehavior behavior;
    protected NpcState npcState;
    protected NavMeshAgent agent;
    protected bool inited;

    public override void OnStart()
    {
      if (!inited)
      {
        agent = gameObject.GetComponent<NavMeshAgent>();
        if ((UnityEngine.Object) agent == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (gameObject.name + ": doesn't contain NavMeshAgent unity component"), (UnityEngine.Object) gameObject);
          return;
        }
        npcState = gameObject.GetComponent<NpcState>();
        if ((UnityEngine.Object) npcState == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component"));
          return;
        }
        inited = true;
      }
      Vector3 vector3 = Target.Value;
      if (false)
        return;
      npcState.MoveCloud(Target.Value);
    }

    public override TaskStatus OnUpdate()
    {
      if (!inited)
        return TaskStatus.Failure;
      Vector3 vector3 = Target.Value;
      if (npcState.Status == NpcStateStatusEnum.Failed)
        return TaskStatus.Failure;
      return npcState.Status == NpcStateStatusEnum.Success ? TaskStatus.Success : TaskStatus.Running;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Target", Target);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      Target = BehaviorTreeDataReadUtility.ReadShared(reader, "Target", Target);
    }
  }
}
