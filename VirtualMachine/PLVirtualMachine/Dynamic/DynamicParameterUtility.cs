// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.DynamicParameterUtility
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using System.Collections.Generic;

#nullable disable
namespace PLVirtualMachine.Dynamic
{
  public static class DynamicParameterUtility
  {
    private static Dictionary<ulong, List<IDependedEventRef>> staticParameterDependedEventsDict = new Dictionary<ulong, List<IDependedEventRef>>();

    public static void AddDependedEvent(ulong paramId, IDependedEventRef dependedEvent)
    {
      if (!DynamicParameterUtility.staticParameterDependedEventsDict.ContainsKey(paramId))
        DynamicParameterUtility.staticParameterDependedEventsDict.Add(paramId, new List<IDependedEventRef>());
      DynamicParameterUtility.staticParameterDependedEventsDict[paramId].Add(dependedEvent);
    }

    public static void RemoveDependedEvent(ulong paramId, IDependedEventRef dependedEvent)
    {
      if (!DynamicParameterUtility.staticParameterDependedEventsDict.ContainsKey(paramId))
        return;
      DynamicParameterUtility.staticParameterDependedEventsDict[paramId].Remove(dependedEvent);
    }

    public static List<IDependedEventRef> GetParameterDependedEventsByStaticGuid(ulong paramId)
    {
      List<IDependedEventRef> dependedEventRefList;
      return DynamicParameterUtility.staticParameterDependedEventsDict.TryGetValue(paramId, out dependedEventRefList) ? dependedEventRefList : (List<IDependedEventRef>) null;
    }

    public static void Clear() => DynamicParameterUtility.staticParameterDependedEventsDict.Clear();
  }
}
