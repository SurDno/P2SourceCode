// Decompiled with JetBrains decompiler
// Type: Pingle.UISelectableHelper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Pingle
{
  public static class UISelectableHelper
  {
    public static GameObject Select(
      IEnumerable<GameObject> objects,
      GameObject origin,
      Vector3 dirrection,
      bool IsDirectionPriority = true)
    {
      RectTransform transform = origin.transform as RectTransform;
      return UISelectableHelper.Select(objects, (Object) transform != (Object) null ? transform.TransformPoint(UISelectableHelper.GetPointOnRectEdge(transform, (Vector2) dirrection)) : origin.transform.position, dirrection, IsDirectionPriority);
    }

    public static GameObject Select(
      IEnumerable<GameObject> objects,
      Vector3 origin,
      Vector3 dirrection,
      bool isDirectionPriority = true)
    {
      float num1 = float.NegativeInfinity;
      GameObject gameObject1 = (GameObject) null;
      foreach (GameObject gameObject2 in objects)
      {
        if (!((Object) gameObject2 == (Object) null) && gameObject2.activeInHierarchy)
        {
          RectTransform transform = gameObject2.transform as RectTransform;
          Vector3 position = (Object) transform != (Object) null ? (Vector3) transform.rect.center : Vector3.zero;
          Vector3 vector3 = gameObject2.transform.TransformPoint(position) - origin;
          float f = Vector3.Dot(dirrection, vector3.normalized);
          if ((double) f > 0.0)
          {
            if (isDirectionPriority)
              f = Mathf.Pow(f, 2f);
            float num2 = f / vector3.magnitude;
            if ((double) num2 > (double) num1)
            {
              num1 = num2;
              gameObject1 = gameObject2;
            }
          }
        }
      }
      return gameObject1;
    }

    public static GameObject SelectClosest(IEnumerable<GameObject> objects, Vector3 origin)
    {
      float num = float.MaxValue;
      GameObject gameObject1 = (GameObject) null;
      foreach (GameObject gameObject2 in objects)
      {
        float sqrMagnitude = ((Vector2) (gameObject2.transform.position - origin)).sqrMagnitude;
        if ((double) sqrMagnitude < (double) num)
        {
          gameObject1 = gameObject2;
          num = sqrMagnitude;
        }
      }
      return gameObject1;
    }

    private static Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir)
    {
      if ((Object) rect == (Object) null)
        return Vector3.zero;
      if (dir != Vector2.zero)
        dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
      dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);
      return (Vector3) dir;
    }
  }
}
