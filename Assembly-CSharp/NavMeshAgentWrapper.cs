﻿using Inspectors;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentWrapper(NavMeshAgent agent) {
  [Inspected]
  private void RestorePath()
  {
    Vector3 destination = agent.destination;
    agent.ResetPath();
    agent.SetDestination(destination);
  }

  [Inspected]
  private bool HasPath => agent.hasPath;

  [Inspected]
  private bool PathPending => agent.pathPending;

  [Inspected]
  private bool IsPathStale => agent.isPathStale;

  [Inspected]
  private bool IsStopped => agent.isStopped;

  [Inspected]
  private NavMeshPathStatus PathStatus => agent.pathStatus;

  [Inspected]
  private float RemainingDistance => agent.remainingDistance;

  [Inspected]
  private float StoppingDistance => agent.stoppingDistance;

  [Inspected]
  private NavMeshPathWrapper Path => new(agent.path);

  [Inspected]
  private bool IsOnNavMesh => agent.isOnNavMesh;

  [Inspected]
  private bool AutoRepth => agent.autoRepath;

  [Inspected]
  private Vector3 Velocity => agent.velocity;

  [Inspected]
  private Vector3 DesiredVelocity => agent.desiredVelocity;
}
