using Cofe.Utility;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Inspectors;
using System;
using System.Reflection;
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
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [StateSaveProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [NeedSaveProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum name;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [NeedSaveProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected TimeSpan value;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [NeedSaveProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected TimeSpan minValue;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [NeedSaveProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected TimeSpan maxValue;
    private TimeSpan storedValue;
    private bool mutableChecked;

    [Inspected(Header = true)]
    public ParameterNameEnum Name => this.name;

    [Inspected(Header = true, Mutable = true)]
    public TimeSpan Value
    {
      get => this.value;
      set
      {
        this.CheckMutable();
        this.value = value;
      }
    }

    [Inspected]
    public TimeSpan BaseValue
    {
      get => this.value;
      set
      {
        Debug.LogError((object) ("Parameter : " + (object) this.name + " , type : " + TypeUtility.GetTypeName(this.GetType()) + " , property : " + MethodBase.GetCurrentMethod().Name + " not supported setter"));
      }
    }

    [Inspected]
    public TimeSpan MinValue
    {
      get => this.minValue;
      set
      {
        this.CheckMutable();
        this.minValue = value;
      }
    }

    [Inspected]
    public TimeSpan MaxValue
    {
      get => this.maxValue;
      set
      {
        this.CheckMutable();
        this.maxValue = value;
      }
    }

    [Inspected]
    public bool Resetable => false;

    public object ValueData => (object) this.Value;

    private void CheckMutable()
    {
      if (this.mutableChecked)
        return;
      this.mutableChecked = true;
      this.storedValue = this.value;
      ServiceLocator.GetService<ParametersUpdater>().AddParameter((IComputeParameter) this);
    }

    void IComputeParameter.ResetResetable()
    {
    }

    void IComputeParameter.CorrectValue()
    {
    }

    void IComputeParameter.ComputeEvent()
    {
      this.mutableChecked = false;
      if (!(this.storedValue != this.value))
        return;
      this.ChangeParameterInvoke((IParameter) this);
    }
  }
}
