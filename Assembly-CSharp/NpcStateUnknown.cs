using Engine.Behaviours.Components;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateUnknown(NpcState npcState, Pivot pivot) : INpcState, INpcStateRagdoll {
  private NavMeshAgent agent;
  private bool agentWasEnabled;
  private bool animatorWasEnabled;
  private float initialRagdollWeight;
  private float actualRagdollWeight;
  private bool inited;

  public GameObject GameObject { get; private set; } = npcState.gameObject;

  [Inspected]
  public NpcStateStatusEnum Status => NpcStateStatusEnum.Running;

  private bool TryInit()
  {
    if (inited)
      return true;
    agent = pivot.GetAgent();
    inited = true;
    return true;
  }

  public void Activate(INpcState previousState)
  {
    if (!TryInit())
      return;
    if ((bool) (Object) agent)
    {
      agentWasEnabled = agent.enabled;
      agent.enabled = false;
    }
    animatorWasEnabled = npcState.AnimatorEnabled;
    npcState.AnimatorEnabled = false;
    initialRagdollWeight = pivot.RagdollWeight;
    if (!(previousState is INpcStateRagdoll))
      return;
    pivot.RagdollWeight = (previousState as INpcStateRagdoll).GetActualRagdollWeight();
    actualRagdollWeight = pivot.RagdollWeight;
  }

  public void Shutdown()
  {
    if ((bool) (Object) agent)
      agent.enabled = agentWasEnabled;
    npcState.AnimatorEnabled = animatorWasEnabled;
    pivot.RagdollWeight = initialRagdollWeight;
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

  public float GetActualRagdollWeight() => actualRagdollWeight;

  public void OnLodStateChanged(bool enabled)
  {
  }
}
