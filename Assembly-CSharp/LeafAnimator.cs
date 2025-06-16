using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using System;
using UnityEngine;

public class LeafAnimator : MonoBehaviour, IUpdatable
{
  private static MaterialPropertyBlock materialProperties;
  private static int cutoffPropertyID;
  public LeafManager manager;
  public float firstRadius = 1f;
  public float firstPeriod = 5f;
  [Range(0.0f, 360f)]
  public float rotation = 0.0f;
  [Space]
  public float secondRadius = 1f;
  public float secondPeriod = -2.15f;
  public float slope = 0.25f;
  [Space]
  public Vector3 velocity = Vector3.down;
  [Space]
  public float flyTime = 60f;
  public float layTime = 30f;
  public float flyFade = 5f;
  public float layFade = 15f;
  [Space]
  [Tooltip("False — лист растворяется вместо приземления")]
  public bool landing = true;
  [Tooltip("Длительность перехода меджу вращением полета и вращением покоя")]
  public float landingBlend = 0.25f;
  public Vector3 landingNormal = Vector3.up;
  [Space]
  public Transform leafTransform;
  public Renderer leafRenderer;
  private float _phase;
  private bool _isOpaque;
  private TimeSpan prevTime;
  private TimeService timeService;
  private IUpdater updater;
  private LeafAnimator.CachedTransform cachedTransform0;
  private float cachedTransform0Time;
  private LeafAnimator.CachedTransform cachedTransform1;
  private float cachedTransform1Time;

  private void Awake()
  {
    if (LeafAnimator.materialProperties == null)
    {
      LeafAnimator.materialProperties = new MaterialPropertyBlock();
      LeafAnimator.cutoffPropertyID = Shader.PropertyToID("_Cutoff");
    }
    this.timeService = ServiceLocator.GetService<TimeService>();
    this.prevTime = this.timeService.RealTime;
    this.updater = InstanceByRequest<UpdateService>.Instance.LeafUpdater;
    this.updater.AddUpdatable((IUpdatable) this);
  }

  private void OnDestroy() => this.updater.RemoveUpdatable((IUpdatable) this);

  private LeafAnimator.CachedTransform CalculateTransformCached(float time)
  {
    if ((double) time > 0.0)
      time = 0.0f;
    if ((UnityEngine.Object) this.manager == (UnityEngine.Object) null || (double) this.manager.minSimulationTime <= 0.0)
      return this.CalculateTransform(time);
    if ((double) time > (double) this.cachedTransform1Time)
    {
      this.cachedTransform0 = this.cachedTransform1;
      this.cachedTransform0Time = this.cachedTransform1Time;
      this.cachedTransform1Time = time + this.manager.minSimulationTime;
      if ((double) this.cachedTransform1Time > 0.0)
        this.cachedTransform1Time = 0.0f;
      this.cachedTransform1 = this.CalculateTransform(this.cachedTransform1Time);
    }
    return LeafAnimator.CachedTransform.LerpUnclamped(this.cachedTransform0, this.cachedTransform1, (float) (((double) time - (double) this.cachedTransform0Time) / ((double) this.cachedTransform1Time - (double) this.cachedTransform0Time)));
  }

  private LeafAnimator.CachedTransform CalculateTransform(float time)
  {
    float t = 0.0f;
    if (this.landing && -(double) time < (double) this.landingBlend)
    {
      float num = (float) (1.0 - -(double) time / (double) this.landingBlend);
      t = num * num;
    }
    LeafAnimator.Point point = this.GetPoint(time);
    Vector3 position = point.position;
    Quaternion a = Quaternion.LookRotation(point.normal);
    if ((double) t > 0.0)
    {
      position *= 1f - t;
      Quaternion b = Quaternion.LookRotation(this.landingNormal, point.normal) * Quaternion.Euler(270f, 180f, 0.0f);
      a = Quaternion.Lerp(a, b, t);
    }
    return new LeafAnimator.CachedTransform()
    {
      position = position,
      rotation = a
    };
  }

