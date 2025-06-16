// Decompiled with JetBrains decompiler
// Type: FlockChild
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Source.Commons;
using System;
using UnityEngine;

#nullable disable
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
    this.Wander(0.0f);
    this.SetRandomScale();
    this.transform.localPosition = this.FindWaypoint();
    this.RandomizeStartAnimationFrame();
    this.InitAvoidanceValues();
    this.speed = flockController._minSpeed;
    this.instantiated = true;
  }

  private void Awake()
  {
    this.time = Time.time;
    this.time2 = Time.time;
    InstanceByRequest<UpdateService>.Instance.FlockMoveUpdater.AddUpdatable((IUpdatable) this);
    this.proxy = new FlockChildProxy(this);
  }

  private void OnDestroy()
  {
    this.proxy.Dispose();
    this.proxy = (FlockChildProxy) null;
    InstanceByRequest<UpdateService>.Instance.FlockMoveUpdater.RemoveUpdatable((IUpdatable) this);
  }

  public void ComputeUpdate()
  {
    float delta = Time.time - this.time;
    this.time = Time.time;
    this.MoveForward(delta);
    this.RotationBasedOnWaypoint(delta);
    this.LimitRotationOfModel(delta);
    this.SoarTimeLimit(delta);
    this.CheckForDistanceToWaypoint(delta);
  }

  public void ProxyUpdate()
  {
    float delta = Time.time - this.time2;
    this.time2 = Time.time;
    this.ComputeAvoidance(delta);
  }

  private void OnEnable()
  {
    if (!this.instantiated)
      return;
    if (this.landing)
      this._model.GetComponentNonAlloc<Animation>().Play(this.flockController._idleAnimation);
    else
      this._model.GetComponentNonAlloc<Animation>().Play(this.flockController._flapAnimation);
  }

  private void OnDisable() => this.CancelInvoke();

  private void RandomizeStartAnimationFrame()
  {
    foreach (AnimationState animationState in this._model.GetComponentNonAlloc<Animation>())
      animationState.time = UnityEngine.Random.value * animationState.length;
  }

  private void InitAvoidanceValues()
  {
    this.avoidValue = UnityEngine.Random.Range(0.3f, 0.1f);
    if ((double) this.flockController._birdAvoidDistanceMax != (double) this.flockController._birdAvoidDistanceMin)
      this.avoidDistance = UnityEngine.Random.Range(this.flockController._birdAvoidDistanceMax, this.flockController._birdAvoidDistanceMin);
    else
      this.avoidDistance = this.flockController._birdAvoidDistanceMin;
  }

  private void SetRandomScale()
  {
    float num = UnityEngine.Random.Range(this.flockController._minScale, this.flockController._maxScale);
    this.transform.localScale = new Vector3(num, num, num);
  }

  private void SoarTimeLimit(float delta)
  {
    if (!this.soar || (double) this.flockController._soarMaxTime <= 0.0)
      return;
    if ((double) this.soarTimer > (double) this.flockController._soarMaxTime)
    {
      this.Flap();
      this.soarTimer = 0.0f;
    }
    else
      this.soarTimer += delta;
  }

  private void CheckForDistanceToWaypoint(float delta)
  {
    if ((UnityEngine.Object) this.transform == (UnityEngine.Object) null || (UnityEngine.Object) this.flockController == (UnityEngine.Object) null)
      return;
    if (!this.landing && (double) (this.transform.localPosition - this.wayPoint).magnitude < (double) this.flockController._waypointDistance + (double) this.stuckCounter)
    {
      this.Wander(0.0f);
      this.stuckCounter = 0.0f;
    }
    else if (!this.landing)
      this.stuckCounter += delta;
    else
      this.stuckCounter = 0.0f;
  }

  private void MoveForward(float delta)
  {
    Vector3 forward = this.landing ? this.wayPoint - this.transform.position : this.wayPoint - this.transform.localPosition;
    if ((double) this.targetSpeed > -1.0 && forward != Vector3.zero)
      this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(forward), delta * this.damping);
    this.speed = Mathf.Lerp(this.speed, this.targetSpeed, delta * 2.5f);
    if (!this.move)
      return;
    this.transform.localPosition += this.transform.forward * this.speed * delta;
  }

  private void RotationBasedOnWaypoint(float delta)
  {
    if (!this.flockController._childTriggerPos || (double) (this.transform.localPosition - this.flockController.targetPosition).magnitude >= 1.0)
      return;
    this.flockController.SetFlockRandomPosition();
  }

  private void ComputeAvoidance(float delta)
  {
    if (!this.move || !this.avoid || !this.flockController._birdAvoid)
      return;
    this.Avoidance(delta);
  }

  private void Avoidance(float delta)
  {
    RaycastHit hitInfo = new RaycastHit();
    Vector3 forward = this._model.transform.forward;
    Vector3 right = this._model.transform.right;
    Quaternion rotation = this.transform.rotation;
    Vector3 eulerAngles = this.transform.rotation.eulerAngles;
    Vector3 localPosition = this.transform.localPosition;
    if (Physics.Raycast(this.transform.localPosition, forward + right * this.avoidValue, out hitInfo, this.avoidDistance, (int) this.flockController._avoidanceMask))
    {
      eulerAngles.y -= (float) this.flockController._birdAvoidHorizontalForce * delta * this.damping;
      rotation.eulerAngles = eulerAngles;
      this.transform.rotation = rotation;
    }
    else if (Physics.Raycast(this.transform.localPosition, forward + right * -this.avoidValue, out hitInfo, this.avoidDistance, (int) this.flockController._avoidanceMask))
    {
      eulerAngles.y += (float) this.flockController._birdAvoidHorizontalForce * delta * this.damping;
      rotation.eulerAngles = eulerAngles;
      this.transform.rotation = rotation;
    }
    if (this.flockController._birdAvoidDown && !this.landing && Physics.Raycast(this.transform.localPosition, -Vector3.up, out hitInfo, this.avoidDistance, (int) this.flockController._avoidanceMask))
    {
      eulerAngles.x -= (float) this.flockController._birdAvoidVerticalForce * delta * this.damping;
      rotation.eulerAngles = eulerAngles;
      this.transform.rotation = rotation;
      localPosition.y += (float) ((double) this.flockController._birdAvoidVerticalForce * (double) delta * 0.0099999997764825821);
      this.transform.localPosition = localPosition;
    }
    else
    {
      if (!this.flockController._birdAvoidUp || this.landing || !Physics.Raycast(this.transform.localPosition, Vector3.up, out hitInfo, this.avoidDistance, (int) this.flockController._avoidanceMask))
        return;
      eulerAngles.x += (float) this.flockController._birdAvoidVerticalForce * delta * this.damping;
      rotation.eulerAngles = eulerAngles;
      this.transform.rotation = rotation;
      localPosition.y -= (float) ((double) this.flockController._birdAvoidVerticalForce * (double) delta * 0.0099999997764825821);
      this.transform.localPosition = localPosition;
    }
  }

  private void LimitRotationOfModel(float delta)
  {
    Quaternion localRotation = this._model.transform.localRotation;
    Vector3 eulerAngles = localRotation.eulerAngles;
    if ((this.soar && this.flockController._flatSoar || this.flockController._flatFly && !this.soar) && (double) this.wayPoint.y > (double) this.transform.localPosition.y || this.landing)
    {
      eulerAngles.x = Mathf.LerpAngle(this._model.transform.localEulerAngles.x, -this.transform.localEulerAngles.x, delta * 1.75f);
      localRotation.eulerAngles = eulerAngles;
      this._model.transform.localRotation = localRotation;
    }
    else
    {
      eulerAngles.x = Mathf.LerpAngle(this._model.transform.localEulerAngles.x, 0.0f, delta * 1.75f);
      localRotation.eulerAngles = eulerAngles;
      this._model.transform.localRotation = localRotation;
    }
  }

  public void Wander(float delay)
  {
    if (this.landing)
      return;
    this.damping = UnityEngine.Random.Range(this.flockController._minDamping, this.flockController._maxDamping);
    this.targetSpeed = UnityEngine.Random.Range(this.flockController._minSpeed, this.flockController._maxSpeed);
    this.Invoke("SetRandomMode", delay);
  }

  private void SetRandomMode()
  {
    this.CancelInvoke(nameof (SetRandomMode));
    if (!this.dived && (double) UnityEngine.Random.value < (double) this.flockController._soarFrequency)
      this.Soar();
    else if (!this.dived && (double) UnityEngine.Random.value < (double) this.flockController._diveFrequency)
      this.Dive();
    else
      this.Flap();
  }

  public void Flap()
  {
    if (!this.move)
      return;
    if ((UnityEngine.Object) this._model != (UnityEngine.Object) null)
      this._model.GetComponentNonAlloc<Animation>().CrossFade(this.flockController._flapAnimation, 0.5f);
    this.soar = false;
    this.AnimationSpeed();
    this.wayPoint = this.FindWaypoint();
    this.dived = false;
  }

  private Vector3 FindWaypoint()
  {
    return Vector3.zero with
    {
      x = UnityEngine.Random.Range(-this.flockController._spawnSphereWidth, this.flockController._spawnSphereWidth) + this.flockController.targetPosition.x,
      y = UnityEngine.Random.Range(-this.flockController._spawnSphereHeight, this.flockController._spawnSphereHeight) + this.flockController.targetPosition.y,
      z = UnityEngine.Random.Range(-this.flockController._spawnSphereDepth, this.flockController._spawnSphereDepth) + this.flockController.targetPosition.z
    };
  }

  private void Soar()
  {
    if (!this.move)
      return;
    this._model.GetComponentNonAlloc<Animation>().CrossFade(this.flockController._soarAnimation, 1.5f);
    this.wayPoint = this.FindWaypoint();
    this.soar = true;
  }

  private void Dive()
  {
    if (this.flockController._soarAnimation != null)
    {
      this._model.GetComponentNonAlloc<Animation>().CrossFade(this.flockController._soarAnimation, 1.5f);
    }
    else
    {
      foreach (AnimationState animationState in this._model.GetComponentNonAlloc<Animation>())
      {
        if ((double) this.transform.localPosition.y < (double) this.wayPoint.y + 25.0)
          animationState.speed = 0.1f;
      }
    }
    this.wayPoint = this.FindWaypoint();
    this.wayPoint.y -= this.flockController._diveValue;
    this.dived = true;
  }

  private void AnimationSpeed()
  {
    foreach (AnimationState animationState in this._model.GetComponentNonAlloc<Animation>())
      animationState.speed = this.dived || this.landing ? this.flockController._maxAnimationSpeed : UnityEngine.Random.Range(this.flockController._minAnimationSpeed, this.flockController._maxAnimationSpeed);
  }
}
