using Engine.Behaviours.Components;
using Engine.Behaviours.Engines.Services;
using System;
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
      get => this.isPaused;
      set
      {
        this.isPaused = value;
        if (!((UnityEngine.Object) this.animator != (UnityEngine.Object) null))
          return;
        if (this.isPaused && this.animator.gameObject.activeSelf)
          this.animator.SetFloat("Mecanim.Speed", 0.0f);
        else
          this.animator.SetFloat("Mecanim.Speed", 1f);
      }
    }

    public bool GeometryVisible
    {
      set => this.weaponService.GeometryVisible = value;
    }

    public void Initialize(GameObject gameObject)
    {
      this.gameObject = gameObject;
      this.collider = gameObject.GetComponent<CapsuleCollider>();
      this.weaponService = gameObject.GetComponent<PlayerWeaponServiceNew>();
      if ((UnityEngine.Object) this.weaponService == (UnityEngine.Object) null)
        Debug.LogWarningFormat("{0} doesn' contain {1} unity component.", (object) gameObject.name, (object) typeof (PlayerWeaponServiceNew).Name);
      this.pivot = gameObject.GetComponent<Pivot>();
      if ((UnityEngine.Object) this.pivot == (UnityEngine.Object) null)
        Debug.LogWarningFormat("{0} doesn' contain {1} unity component.", (object) gameObject.name, (object) typeof (Pivot).Name);
      this.characterController = gameObject.GetComponent<CharacterController>();
      if ((UnityEngine.Object) this.characterController == (UnityEngine.Object) null)
        Debug.LogWarningFormat("{0} doesn' contain {1} unity component.", (object) gameObject.name, (object) typeof (CharacterController).Name);
      this.animator = gameObject.GetComponent<Animator>();
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
      this.gameObject.transform.rotation *= Quaternion.AngleAxis(57.29578f * this.animator.angularVelocity.y * (this.animator.updateMode == AnimatorUpdateMode.AnimatePhysics ? Time.fixedDeltaTime : Time.deltaTime), Vector3.up);
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }
  }
}
