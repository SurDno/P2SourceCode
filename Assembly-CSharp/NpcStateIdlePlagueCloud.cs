using Engine.Behaviours.Components;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateIdlePlagueCloud : INpcState
{
  private EngineBehavior behavior;
  private NavMeshAgent agent;
  private Pivot pivot;
  private NpcState npcState;
  private bool agentWasEnabled;
  private bool inited;
  private bool failed;

  public GameObject GameObject { get; private set; }

  [Inspected]
  public NpcStateStatusEnum Status => NpcStateStatusEnum.Running;

  private bool TryInit()
  {
    if (this.inited)
      return true;
    this.behavior = this.pivot.GetBehavior();
    this.agent = this.pivot.GetAgent();
    this.failed = false;
    this.inited = true;
    return true;
  }

  public NpcStateIdlePlagueCloud(NpcState npcState, Pivot pivot)
  {
    this.npcState = npcState;
    this.pivot = pivot;
    this.GameObject = npcState.gameObject;
  }

  public void Activate()
  {
    if (!this.TryInit() || !(bool) (Object) this.agent)
      return;
    this.agentWasEnabled = this.agent.enabled;
    this.agent.enabled = true;
  }

  public void Shutdown()
  {
    if (this.failed || !(bool) (Object) this.agent)
      return;
    this.agent.enabled = this.agentWasEnabled;
  }

  public void OnAnimatorMove()
  {
    if (!this.failed)
      ;
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!this.failed)
      ;
  }

  public void Update()
  {
    if (this.failed || !InstanceByRequest<EngineApplication>.Instance.IsPaused)
      ;
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
