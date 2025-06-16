// Decompiled with JetBrains decompiler
// Type: PlagueWebLink1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
    if ((Object) lineRenderer == (Object) null)
      lineRenderer = this.GetComponent<LineRenderer>();
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
    this.isCanceled = false;
    this.time = 0.0f;
    this.noiseSalt = new Vector3(Random.value * 10f, Random.value * 10f, Random.value * 10f);
    if (this.baseAlphaKeys != null)
      this.ApplyAlphaKeys(this.baseAlphaKeys);
    this.Rebuild();
    this.gameObject.SetActive(true);
  }

  private void CancelLink()
  {
    if (this.isCanceled)
      return;
    this.isCanceled = true;
    this.cancelTime = 0.0f;
  }

  public void RemoveLink()
  {
    this.gameObject.SetActive(false);
    this.manager.RemoveLink((PlagueWebLink) this);
  }

  public override void OnPointDisable(PlagueWebPoint point)
  {
    if (point == this.pointA)
    {
      this.pointA = (PlagueWebPoint) null;
      this.CancelLink();
    }
    if (point != this.pointB)
      return;
    this.pointB = (PlagueWebPoint) null;
    this.CancelLink();
  }

  private void Rebuild()
  {
    if (this.linePositions == null)
      this.linePositions = new Vector3[this.TailSteps < 1 ? 2 : this.TailSteps + 1];
    if (this.pointA != null)
    {
      this.p0 = this.pointA.Position;
      this.p1 = new Vector3(this.p0.x + this.pointA.Directionality.x, this.p0.y + this.pointA.Directionality.y, this.p0.z + this.pointA.Directionality.z);
    }
    if (this.pointB != null)
    {
      this.p3 = this.pointB.Position;
      this.p2 = new Vector3(this.p3.x + this.pointB.Directionality.x, this.p3.y + this.pointB.Directionality.y, this.p3.z + this.pointB.Directionality.z);
    }
    for (int index = 0; index < this.linePositions.Length; ++index)
      this.linePositions[index] = this.CalculatePosition(Mathf.Clamp01((this.time - this.TailLifeTime / (float) (this.linePositions.Length - 1) * (float) index) / this.FlightTime));
    LineRenderer component = this.GetComponent<LineRenderer>();
    component.positionCount = this.linePositions.Length;
    component.SetPositions(this.linePositions);
    if (this.isCanceled)
    {
      float num = (float) (1.0 - (double) this.cancelTime / (double) this.CancelFadeTime);
      if (this.baseAlphaKeys == null)
      {
        this.baseAlphaKeys = component.colorGradient.alphaKeys;
        this.currentAlphaKeys = new GradientAlphaKey[this.baseAlphaKeys.Length];
        for (int index = 0; index < this.currentAlphaKeys.Length; ++index)
          this.currentAlphaKeys[index].time = this.baseAlphaKeys[index].time;
      }
      for (int index = 0; index < this.currentAlphaKeys.Length; ++index)
        this.currentAlphaKeys[index].alpha = this.baseAlphaKeys[index].alpha * num;
      this.ApplyAlphaKeys(this.currentAlphaKeys, component);
    }
    else if (this.baseAlphaKeys != null)
      this.ApplyAlphaKeys(this.baseAlphaKeys, component);
    this.transform.localPosition = this.CalculatePosition(Mathf.Clamp01((this.time - this.TailLifeTime * 0.5f) / this.FlightTime));
  }

  private void Update()
  {
    this.time += Time.deltaTime;
    if ((double) this.time >= (double) this.FlightTime + (double) this.TailLifeTime)
    {
      this.RemoveLink();
    }
    else
    {
      if (!this.isCanceled && this.manager.Raycast(this.pointA, this.pointB))
        this.CancelLink();
      if (this.isCanceled)
      {
        this.cancelTime += Time.deltaTime;
        if ((double) this.cancelTime >= (double) this.CancelFadeTime)
        {
          this.RemoveLink();
          return;
        }
      }
      this.Rebuild();
    }
  }

  public Vector3 CalculatePosition(float time)
  {
    float num1 = time * time;
    float num2 = 1f - time;
    float num3 = num2 * num2;
    Vector3 vector3_1 = num3 * num2 * this.p0 + 3f * num3 * time * this.p1 + 3f * num2 * num1 * this.p2 + num1 * time * this.p3;
    time = Mathf.Abs(time - 0.5f) * 2f;
    time = (float) (1.0 - (double) time * (double) time);
    Vector3 vector3_2 = new Vector3((vector3_1.x + this.noiseSalt.x) * this.NoiseFrequency, (vector3_1.y + this.noiseSalt.y) * this.NoiseFrequency, (vector3_1.z + this.noiseSalt.z) * this.NoiseFrequency);
    vector3_2 = new Vector3((float) ((double) Mathf.PerlinNoise(vector3_2.z, vector3_2.y) * 2.0 - 1.0), (float) ((double) Mathf.PerlinNoise(vector3_2.x, vector3_2.z) * 2.0 - 1.0), (float) ((double) Mathf.PerlinNoise(vector3_2.y, vector3_2.x) * 2.0 - 1.0));
    Vector3 vector3_3 = vector3_2 * (time * this.NoiseAmplitude);
    return new Vector3(vector3_1.x + vector3_3.x, vector3_1.y + vector3_3.y, vector3_1.z + vector3_3.z);
  }
}
