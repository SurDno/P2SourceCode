using System;
using Engine.Common;
using Engine.Source.Commons;

public class FlockChild : MonoBehaviour, IUpdatable
{
  [NonSerialized]
  public FlockController flockController;
  [NonSerialized]
  public Vector3 wayPoint;
  [NonSerialized]
  public float speed;
  [NonSerialized]
  public bool dived = true;
  private float stuckCounter;
  [NonSerialized]
  public float damping;
  private bool soar = true;
  [NonSerialized]
  public bool landing;
  [NonSerialized]
  public float targetSpeed;
  [NonSerialized]
  public bool move = true;
  public GameObject _model;
  [NonSerialized]
  public float avoidValue;
  [NonSerialized]
  public float avoidDistance;
  [NonSerialized]
  public bool avoid = true;
  private float soarTimer;
  private bool instantiated;
  public Vector3 _landingPosOffset;
  private float time;
  private float time2;
  private FlockChildProxy proxy;

  public void Initialise(FlockController flockController)
  {
    this.flockController = flockController;
    Wander(0.0f);
    SetRandomScale();
    this.transform.localPosition = FindWaypoint();
    RandomizeStartAnimationFrame();
    InitAvoidanceValues();
    speed = flockController._minSpeed;
    instantiated = true;
  }

  private void Awake()
  {
    time = Time.time;
    time2 = Time.time;
    InstanceByRequest<UpdateService>.Instance.FlockMoveUpdater.AddUpdatable(this);
    proxy = new FlockChildProxy(this);
  }

  private void OnDestroy()
  {
    proxy.Dispose();
    proxy = null;
    InstanceByRequest<UpdateService>.Instance.FlockMoveUpdater.RemoveUpdatable(this);
  }

  public void ComputeUpdate()
  {
    float delta = Time.time - time;
    time = Time.time;
    MoveForward(delta);
    RotationBasedOnWaypoint(delta);
    LimitRotationOfModel(delta);
    SoarTimeLimit(delta);
    CheckForDistanceToWaypoint(delta);
  }

  public void ProxyUpdate()
  {
    float delta = Time.time - time2;
    time2 = Time.time;
    ComputeAvoidance(delta);
  }

  private void OnEnable()
  {
    if (!instantiated)
      return;
    if (landing)
      _model.GetComponentNonAlloc<Animation>().Play(flockController._idleAnimation);
    else
      _model.GetComponentNonAlloc<Animation>().Play(flockController._flapAnimation);
  }

  private void OnDisable() => this.CancelInvoke();

  private void RandomizeStartAnimationFrame()
  {
    foreach (AnimationState animationState in _model.GetComponentNonAlloc<Animation>())
      animationState.time = UnityEngine.Random.value * animationState.length;
  }

  private void InitAvoidanceValues()
  {
    avoidValue = UnityEngine.Random.Range(0.3f, 0.1f);
    if (flockController._birdAvoidDistanceMax != (double) flockController._birdAvoidDistanceMin)
      avoidDistance = UnityEngine.Random.Range(flockController._birdAvoidDistanceMax, flockController._birdAvoidDistanceMin);
    else
      avoidDistance = flockController._birdAvoidDistanceMin;
  }

  private void SetRandomScale()
  {
    float num = UnityEngine.Random.Range(flockController._minScale, flockController._maxScale);
    this.transform.localScale = new Vector3(num, num, num);
  }

  private void SoarTimeLimit(float delta)
  {
    if (!soar || flockController._soarMaxTime <= 0.0)
      return;
    if (soarTimer > (double) flockController._soarMaxTime)
    {
      Flap();
      soarTimer = 0.0f;
    }
    else
      soarTimer += delta;
  }

  private void CheckForDistanceToWaypoint(float delta)
  {
    if ((UnityEngine.Object) this.transform == (UnityEngine.Object) null || (UnityEngine.Object) flockController == (UnityEngine.Object) null)
      return;
    if (!landing && (double) (this.transform.localPosition - wayPoint).magnitude < flockController._waypointDistance + (double) stuckCounter)
    {
      Wander(0.0f);
      stuckCounter = 0.0f;
    }
    else if (!landing)
      stuckCounter += delta;
    else
      stuckCounter = 0.0f;
  }

