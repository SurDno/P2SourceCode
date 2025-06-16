// Decompiled with JetBrains decompiler
// Type: PathfindingHelper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.Gizmos;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
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
    for (int index = 0; index < PathfindingHelper.retreatRayCount; ++index)
    {
      float angle = (float) ((double) index * (double) PathfindingHelper.retreatSectorAngleInDegrees / (double) (PathfindingHelper.retreatRayCount - 1) - (double) PathfindingHelper.retreatSectorAngleInDegrees / 2.0);
      Vector3 vector3_2 = -(Quaternion.AngleAxis(angle, Vector3.up) * vector3_1);
      NavMeshHit hit;
      float num5 = !NavMesh.Raycast(myPosition, myPosition + vector3_2 * PathfindingHelper.retreatSearchDistance, out hit, -1) ? PathfindingHelper.retreatSearchDistance : hit.distance;
      float num6 = hit.distance * Mathf.Cos((float) (0.5 * (double) angle * (Math.PI / 180.0)));
      if ((double) num6 > (double) num2)
      {
        num2 = num6;
        num3 = num5;
        num4 = angle;
      }
    }
    return (double) num2 == (double) num1 ? new float?() : new float?(num4);
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
    for (int index = 0; index < PathfindingHelper.retreatRayCount; ++index)
    {
      float angle = (float) index * num5 / (float) PathfindingHelper.retreatRayCount;
      Vector3 to = -(Quaternion.AngleAxis(angle, Vector3.up) * from);
      NavMeshHit hit;
      float num6 = !NavMesh.Raycast(myTansform.position, myTansform.position + to * PathfindingHelper.retreatSearchDistance, out hit, -1) ? PathfindingHelper.retreatSearchDistance : hit.distance;
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
      if ((double) num9 > (double) num2)
      {
        num2 = num9;
        num3 = num6;
        num4 = angle;
      }
    }
    return (double) num2 == (double) num1 ? new float?() : new float?(num4);
  }

  private static IEnumerator ExecuteSecond(float delay, Action action)
  {
    float time = Time.unscaledTime;
    while ((double) time + (double) delay > (double) Time.unscaledTime)
    {
      Action action1 = action;
      if (action1 != null)
        action1();
      yield return (object) null;
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
