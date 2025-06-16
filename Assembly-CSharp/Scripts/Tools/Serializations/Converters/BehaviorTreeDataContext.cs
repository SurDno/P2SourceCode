// Decompiled with JetBrains decompiler
// Type: Scripts.Tools.Serializations.Converters.BehaviorTreeDataContext
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Scripts.Tools.Serializations.Converters
{
  public static class BehaviorTreeDataContext
  {
    public static List<Object> ContextUnityObjects;
    public static Dictionary<int, Task> Tasks;
    public static Dictionary<string, SharedVariable> Variables;

    public static int GetObjectIndex(Object object2, List<Object> unityObjects)
    {
      for (int index = 0; index < unityObjects.Count; ++index)
      {
        if (unityObjects[index] == object2)
          return index;
      }
      int count = unityObjects.Count;
      unityObjects.Add(object2);
      return count;
    }
  }
}
