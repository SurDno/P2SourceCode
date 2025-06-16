using System;
using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;

namespace Engine.Behaviours.Engines.Controllers
{
  public class Rootmotion45MovementController : IMovementController
  {
    private Animator animator;
    private Rigidbody rigidbody;
    private Pivot pivot;
    private Rootmotion45 rootmotion45;
    private CapsuleCollider collider;
    private NavMeshAgent agent;
    private GameObject gameObject;
    private Quaternion deltaRotation = Quaternion.identity;
    private float movementSpeed;
    private float rotationError;
    private bool isPaused;
    private bool movementDone;
    private float desiredMovemetSpeed;
    private Vector3 direction;
    private float remainingDistance;
    private EngineBehavior.GaitType gait;
    private float rotationSpeed = 270f;

    public bool IsPaused
    {
      get => isPaused;
      set
      {
        isPaused = value;
        if (!((UnityEngine.Object) animator != (UnityEngine.Object) null) || !animator.gameObject.activeSelf)
          return;
        if (isPaused)
          animator.SetFloat("Mecanim.Speed", 0.0f);
        else
          animator.SetFloat("Mecanim.Speed", 1f);
      }
    }

    public bool IsRotating => AnimatorState45.GetAnimatorState(animator).IsRotate;

    public bool IsStopping => AnimatorState45.GetAnimatorState(animator).IsMovementStop;

