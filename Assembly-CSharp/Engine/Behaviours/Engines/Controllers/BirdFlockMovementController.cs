using Engine.Behaviours.Components;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Engine.Behaviours.Engines.Controllers
{
  public class BirdFlockMovementController : IMovementController
  {
    private NavMeshAgent agent;
    private GameObject gameObject;
    private const float flockSpeed = 5f;

    public bool IsPaused { get; set; }

    public bool GeometryVisible
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public void Initialize(GameObject gameObject)
    {
      this.gameObject = gameObject;
      this.agent = gameObject.GetComponent<NavMeshAgent>();
      if (!(bool) (UnityEngine.Object) this.agent)
        return;
      this.agent.enabled = false;
    }

    public void StartMovement(Vector3 direction, EngineBehavior.GaitType gait)
    {
    }

    public bool Move(Vector3 direction, float remainingDistance, EngineBehavior.GaitType gait)
    {
      float num = gait == EngineBehavior.GaitType.Walk ? 5f : 10f;
      this.gameObject.transform.position += direction.normalized * num * Time.deltaTime;
      return (double) remainingDistance < 1.0;
    }

    public bool Rotate(Vector3 direction) => true;

    public void OnAnimatorMove()
    {
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }
  }
}
