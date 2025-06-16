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
  [FactoryProxy(typeof (Wait))]
  [TaskDescription("Wait a specified amount of time. The task will return running until the task is done waiting. It will return success after the wait time has elapsed.")]
  [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=22")]
  [TaskIcon("{SkinColor}WaitIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class Wait : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [Tooltip("The amount of time to wait")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat waitTime = (SharedFloat) 1f;
    [Tooltip("Should the wait be randomized?")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool randomWait = (SharedBool) false;
    [Tooltip("The minimum wait time if random wait is enabled")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat randomWaitMin = (SharedFloat) 1f;
    [Tooltip("The maximum wait time if random wait is enabled")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat randomWaitMax = (SharedFloat) 1f;
    private float waitDuration;
    private float startTime;
    private float pauseTime;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "WaitTime", this.waitTime);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "RandomWait", this.randomWait);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "RandomWaitMin", this.randomWaitMin);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "RandomWaitMax", this.randomWaitMax);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.waitTime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "WaitTime", this.waitTime);
      this.randomWait = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "RandomWait", this.randomWait);
      this.randomWaitMin = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "RandomWaitMin", this.randomWaitMin);
      this.randomWaitMax = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "RandomWaitMax", this.randomWaitMax);
    }

    public override void OnStart()
    {
      this.startTime = Time.time;
      if (this.randomWait.Value)
        this.waitDuration = UnityEngine.Random.Range(this.randomWaitMin.Value, this.randomWaitMax.Value);
      else
        this.waitDuration = this.waitTime.Value;
    }

    public override TaskStatus OnUpdate()
    {
      return (double) this.startTime + (double) this.waitDuration < (double) Time.time ? TaskStatus.Success : TaskStatus.Running;
    }

    public override void OnPause(bool paused)
    {
      if (paused)
        this.pauseTime = Time.time;
      else
        this.startTime += Time.time - this.pauseTime;
    }

    public override void OnReset()
    {
      this.waitTime = (SharedFloat) 1f;
      this.randomWait = (SharedBool) false;
      this.randomWaitMin = (SharedFloat) 1f;
      this.randomWaitMax = (SharedFloat) 1f;
    }
  }
}
