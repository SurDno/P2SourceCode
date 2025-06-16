// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Utilities.NavMeshUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Services.Gizmos;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
namespace Engine.Source.Components.Utilities
{
  public static class NavMeshUtility
  {
    public const int chankSize = 1024;
    private static Vector3[] tmp = new Vector3[1024];

    public static float GetPathLength(NavMeshPath path)
    {
      if (path == null)
        return 0.0f;
      int pathAndCount = NavMeshUtility.GetPathAndCount(path);
      if (pathAndCount < 2)
        return 0.0f;
      float pathLength = 0.0f;
      Vector3 a = NavMeshUtility.tmp[0];
      for (int index = 1; index < pathAndCount; ++index)
      {
        Vector3 b = NavMeshUtility.tmp[index];
        pathLength += Vector3.Distance(a, b);
        a = b;
      }
      return pathLength;
    }

    public static bool IsBrokenPath(NavMeshAgent agent)
    {
      if ((Object) agent == (Object) null || !agent.hasPath)
        return false;
      NavMeshPath path = agent.path;
      return path != null && NavMeshUtility.GetPathAndCount(path) < 2;
    }

    public static bool HasPathNoGarbage(NavMeshAgent agent)
    {
      return !((Object) agent == (Object) null) && agent.hasPath;
    }

    public static bool HasPathWithGarbage(NavMeshAgent agent)
    {
      if ((Object) agent == (Object) null || !agent.hasPath)
        return false;
      NavMeshPath path = agent.path;
      return path != null && NavMeshUtility.GetPathAndCount(path) >= 2;
    }

    public static void DrawPath(NavMeshAgent agent)
    {
      if ((Object) agent == (Object) null || !agent.hasPath)
        return;
      NavMeshPath path = agent.path;
      if (path == null)
        return;
      int pathAndCount = NavMeshUtility.GetPathAndCount(path);
      if (pathAndCount < 2)
        return;
      Vector3 start = NavMeshUtility.tmp[0];
      for (int index = 1; index < pathAndCount; ++index)
      {
        Vector3 end = NavMeshUtility.tmp[index];
        Debug.DrawLine(start, end, Color.magenta);
        Debug.DrawLine(start, start + Vector3.up, Color.green);
        start = end;
      }
      Debug.DrawLine(start, start + Vector3.up, Color.green);
    }

    public static int DrawPath(NavMeshAgent agent, GizmoService gizmoService)
    {
      if ((Object) agent == (Object) null || !agent.hasPath)
        return 0;
      NavMeshPath path = agent.path;
      if (path == null)
        return 0;
      int pathAndCount = NavMeshUtility.GetPathAndCount(path);
      if (pathAndCount < 2)
        return pathAndCount;
      Vector3 start = NavMeshUtility.tmp[0];
      for (int index = 1; index < pathAndCount; ++index)
      {
        Vector3 end = NavMeshUtility.tmp[index];
        gizmoService.DrawLine(start, end, Color.magenta);
        gizmoService.DrawLine(start, start + Vector3.up, Color.green);
        start = end;
      }
      gizmoService.DrawLine(start, start + Vector3.up, Color.green);
      return pathAndCount;
    }

    public static void GetCornersNonAlloc(NavMeshAgent agent, List<Vector3> result)
    {
      result.Clear();
      if ((Object) agent == (Object) null || !agent.hasPath)
        return;
      NavMeshPath path = agent.path;
      if (path == null)
        return;
      int pathAndCount = NavMeshUtility.GetPathAndCount(path);
      if (pathAndCount < 2)
        return;
      for (int index = 1; index < pathAndCount; ++index)
      {
        Vector3 vector3 = NavMeshUtility.tmp[index];
        result.Add(vector3);
      }
    }

    public static bool SampleRaycastPosition(
      ref Vector3 point,
      float distanceUp,
      float distanceDown,
      int mask)
    {
      int layerMask = 9519617;
      RaycastHit hitInfo;
      if (Physics.Raycast(point + new Vector3(0.0f, distanceUp, 0.0f), Vector3.down, out hitInfo, distanceDown, layerMask, QueryTriggerInteraction.Ignore))
        point = hitInfo.point;
      return NavMeshUtility.SamplePosition(ref point, mask);
    }

    public static bool SamplePosition(ref Vector3 point, int mask, int maxRadius = 256)
    {
      return NavMeshUtility.SamplePosition(ref point, mask, out int _, maxRadius);
    }

    public static bool SamplePosition(
      ref Vector3 point,
      int mask,
      out int resultRadius,
      int maxRadius = 256)
    {
      resultRadius = 0;
      for (int maxDistance = 2; maxDistance <= maxRadius; maxDistance *= 2)
      {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(point, out hit, (float) maxDistance, mask))
        {
          point = hit.position;
          resultRadius = maxDistance;
          return true;
        }
      }
      return false;
    }

    private static int GetPathAndCount(NavMeshPath path)
    {
      if (path == null)
        return 0;
      int cornersNonAlloc;
      while (true)
      {
        cornersNonAlloc = path.GetCornersNonAlloc(NavMeshUtility.tmp);
        if (cornersNonAlloc == NavMeshUtility.tmp.Length)
          NavMeshUtility.tmp = new Vector3[NavMeshUtility.tmp.Length + 1024];
        else
          break;
      }
      return cornersNonAlloc;
    }
  }
}
