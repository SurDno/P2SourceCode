using Engine.Common.Components.Parameters;
using Inspectors;
using System;

namespace Engine.Source.Commons.Parameters
{
  public class PriorityParameterValue<T> : 
    IPriorityParameterValue<T>,
    IParameterValue<T>,
    IParameterValueSet<T>,
    IChangeParameterListener
    where T : struct
  {
    [Inspected]
    private PriorityParameter<T> parameter;

    public void Set(IParameter<T> parameter)
    {
      if (this.parameter != null)
        this.parameter.RemoveListener((IChangeParameterListener) this);
      this.parameter = parameter as PriorityParameter<T>;
      if (this.parameter == null)
        return;
      this.parameter.AddListener((IChangeParameterListener) this);
    }

    public T Value
    {
      get => this.parameter != null ? this.parameter.Value : default (T);
      set
      {
        if (this.parameter == null)
          return;
        this.parameter.Value = value;
      }
    }

    public T MinValue
    {
      get => this.parameter != null ? this.parameter.MinValue : default (T);
      set
      {
        if (this.parameter == null)
          return;
        this.parameter.MinValue = value;
      }
    }

    public T MaxValue
    {
      get => this.parameter != null ? this.parameter.MaxValue : default (T);
      set
      {
        if (this.parameter == null)
          return;
        this.parameter.MaxValue = value;
      }
    }

    public event Action<T> ChangeValueEvent;

    public void SetValue(PriorityParameterEnum priority, T value)
    {
      if (this.parameter == null)
        return;
      this.parameter.SetValue(priority, value);
    }

    public bool TryGetValue(PriorityParameterEnum priority, out T result)
    {
      if (this.parameter != null)
        return this.parameter.TryGetValue(priority, out result);
      result = default (T);
      return false;
    }

    public void ResetValue(PriorityParameterEnum priority)
    {
      if (this.parameter == null)
        return;
      this.parameter.ResetValue(priority);
    }

    public void OnParameterChanged(IParameter parameter)
    {
      Action<T> changeValueEvent = this.ChangeValueEvent;
      if (changeValueEvent == null)
        return;
      changeValueEvent(((IParameter<T>) parameter).Value);
    }
  }
}
