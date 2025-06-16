using System.Reflection;
using Cofe.Proxies;
using Cofe.Utility;
using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Commons.Parameters
{
  public abstract class PriorityParameter<T> : 
    ParameterListener,
    IParameter<T>,
    IParameter,
    IComputeParameter,
    INeedSave
    where T : struct
  {
    [DataReadProxy]
    [DataWriteProxy]
    [StateSaveProxy]
    [CopyableProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum name;
    [DataReadProxy]
    [DataWriteProxy]
    [StateSaveProxy]
    [StateLoadProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected PriorityContainer<T> container = ProxyFactory.Create<PriorityContainer<T>>();
    private T storedValue;
    private bool mutableChecked;

    [Inspected(Header = true)]
    public ParameterNameEnum Name => name;

    [Inspected(Header = true)]
    public T Value
    {
      get => container.Value;
      set
      {
        CheckMutable();
        container.Value = value;
      }
    }

    [Inspected]
    public T BaseValue
    {
      get => default (T);
      set
      {
        Debug.LogError("Parameter : " + name + " , type : " + TypeUtility.GetTypeName(GetType()) + " , property : " + MethodBase.GetCurrentMethod().Name + " not supported setter");
      }
    }

    [Inspected]
    public T MinValue
    {
      get => default (T);
      set
      {
        Debug.LogError("Parameter : " + name + " , type : " + TypeUtility.GetTypeName(GetType()) + " , property : " + MethodBase.GetCurrentMethod().Name + " not supported setter");
      }
    }

    [Inspected]
    public T MaxValue
    {
      get => default (T);
      set
      {
        Debug.LogError("Parameter : " + name + " , type : " + TypeUtility.GetTypeName(GetType()) + " , property : " + MethodBase.GetCurrentMethod().Name + " not supported setter");
      }
    }

    [Inspected]
    public bool Resetable => false;

    [Inspected]
    public bool NeedSave => container.NeedSave;

    public object ValueData => Value;

    public void SetValue(PriorityParameterEnum priority, T value)
    {
      CheckMutable();
      container.SetValue(priority, value);
    }

    public bool TryGetValue(PriorityParameterEnum priority, out T result)
    {
      return container.TryGetValue(priority, out result);
    }

    public void ResetValue(PriorityParameterEnum priority)
    {
      CheckMutable();
      container.ResetValue(priority);
    }

    private void CheckMutable()
    {
      if (mutableChecked)
        return;
      mutableChecked = true;
      storedValue = Value;
      ServiceLocator.GetService<ParametersUpdater>().AddParameter(this);
    }

    protected abstract bool Compare(T a, T b);

    void IComputeParameter.ResetResetable()
    {
    }

    void IComputeParameter.CorrectValue()
    {
    }

    void IComputeParameter.ComputeEvent()
    {
      mutableChecked = false;
      if (Compare(storedValue, Value))
        return;
      ChangeParameterInvoke(this);
    }
  }
}
