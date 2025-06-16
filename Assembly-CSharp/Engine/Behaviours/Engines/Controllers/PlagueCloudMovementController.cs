using System;
using Engine.Behaviours.Components;
using UnityEngine;
using UnityEngine.AI;

namespace Engine.Behaviours.Engines.Controllers
{
  public class PlagueCloudMovementController : IMovementController
  {
    private NavMeshAgent agent;
    private GameObject gameObject;
    private Quaternion deltaRotation = Quaternion.identity;

    public bool IsPaused { get; set; }

    public bool GeometryVisible
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public void Initialize(GameObject gameObject)
    {
      this.gameObject = gameObject;
      agent = gameObject.GetComponent<NavMeshAgent>();
      agent.updatePosition = false;
      agent.updateRotation = false;
    }

    public void StartMovement(Vector3 direction, EngineBehavior.GaitType gait)
    {
    }

    public bool Move(Vector3 direction, float remainingDistance, EngineBehavior.GaitType gait)
    {
      float num = gait == EngineBehavior.GaitType.Walk ? agent.speed : 2f * agent.speed;
      Vector3 vector3 = Vector3.ProjectOnPlane(gameObject.transform.InverseTransformDirection(direction), Vector3.up);
      deltaRotation *= Quaternion.Euler(0.0f, Mathf.Clamp(Mathf.Atan2(vector3.x, vector3.z) * 57.29578f / Time.deltaTime, -agent.angularSpeed, agent.angularSpeed) * Time.deltaTime, 0.0f);
      agent.Move(direction.normalized * num * Time.deltaTime);
      gameObject.transform.position = agent.nextPosition;
      gameObject.transform.rotation *= deltaRotation;
      deltaRotation = Quaternion.identity;
      return remainingDistance < (double) agent.stoppingDistance;
    }

    public bool Rotate(Vector3 direction)
    {
      gameObject.transform.rotation = Quaternion.LookRotation(direction);
      return true;
    }

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
