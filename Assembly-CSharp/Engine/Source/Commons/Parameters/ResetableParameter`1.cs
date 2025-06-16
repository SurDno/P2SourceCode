// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Parameters.ResetableParameter`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Inspectors;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace Engine.Source.Commons.Parameters
{
  public abstract class ResetableParameter<T> : 
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
    protected T baseValue;
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
      get => this.baseValue;
      set
      {
        this.CheckMutable();
        this.baseValue = value;
      }
    }

    [Inspected]
    public T MinValue
    {
      get => this.value;
      set
      {
        Debug.LogError((object) ("Parameter : " + (object) this.name + " , type : " + TypeUtility.GetTypeName(this.GetType()) + " , property : " + MethodBase.GetCurrentMethod().Name + " not supported setter"));
      }
    }

    [Inspected]
    public T MaxValue
    {
      get => this.value;
      set
      {
        Debug.LogError((object) ("Parameter : " + (object) this.name + " , type : " + TypeUtility.GetTypeName(this.GetType()) + " , property : " + MethodBase.GetCurrentMethod().Name + " not supported setter"));
      }
    }

    [Inspected]
    public bool Resetable => true;

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

    void IComputeParameter.ResetResetable() => this.Value = this.baseValue;

    void IComputeParameter.CorrectValue()
    {
    }

    void IComputeParameter.ComputeEvent()
    {
      this.mutableChecked = false;
      if (this.Compare(this.storedValue, this.value))
        return;
      this.ChangeParameterInvoke((IParameter) this);
    }
  }
}
