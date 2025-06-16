using Engine.Source.Components.Utilities;
using Inspectors;

public class PlagueIntroRat : MonoBehaviour
{
  private NavMeshAgent agent;
  private PlagueIntroRatsManager manager;
  private bool disposed;
  private Animator animator;
  private int lockIndex;
  private Vector3 prevForward = Vector3.forward;

  [Inspected]
  private NavMeshAgentWrapper agentWrapper => new NavMeshAgentWrapper(agent);

  private void Awake()
  {
    agent = this.GetComponent<NavMeshAgent>();
    animator = this.GetComponent<Animator>();
    disposed = true;
  }

  public void Init(PlagueIntroRatsManager manager) => this.manager = manager;

  public void UpdateAreaCost()
  {
    for (int areaIndex = 0; areaIndex < 32; ++areaIndex)
      agent.SetAreaCost(areaIndex, 1f);
  }

  public void Spawn(int lockIndex, Vector3 from, Vector3 to)
  {
    this.lockIndex = lockIndex;
    this.transform.position = from;
    agent.updatePosition = false;
    agent.updateRotation = false;
    agent.Warp(from);
    agent.enabled = true;
    agent.destination = to;
    disposed = false;
    UpdateAreaCost();
  }

  public void Dispose()
  {
    disposed = true;
    agent.enabled = false;
    manager.Dispose(this, lockIndex);
  }

  private void Update()
  {
    if (disposed)
      return;
    if ((double) Random.value < (double) Time.deltaTime / 0.5 && NavMeshUtility.IsBrokenPath(agent))
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  broken path detected, trying to reset: ").GetInfo((Object) this.gameObject));
      Vector3 destination = agent.destination;
      agent.ResetPath();
      agent.SetDestination(destination);
    }
    else if (!agent.hasPath && !agent.pathPending)
    {
      Dispose();
    }
    else
    {
      float max = 60f;
      animator.SetFloat("Turn", Mathf.Clamp(Vector3.SignedAngle(this.transform.forward, agent.desiredVelocity, Vector3.up), -max, max) / max, 0.5f, Time.deltaTime);
      animator.SetFloat("Forward", 2f * agent.desiredVelocity.magnitude / agent.speed, 0.5f, Time.deltaTime);
      prevForward = this.transform.forward;
      if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        Dispose();
      else if (agent.hasPath && (double) agent.remainingDistance < 0.5)
      {
        Dispose();
      }
      else
      {
        if ((double) Random.value >= (double) Time.deltaTime / 5.0 || agent.SamplePathPosition(-1, 3f, out NavMeshHit _))
          return;
        animator.SetTrigger("Jump");
      }
    }
  }

  public void OnAnimatorMove()
  {
    this.transform.position = animator.rootPosition with
    {
      y = agent.nextPosition.y
    };
  }
}
