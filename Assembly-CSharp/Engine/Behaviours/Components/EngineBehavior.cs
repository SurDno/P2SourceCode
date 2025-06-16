using System;
using System.Runtime.CompilerServices;
using Engine.Behaviours.Engines.Controllers;
using Engine.Source.Commons;
using Inspectors;
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
    public GaitType Gait { get; set; }

    public bool IsPaused
    {
      get => movementController.IsPaused;
      set => movementController.IsPaused = value;
    }

    private void OnEnable()
    {
      IsPaused = InstanceByRequest<EngineApplication>.Instance.IsPaused;
    }

    protected virtual void Awake()
    {
      Gait = GaitType.Walk;
      if (movementControllerKind == MovementControllerEnum.PlagueCloud)
        movementController = new PlagueCloudMovementController();
      else if (movementControllerKind == MovementControllerEnum.Player)
        movementController = new PlayerMovementController();
      else if (movementControllerKind == MovementControllerEnum.InteractiveCutsceneActor)
        movementController = new InteractiveCutsceneActorMovementController();
      else if (movementControllerKind == MovementControllerEnum.BirdFlock)
        movementController = new BirdFlockMovementController();
      else if (movementControllerKind == MovementControllerEnum.Rootmotion45)
        movementController = new Rootmotion45MovementController();
      else if (movementControllerKind == MovementControllerEnum.DeprecatedChangeToRootmotion45)
      {
        Debug.LogError(string.Format("Deprecated movement controller type {0}", movementControllerKind), gameObject);
        movementController = new Rootmotion45MovementController();
      }
      else
      {
        Debug.LogError(string.Format("Wrong movement controller type {0}", movementControllerKind), gameObject);
        return;
      }
      movementController.Initialize(gameObject);
    }

    private void Update() => movementController.Update();

    protected void FixedUpdate() => movementController.FixedUpdate();

    public bool GeometryVisible
    {
      set => movementController.GeometryVisible = value;
    }

    public void StartMovement(Vector3 direction)
    {
      movementController.StartMovement(direction, Gait);
    }

    public bool Move(Vector3 direction, float remainingDistance)
    {
      return movementController.Move(direction, remainingDistance, Gait);
    }

    public bool Rotate(Vector3 direction) => movementController.Rotate(direction);

    public void OnExternalAnimatorMove()
    {
      if (movementController == null)
        return;
      movementController.OnAnimatorMove();
    }

    public enum GaitType
    {
      Walk = 1,
      Run = 2,
    }
  }
}