  private LeafAnimator.Point GetPoint(float time)
  {
    LeafAnimator.Point point = new LeafAnimator.Point();
    float f1 = (float) ((double) time / (double) this.firstPeriod * 3.1415927410125732 * 2.0 + Math.PI / 180.0 * (double) this.rotation);
    Vector3 vector3_1 = new Vector3(Mathf.Sin(f1), 0.0f, Mathf.Cos(f1));
    Vector3 vector3_2 = new Vector3(vector3_1.z, 0.0f, -vector3_1.x);
    float f2 = (float) (((double) time / (double) this.secondPeriod + 0.5) * 3.1415927410125732 * 2.0);
    Vector3 vector3_3 = new Vector3(Mathf.Sin(f2), 0.0f, Mathf.Cos(f2));
    vector3_3.y = vector3_3.z * this.slope;
    Vector3 vector3_4 = new Vector3()
    {
      x = (float) ((double) vector3_2.x * (double) vector3_3.x + (double) vector3_1.x * (double) vector3_3.z),
      y = (float) ((double) vector3_2.y * (double) vector3_3.x + (double) vector3_3.y + (double) vector3_1.y * (double) vector3_3.z),
      z = (float) ((double) vector3_2.z * (double) vector3_3.x + (double) vector3_1.z * (double) vector3_3.z)
    };
    point.position = new Vector3()
    {
      x = (float) ((double) this.velocity.x * (double) time + (double) vector3_1.x * (double) this.firstRadius + (double) vector3_4.x * (double) this.secondRadius),
      y = (float) ((double) this.velocity.y * (double) time + (double) vector3_1.y * (double) this.firstRadius + ((double) this.slope + (double) vector3_4.y) * (double) this.secondRadius),
      z = (float) ((double) this.velocity.z * (double) time + (double) vector3_1.z * (double) this.firstRadius + (double) vector3_4.z * (double) this.secondRadius)
    };
    point.normal = vector3_4;
    return point;
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = !this.landing ? new Color(1f, 0.75f, 0.0f) : new Color(0.0f, 1f, 0.0f);
    int num1 = Mathf.RoundToInt(this.flyTime * 4f);
    float num2 = this.flyTime / (float) num1;
    Matrix4x4 localToWorldMatrix = this.transform.localToWorldMatrix;
    Vector3 from = localToWorldMatrix.MultiplyPoint(this.GetPoint(0.0f).position);
    for (int index = 1; index < num1 + 1; ++index)
    {
      Vector3 to = localToWorldMatrix.MultiplyPoint(this.GetPoint(-num2 * (float) index).position);
      Gizmos.DrawLine(from, to);
      from = to;
    }
  }

  private void Remove()
  {
    if ((UnityEngine.Object) this.manager != (UnityEngine.Object) null)
      this.manager.ReturnAnimator(this);
    else
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
  }

  private void SetOpacity(float opacity)
  {
    if ((double) opacity == 1.0)
    {
      this.leafRenderer.SetPropertyBlock((MaterialPropertyBlock) null);
    }
    else
    {
      LeafAnimator.materialProperties.SetFloat(LeafAnimator.cutoffPropertyID, (float) (1.0 - (double) opacity * 0.5));
      this.leafRenderer.SetPropertyBlock(LeafAnimator.materialProperties);
    }
  }

  public void SetToStart()
  {
    this.cachedTransform0Time = float.MinValue;
    this.cachedTransform1Time = float.MinValue;
    this.cachedTransform0 = new LeafAnimator.CachedTransform()
    {
      rotation = Quaternion.identity
    };
    this.cachedTransform1 = this.cachedTransform0;
    this._phase = -this.flyTime;
    this._isOpaque = false;
    this.SetOpacity(0.0f);
  }

  private void UpdateTransform()
  {
    LeafAnimator.CachedTransform transformCached = this.CalculateTransformCached(this._phase);
    this.leafTransform.localPosition = transformCached.position;
    this.leafTransform.localRotation = transformCached.rotation;
  }

  void IUpdatable.ComputeUpdate()
  {
    if (!this.isActiveAndEnabled)
      return;
    TimeSpan timeSpan = this.timeService.RealTime - this.prevTime;
    this.prevTime = this.timeService.RealTime;
    bool flag = (double) this._phase < 0.0;
    this._phase += (float) timeSpan.TotalSeconds;
    if (flag)
      this.UpdateTransform();
    if (this.landing)
    {
      if ((double) this._phase >= (double) this.layTime)
      {
        this.Remove();
        return;
      }
    }
    else if ((double) this._phase >= 0.0)
    {
      this.Remove();
      return;
    }
    float num = 1f * Mathf.Clamp01((this.flyTime + this._phase) / this.flyFade);
    float opacity = !this.landing ? num * Mathf.Clamp01(-this._phase / this.flyFade) : num * Mathf.Clamp01((this.layTime - this._phase) / this.layFade);
    if ((double) opacity >= 0.99000000953674316)
    {
      if (this._isOpaque)
        return;
      this.SetOpacity(1f);
      this._isOpaque = true;
    }
    else
    {
      this.SetOpacity(opacity);
      this._isOpaque = false;
    }
  }

  private struct Point
  {
    public Vector3 position;
    public Vector3 normal;
  }

  private struct CachedTransform
  {
    public Vector3 position;
    public Quaternion rotation;

    public static LeafAnimator.CachedTransform LerpUnclamped(
      LeafAnimator.CachedTransform a,
      LeafAnimator.CachedTransform b,
      float time)
    {
      return new LeafAnimator.CachedTransform()
      {
        position = Vector3.LerpUnclamped(a.position, b.position, time),
        rotation = Quaternion.LerpUnclamped(a.rotation, b.rotation, time)
      };
    }
  }
}
