using System;
using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;

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
  private CachedTransform cachedTransform0;
  private float cachedTransform0Time;
  private CachedTransform cachedTransform1;
  private float cachedTransform1Time;

  private void Awake()
  {
    if (materialProperties == null)
    {
      materialProperties = new MaterialPropertyBlock();
      cutoffPropertyID = Shader.PropertyToID("_Cutoff");
    }
    timeService = ServiceLocator.GetService<TimeService>();
    prevTime = timeService.RealTime;
    updater = InstanceByRequest<UpdateService>.Instance.LeafUpdater;
    updater.AddUpdatable(this);
  }

  private void OnDestroy() => updater.RemoveUpdatable(this);

  private CachedTransform CalculateTransformCached(float time)
  {
    if (time > 0.0)
      time = 0.0f;
    if ((UnityEngine.Object) manager == (UnityEngine.Object) null || manager.minSimulationTime <= 0.0)
      return CalculateTransform(time);
    if (time > (double) cachedTransform1Time)
    {
      cachedTransform0 = cachedTransform1;
      cachedTransform0Time = cachedTransform1Time;
      cachedTransform1Time = time + manager.minSimulationTime;
      if (cachedTransform1Time > 0.0)
        cachedTransform1Time = 0.0f;
      cachedTransform1 = CalculateTransform(cachedTransform1Time);
    }
    return CachedTransform.LerpUnclamped(cachedTransform0, cachedTransform1, (float) ((time - (double) cachedTransform0Time) / (cachedTransform1Time - (double) cachedTransform0Time)));
  }

  private CachedTransform CalculateTransform(float time)
  {
    float t = 0.0f;
    if (landing && -(double) time < landingBlend)
    {
      float num = (float) (1.0 - -(double) time / landingBlend);
      t = num * num;
    }
    Point point = GetPoint(time);
    Vector3 position = point.position;
    Quaternion a = Quaternion.LookRotation(point.normal);
    if (t > 0.0)
    {
      position *= 1f - t;
      Quaternion b = Quaternion.LookRotation(landingNormal, point.normal) * Quaternion.Euler(270f, 180f, 0.0f);
      a = Quaternion.Lerp(a, b, t);
    }
    return new CachedTransform {
      position = position,
      rotation = a
    };
  }

  private Point GetPoint(float time)
  {
    Point point = new Point();
    float f1 = (float) (time / (double) firstPeriod * 3.1415927410125732 * 2.0 + Math.PI / 180.0 * rotation);
    Vector3 vector3_1 = new Vector3(Mathf.Sin(f1), 0.0f, Mathf.Cos(f1));
    Vector3 vector3_2 = new Vector3(vector3_1.z, 0.0f, -vector3_1.x);
    float f2 = (float) ((time / (double) secondPeriod + 0.5) * 3.1415927410125732 * 2.0);
    Vector3 vector3_3 = new Vector3(Mathf.Sin(f2), 0.0f, Mathf.Cos(f2));
    vector3_3.y = vector3_3.z * slope;
    Vector3 vector3_4 = new Vector3 {
      x = (float) ((double) vector3_2.x * (double) vector3_3.x + (double) vector3_1.x * (double) vector3_3.z),
      y = (float) ((double) vector3_2.y * (double) vector3_3.x + (double) vector3_3.y + (double) vector3_1.y * (double) vector3_3.z),
      z = (float) ((double) vector3_2.z * (double) vector3_3.x + (double) vector3_1.z * (double) vector3_3.z)
    };
    point.position = new Vector3 {
      x = (float) ((double) velocity.x * time + (double) vector3_1.x * firstRadius + (double) vector3_4.x * secondRadius),
      y = (float) ((double) velocity.y * time + (double) vector3_1.y * firstRadius + (slope + (double) vector3_4.y) * secondRadius),
      z = (float) ((double) velocity.z * time + (double) vector3_1.z * firstRadius + (double) vector3_4.z * secondRadius)
    };
    point.normal = vector3_4;
    return point;
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = !landing ? new Color(1f, 0.75f, 0.0f) : new Color(0.0f, 1f, 0.0f);
    int num1 = Mathf.RoundToInt(flyTime * 4f);
    float num2 = flyTime / num1;
    Matrix4x4 localToWorldMatrix = this.transform.localToWorldMatrix;
    Vector3 from = localToWorldMatrix.MultiplyPoint(GetPoint(0.0f).position);
    for (int index = 1; index < num1 + 1; ++index)
    {
      Vector3 to = localToWorldMatrix.MultiplyPoint(GetPoint(-num2 * index).position);
      Gizmos.DrawLine(from, to);
      from = to;
    }
  }

  private void Remove()
  {
    if ((UnityEngine.Object) manager != (UnityEngine.Object) null)
      manager.ReturnAnimator(this);
    else
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
  }

  private void SetOpacity(float opacity)
  {
    if (opacity == 1.0)
    {
      leafRenderer.SetPropertyBlock((MaterialPropertyBlock) null);
    }
    else
    {
      materialProperties.SetFloat(cutoffPropertyID, (float) (1.0 - opacity * 0.5));
      leafRenderer.SetPropertyBlock(materialProperties);
    }
  }

  public void SetToStart()
  {
    cachedTransform0Time = float.MinValue;
    cachedTransform1Time = float.MinValue;
    cachedTransform0 = new CachedTransform {
      rotation = Quaternion.identity
    };
    cachedTransform1 = cachedTransform0;
    _phase = -flyTime;
    _isOpaque = false;
    SetOpacity(0.0f);
  }

  private void UpdateTransform()
  {
    CachedTransform transformCached = CalculateTransformCached(_phase);
    leafTransform.localPosition = transformCached.position;
    leafTransform.localRotation = transformCached.rotation;
  }

  void IUpdatable.ComputeUpdate()
  {
    if (!this.isActiveAndEnabled)
      return;
    TimeSpan timeSpan = timeService.RealTime - prevTime;
    prevTime = timeService.RealTime;
    bool flag = _phase < 0.0;
    _phase += (float) timeSpan.TotalSeconds;
    if (flag)
      UpdateTransform();
    if (landing)
    {
      if (_phase >= (double) layTime)
      {
        Remove();
        return;
      }
    }
    else if (_phase >= 0.0)
    {
      Remove();
      return;
    }
    float num = 1f * Mathf.Clamp01((flyTime + _phase) / flyFade);
    float opacity = !landing ? num * Mathf.Clamp01(-_phase / flyFade) : num * Mathf.Clamp01((layTime - _phase) / layFade);
    if (opacity >= 0.99000000953674316)
    {
      if (_isOpaque)
        return;
      SetOpacity(1f);
      _isOpaque = true;
    }
    else
    {
      SetOpacity(opacity);
      _isOpaque = false;
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

    public static CachedTransform LerpUnclamped(
      CachedTransform a,
      CachedTransform b,
      float time)
    {
      return new CachedTransform {
        position = Vector3.LerpUnclamped(a.position, b.position, time),
        rotation = Quaternion.LerpUnclamped(a.rotation, b.rotation, time)
      };
    }
  }
}
