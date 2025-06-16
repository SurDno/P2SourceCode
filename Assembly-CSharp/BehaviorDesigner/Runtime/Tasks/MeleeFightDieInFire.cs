// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.MeleeFightDieInFire
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;

#nullable disable
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
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat runTime = (SharedFloat) 1f;
    [Tooltip("Should the wait be randomized?")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool randomRunTime = (SharedBool) false;
    [Tooltip("The minimum wait time if random wait is enabled")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat randomRunTimeMin = (SharedFloat) 1f;
    [Tooltip("The maximum wait time if random wait is enabled")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat randomRunTimeMax = (SharedFloat) 1f;
    private float waitDuration;
    private float startTime;
    private float pauseTime;
    private bool infinite = false;
    private NpcState npcState;

    public override void OnAwake()
    {
      this.npcState = this.gameObject.GetComponent<NpcState>();
      if (!((UnityEngine.Object) this.npcState == (UnityEngine.Object) null))
        return;
      Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component"));
    }

    public override void OnStart()
    {
      if ((UnityEngine.Object) this.npcState == (UnityEngine.Object) null)
        return;
      this.infinite = (double) this.runTime.Value == 0.0 && !this.randomRunTime.Value;
      this.npcState.InFire();
      this.startTime = Time.time;
      if (this.randomRunTime.Value)
        this.waitDuration = UnityEngine.Random.Range(this.randomRunTimeMin.Value, this.randomRunTimeMax.Value);
      else
        this.waitDuration = this.runTime.Value;
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.npcState == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      if (this.infinite || (double) this.startTime + (double) this.waitDuration >= (double) Time.time)
        return TaskStatus.Running;
      this.npcState.Ragdoll(true);
      return TaskStatus.Success;
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
      this.runTime = (SharedFloat) 5f;
      this.randomRunTime = (SharedBool) false;
      this.randomRunTimeMin = (SharedFloat) 1f;
      this.randomRunTimeMax = (SharedFloat) 1f;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "RunTime", this.runTime);
      BehaviorTreeDataWriteUtility.WriteShared<SharedBool>(writer, "RandomRunTime", this.randomRunTime);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "RandomRunTimeMin", this.randomRunTimeMin);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "RandomRunTimeMax", this.randomRunTimeMax);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.runTime = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "RunTime", this.runTime);
      this.randomRunTime = BehaviorTreeDataReadUtility.ReadShared<SharedBool>(reader, "RandomRunTime", this.randomRunTime);
      this.randomRunTimeMin = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "RandomRunTimeMin", this.randomRunTimeMin);
      this.randomRunTimeMax = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "RandomRunTimeMax", this.randomRunTimeMax);
    }
  }
}
