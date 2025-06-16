using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (FightBase))]
  public class FightBase : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    protected NPCEnemy owner;
    protected float waitDuration;
    protected float startTime;
    protected float lastTime;
    private float pauseTime;
    private bool initialized;

    public virtual TaskStatus DoUpdate(float deltaTime) => TaskStatus.Running;

    public override void OnPause(bool paused)
    {
      if (paused)
      {
        this.pauseTime = Time.time;
      }
      else
      {
        this.startTime += Time.time - this.pauseTime;
        this.lastTime += Time.time - this.pauseTime;
      }
    }

    public override void OnStart()
    {
      if (!this.initialized)
      {
        Pivot component = this.GetComponent<Pivot>();
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (Pivot).Name + " engine component"), (UnityEngine.Object) this.gameObject);
          return;
        }
        this.owner = component.GetNpcEnemy();
        if ((UnityEngine.Object) this.owner == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (NPCEnemy).Name + " engine component"), (UnityEngine.Object) this.gameObject);
          return;
        }
        this.initialized = true;
      }
      this.lastTime = this.startTime = Time.time;
    }

    public override TaskStatus OnUpdate()
    {
      if (!this.initialized || (UnityEngine.Object) this.owner.Enemy == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      float deltaTime = Time.time - this.lastTime;
      this.lastTime = Time.time;
      return this.DoUpdate(deltaTime);
    }

    public override void OnReset()
    {
    }

    public override void OnEnd()
    {
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
    }
  }
}
