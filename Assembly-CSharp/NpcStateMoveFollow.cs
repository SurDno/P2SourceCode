using Engine.Behaviours.Components;
using UnityEngine;

public class NpcStateMoveFollow : NpcStateMoveBase
{
  private Transform target;
  private float followDistance;
  private Vector3 lastDestination;
  private bool completed;

  public NpcStateMoveFollow(NpcState npcState, Pivot pivot)
    : base(npcState, pivot, false)
  {
  }

  public void Activate(Transform target, float followDistance)
  {
    if (!this.Activate())
      return;
    this.target = target;
    this.followDistance = followDistance;
    this.completed = false;
    if ((Object) target == (Object) null)
    {
      Debug.LogWarning((object) (this.GameObject.name + ": target is null"));
      this.Status = NpcStateStatusEnum.Failed;
    }
    else
    {
      this.lastDestination = target.position;
      if ((double) (target.position - this.GameObject.transform.position).magnitude < 1.1000000238418579 * (double) followDistance)
        this.Status = NpcStateStatusEnum.Success;
      else
        this.agent.SetDestination(this.lastDestination);
    }
  }

  public override void DoUpdate()
  {
    if ((Object) this.target == (Object) null)
    {
      Debug.LogWarning((object) (this.GameObject.name + ": target is null"));
      this.Status = NpcStateStatusEnum.Failed;
    }
    else
    {
      if (this.completed)
        return;
      if (this.agent.hasPath && (double) this.agent.remainingDistance < (double) this.followDistance)
      {
        this.CompleteTask(false);
        this.completed = true;
      }
      Vector3 vector3 = this.lastDestination - this.target.position;
      float magnitude = vector3.magnitude;
      vector3 = this.target.position - this.GameObject.transform.position;
      if ((double) vector3.magnitude > (double) this.followDistance * 5.0)
      {
        if ((double) magnitude > (double) this.followDistance)
        {
          this.lastDestination = this.target.position;
          this.agent.SetDestination(this.lastDestination);
        }
      }
      else if ((double) magnitude > 0.30000001192092896 * (double) this.followDistance)
      {
        this.lastDestination = this.target.position;
        this.agent.SetDestination(this.lastDestination);
      }
    }
  }
}
