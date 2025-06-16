// Decompiled with JetBrains decompiler
// Type: PlagueIntroRat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Components.Utilities;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
public class PlagueIntroRat : MonoBehaviour
{
  private NavMeshAgent agent;
  private PlagueIntroRatsManager manager;
  private bool disposed;
  private Animator animator;
  private int lockIndex;
  private Vector3 prevForward = Vector3.forward;

  [Inspected]
  private NavMeshAgentWrapper agentWrapper => new NavMeshAgentWrapper(this.agent);

  private void Awake()
  {
    this.agent = this.GetComponent<NavMeshAgent>();
    this.animator = this.GetComponent<Animator>();
    this.disposed = true;
  }

  public void Init(PlagueIntroRatsManager manager) => this.manager = manager;

  public void UpdateAreaCost()
  {
    for (int areaIndex = 0; areaIndex < 32; ++areaIndex)
      this.agent.SetAreaCost(areaIndex, 1f);
  }

  public void Spawn(int lockIndex, Vector3 from, Vector3 to)
  {
    this.lockIndex = lockIndex;
    this.transform.position = from;
    this.agent.updatePosition = false;
    this.agent.updateRotation = false;
    this.agent.Warp(from);
    this.agent.enabled = true;
    this.agent.destination = to;
    this.disposed = false;
    this.UpdateAreaCost();
  }

  public void Dispose()
  {
    this.disposed = true;
    this.agent.enabled = false;
    this.manager.Dispose(this, this.lockIndex);
  }

  private void Update()
  {
    if (this.disposed)
      return;
    if ((double) Random.value < (double) Time.deltaTime / 0.5 && NavMeshUtility.IsBrokenPath(this.agent))
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Navigation]").Append("  broken path detected, trying to reset: ").GetInfo((Object) this.gameObject));
      Vector3 destination = this.agent.destination;
      this.agent.ResetPath();
      this.agent.SetDestination(destination);
    }
    else if (!this.agent.hasPath && !this.agent.pathPending)
    {
      this.Dispose();
    }
    else
    {
      float max = 60f;
      this.animator.SetFloat("Turn", Mathf.Clamp(Vector3.SignedAngle(this.transform.forward, this.agent.desiredVelocity, Vector3.up), -max, max) / max, 0.5f, Time.deltaTime);
      this.animator.SetFloat("Forward", 2f * this.agent.desiredVelocity.magnitude / this.agent.speed, 0.5f, Time.deltaTime);
      this.prevForward = this.transform.forward;
      if (this.agent.pathStatus == NavMeshPathStatus.PathInvalid)
        this.Dispose();
      else if (this.agent.hasPath && (double) this.agent.remainingDistance < 0.5)
      {
        this.Dispose();
      }
      else
      {
        if ((double) Random.value >= (double) Time.deltaTime / 5.0 || this.agent.SamplePathPosition(-1, 3f, out NavMeshHit _))
          return;
        this.animator.SetTrigger("Jump");
      }
    }
  }

  public void OnAnimatorMove()
  {
    this.transform.position = this.animator.rootPosition with
    {
      y = this.agent.nextPosition.y
    };
  }
}
