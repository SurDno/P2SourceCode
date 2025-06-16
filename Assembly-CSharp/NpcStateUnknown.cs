using Engine.Behaviours.Components;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateUnknown : INpcState, INpcStateRagdoll
{
  private NpcState npcState;
  private Pivot pivot;
  private NavMeshAgent agent;
  private bool agentWasEnabled;
  private bool animatorWasEnabled;
  private float initialRagdollWeight;
  private float actualRagdollWeight;
  private bool inited;

  public GameObject GameObject { get; private set; }

  [Inspected]
  public NpcStateStatusEnum Status => NpcStateStatusEnum.Running;

  private bool TryInit()
  {
    if (this.inited)
      return true;
    this.agent = this.pivot.GetAgent();
    this.inited = true;
    return true;
  }

  public NpcStateUnknown(NpcState npcState, Pivot pivot)
  {
    this.GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(INpcState previousState)
  {
    if (!this.TryInit())
      return;
    if ((bool) (Object) this.agent)
    {
      this.agentWasEnabled = this.agent.enabled;
      this.agent.enabled = false;
    }
    this.animatorWasEnabled = this.npcState.AnimatorEnabled;
    this.npcState.AnimatorEnabled = false;
    this.initialRagdollWeight = this.pivot.RagdollWeight;
    if (!(previousState is INpcStateRagdoll))
      return;
    this.pivot.RagdollWeight = (previousState as INpcStateRagdoll).GetActualRagdollWeight();
    this.actualRagdollWeight = this.pivot.RagdollWeight;
  }

  public void Shutdown()
  {
    if ((bool) (Object) this.agent)
      this.agent.enabled = this.agentWasEnabled;
    this.npcState.AnimatorEnabled = this.animatorWasEnabled;
    this.pivot.RagdollWeight = this.initialRagdollWeight;
  }

  public void OnAnimatorMove()
  {
    if (!InstanceByRequest<EngineApplication>.Instance.IsPaused)
      ;
  }

  public void OnAnimatorEventEvent(string obj)
  {
  }

  public void Update()
  {
    if (!InstanceByRequest<EngineApplication>.Instance.IsPaused)
      ;
  }

  public float GetActualRagdollWeight() => this.actualRagdollWeight;

  public void OnLodStateChanged(bool enabled)
  {
  }
}
