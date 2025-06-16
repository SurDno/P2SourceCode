using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BehaviorDesigner.Runtime.Tasks
{
  [TaskCategory("Pathologic/Fight/Melee")]
  [TaskDescription("умереть")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (MeleeFightDieInFire))]
  public class MeleeFightDieInFire : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("The amount of time to wait. Use 0 for infinite idle.")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat runTime = 1f;
    [Tooltip("Should the wait be randomized?")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedBool randomRunTime = false;
    [Tooltip("The minimum wait time if random wait is enabled")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat randomRunTimeMin = 1f;
    [Tooltip("The maximum wait time if random wait is enabled")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedFloat randomRunTimeMax = 1f;
    private float waitDuration;
    private float startTime;
    private float pauseTime;
    private bool infinite;
    private NpcState npcState;

    public override void OnAwake()
    {
      npcState = gameObject.GetComponent<NpcState>();
      if (!(npcState == null))
        return;
      Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component");
    }

    public override void OnStart()
    {
      if (npcState == null)
        return;
      infinite = runTime.Value == 0.0 && !randomRunTime.Value;
      npcState.InFire();
      startTime = Time.time;
      if (randomRunTime.Value)
        waitDuration = Random.Range(randomRunTimeMin.Value, randomRunTimeMax.Value);
      else
        waitDuration = runTime.Value;
    }

    public override TaskStatus OnUpdate()
    {
      if (npcState == null)
        return TaskStatus.Failure;
      if (infinite || startTime + (double) waitDuration >= Time.time)
        return TaskStatus.Running;
      npcState.Ragdoll(true);
      return TaskStatus.Success;
    }

    public override void OnPause(bool paused)
    {
      if (paused)
        pauseTime = Time.time;
      else
        startTime += Time.time - pauseTime;
    }

    public override void OnReset()
    {
      runTime = 5f;
      randomRunTime = false;
      randomRunTimeMin = 1f;
      randomRunTimeMax = 1f;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "RunTime", runTime);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "RandomRunTime", randomRunTime);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "RandomRunTimeMin", randomRunTimeMin);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "RandomRunTimeMax", randomRunTimeMax);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      runTime = BehaviorTreeDataReadUtility.ReadShared(reader, "RunTime", runTime);
      randomRunTime = BehaviorTreeDataReadUtility.ReadShared(reader, "RandomRunTime", randomRunTime);
      randomRunTimeMin = BehaviorTreeDataReadUtility.ReadShared(reader, "RandomRunTimeMin", randomRunTimeMin);
      randomRunTimeMax = BehaviorTreeDataReadUtility.ReadShared(reader, "RandomRunTimeMax", randomRunTimeMax);
    }
  }
}
