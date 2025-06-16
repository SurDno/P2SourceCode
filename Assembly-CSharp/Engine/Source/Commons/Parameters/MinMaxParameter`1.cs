using Cofe.Utility;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Inspectors;
using System.Reflection;
using UnityEngine;

namespace Engine.Source.Commons.Parameters
{
  public abstract class MinMaxParameter<T> : 
    ParameterListener,
    IParameter<T>,
    IParameter,
    IComputeParameter
    where T : struct
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
    protected T value;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [NeedSaveProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected T minValue;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [NeedSaveProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected T maxValue;
    private T storedValue;
    private bool mutableChecked;

    [Inspected(Header = true)]
    public ParameterNameEnum Name => this.name;

    [Inspected(Header = true, Mutable = true)]
    public T Value
    {
      get => this.value;
      set
      {
        this.CheckMutable();
        this.value = value;
      }
    }

    [Inspected]
    public T BaseValue
    {
      get => this.value;
      set
      {
        Debug.LogError((object) ("Parameter : " + (object) this.name + " , type : " + TypeUtility.GetTypeName(this.GetType()) + " , property : " + MethodBase.GetCurrentMethod().Name + " not supported setter"));
      }
    }

    [Inspected]
    public T MinValue
    {
      get => this.minValue;
      set
      {
        this.CheckMutable();
        this.minValue = value;
      }
    }

    [Inspected]
    public T MaxValue
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

    protected abstract bool Compare(T a, T b);

    protected virtual void Correct()
    {
    }

    void IComputeParameter.ResetResetable()
    {
    }

    void IComputeParameter.CorrectValue() => this.Correct();

    void IComputeParameter.ComputeEvent()
    {
      this.mutableChecked = false;
      if (this.Compare(this.storedValue, this.value))
        return;
      this.ChangeParameterInvoke((IParameter) this);
    }
  }
}
