// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.ObjectPool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace BehaviorDesigner.Runtime
{
  public static class ObjectPool
  {
    private static Dictionary<Type, object> poolDictionary = new Dictionary<Type, object>();

    public static void Clear() => ObjectPool.poolDictionary.Clear();

    public static T Get<T>()
    {
      object obj;
      if (ObjectPool.poolDictionary.TryGetValue(typeof (T), out obj))
      {
        Stack<T> objStack = obj as Stack<T>;
        if (objStack.Count > 0)
          return objStack.Pop();
      }
      return (T) TaskUtility.CreateInstance(typeof (T));
    }

    public static void Return<T>(T obj)
    {
      if ((object) obj == null)
        return;
      object obj1;
      if (ObjectPool.poolDictionary.TryGetValue(typeof (T), out obj1))
      {
        (obj1 as Stack<T>).Push(obj);
      }
      else
      {
        Stack<T> objStack = new Stack<T>();
        objStack.Push(obj);
        ObjectPool.poolDictionary.Add(typeof (T), (object) objStack);
      }
    }
  }
}
