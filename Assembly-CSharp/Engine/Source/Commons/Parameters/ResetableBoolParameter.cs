using System.Reflection;
using Cofe.Utility;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Inspectors;

namespace Engine.Source.Commons.Parameters
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad | TypeEnum.NeedSave)]
  public class ResetableBoolParameter : 
    ParameterListener,
    IParameter<bool>,
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
    protected bool value;
    [DataReadProxy]
    [DataWriteProxy]
    [StateSaveProxy]
    [StateLoadProxy]
    [CopyableProxy()]
    [NeedSaveProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool baseValue;
    private bool storedValue;
    private bool mutableChecked;

    [Inspected(Header = true)]
    public ParameterNameEnum Name => name;

    [Inspected(Header = true, Mutable = true)]
    public bool Value
    {
      get => value;
      set
      {
        CheckMutable();
        this.value = value;
      }
    }

    [Inspected]
    public bool BaseValue
    {
      get => baseValue;
      set
      {
        CheckMutable();
        baseValue = value;
      }
    }

    [Inspected]
    public bool MinValue
    {
      get => value;
      set
      {
        Debug.LogError((object) ("Parameter : " + name + " , type : " + TypeUtility.GetTypeName(GetType()) + " , property : " + MethodBase.GetCurrentMethod().Name + " not supported setter"));
      }
    }

    [Inspected]
    public bool MaxValue
    {
      get => value;
      set
      {
        Debug.LogError((object) ("Parameter : " + name + " , type : " + TypeUtility.GetTypeName(GetType()) + " , property : " + MethodBase.GetCurrentMethod().Name + " not supported setter"));
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

    void IComputeParameter.ResetResetable() => Value = baseValue;

    void IComputeParameter.CorrectValue()
    {
    }

    void IComputeParameter.ComputeEvent()
    {
      mutableChecked = false;
      if (storedValue == value)
        return;
      ChangeParameterInvoke(this);
    }
  }
}
