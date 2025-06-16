// Decompiled with JetBrains decompiler
// Type: SplineBendMarker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class SplineBendMarker : MonoBehaviour
{
  public SplineBend splineScript;
  [HideInInspector]
  public int num;
  public SplineBendMarker.MarkerType type;
  [HideInInspector]
  public Vector3 position;
  [HideInInspector]
  public Vector3 up;
  [HideInInspector]
  public Vector3 prewHandle;
  [HideInInspector]
  public Vector3 nextHandle;
  [HideInInspector]
  public float dist;
  [HideInInspector]
  public float percent;
  [HideInInspector]
  public Vector3[] subPoints = new Vector3[10];
  [HideInInspector]
  public float[] subPointPercents = new float[10];
  [HideInInspector]
  public float[] subPointFactors = new float[10];
  [HideInInspector]
  public float[] subPointMustPercents = new float[10];
  public bool expandWithScale;
  [HideInInspector]
  public Vector3 oldPos;
  [HideInInspector]
  public Vector3 oldScale;
  [HideInInspector]
  public Quaternion oldRot;

  public void Init(SplineBend script, int mnum)
  {
    this.splineScript = script;
    this.num = mnum;
    this.up = this.transform.up;
    this.position = script.transform.InverseTransformPoint(this.transform.position);
    SplineBendMarker marker2 = (SplineBendMarker) null;
    SplineBendMarker marker1 = (SplineBendMarker) null;
    if (this.num > 0)
      marker1 = this.splineScript.markers[this.num - 1];
    if (this.num < this.splineScript.markers.Length - 1)
      marker2 = this.splineScript.markers[this.num + 1];
    this.dist = !(bool) (Object) marker1 ? 0.0f : marker1.dist + SplineBend.GetBeizerLength(marker1, this);
    if (this.splineScript.closed && this.num == this.splineScript.markers.Length - 1)
    {
      SplineBendMarker marker = this.splineScript.markers[this.splineScript.markers.Length - 2];
      marker2 = this.splineScript.markers[1];
    }
    if ((bool) (Object) marker2)
    {
      if (this.subPoints == null)
        this.subPoints = new Vector3[10];
      float num1 = 1f / (float) (this.subPoints.Length - 1);
      for (int index = 0; index < this.subPoints.Length; ++index)
        this.subPoints[index] = SplineBend.AlignPoint(this, marker2, num1 * (float) index, new Vector3(0.0f, 0.0f, 0.0f));
      float num2 = 0.0f;
      this.subPointPercents[0] = 0.0f;
      float num3 = 1f / (float) (this.subPoints.Length - 1);
      for (int index = 1; index < this.subPoints.Length; ++index)
      {
        this.subPointPercents[index] = num2 + (this.subPoints[index - 1] - this.subPoints[index]).magnitude;
        num2 = this.subPointPercents[index];
        this.subPointMustPercents[index] = num3 * (float) index;
      }
      for (int index = 1; index < this.subPoints.Length; ++index)
        this.subPointPercents[index] = this.subPointPercents[index] / num2;
      for (int index = 0; index < this.subPoints.Length - 1; ++index)
        this.subPointFactors[index] = num3 / (this.subPointPercents[index + 1] - this.subPointPercents[index]);
    }
    Vector3 vector3_1 = new Vector3(0.0f, 0.0f, 0.0f);
    if ((bool) (Object) marker2)
      vector3_1 = script.transform.InverseTransformPoint(marker2.transform.position);
    Vector3 vector3_2;
    switch (this.type)
    {
      case SplineBendMarker.MarkerType.Smooth:
        if (!(bool) (Object) marker2)
        {
          this.prewHandle = (marker1.position - this.position) * 0.333f;
          this.nextHandle = -this.prewHandle * 0.99f;
          break;
        }
        if (!(bool) (Object) marker1)
        {
          this.nextHandle = (vector3_1 - this.position) * 0.333f;
          this.prewHandle = -this.nextHandle * 0.99f;
          break;
        }
        this.nextHandle = Vector3.Slerp(-(marker1.position - this.position) * 0.333f, (vector3_1 - this.position) * 0.333f, 0.5f);
        this.prewHandle = Vector3.Slerp((marker1.position - this.position) * 0.333f, -(vector3_1 - this.position) * 0.333f, 0.5f);
        break;
      case SplineBendMarker.MarkerType.Transform:
        if ((bool) (Object) marker1)
          this.prewHandle = -this.transform.forward * this.transform.localScale.z * (this.position - marker1.position).magnitude * 0.4f;
        if ((bool) (Object) marker2)
        {
          vector3_2 = this.position - vector3_1;
          this.nextHandle = this.transform.forward * this.transform.localScale.z * vector3_2.magnitude * 0.4f;
          break;
        }
        break;
      case SplineBendMarker.MarkerType.Corner:
        this.prewHandle = !(bool) (Object) marker1 ? new Vector3(0.0f, 0.0f, 0.0f) : (marker1.position - this.position) * 0.333f;
        this.nextHandle = !(bool) (Object) marker2 ? new Vector3(0.0f, 0.0f, 0.0f) : (vector3_1 - this.position) * 0.333f;
        break;
    }
    vector3_2 = this.nextHandle - this.prewHandle;
    if ((double) vector3_2.sqrMagnitude >= 0.0099999997764825821)
      return;
    this.nextHandle += new Vector3(1f / 1000f, 0.0f, 0.0f);
  }

  public enum MarkerType
  {
    Smooth,
    Transform,
    Beizer,
    BeizerCorner,
    Corner,
  }
}
