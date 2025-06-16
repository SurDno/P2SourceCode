using Scripts.Utility;
using System.Collections.Generic;

namespace UnityEngine
{
  public static class PhysicsUtility
  {
    private const int chankSize = 1024;
    private static RaycastHit[] tmp = new RaycastHit[1024];

    public static void Raycast(
      List<RaycastHit> result,
      Vector3 origin,
      Vector3 direction,
      float maxDistance)
    {
      result.Clear();
      int num;
      while (true)
      {
        num = Physics.RaycastNonAlloc(origin, direction, PhysicsUtility.tmp, maxDistance);
        if (num == PhysicsUtility.tmp.Length)
          PhysicsUtility.tmp = new RaycastHit[PhysicsUtility.tmp.Length + 1024];
        else
          break;
      }
      for (int index = 0; index < num; ++index)
        result.Add(PhysicsUtility.tmp[index]);
      result.Sort((IComparer<RaycastHit>) RaycastComparer.Instance);
    }

    public static void Raycast(
      List<RaycastHit> result,
      Vector3 origin,
      Vector3 direction,
      float maxDistance,
      int layerMask,
      QueryTriggerInteraction queryTriggerInteraction)
    {
      result.Clear();
      int num;
      while (true)
      {
        num = Physics.RaycastNonAlloc(origin, direction, PhysicsUtility.tmp, maxDistance, layerMask, queryTriggerInteraction);
        if (num == PhysicsUtility.tmp.Length)
          PhysicsUtility.tmp = new RaycastHit[PhysicsUtility.tmp.Length + 1024];
        else
          break;
      }
      for (int index = 0; index < num; ++index)
        result.Add(PhysicsUtility.tmp[index]);
      result.Sort((IComparer<RaycastHit>) RaycastComparer.Instance);
    }

    public static void Raycast(
      List<RaycastHit> result,
      Vector3 origin,
      Vector3 direction,
      float maxDistance,
      int layerMask)
    {
      result.Clear();
      int num;
      while (true)
      {
        num = Physics.RaycastNonAlloc(origin, direction, PhysicsUtility.tmp, maxDistance, layerMask);
        if (num == PhysicsUtility.tmp.Length)
          PhysicsUtility.tmp = new RaycastHit[PhysicsUtility.tmp.Length + 1024];
        else
          break;
      }
      for (int index = 0; index < num; ++index)
        result.Add(PhysicsUtility.tmp[index]);
      result.Sort((IComparer<RaycastHit>) RaycastComparer.Instance);
    }

    public static void Raycast(
      List<RaycastHit> result,
      Ray ray,
      float maxDistance,
      int layerMask,
      QueryTriggerInteraction queryTriggerInteraction)
    {
      result.Clear();
      int num;
      while (true)
      {
        num = Physics.RaycastNonAlloc(ray, PhysicsUtility.tmp, maxDistance, layerMask, queryTriggerInteraction);
        if (num == PhysicsUtility.tmp.Length)
          PhysicsUtility.tmp = new RaycastHit[PhysicsUtility.tmp.Length + 1024];
        else
          break;
      }
      for (int index = 0; index < num; ++index)
        result.Add(PhysicsUtility.tmp[index]);
      result.Sort((IComparer<RaycastHit>) RaycastComparer.Instance);
    }

    public static void SphereCast(
      List<RaycastHit> result,
      Ray ray,
      float radius,
      float maxDistance,
      int layerMask,
      QueryTriggerInteraction queryTriggerInteraction)
    {
      result.Clear();
      int num;
      while (true)
      {
        num = Physics.SphereCastNonAlloc(ray, radius, PhysicsUtility.tmp, maxDistance, layerMask, queryTriggerInteraction);
        if (num == PhysicsUtility.tmp.Length)
          PhysicsUtility.tmp = new RaycastHit[PhysicsUtility.tmp.Length + 1024];
        else
          break;
      }
      for (int index = 0; index < num; ++index)
        result.Add(PhysicsUtility.tmp[index]);
      result.Sort((IComparer<RaycastHit>) RaycastComparer.Instance);
    }
  }
}
