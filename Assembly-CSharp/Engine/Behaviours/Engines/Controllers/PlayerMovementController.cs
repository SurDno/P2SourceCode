using System;
using Engine.Behaviours.Components;
using Engine.Behaviours.Engines.Services;
using UnityEngine;

namespace Engine.Behaviours.Engines.Controllers
{
  public class PlayerMovementController : IMovementController
  {
    private Animator animator;
    private GameObject gameObject;
    private Pivot pivot;
    private CharacterController characterController;
    private CapsuleCollider collider;
    private PlayerWeaponServiceNew weaponService;
    private bool isPaused;

    public bool IsPaused
    {
      get => isPaused;
      set
      {
        isPaused = value;
        if (!(animator != null))
          return;
        if (isPaused && animator.gameObject.activeSelf)
          animator.SetFloat("Mecanim.Speed", 0.0f);
        else
          animator.SetFloat("Mecanim.Speed", 1f);
      }
    }

    public bool GeometryVisible
    {
      set => weaponService.GeometryVisible = value;
    }

    public void Initialize(GameObject gameObject)
    {
      this.gameObject = gameObject;
      collider = gameObject.GetComponent<CapsuleCollider>();
      weaponService = gameObject.GetComponent<PlayerWeaponServiceNew>();
      if (weaponService == null)
        Debug.LogWarningFormat("{0} doesn' contain {1} unity component.", gameObject.name, typeof (PlayerWeaponServiceNew).Name);
      pivot = gameObject.GetComponent<Pivot>();
      if (pivot == null)
        Debug.LogWarningFormat("{0} doesn' contain {1} unity component.", gameObject.name, typeof (Pivot).Name);
      characterController = gameObject.GetComponent<CharacterController>();
      if (characterController == null)
        Debug.LogWarningFormat("{0} doesn' contain {1} unity component.", gameObject.name, typeof (CharacterController).Name);
      animator = gameObject.GetComponent<Animator>();
    }

    public void StartMovement(Vector3 direction, EngineBehavior.GaitType gait)
    {
    }

    public bool Move(Vector3 direction, float remainingDistance, EngineBehavior.GaitType gait)
    {
      throw new NotImplementedException();
    }

    public bool Rotate(Vector3 direction) => true;

    public void OnAnimatorMove()
    {
      gameObject.transform.rotation *= Quaternion.AngleAxis(57.29578f * animator.angularVelocity.y * (animator.updateMode == AnimatorUpdateMode.AnimatePhysics ? Time.fixedDeltaTime : Time.deltaTime), Vector3.up);
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }
  }
}
