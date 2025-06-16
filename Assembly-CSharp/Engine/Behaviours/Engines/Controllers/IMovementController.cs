using Engine.Behaviours.Components;
using UnityEngine;

namespace Engine.Behaviours.Engines.Controllers
{
  public interface IMovementController
  {
    bool IsPaused { get; set; }

    bool GeometryVisible { set; }

    void Initialize(GameObject gameObject);

    void StartMovement(Vector3 direction, EngineBehavior.GaitType gait);

    bool Move(Vector3 direction, float remainingDistance, EngineBehavior.GaitType gait);

    bool Rotate(Vector3 direction);

    void OnAnimatorMove();

    void Update();

    void FixedUpdate();
  }
}
