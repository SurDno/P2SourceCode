using System;
using Engine.Behaviours.Components;
using UnityEngine;

namespace Engine.Behaviours.Engines.Controllers
{
  public class InteractiveCutsceneActorMovementController : IMovementController
  {
    private Animator animator;
    private Rigidbody rigidbody;
    private CapsuleCollider collider;
    private Pivot pivot;
    private GameObject gameObject;

    public bool IsPaused { get; set; }

    public bool GeometryVisible
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public void Initialize(GameObject gameObject)
    {
      this.gameObject = gameObject;
      collider = gameObject.GetComponent<CapsuleCollider>();
      rigidbody = gameObject.GetComponent<Rigidbody>();
      if (rigidbody != null)
        rigidbody.isKinematic = true;
      Pivot component = gameObject.GetComponent<Pivot>();
      if (component != null)
        component.RagdollWeight = 0.0f;
      animator = component.GetAnimator();
      animator.updateMode = AnimatorUpdateMode.Normal;
    }

    public void StartMovement(Vector3 direction, EngineBehavior.GaitType gait)
    {
    }

    public bool Move(Vector3 direction, float remainingDistance, EngineBehavior.GaitType gait)
    {
      return false;
    }

    public void OnAnimatorMove() => animator.ApplyBuiltinRootMotion();

    public bool Rotate(Vector3 direction) => throw new NotImplementedException();

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }
  }
}
