using System.Collections.Generic;

namespace PLVirtualMachine.Dynamic
{
  public static class DynamicParameterUtility
  {
    private static Dictionary<ulong, List<IDependedEventRef>> staticParameterDependedEventsDict = new();

    public static void AddDependedEvent(ulong paramId, IDependedEventRef dependedEvent)
    {
      if (!staticParameterDependedEventsDict.ContainsKey(paramId))
        staticParameterDependedEventsDict.Add(paramId, []);
      staticParameterDependedEventsDict[paramId].Add(dependedEvent);
    }

    public static void RemoveDependedEvent(ulong paramId, IDependedEventRef dependedEvent)
    {
      if (!staticParameterDependedEventsDict.ContainsKey(paramId))
        return;
      staticParameterDependedEventsDict[paramId].Remove(dependedEvent);
    }

    public static List<IDependedEventRef> GetParameterDependedEventsByStaticGuid(ulong paramId)
    {
      return staticParameterDependedEventsDict.TryGetValue(paramId, out List<IDependedEventRef> dependedEventRefList) ? dependedEventRefList : null;
    }

    public static void Clear() => staticParameterDependedEventsDict.Clear();
  }
}
