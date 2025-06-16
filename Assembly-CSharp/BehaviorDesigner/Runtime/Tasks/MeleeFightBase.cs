using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (MeleeFightBase))]
  public class MeleeFightBase : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    protected Pivot pivot;
    protected EnemyBase owner;
    protected Animator animator;
    protected FightAnimatorBehavior.AnimatorState fightAnimatorState;
    protected AnimatorState45 animatorState;
    protected NavMeshAgent agent;
    protected NpcState npcState;
    protected float waitDuration;
    protected float startTime;
    protected float lastTime;
    private float pauseTime;

    public virtual TaskStatus DoUpdate(float deltaTime) => TaskStatus.Running;

    public override void OnAwake()
    {
    }

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
      if (pivot == null)
      {
        pivot = GetComponent<Pivot>();
        if (pivot == null)
        {
          Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof (Pivot).Name + " engine component", gameObject);
          return;
        }
        owner = gameObject.GetComponent<EnemyBase>();
        animator = pivot.GetAnimator();
        agent = pivot.GetAgent();
        fightAnimatorState = FightAnimatorBehavior.GetAnimatorState(animator);
        animatorState = AnimatorState45.GetAnimatorState(animator);
        npcState = gameObject.GetComponent<NpcState>();
        if (npcState == null)
        {
          Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component", gameObject);
          return;
        }
      }
      lastTime = startTime = Time.time;
    }

    public override TaskStatus OnUpdate()
    {
      if (owner.Enemy == null)
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
