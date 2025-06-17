using Engine.Behaviours.Components;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

public class NpcStateMoveCloud(NpcState npcState, Pivot pivot) : INpcState 
  {
  private NpcState npcState = npcState;
  [Inspected]
  private StateEnum state = StateEnum.Moving;
  [Inspected]
  private NavMeshAgent agent;
  [Inspected]
  private Vector3 destination;
  private int prevAreaMask;
  private bool agentWasEnabled;
  private float speed = 2f;

  public GameObject GameObject { get; private set; } = npcState.gameObject;

  [Inspected]
  public NpcStateStatusEnum Status { get; private set; }

  public void Activate(Vector3 destination)
  {
    Status = NpcStateStatusEnum.Running;
    agent = pivot.GetAgent();
    if (agent != null)
    {
      speed = agent.speed;
      agentWasEnabled = agent.enabled;
      agent.enabled = false;
    }
    this.destination = destination;
    state = StateEnum.Moving;
  }

  public void Shutdown()
  {
    if (!(agent != null))
      return;
    agent.enabled = agentWasEnabled;
  }

  public void OnAnimatorMove()
  {
  }

  public void OnAnimatorEventEvent(string obj)
  {
  }

  public void Update()
  {
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
      return;
    if (state == StateEnum.Moving)
      OnUpdateMove();
    if (state != StateEnum.Done)
      return;
    Status = NpcStateStatusEnum.Success;
  }

  public void OnUpdateMove()
  {
    Vector3 vector3 = destination - GameObject.transform.position;
    float magnitude = vector3.magnitude;
    float num = 0.2f;
    GameObject.transform.position += vector3.normalized * speed * Time.deltaTime;
    if (magnitude >= (double) num)
      return;
    state = StateEnum.Done;
  }

  public void OnLodStateChanged(bool enabled)
  {
  }

  private enum StateEnum
  {
    Moving,
    Done,
  }
}
