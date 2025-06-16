using System.Collections.Generic;
using Cofe.Proxies;
using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Commons.Parameters
{
  public abstract class PriorityContainer<T> : INeedSave
  {
    [DataReadProxy]
    [DataWriteProxy]
    [StateSaveProxy]
    [StateLoadProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<PriorityItem<T>> items = new List<PriorityItem<T>>();
    private static PriorityItemComprer<T> comparer = new PriorityItemComprer<T>();

    [Inspected]
    private PriorityParameterEnum Priority
    {
      get
      {
        if (items.Count > 0)
        {
          PriorityItem<T> priorityItem = items[items.Count - 1];
          if (priorityItem != null)
            return priorityItem.Priority;
        }
        return PriorityParameterEnum.None;
      }
    }

    [Inspected]
    public T Value
    {
      get
      {
        if (items.Count > 0)
        {
          PriorityItem<T> priorityItem = items[items.Count - 1];
          if (priorityItem != null)
            return priorityItem.Value;
        }
        return default (T);
      }
      set
      {
        if (items.Count > 0)
        {
          PriorityItem<T> priorityItem = items[items.Count - 1];
          if (priorityItem == null)
            return;
          priorityItem.Value = value;
        }
        else
          SetValue(PriorityParameterEnum.Default, value);
      }
    }

    public bool NeedSave
    {
      get
      {
        if (items.Count == 0)
          return false;
        if (items.Count == 1)
        {
          PriorityItem<T> priorityItem = items[0];
          if (priorityItem == null || priorityItem.Priority == PriorityParameterEnum.Default && IsDefault(priorityItem.Value))
            return false;
        }
        return true;
      }
    }

    public bool TryGetValue(PriorityParameterEnum priority, out T result)
    {
      result = default (T);
      for (int index = 0; index < items.Count; ++index)
      {
        PriorityItem<T> priorityItem = items[index];
        if (priorityItem.Priority == priority)
        {
          result = priorityItem.Value;
          return true;
        }
      }
      return false;
    }

    public void SetValue(PriorityParameterEnum priority, T value)
    {
      bool flag = false;
      for (int index = 0; index < items.Count; ++index)
      {
        PriorityItem<T> priorityItem = items[index];
        if (priorityItem.Priority == priority)
        {
          flag = true;
          priorityItem.Value = value;
          break;
        }
      }
      if (flag)
        return;
      PriorityItem<T> priorityItem1 = ProxyFactory.Create<PriorityItem<T>>();
      priorityItem1.Priority = priority;
      priorityItem1.Value = value;
      items.Add(priorityItem1);
      items.Sort(comparer);
    }

    public void ResetValue(PriorityParameterEnum priority)
    {
      for (int index = 0; index < items.Count; ++index)
      {
        if (items[index].Priority == priority)
        {
          items.RemoveAt(index);
          break;
        }
      }
    }

    protected abstract bool IsDefault(T value);
  }
}
