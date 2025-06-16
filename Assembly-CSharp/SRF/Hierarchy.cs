using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SRF
{
  public class Hierarchy
  {
    private static readonly char[] Seperator = new char[1]
    {
      '/'
    };
    private static readonly Dictionary<string, Transform> Cache = new Dictionary<string, Transform>();

    [Obsolete("Use static Get() instead")]
    public Transform this[string key] => Hierarchy.Get(key);

    public static Transform Get(string key)
    {
      Transform transform1;
      if (Hierarchy.Cache.TryGetValue(key, out transform1))
        return transform1;
      GameObject gameObject = GameObject.Find(key);
      if ((bool) (UnityEngine.Object) gameObject)
      {
        Transform transform2 = gameObject.transform;
        Hierarchy.Cache.Add(key, transform2);
        return transform2;
      }
      string[] source = key.Split(Hierarchy.Seperator, StringSplitOptions.RemoveEmptyEntries);
      Transform transform3 = new GameObject(((IEnumerable<string>) source).Last<string>()).transform;
      Hierarchy.Cache.Add(key, transform3);
      if (source.Length == 1)
        return transform3;
      transform3.parent = Hierarchy.Get(string.Join("/", source, 0, source.Length - 1));
      return transform3;
    }
  }
}
