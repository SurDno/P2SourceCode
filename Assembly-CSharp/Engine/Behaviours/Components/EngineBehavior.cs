using Engine.Behaviours.Engines.Controllers;
using Engine.Source.Commons;
using Inspectors;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Engine.Behaviours.Components
{
  [DisallowMultipleComponent]
  [Serializable]
  public class EngineBehavior : MonoBehaviour, IFlamable
  {
    private IMovementController movementController;
    [SerializeField]
    private MovementControllerEnum movementControllerKind = MovementControllerEnum.Rootmotion45;

    [Inspected]
    public EngineBehavior.GaitType Gait { get; set; }

    public bool IsPaused
    {
      get => this.movementController.IsPaused;
      set => this.movementController.IsPaused = value;
    }

    private void OnEnable()
    {
      this.IsPaused = InstanceByRequest<EngineApplication>.Instance.IsPaused;
    }

    protected virtual void Awake()
    {
      this.Gait = EngineBehavior.GaitType.Walk;
      if (this.movementControllerKind == MovementControllerEnum.PlagueCloud)
        this.movementController = (IMovementController) new PlagueCloudMovementController();
      else if (this.movementControllerKind == MovementControllerEnum.Player)
        this.movementController = (IMovementController) new PlayerMovementController();
      else if (this.movementControllerKind == MovementControllerEnum.InteractiveCutsceneActor)
        this.movementController = (IMovementController) new InteractiveCutsceneActorMovementController();
      else if (this.movementControllerKind == MovementControllerEnum.BirdFlock)
        this.movementController = (IMovementController) new BirdFlockMovementController();
      else if (this.movementControllerKind == MovementControllerEnum.Rootmotion45)
        this.movementController = (IMovementController) new Rootmotion45MovementController();
      else if (this.movementControllerKind == MovementControllerEnum.DeprecatedChangeToRootmotion45)
      {
        Debug.LogError((object) string.Format("Deprecated movement controller type {0}", (object) this.movementControllerKind), (UnityEngine.Object) this.gameObject);
        this.movementController = (IMovementController) new Rootmotion45MovementController();
      }
      else
      {
        Debug.LogError((object) string.Format("Wrong movement controller type {0}", (object) this.movementControllerKind), (UnityEngine.Object) this.gameObject);
        return;
      }
      this.movementController.Initialize(this.gameObject);
    }

    private void Update() => this.movementController.Update();

    protected void FixedUpdate() => this.movementController.FixedUpdate();

    public bool GeometryVisible
    {
      set => this.movementController.GeometryVisible = value;
    }

    public void StartMovement(Vector3 direction)
    {
      this.movementController.StartMovement(direction, this.Gait);
    }

    public bool Move(Vector3 direction, float remainingDistance)
    {
      return this.movementController.Move(direction, remainingDistance, this.Gait);
    }

    public bool Rotate(Vector3 direction) => this.movementController.Rotate(direction);

    public void OnExternalAnimatorMove()
    {
      if (this.movementController == null)
        return;
      this.movementController.OnAnimatorMove();
    }

    [SpecialName]
    GameObject IFlamable.get_gameObject() => this.gameObject;

    public enum GaitType
    {
      Walk = 1,
      Run = 2,
    }
  }
}
