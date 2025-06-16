using Cofe.Proxies;
using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.Commons.Parameters
{
  public abstract class PriorityContainer<T> : INeedSave
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<PriorityItem<T>> items = new List<PriorityItem<T>>();
    private static PriorityItemComprer<T> comparer = new PriorityItemComprer<T>();

    [Inspected]
    private PriorityParameterEnum Priority
    {
      get
      {
        if (this.items.Count > 0)
        {
          PriorityItem<T> priorityItem = this.items[this.items.Count - 1];
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
        if (this.items.Count > 0)
        {
          PriorityItem<T> priorityItem = this.items[this.items.Count - 1];
          if (priorityItem != null)
            return priorityItem.Value;
        }
        return default (T);
      }
      set
      {
        if (this.items.Count > 0)
        {
          PriorityItem<T> priorityItem = this.items[this.items.Count - 1];
          if (priorityItem == null)
            return;
          priorityItem.Value = value;
        }
        else
          this.SetValue(PriorityParameterEnum.Default, value);
      }
    }

    public bool NeedSave
    {
      get
      {
        if (this.items.Count == 0)
          return false;
        if (this.items.Count == 1)
        {
          PriorityItem<T> priorityItem = this.items[0];
          if (priorityItem == null || priorityItem.Priority == PriorityParameterEnum.Default && this.IsDefault(priorityItem.Value))
            return false;
        }
        return true;
      }
    }

    public bool TryGetValue(PriorityParameterEnum priority, out T result)
    {
      result = default (T);
      for (int index = 0; index < this.items.Count; ++index)
      {
        PriorityItem<T> priorityItem = this.items[index];
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
      for (int index = 0; index < this.items.Count; ++index)
      {
        PriorityItem<T> priorityItem = this.items[index];
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
      this.items.Add(priorityItem1);
      this.items.Sort((IComparer<PriorityItem<T>>) PriorityContainer<T>.comparer);
    }

    public void ResetValue(PriorityParameterEnum priority)
    {
      for (int index = 0; index < this.items.Count; ++index)
      {
        if (this.items[index].Priority == priority)
        {
          this.items.RemoveAt(index);
          break;
        }
      }
    }

    protected abstract bool IsDefault(T value);
  }
}
