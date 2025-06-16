using UnityEngine;

public class SplineBendMarker : MonoBehaviour
{
  public SplineBend splineScript;
  [HideInInspector]
  public int num;
  public MarkerType type;
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
    splineScript = script;
    num = mnum;
    up = transform.up;
    position = script.transform.InverseTransformPoint(transform.position);
    SplineBendMarker marker2 = null;
    SplineBendMarker marker1 = null;
    if (num > 0)
      marker1 = splineScript.markers[num - 1];
    if (num < splineScript.markers.Length - 1)
      marker2 = splineScript.markers[num + 1];
    dist = !(bool) (Object) marker1 ? 0.0f : marker1.dist + SplineBend.GetBeizerLength(marker1, this);
    if (splineScript.closed && num == splineScript.markers.Length - 1)
    {
      SplineBendMarker marker = splineScript.markers[splineScript.markers.Length - 2];
      marker2 = splineScript.markers[1];
    }
    if ((bool) (Object) marker2)
    {
      if (subPoints == null)
        subPoints = new Vector3[10];
      float num1 = 1f / (subPoints.Length - 1);
      for (int index = 0; index < subPoints.Length; ++index)
        subPoints[index] = SplineBend.AlignPoint(this, marker2, num1 * index, new Vector3(0.0f, 0.0f, 0.0f));
      float num2 = 0.0f;
      subPointPercents[0] = 0.0f;
      float num3 = 1f / (subPoints.Length - 1);
      for (int index = 1; index < subPoints.Length; ++index)
      {
        subPointPercents[index] = num2 + (subPoints[index - 1] - subPoints[index]).magnitude;
        num2 = subPointPercents[index];
        subPointMustPercents[index] = num3 * index;
      }
      for (int index = 1; index < subPoints.Length; ++index)
        subPointPercents[index] = subPointPercents[index] / num2;
      for (int index = 0; index < subPoints.Length - 1; ++index)
        subPointFactors[index] = num3 / (subPointPercents[index + 1] - subPointPercents[index]);
    }
    Vector3 vector3_1 = new Vector3(0.0f, 0.0f, 0.0f);
    if ((bool) (Object) marker2)
      vector3_1 = script.transform.InverseTransformPoint(marker2.transform.position);
    Vector3 vector3_2;
    switch (type)
    {
      case MarkerType.Smooth:
        if (!(bool) (Object) marker2)
        {
          prewHandle = (marker1.position - position) * 0.333f;
          nextHandle = -prewHandle * 0.99f;
          break;
        }
        if (!(bool) (Object) marker1)
        {
          nextHandle = (vector3_1 - position) * 0.333f;
          prewHandle = -nextHandle * 0.99f;
          break;
        }
        nextHandle = Vector3.Slerp(-(marker1.position - position) * 0.333f, (vector3_1 - position) * 0.333f, 0.5f);
        prewHandle = Vector3.Slerp((marker1.position - position) * 0.333f, -(vector3_1 - position) * 0.333f, 0.5f);
        break;
      case MarkerType.Transform:
        if ((bool) (Object) marker1)
          prewHandle = -transform.forward * transform.localScale.z * (position - marker1.position).magnitude * 0.4f;
        if ((bool) (Object) marker2)
        {
          vector3_2 = position - vector3_1;
          nextHandle = transform.forward * transform.localScale.z * vector3_2.magnitude * 0.4f;
        }
        break;
      case MarkerType.Corner:
        prewHandle = !(bool) (Object) marker1 ? new Vector3(0.0f, 0.0f, 0.0f) : (marker1.position - position) * 0.333f;
        nextHandle = !(bool) (Object) marker2 ? new Vector3(0.0f, 0.0f, 0.0f) : (vector3_1 - position) * 0.333f;
        break;
    }
    vector3_2 = nextHandle - prewHandle;
    if (vector3_2.sqrMagnitude >= 0.0099999997764825821)
      return;
    nextHandle += new Vector3(1f / 1000f, 0.0f, 0.0f);
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
