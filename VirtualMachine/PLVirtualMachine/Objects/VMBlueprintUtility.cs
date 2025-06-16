using System.Collections.Generic;
using PLVirtualMachine.Common;
using PLVirtualMachine.GameLogic;

namespace PLVirtualMachine.Objects
{
  public static class VMBlueprintUtility
  {
    public static void MergeComponents(
      Dictionary<string, IFunctionalComponent> mergeTo,
      List<IFunctionalComponent> mergeFrom)
    {
      for (int index = 0; index < mergeFrom.Count; ++index)
      {
        IFunctionalComponent functionalComponent = mergeFrom[index];
        if (!mergeTo.ContainsKey(functionalComponent.Name))
          mergeTo.Add(functionalComponent.Name, functionalComponent);
      }
    }

    public static void MergeInheritedComponents(
      Dictionary<string, IFunctionalComponent> mergeTo,
      Dictionary<string, IFunctionalComponent> mergeFrom)
    {
      foreach (KeyValuePair<string, IFunctionalComponent> keyValuePair in mergeFrom)
      {
        if (mergeTo.ContainsKey(keyValuePair.Key))
        {
          if (((VMFunctionalComponent) mergeTo[keyValuePair.Key]).LoadPriority > ((VMFunctionalComponent) keyValuePair.Value).LoadPriority)
            mergeTo[keyValuePair.Key] = keyValuePair.Value;
        }
        else
          mergeTo.Add(keyValuePair.Key, keyValuePair.Value);
      }
    }

    public static void MergeContextParams(
      List<IVariable> mergedParamsTo,
      IEnumerable<IVariable> mergedParamsFrom)
    {
      foreach (IVariable variable1 in mergedParamsFrom)
      {
        if (!(typeof (VMParameter) != variable1.GetType()))
        {
          IVariable variable2 = variable1;
          mergedParamsTo.Add(variable2);
        }
      }
    }

    public static void MergeCustomEvents(List<IEvent> mergedEventsTo, List<IEvent> mergedEventsFrom)
    {
      if (mergedEventsFrom == null)
        return;
      for (int index1 = 0; index1 < mergedEventsFrom.Count; ++index1)
      {
        IEvent @event = mergedEventsFrom[index1];
        int index2 = -1;
        for (int index3 = 0; index3 < mergedEventsTo.Count; ++index3)
        {
          if (mergedEventsTo[index3].Name == mergedEventsFrom[index1].Name)
          {
            index2 = index3;
            break;
          }
        }
        if (index2 < 0)
          mergedEventsTo.Add(mergedEventsFrom[index1]);
        else
          mergedEventsTo[index2] = mergedEventsFrom[index1];
      }
    }
  }
}
