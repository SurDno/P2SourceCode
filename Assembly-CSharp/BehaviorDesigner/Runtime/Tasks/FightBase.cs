using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;

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
        pauseTime = Time.time;
      }
      else
      {
        startTime += Time.time - pauseTime;
        lastTime += Time.time - pauseTime;
      }
    }

    public override void OnStart()
    {
      if (!initialized)
      {
        Pivot component = this.GetComponent<Pivot>();
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (gameObject.name + ": doesn't contain " + typeof (Pivot).Name + " engine component"), (UnityEngine.Object) gameObject);
          return;
        }
        owner = component.GetNpcEnemy();
        if ((UnityEngine.Object) owner == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) (gameObject.name + ": doesn't contain " + typeof (NPCEnemy).Name + " engine component"), (UnityEngine.Object) gameObject);
          return;
        }
        initialized = true;
      }
      lastTime = startTime = Time.time;
    }

    public override TaskStatus OnUpdate()
    {
      if (!initialized || (UnityEngine.Object) owner.Enemy == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      float deltaTime = Time.time - lastTime;
      lastTime = Time.time;
      return DoUpdate(deltaTime);
    }

    public override void OnReset()
    {
    }

    public override void OnEnd()
    {
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
    }
  }
}
