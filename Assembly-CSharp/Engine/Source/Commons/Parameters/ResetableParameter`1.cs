using System.Reflection;
using Cofe.Utility;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Commons.Parameters
{
  public abstract class ResetableParameter<T> : 
    ParameterListener,
    IParameter<T>,
    IParameter,
    IComputeParameter
    where T : struct
  {
    [DataReadProxy]
    [DataWriteProxy]
    [StateSaveProxy]
    [CopyableProxy]
    [NeedSaveProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum name;
    [DataReadProxy]
    [DataWriteProxy]
    [StateSaveProxy]
    [StateLoadProxy]
    [CopyableProxy]
    [NeedSaveProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected T value;
    [DataReadProxy]
    [DataWriteProxy]
    [StateSaveProxy]
    [StateLoadProxy]
    [CopyableProxy()]
    [NeedSaveProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected T baseValue;
    private T storedValue;
    private bool mutableChecked;

    [Inspected(Header = true)]
    public ParameterNameEnum Name => name;

    [Inspected(Header = true, Mutable = true)]
    public T Value
    {
      get => value;
      set
      {
        CheckMutable();
        this.value = value;
      }
    }

    [Inspected]
    public T BaseValue
    {
      get => baseValue;
      set
      {
        CheckMutable();
        baseValue = value;
      }
    }

    [Inspected]
    public T MinValue
    {
      get => value;
      set
      {
        Debug.LogError("Parameter : " + name + " , type : " + TypeUtility.GetTypeName(GetType()) + " , property : " + MethodBase.GetCurrentMethod().Name + " not supported setter");
      }
    }

    [Inspected]
    public T MaxValue
    {
      get => value;
      set
      {
        Debug.LogError("Parameter : " + name + " , type : " + TypeUtility.GetTypeName(GetType()) + " , property : " + MethodBase.GetCurrentMethod().Name + " not supported setter");
      }
    }

    [Inspected]
    public bool Resetable => true;

    public object ValueData => Value;

    private void CheckMutable()
    {
      if (mutableChecked)
        return;
      mutableChecked = true;
      storedValue = value;
      ServiceLocator.GetService<ParametersUpdater>().AddParameter(this);
    }

    protected abstract bool Compare(T a, T b);

    void IComputeParameter.ResetResetable() => Value = baseValue;

    void IComputeParameter.CorrectValue()
    {
    }

    void IComputeParameter.ComputeEvent()
    {
      mutableChecked = false;
      if (Compare(storedValue, value))
        return;
      ChangeParameterInvoke(this);
    }
  }
}
