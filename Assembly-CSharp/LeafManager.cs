using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Settings.External;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LeafManager : MonoBehaviourInstance<LeafManager>, IUpdatable
{
  [Tooltip("Основной префаб для создания траекторий")]
  public GameObject animatorPrefab;
  [Tooltip("Настройки новых листьев будут выбраны случайно в диапазоне между префабом и этим состоянием")]
  public LeafAnimator alternativeSettings;
  [Tooltip("Отсюда будет случайным образом выбираться листья")]
  public GameObject[] leafPrefabs;
  public int poolCapacity = 64;
  public LayerMask collideLayers = (LayerMask) (int) byte.MaxValue;
  [Space]
  public float radius = 30f;
  [Tooltip("Высота над playerPosition, с которой начинается поиск столкновений. Должно быть выше крыш")]
  public float raycastOriginElevation = 50f;
  [Space]
  [Tooltip("Если Y нормали в точке столкновения ниже, лист растворится без приземления")]
  public float minLandingNormalY = 0.75f;
  [Space]
  public Vector2 wind;
  public float deviation;
  [Space]
  public Vector3 playerPosition;
  public float rate = 1f;
  [Space]
  public float minSimulationTime = 0.25f;
  private Stack<LeafAnimator> _pool;
  private float _spawnPhase = 1f;
  private const float VelocitySampleLength = 2f;
  private const float MaxVelocity = 10f;
  private float _velocitySampleTime = -1f;
  private Vector3 _velocitySamplePos;
  private Vector3 _halfPlayerVelocity;
  private TimeSpan prevTime;
  private TimeService timeService;
  private IUpdater updater;

  public void ReturnAnimator(LeafAnimator leafAnimator)
  {
    if (this._pool.Count < this.poolCapacity)
    {
      leafAnimator.gameObject.SetActive(false);
      this._pool.Push(leafAnimator);
    }
    else
      UnityEngine.Object.Destroy((UnityEngine.Object) leafAnimator.gameObject);
  }

  private void Spawn()
  {
    LeafAnimator leafAnimator = (LeafAnimator) null;
    do
    {
      if (this._pool.Count > 0)
        leafAnimator = this._pool.Pop();
      else
        goto label_4;
    }
    while ((UnityEngine.Object) leafAnimator == (UnityEngine.Object) null);
    leafAnimator.gameObject.SetActive(true);
label_4:
    if ((UnityEngine.Object) leafAnimator == (UnityEngine.Object) null)
    {
      GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(this.animatorPrefab);
      gameObject1.transform.SetParent(this.transform, false);
      leafAnimator = gameObject1.GetComponent<LeafAnimator>();
      leafAnimator.manager = this;
      GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.leafPrefabs[UnityEngine.Random.Range(0, this.leafPrefabs.Length)]);
      gameObject2.transform.SetParent(leafAnimator.transform, false);
      leafAnimator.leafTransform = gameObject2.transform;
      leafAnimator.leafRenderer = gameObject2.GetComponent<Renderer>();
    }
    float t = UnityEngine.Random.value;
    leafAnimator.flyFade = Mathf.Lerp(leafAnimator.flyFade, this.alternativeSettings.flyFade, t);
    leafAnimator.layFade = Mathf.Lerp(leafAnimator.layFade, this.alternativeSettings.layFade, t);
    leafAnimator.firstPeriod = Mathf.Lerp(leafAnimator.firstPeriod, this.alternativeSettings.firstPeriod, t);
    leafAnimator.firstRadius = Mathf.Lerp(leafAnimator.firstRadius, this.alternativeSettings.firstRadius, t);
    leafAnimator.flyTime = Mathf.Lerp(leafAnimator.flyTime, this.alternativeSettings.flyTime, t);
    leafAnimator.landingBlend = Mathf.Lerp(leafAnimator.landingBlend, this.alternativeSettings.landingBlend, t);
    leafAnimator.layTime = Mathf.Lerp(leafAnimator.layTime, this.alternativeSettings.layTime, t);
    leafAnimator.secondPeriod = Mathf.Lerp(leafAnimator.secondPeriod, this.alternativeSettings.secondPeriod, t);
    leafAnimator.secondRadius = Mathf.Lerp(leafAnimator.secondRadius, this.alternativeSettings.secondRadius, t);
    leafAnimator.slope = Mathf.Lerp(leafAnimator.slope, this.alternativeSettings.slope, t);
    leafAnimator.velocity = Vector3.Lerp(leafAnimator.velocity, this.alternativeSettings.velocity, t);
    leafAnimator.rotation = UnityEngine.Random.Range(0.0f, 360f);
    Vector3 vector3_1 = this.playerPosition + this._halfPlayerVelocity * leafAnimator.flyTime;
    Vector2 vector2_1 = UnityEngine.Random.insideUnitCircle * this.radius;
    Vector3 vector3_2 = new Vector3(vector3_1.x + vector2_1.x, this.playerPosition.y, vector3_1.z + vector2_1.y);
    Vector2 vector2_2 = UnityEngine.Random.insideUnitCircle * this.deviation + this.wind;
    Vector3 direction = new Vector3(vector2_2.x, -1f, vector2_2.y);
    float radius = leafAnimator.firstRadius + leafAnimator.secondRadius;
    Vector3 origin1 = vector3_2 - direction * this.raycastOriginElevation;
    RaycastHit hitInfo;
    Vector3 vector3_3;
    if (Physics.SphereCast(origin1, radius, direction, out hitInfo, (float) ((double) this.raycastOriginElevation * (double) direction.magnitude * 2.0), (int) this.collideLayers, QueryTriggerInteraction.Ignore))
    {
      Vector3 origin2 = origin1 + direction.normalized * hitInfo.distance;
      if (Physics.Raycast(origin2, direction, out hitInfo, (float) ((double) radius * (double) direction.magnitude * 1.5), (int) this.collideLayers, QueryTriggerInteraction.Ignore))
      {
        if ((double) hitInfo.normal.y > (double) this.minLandingNormalY)
        {
          leafAnimator.landing = true;
          leafAnimator.landingNormal = hitInfo.normal;
        }
        else
          leafAnimator.landing = false;
        vector3_3 = hitInfo.point;
      }
      else
      {
        leafAnimator.landing = false;
        vector3_3 = origin2 + radius * direction.normalized;
      }
    }
    else
    {
      vector3_3 = vector3_2 + leafAnimator.velocity * leafAnimator.flyTime * 0.5f;
      leafAnimator.landing = false;
    }
    leafAnimator.transform.position = vector3_3;
    leafAnimator.velocity = -direction * leafAnimator.velocity.y;
    leafAnimator.SetToStart();
  }

  private void SampleVelocity()
  {
    if ((double) this._velocitySampleTime == -1.0)
    {
      this._velocitySampleTime = Time.time;
      this._velocitySamplePos = this.playerPosition;
      this._halfPlayerVelocity = Vector3.zero;
    }
    else
    {
      float num = Time.time - this._velocitySampleTime;
      if ((double) num >= 2.0)
      {
        this._halfPlayerVelocity = (this.playerPosition - this._velocitySamplePos) / num;
        float magnitude = this._halfPlayerVelocity.magnitude;
        if ((double) magnitude > 10.0)
          this._halfPlayerVelocity *= (float) (10.0 / (double) magnitude * 0.5);
        this._velocitySamplePos = this.playerPosition;
        this._velocitySampleTime = Time.time;
      }
    }
  }

  private void Start()
  {
    this._pool = new Stack<LeafAnimator>(this.poolCapacity);
    this.timeService = ServiceLocator.GetService<TimeService>();
    this.prevTime = this.timeService.RealTime;
    this.updater = InstanceByRequest<UpdateService>.Instance.LeafSpawner;
    this.updater.AddUpdatable((IUpdatable) this);
  }

  private void OnDestroy() => this.updater.RemoveUpdatable((IUpdatable) this);

  void IUpdatable.ComputeUpdate()
  {
    if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DisableLeaf || !this.gameObject.activeInHierarchy)
      return;
    TimeSpan timeSpan = this.timeService.RealTime - this.prevTime;
    this.prevTime = this.timeService.RealTime;
    this.SampleVelocity();
    for (this._spawnPhase += (float) timeSpan.TotalSeconds * this.rate; (double) this._spawnPhase >= 1.0; --this._spawnPhase)
      this.Spawn();
  }
}