    public bool GeometryVisible
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public void Initialize(GameObject gameObject)
    {
      this.gameObject = gameObject;
      pivot = gameObject.GetComponent<Pivot>();
      if ((UnityEngine.Object) pivot == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) (gameObject.name + " doesn't contain " + typeof (Pivot).Name + " unity component."), (UnityEngine.Object) gameObject);
      }
      else
      {
        animator = pivot.GetAnimator();
        if ((UnityEngine.Object) animator == (UnityEngine.Object) null)
          return;
        rootmotion45 = animator.gameObject.GetComponent<Rootmotion45>();
        agent = pivot.GetAgent();
        if ((UnityEngine.Object) agent == (UnityEngine.Object) null)
        {
          Debug.LogError((object) (gameObject.name + " doesn't contain " + typeof (NavMeshAgent).Name + " unity component."), (UnityEngine.Object) gameObject);
        }
        else
        {
          agent.updatePosition = false;
          agent.updateRotation = false;
          rigidbody = pivot.GetRigidbody();
          AnimatorState45.GetAnimatorState(animator).StopDoneEvent += OnMoveDone;
          animator.SetLayerWeight(animator.GetLayerIndex("Dialog Layer"), 0.0f);
        }
      }
    }

    private float CalculateAngleInLocalSpace(Vector3 directionInWorldSpace)
    {
      Vector3 vector3 = Vector3.ProjectOnPlane(gameObject.transform.InverseTransformDirection(directionInWorldSpace), Vector3.up);
      return Mathf.Atan2(vector3.x, vector3.z) * 57.29578f;
    }

    public void StartMovement(Vector3 direction, EngineBehavior.GaitType gait)
    {
      if ((UnityEngine.Object) rootmotion45 == (UnityEngine.Object) null)
      {
        Debug.LogError((object) ("StartMovement " + gameObject.name + " doesn't contain " + typeof (Rootmotion45).Name + " unity component."), (UnityEngine.Object) gameObject);
      }
      else
      {
        AnimatorState45 animatorState = AnimatorState45.GetAnimatorState(animator);
        animatorState.MovableStop = false;
        if (animatorState.IsMove && !animatorState.IsMovementStop)
          return;
        animatorState.MovableAngleStart = CalculateAngleInLocalSpace(direction);
        if (animatorState.MovableAngleStart > 0.0)
        {
          if (gait == EngineBehavior.GaitType.Walk)
          {
            int num = (int) (animatorState.MovableAngleStart / 45.0 + 0.5);
            rotationError = animatorState.MovableAngleStart - num * 45;
            switch (num)
            {
              case 0:
                animatorState.NextMoveIsLeft = rootmotion45.AngleR000ToLeft;
                break;
              case 1:
                animatorState.NextMoveIsLeft = rootmotion45.AngleR045ToLeft;
                break;
              case 2:
                animatorState.NextMoveIsLeft = rootmotion45.AngleR090ToLeft;
                break;
              case 3:
                animatorState.NextMoveIsLeft = rootmotion45.AngleR135ToLeft;
                break;
              case 4:
                animatorState.NextMoveIsLeft = rootmotion45.AngleR180ToLeft;
                break;
            }
          }
          else
          {
            int num = (int) (animatorState.MovableAngleStart / 90.0 + 0.5);
            rotationError = animatorState.MovableAngleStart - num * 90;
            switch (num)
            {
              case 0:
                animatorState.NextMoveIsLeft = rootmotion45.AngleRunR000ToLeft;
                break;
              case 1:
                animatorState.NextMoveIsLeft = rootmotion45.AngleRunR090ToLeft;
                break;
              case 2:
                animatorState.NextMoveIsLeft = rootmotion45.AngleRunR180ToLeft;
                break;
            }
          }
        }
        else if (gait == EngineBehavior.GaitType.Walk)
        {
          int num = (int) (-(double) animatorState.MovableAngleStart / 45.0 + 0.5);
          rotationError = (float) -(-(double) animatorState.MovableAngleStart - num * 45);
          switch (num)
          {
            case 0:
              animatorState.NextMoveIsLeft = rootmotion45.AngleL000ToLeft;
              break;
            case 1:
              animatorState.NextMoveIsLeft = rootmotion45.AngleL045ToLeft;
              break;
            case 2:
              animatorState.NextMoveIsLeft = rootmotion45.AngleL090ToLeft;
              break;
            case 3:
              animatorState.NextMoveIsLeft = rootmotion45.AngleL135ToLeft;
              break;
            case 4:
              animatorState.NextMoveIsLeft = rootmotion45.AngleL180ToLeft;
              break;
          }
        }
        else
        {
          int num = (int) (-(double) animatorState.MovableAngleStart / 90.0 + 0.5);
          rotationError = (float) -(-(double) animatorState.MovableAngleStart - num * 90);
          switch (num)
          {
            case 0:
              animatorState.NextMoveIsLeft = rootmotion45.AngleRunL000ToLeft;
              break;
            case 1:
              animatorState.NextMoveIsLeft = rootmotion45.AngleRunL090ToLeft;
              break;
            case 2:
              animatorState.NextMoveIsLeft = rootmotion45.AngleRunL180ToLeft;
              break;
          }
        }
        movementSpeed = 0.0f;
        switch (gait)
        {
          case EngineBehavior.GaitType.Walk:
            movementSpeed = 1f;
            break;
          case EngineBehavior.GaitType.Run:
            movementSpeed = 2f;
            break;
        }
        animatorState.MovableSpeed = movementSpeed;
        movementDone = false;
        animatorState.ControlMovableState = AnimatorState45.MovableState45.Move;
        animatorState.VelocityScale = 1f;
      }
    }

    public bool Move(Vector3 direction, float remainingDistance, EngineBehavior.GaitType gait)
    {
      this.direction = direction;
      this.remainingDistance = remainingDistance;
      this.gait = gait;
      AnimatorState45 animatorState = AnimatorState45.GetAnimatorState(animator);
      animatorState.RemainingDistance = remainingDistance;
      if (!animatorState.IsMove)
      {
        if (movementDone)
          return movementDone;
        StartMovement(direction, gait);
        return false;
      }
      if (animatorState.IsMovementStart)
        return movementDone;
      SetAgentSpeed(agent, gait);
      desiredMovemetSpeed = 1f;
      switch (gait)
      {
        case EngineBehavior.GaitType.Walk:
          desiredMovemetSpeed = 1f;
          break;
        case EngineBehavior.GaitType.Run:
          desiredMovemetSpeed = 2f;
          break;
      }
      if (animatorState.RemainingDistance > 0.25)
        deltaRotation *= Quaternion.Euler(0.0f, CalculateRotationDeltaAngle(direction, Time.deltaTime, gait), 0.0f);
      return movementDone;
    }

    private void UpdateMovement()
    {
      AnimatorState45 animatorState = AnimatorState45.GetAnimatorState(animator);
      if (!animatorState.IsMove)
        return;
      if (animatorState.IsMovementStart)
      {
        deltaRotation *= Quaternion.Euler(0.0f, rotationError * Time.deltaTime / animatorState.StateLength, 0.0f);
      }
      else
      {
        movementSpeed = Mathf.MoveTowards(movementSpeed, desiredMovemetSpeed, Time.deltaTime / 0.5f);
        animatorState.MovableSpeed = movementSpeed;
        if (animatorState.RemainingDistance <= 0.25)
          return;
        deltaRotation *= Quaternion.Euler(0.0f, CalculateRotationDeltaAngle(direction, Time.deltaTime, gait), 0.0f);
      }
    }

    private void OnMoveDone()
    {
      movementDone = true;
      AnimatorState45.GetAnimatorState(animator).ControlMovableState = AnimatorState45.MovableState45.Idle;
    }

    private void SetAgentSpeed(NavMeshAgent agent, EngineBehavior.GaitType gait)
    {
      if ((UnityEngine.Object) agent == (UnityEngine.Object) null)
        Debug.LogWarning((object) ("gameobject " + (object) gameObject + " doesn`t have nav mesh agent!"));
      else if ((UnityEngine.Object) rootmotion45 == (UnityEngine.Object) null)
        Debug.LogWarning((object) ("gameobject " + (object) gameObject + " doesn`t have rootmotion45!"));
      else if (gait == EngineBehavior.GaitType.Walk)
      {
        if ((double) agent.speed == rootmotion45.WalkSpeed)
          return;
        agent.speed = rootmotion45.WalkSpeed;
      }
      else
      {
        if (gait != EngineBehavior.GaitType.Run)
          throw new NotSupportedException();
        if ((double) agent.speed == rootmotion45.RunSpeed)
          return;
        agent.speed = rootmotion45.RunSpeed;
      }
    }

    public bool Rotate(Vector3 direction)
    {
      AnimatorState45 animatorState = AnimatorState45.GetAnimatorState(animator);
      if (!animatorState.IsRotate)
        animatorState.ControlMovableState = AnimatorState45.MovableState45.Rotate;
      Vector3 vector3 = Vector3.ProjectOnPlane(gameObject.transform.InverseTransformDirection(direction), Vector3.up);
      float num = Mathf.Atan2(vector3.x, vector3.z) * 57.29578f;
      animatorState.MovableAngleStart = num;
      return num < 25.0;
    }

    public bool Rotate(float angle)
    {
      AnimatorState45 animatorState = AnimatorState45.GetAnimatorState(animator);
      animatorState.ControlMovableState = AnimatorState45.MovableState45.Rotate;
      animatorState.MovableAngleStart = (double) Mathf.Abs(angle) > 5.0 ? angle : 0.0f;
      animatorState.MovableSpeed = 0.0f;
      return (double) Mathf.Abs(angle) < 10.0;
    }

    private float CalculateRotationDeltaAngle(
      Vector3 direction,
      float dt,
      EngineBehavior.GaitType gait)
    {
      float angleInLocalSpace = CalculateAngleInLocalSpace(direction);
      if (gait == EngineBehavior.GaitType.Run)
      {
        float num1 = Mathf.InverseLerp(0.0f, 40f, Mathf.Abs(angleInLocalSpace));
        float num2 = Mathf.Lerp(0.0f, 1f, num1 * num1);
        return Mathf.Clamp(angleInLocalSpace / dt, -num2 * rotationSpeed, rotationSpeed * num2) * dt;
      }
      float num3 = Mathf.InverseLerp(0.0f, 90f, Mathf.Abs(angleInLocalSpace));
      float num4 = Mathf.Lerp(0.0f, 1f, num3 * num3);
      return Mathf.Clamp(angleInLocalSpace / dt, -num4 * rotationSpeed, num4 * rotationSpeed) * dt;
    }

    public void OnAnimatorMove()
    {
      float num = animator.updateMode == AnimatorUpdateMode.AnimatePhysics ? Time.fixedDeltaTime : Time.deltaTime;
      if (agent.isActiveAndEnabled && agent.isOnNavMesh)
      {
        agent.nextPosition = gameObject.transform.position + animator.deltaPosition;
        gameObject.transform.position = agent.nextPosition;
      }
      gameObject.transform.rotation *= Quaternion.AngleAxis(57.29578f * animator.angularVelocity.y * num, Vector3.up);
      gameObject.transform.rotation *= deltaRotation;
      deltaRotation = Quaternion.identity;
    }

    public void LateUpdate()
    {
    }

    public void Update() => UpdateMovement();

    public void FixedUpdate()
    {
    }
  }
}
