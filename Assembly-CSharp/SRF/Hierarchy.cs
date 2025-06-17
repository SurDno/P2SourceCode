using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SRF
{
  public class Hierarchy
  {
    private static readonly char[] Seperator = [
      '/'
    ];
    private static readonly Dictionary<string, Transform> Cache = new();

    [Obsolete("Use static Get() instead")]
    public Transform this[string key] => Get(key);

    public static Transform Get(string key)
    {
      if (Cache.TryGetValue(key, out Transform transform1))
        return transform1;
      GameObject gameObject = GameObject.Find(key);
      if ((bool) (Object) gameObject)
      {
        Transform transform2 = gameObject.transform;
        Cache.Add(key, transform2);
        return transform2;
      }
      string[] source = key.Split(Seperator, StringSplitOptions.RemoveEmptyEntries);
      Transform transform3 = new GameObject(source.Last()).transform;
      Cache.Add(key, transform3);
      if (source.Length == 1)
        return transform3;
      transform3.parent = Get(string.Join("/", source, 0, source.Length - 1));
      return transform3;
    }
  }
}
