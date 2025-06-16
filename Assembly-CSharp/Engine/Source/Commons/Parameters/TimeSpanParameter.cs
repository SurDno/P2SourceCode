using System;
using System.Reflection;
using Cofe.Utility;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Commons.Parameters
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad | TypeEnum.NeedSave)]
  public class TimeSpanParameter : 
    ParameterListener,
    IParameter<TimeSpan>,
    IParameter,
    IComputeParameter
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
    protected TimeSpan value;
    [DataReadProxy]
    [DataWriteProxy]
    [StateSaveProxy]
    [StateLoadProxy]
    [CopyableProxy]
    [NeedSaveProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected TimeSpan minValue;
    [DataReadProxy]
    [DataWriteProxy]
    [StateSaveProxy]
    [StateLoadProxy]
    [CopyableProxy()]
    [NeedSaveProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected TimeSpan maxValue;
    private TimeSpan storedValue;
    private bool mutableChecked;

    [Inspected(Header = true)]
    public ParameterNameEnum Name => name;

    [Inspected(Header = true, Mutable = true)]
    public TimeSpan Value
    {
      get => value;
      set
      {
        CheckMutable();
        this.value = value;
      }
    }

    [Inspected]
    public TimeSpan BaseValue
    {
      get => value;
      set
      {
        Debug.LogError("Parameter : " + name + " , type : " + TypeUtility.GetTypeName(GetType()) + " , property : " + MethodBase.GetCurrentMethod().Name + " not supported setter");
      }
    }

    [Inspected]
    public TimeSpan MinValue
    {
      get => minValue;
      set
      {
        CheckMutable();
        minValue = value;
      }
    }

    [Inspected]
    public TimeSpan MaxValue
    {
      get => maxValue;
      set
      {
        CheckMutable();
        maxValue = value;
      }
    }

    [Inspected]
    public bool Resetable => false;

    public object ValueData => Value;

    private void CheckMutable()
    {
      if (mutableChecked)
        return;
      mutableChecked = true;
      storedValue = value;
      ServiceLocator.GetService<ParametersUpdater>().AddParameter(this);
    }

    void IComputeParameter.ResetResetable()
    {
    }

    void IComputeParameter.CorrectValue()
    {
    }

    void IComputeParameter.ComputeEvent()
    {
      mutableChecked = false;
      if (!(storedValue != value))
        return;
      ChangeParameterInvoke(this);
    }
  }
}