  private void MoveForward(float delta)
  {
    Vector3 forward = landing ? wayPoint - this.transform.position : wayPoint - this.transform.localPosition;
    if (targetSpeed > -1.0 && forward != Vector3.zero)
      this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(forward), delta * damping);
    speed = Mathf.Lerp(speed, targetSpeed, delta * 2.5f);
    if (!move)
      return;
    this.transform.localPosition += this.transform.forward * speed * delta;
  }

  private void RotationBasedOnWaypoint(float delta)
  {
    if (!flockController._childTriggerPos || (double) (this.transform.localPosition - flockController.targetPosition).magnitude >= 1.0)
      return;
    flockController.SetFlockRandomPosition();
  }

  private void ComputeAvoidance(float delta)
  {
    if (!move || !avoid || !flockController._birdAvoid)
      return;
    Avoidance(delta);
  }

  private void Avoidance(float delta)
  {
    RaycastHit hitInfo = new RaycastHit();
    Vector3 forward = _model.transform.forward;
    Vector3 right = _model.transform.right;
    Quaternion rotation = this.transform.rotation;
    Vector3 eulerAngles = this.transform.rotation.eulerAngles;
    Vector3 localPosition = this.transform.localPosition;
    if (Physics.Raycast(this.transform.localPosition, forward + right * avoidValue, out hitInfo, avoidDistance, (int) flockController._avoidanceMask))
    {
      eulerAngles.y -= flockController._birdAvoidHorizontalForce * delta * damping;
      rotation.eulerAngles = eulerAngles;
      this.transform.rotation = rotation;
    }
    else if (Physics.Raycast(this.transform.localPosition, forward + right * -avoidValue, out hitInfo, avoidDistance, (int) flockController._avoidanceMask))
    {
      eulerAngles.y += flockController._birdAvoidHorizontalForce * delta * damping;
      rotation.eulerAngles = eulerAngles;
      this.transform.rotation = rotation;
    }
    if (flockController._birdAvoidDown && !landing && Physics.Raycast(this.transform.localPosition, -Vector3.up, out hitInfo, avoidDistance, (int) flockController._avoidanceMask))
    {
      eulerAngles.x -= flockController._birdAvoidVerticalForce * delta * damping;
      rotation.eulerAngles = eulerAngles;
      this.transform.rotation = rotation;
      localPosition.y += (float) (flockController._birdAvoidVerticalForce * (double) delta * 0.0099999997764825821);
      this.transform.localPosition = localPosition;
    }
    else
    {
      if (!flockController._birdAvoidUp || landing || !Physics.Raycast(this.transform.localPosition, Vector3.up, out hitInfo, avoidDistance, (int) flockController._avoidanceMask))
        return;
      eulerAngles.x += flockController._birdAvoidVerticalForce * delta * damping;
      rotation.eulerAngles = eulerAngles;
      this.transform.rotation = rotation;
      localPosition.y -= (float) (flockController._birdAvoidVerticalForce * (double) delta * 0.0099999997764825821);
      this.transform.localPosition = localPosition;
    }
  }

  private void LimitRotationOfModel(float delta)
  {
    Quaternion localRotation = _model.transform.localRotation;
    Vector3 eulerAngles = localRotation.eulerAngles;
    if ((soar && flockController._flatSoar || flockController._flatFly && !soar) && (double) wayPoint.y > (double) this.transform.localPosition.y || landing)
    {
      eulerAngles.x = Mathf.LerpAngle(_model.transform.localEulerAngles.x, -this.transform.localEulerAngles.x, delta * 1.75f);
      localRotation.eulerAngles = eulerAngles;
      _model.transform.localRotation = localRotation;
    }
    else
    {
      eulerAngles.x = Mathf.LerpAngle(_model.transform.localEulerAngles.x, 0.0f, delta * 1.75f);
      localRotation.eulerAngles = eulerAngles;
      _model.transform.localRotation = localRotation;
    }
  }

  public void Wander(float delay)
  {
    if (landing)
      return;
    damping = UnityEngine.Random.Range(flockController._minDamping, flockController._maxDamping);
    targetSpeed = UnityEngine.Random.Range(flockController._minSpeed, flockController._maxSpeed);
    this.Invoke("SetRandomMode", delay);
  }

  private void SetRandomMode()
  {
    this.CancelInvoke(nameof (SetRandomMode));
    if (!dived && (double) UnityEngine.Random.value < flockController._soarFrequency)
      Soar();
    else if (!dived && (double) UnityEngine.Random.value < flockController._diveFrequency)
      Dive();
    else
      Flap();
  }

  public void Flap()
  {
    if (!move)
      return;
    if ((UnityEngine.Object) _model != (UnityEngine.Object) null)
      _model.GetComponentNonAlloc<Animation>().CrossFade(flockController._flapAnimation, 0.5f);
    soar = false;
    AnimationSpeed();
    wayPoint = FindWaypoint();
    dived = false;
  }

  private Vector3 FindWaypoint()
  {
    return Vector3.zero with
    {
      x = UnityEngine.Random.Range(-flockController._spawnSphereWidth, flockController._spawnSphereWidth) + flockController.targetPosition.x,
      y = UnityEngine.Random.Range(-flockController._spawnSphereHeight, flockController._spawnSphereHeight) + flockController.targetPosition.y,
      z = UnityEngine.Random.Range(-flockController._spawnSphereDepth, flockController._spawnSphereDepth) + flockController.targetPosition.z
    };
  }

  private void Soar()
  {
    if (!move)
      return;
    _model.GetComponentNonAlloc<Animation>().CrossFade(flockController._soarAnimation, 1.5f);
    wayPoint = FindWaypoint();
    soar = true;
  }

  private void Dive()
  {
    if (flockController._soarAnimation != null)
    {
      _model.GetComponentNonAlloc<Animation>().CrossFade(flockController._soarAnimation, 1.5f);
    }
    else
    {
      foreach (AnimationState animationState in _model.GetComponentNonAlloc<Animation>())
      {
        if ((double) this.transform.localPosition.y < (double) wayPoint.y + 25.0)
          animationState.speed = 0.1f;
      }
    }
    wayPoint = FindWaypoint();
    wayPoint.y -= flockController._diveValue;
    dived = true;
  }

  private void AnimationSpeed()
  {
    foreach (AnimationState animationState in _model.GetComponentNonAlloc<Animation>())
      animationState.speed = dived || landing ? flockController._maxAnimationSpeed : UnityEngine.Random.Range(flockController._minAnimationSpeed, flockController._maxAnimationSpeed);
  }
}
