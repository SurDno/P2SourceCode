using UnityEngine;

public class PlagueWebLink1 : PlagueWebLink
{
  public float FlightTime = 1f;
  public float CancelFadeTime = 0.5f;
  [Space]
  public float TailLifeTime = 1f;
  public int TailSteps = 8;
  [Space]
  public float NoiseFrequency = 1f;
  public float NoiseAmplitude = 1f;
  private PlagueWebPoint pointA;
  private PlagueWebPoint pointB;
  private PlagueWeb1 manager;
  private Vector3 noiseSalt;
  private float time;
  private Vector3[] linePositions;
  private bool isCanceled;
  private float cancelTime;
  private GradientAlphaKey[] baseAlphaKeys;
  private GradientAlphaKey[] currentAlphaKeys;
  private Vector3 p0;
  private Vector3 p1;
  private Vector3 p2;
  private Vector3 p3;

  private void ApplyAlphaKeys(GradientAlphaKey[] keys, LineRenderer lineRenderer = null)
  {
    if (lineRenderer == null)
      lineRenderer = GetComponent<LineRenderer>();
    Gradient colorGradient = lineRenderer.colorGradient;
    colorGradient.alphaKeys = keys;
    lineRenderer.colorGradient = colorGradient;
  }

  public override void BeginAnimation(
    PlagueWeb1 manager,
    PlagueWebPoint pointA,
    PlagueWebPoint pointB)
  {
    this.manager = manager;
    this.pointA = pointA;
    this.pointB = pointB;
    isCanceled = false;
    time = 0.0f;
    noiseSalt = new Vector3(Random.value * 10f, Random.value * 10f, Random.value * 10f);
    if (baseAlphaKeys != null)
      ApplyAlphaKeys(baseAlphaKeys);
    Rebuild();
    gameObject.SetActive(true);
  }

  private void CancelLink()
  {
    if (isCanceled)
      return;
    isCanceled = true;
    cancelTime = 0.0f;
  }

  public void RemoveLink()
  {
    gameObject.SetActive(false);
    manager.RemoveLink(this);
  }

  public override void OnPointDisable(PlagueWebPoint point)
  {
    if (point == pointA)
    {
      pointA = null;
      CancelLink();
    }
    if (point != pointB)
      return;
    pointB = null;
    CancelLink();
  }

  private void Rebuild()
  {
    if (linePositions == null)
      linePositions = new Vector3[TailSteps < 1 ? 2 : TailSteps + 1];
    if (pointA != null)
    {
      p0 = pointA.Position;
      p1 = new Vector3(p0.x + pointA.Directionality.x, p0.y + pointA.Directionality.y, p0.z + pointA.Directionality.z);
    }
    if (pointB != null)
    {
      p3 = pointB.Position;
      p2 = new Vector3(p3.x + pointB.Directionality.x, p3.y + pointB.Directionality.y, p3.z + pointB.Directionality.z);
    }
    for (int index = 0; index < linePositions.Length; ++index)
      linePositions[index] = CalculatePosition(Mathf.Clamp01((time - TailLifeTime / (linePositions.Length - 1) * index) / FlightTime));
    LineRenderer component = GetComponent<LineRenderer>();
    component.positionCount = linePositions.Length;
    component.SetPositions(linePositions);
    if (isCanceled)
    {
      float num = (float) (1.0 - cancelTime / (double) CancelFadeTime);
      if (baseAlphaKeys == null)
      {
        baseAlphaKeys = component.colorGradient.alphaKeys;
        currentAlphaKeys = new GradientAlphaKey[baseAlphaKeys.Length];
        for (int index = 0; index < currentAlphaKeys.Length; ++index)
          currentAlphaKeys[index].time = baseAlphaKeys[index].time;
      }
      for (int index = 0; index < currentAlphaKeys.Length; ++index)
        currentAlphaKeys[index].alpha = baseAlphaKeys[index].alpha * num;
      ApplyAlphaKeys(currentAlphaKeys, component);
    }
    else if (baseAlphaKeys != null)
      ApplyAlphaKeys(baseAlphaKeys, component);
    transform.localPosition = CalculatePosition(Mathf.Clamp01((time - TailLifeTime * 0.5f) / FlightTime));
  }

  private void Update()
  {
    time += Time.deltaTime;
    if (time >= FlightTime + (double) TailLifeTime)
    {
      RemoveLink();
    }
    else
    {
      if (!isCanceled && manager.Raycast(pointA, pointB))
        CancelLink();
      if (isCanceled)
      {
        cancelTime += Time.deltaTime;
        if (cancelTime >= (double) CancelFadeTime)
        {
          RemoveLink();
          return;
        }
      }
      Rebuild();
    }
  }

  public Vector3 CalculatePosition(float time)
  {
    float num1 = time * time;
    float num2 = 1f - time;
    float num3 = num2 * num2;
    Vector3 vector3_1 = num3 * num2 * p0 + 3f * num3 * time * p1 + 3f * num2 * num1 * p2 + num1 * time * p3;
    time = Mathf.Abs(time - 0.5f) * 2f;
    time = (float) (1.0 - time * (double) time);
    Vector3 vector3_2 = new Vector3((vector3_1.x + noiseSalt.x) * NoiseFrequency, (vector3_1.y + noiseSalt.y) * NoiseFrequency, (vector3_1.z + noiseSalt.z) * NoiseFrequency);
    vector3_2 = new Vector3((float) (Mathf.PerlinNoise(vector3_2.z, vector3_2.y) * 2.0 - 1.0), (float) (Mathf.PerlinNoise(vector3_2.x, vector3_2.z) * 2.0 - 1.0), (float) (Mathf.PerlinNoise(vector3_2.y, vector3_2.x) * 2.0 - 1.0));
    Vector3 vector3_3 = vector3_2 * (time * NoiseAmplitude);
    return new Vector3(vector3_1.x + vector3_3.x, vector3_1.y + vector3_3.y, vector3_1.z + vector3_3.z);
  }
}
