// Decompiled with JetBrains decompiler
// Type: SRF.Hierarchy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
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
