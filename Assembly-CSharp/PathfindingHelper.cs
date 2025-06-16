using System;
using System.Collections;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.Gizmos;

public static class PathfindingHelper
{
  private static float retreatSectorAngleInDegrees = 180f;
  private static int retreatRayCount = 20;
  private static float retreatSearchDistance = 30f;

  public static float? FindBestRetreatDirection(Vector3 myPosition, Vector3 enemyPosition)
  {
    Vector3 vector3_1 = (enemyPosition - myPosition) with
    {
      y = 0.0f
    };
    vector3_1.Normalize();
    float num1 = 3f;
    float num2 = num1;
    float num3 = num1;
    float num4 = 0.0f;
    for (int index = 0; index < retreatRayCount; ++index)
    {
      float angle = (float) (index * (double) retreatSectorAngleInDegrees / (retreatRayCount - 1) - retreatSectorAngleInDegrees / 2.0);
      Vector3 vector3_2 = -(Quaternion.AngleAxis(angle, Vector3.up) * vector3_1);
      NavMeshHit hit;
      float num5 = !NavMesh.Raycast(myPosition, myPosition + vector3_2 * retreatSearchDistance, out hit, -1) ? retreatSearchDistance : hit.distance;
      float num6 = hit.distance * Mathf.Cos((float) (0.5 * angle * (Math.PI / 180.0)));
      if (num6 > (double) num2)
      {
        num2 = num6;
        num3 = num5;
        num4 = angle;
      }
    }
    return num2 == (double) num1 ? new float?() : num4;
  }

  public static float? FindBestRetreatDirection2(Transform myTansform, Transform enemyTransform)
  {
    Vector3 from = (enemyTransform.position - myTansform.position) with
    {
      y = 0.0f
    };
    from.Normalize();
    float num1 = 0.1f;
    float num2 = num1;
    float num3 = num1;
    float num4 = 0.0f;
    float num5 = 360f;
    for (int index = 0; index < retreatRayCount; ++index)
    {
      float angle = index * num5 / retreatRayCount;
      Vector3 to = -(Quaternion.AngleAxis(angle, Vector3.up) * from);
      NavMeshHit hit;
      float num6 = !NavMesh.Raycast(myTansform.position, myTansform.position + to * retreatSearchDistance, out hit, -1) ? retreatSearchDistance : hit.distance;
      float num7 = Mathf.Sin((float) (0.5 * (double) Vector3.Angle(from, to) * (Math.PI / 180.0)));
      float num8 = Mathf.Cos((float) (0.5 * (double) Vector3.Angle(myTansform.forward, to) * (Math.PI / 180.0)));
      float num9 = hit.distance * num7 * num8 * num8;
      if (InstanceByRequest<EngineApplication>.Instance.IsDebug)
      {
        Vector3 position = myTansform.position;
        Vector3 end = myTansform.position + num9 * to;
        Color green = Color.green;
        ServiceLocator.GetService<GizmoService>().DrawLine(position, end, green);
      }
      if (num9 > (double) num2)
      {
        num2 = num9;
        num3 = num6;
        num4 = angle;
      }
    }
    return num2 == (double) num1 ? new float?() : num4;
  }

  private static IEnumerator ExecuteSecond(float delay, Action action)
  {
    float time = Time.unscaledTime;
    while (time + (double) delay > (double) Time.unscaledTime)
    {
      Action action1 = action;
      if (action1 != null)
        action1();
      yield return null;
    }
  }

  public static float FindDistance(Vector3 from, Vector3 direction, float maxTraceLength)
  {
    NavMeshHit hit;
    return !NavMesh.Raycast(from, from + direction.normalized * maxTraceLength, out hit, -1) ? maxTraceLength : hit.distance;
  }

  public static bool IsFreeSpace(Vector3 startPos, Vector3 endPos)
  {
    return !NavMesh.Raycast(startPos, endPos, out NavMeshHit _, -1);
  }

  public static float GetSurrenderRange(Vector3 pos, Vector3 back)
  {
    float num = 3f;
    NavMeshHit hit;
    return NavMesh.Raycast(pos, pos + back * num, out hit, -1) ? hit.distance : num;
  }
}
