using System.Collections.Generic;
using Engine.Behaviours.Components;
using Engine.Source.Commons;

public class NpcStateMoveByPathCloud : INpcState
{
  private StateEnum state;
  private bool failed;
  private NpcState npcState;
  private Pivot pivot;
  private NavMeshAgent agent;
  private bool agentWasEnabled;
  private List<Vector3> path;
  private int currentIndex;
  private Vector3 currentPoint;
  private float remainingPointsDistance;
  private float speed = 2f;
  private bool inited;

  public GameObject GameObject { get; private set; }

  public NpcStateStatusEnum Status { get; private set; }

  private bool TryInit()
  {
    if (inited)
      return true;
    agent = pivot.GetAgent();
    if ((Object) agent != (Object) null)
      speed = agent.speed;
    inited = true;
    return true;
  }

  public NpcStateMoveByPathCloud(NpcState npcState, Pivot pivot)
  {
    GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(List<Vector3> path)
  {
    failed = false;
    currentIndex = 0;
    Status = NpcStateStatusEnum.Running;
    if (!TryInit())
      return;
    agentWasEnabled = agent.enabled;
    agent.enabled = false;
    this.path = path;
    GoToNextPoint();
  }

  private void GoToNextPoint()
  {
    currentPoint = path[currentIndex];
    ++currentIndex;
    state = StateEnum.Moving;
    RecountRemainingDistance();
  }

  private void RecountRemainingDistance()
  {
    remainingPointsDistance = 0.0f;
    for (int currentIndex = this.currentIndex; currentIndex < path.Count; ++currentIndex)
      remainingPointsDistance += (path[currentIndex - 1] - path[currentIndex]).magnitude;
  }

  public void Shutdown()
  {
    if (failed)
      return;
    agent.enabled = agentWasEnabled;
  }

  public void OnAnimatorMove()
  {
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!failed)
      ;
  }

  public void Update()
  {
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused || !inited)
      return;
    if (state == StateEnum.Moving)
      OnUpdateMove();
    if (state != StateEnum.Done)
      return;
    Status = failed ? NpcStateStatusEnum.Failed : NpcStateStatusEnum.Success;
  }

  public void OnUpdateMove()
  {
    Vector3 vector3 = currentPoint - GameObject.transform.position;
    float magnitude = vector3.magnitude;
    float num = 0.5f;
    GameObject.transform.position += vector3.normalized * speed * Time.deltaTime;
    if (magnitude >= (double) num)
      return;
    PointReached();
  }

  private void PointReached()
  {
    if (currentIndex == path.Count)
      state = StateEnum.Done;
    else
      GoToNextPoint();
  }

  public void OnLodStateChanged(bool enabled)
  {
  }

  private enum StateEnum
  {
    Moving,
    Stopping,
    Done,
  }
}
