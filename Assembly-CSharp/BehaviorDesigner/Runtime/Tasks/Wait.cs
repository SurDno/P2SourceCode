using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;

namespace BehaviorDesigner.Runtime.Tasks
{
  [FactoryProxy(typeof (Wait))]
  [TaskDescription("Wait a specified amount of time. The task will return running until the task is done waiting. It will return success after the wait time has elapsed.")]
  [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=22")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class Wait : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("The amount of time to wait")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat waitTime = 1f;
    [Tooltip("Should the wait be randomized?")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedBool randomWait = false;
    [Tooltip("The minimum wait time if random wait is enabled")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat randomWaitMin = 1f;
    [Tooltip("The maximum wait time if random wait is enabled")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedFloat randomWaitMax = 1f;
    private float waitDuration;
    private float startTime;
    private float pauseTime;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "WaitTime", waitTime);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "RandomWait", randomWait);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "RandomWaitMin", randomWaitMin);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "RandomWaitMax", randomWaitMax);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      waitTime = BehaviorTreeDataReadUtility.ReadShared(reader, "WaitTime", waitTime);
      randomWait = BehaviorTreeDataReadUtility.ReadShared(reader, "RandomWait", randomWait);
      randomWaitMin = BehaviorTreeDataReadUtility.ReadShared(reader, "RandomWaitMin", randomWaitMin);
      randomWaitMax = BehaviorTreeDataReadUtility.ReadShared(reader, "RandomWaitMax", randomWaitMax);
    }

    public override void OnStart()
    {
      startTime = Time.time;
      if (randomWait.Value)
        waitDuration = UnityEngine.Random.Range(randomWaitMin.Value, randomWaitMax.Value);
      else
        waitDuration = waitTime.Value;
    }

    public override TaskStatus OnUpdate()
    {
      return startTime + (double) waitDuration < (double) Time.time ? TaskStatus.Success : TaskStatus.Running;
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
      waitTime = 1f;
      randomWait = false;
      randomWaitMin = 1f;
      randomWaitMax = 1f;
    }
  }
}
