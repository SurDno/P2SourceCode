using Engine.Behaviours.Components;
using System;
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
      this.agent = gameObject.GetComponent<NavMeshAgent>();
      this.agent.updatePosition = false;
      this.agent.updateRotation = false;
    }

    public void StartMovement(Vector3 direction, EngineBehavior.GaitType gait)
    {
    }

    public bool Move(Vector3 direction, float remainingDistance, EngineBehavior.GaitType gait)
    {
      float num = gait == EngineBehavior.GaitType.Walk ? this.agent.speed : 2f * this.agent.speed;
      Vector3 vector3 = Vector3.ProjectOnPlane(this.gameObject.transform.InverseTransformDirection(direction), Vector3.up);
      this.deltaRotation *= Quaternion.Euler(0.0f, Mathf.Clamp(Mathf.Atan2(vector3.x, vector3.z) * 57.29578f / Time.deltaTime, -this.agent.angularSpeed, this.agent.angularSpeed) * Time.deltaTime, 0.0f);
      this.agent.Move(direction.normalized * num * Time.deltaTime);
      this.gameObject.transform.position = this.agent.nextPosition;
      this.gameObject.transform.rotation *= this.deltaRotation;
      this.deltaRotation = Quaternion.identity;
      return (double) remainingDistance < (double) this.agent.stoppingDistance;
    }

    public bool Rotate(Vector3 direction)
    {
      this.gameObject.transform.rotation = Quaternion.LookRotation(direction);
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
