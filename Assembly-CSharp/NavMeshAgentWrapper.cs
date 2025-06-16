// Decompiled with JetBrains decompiler
// Type: NavMeshAgentWrapper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Inspectors;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
public class NavMeshAgentWrapper
{
  private NavMeshAgent agent;

  [Inspected]
  private void RestorePath()
  {
    Vector3 destination = this.agent.destination;
    this.agent.ResetPath();
    this.agent.SetDestination(destination);
  }

  public NavMeshAgentWrapper(NavMeshAgent agent) => this.agent = agent;

  [Inspected]
  private bool HasPath => this.agent.hasPath;

  [Inspected]
  private bool PathPending => this.agent.pathPending;

  [Inspected]
  private bool IsPathStale => this.agent.isPathStale;

  [Inspected]
  private bool IsStopped => this.agent.isStopped;

  [Inspected]
  private NavMeshPathStatus PathStatus => this.agent.pathStatus;

  [Inspected]
  private float RemainingDistance => this.agent.remainingDistance;

  [Inspected]
  private float StoppingDistance => this.agent.stoppingDistance;

  [Inspected]
  private NavMeshPathWrapper Path => new NavMeshPathWrapper(this.agent.path);

  [Inspected]
  private bool IsOnNavMesh => this.agent.isOnNavMesh;

  [Inspected]
  private bool AutoRepth => this.agent.autoRepath;

  [Inspected]
  private Vector3 Velocity => this.agent.velocity;

  [Inspected]
  private Vector3 DesiredVelocity => this.agent.desiredVelocity;
}
