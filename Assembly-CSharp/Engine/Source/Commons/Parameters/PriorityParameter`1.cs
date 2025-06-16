// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Parameters.PriorityParameter`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Utility;
using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Inspectors;
using System.Reflection;
using UnityEngine;

#nullable disable
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
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [StateSaveProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum name;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected PriorityContainer<T> container = ProxyFactory.Create<PriorityContainer<T>>();
    private T storedValue;
    private bool mutableChecked;

    [Inspected(Header = true)]
    public ParameterNameEnum Name => this.name;

    [Inspected(Header = true)]
    public T Value
    {
      get => this.container.Value;
      set
      {
        this.CheckMutable();
        this.container.Value = value;
      }
    }

    [Inspected]
    public T BaseValue
    {
      get => default (T);
      set
      {
        Debug.LogError((object) ("Parameter : " + (object) this.name + " , type : " + TypeUtility.GetTypeName(this.GetType()) + " , property : " + MethodBase.GetCurrentMethod().Name + " not supported setter"));
      }
    }

    [Inspected]
    public T MinValue
    {
      get => default (T);
      set
      {
        Debug.LogError((object) ("Parameter : " + (object) this.name + " , type : " + TypeUtility.GetTypeName(this.GetType()) + " , property : " + MethodBase.GetCurrentMethod().Name + " not supported setter"));
      }
    }

    [Inspected]
    public T MaxValue
    {
      get => default (T);
      set
      {
        Debug.LogError((object) ("Parameter : " + (object) this.name + " , type : " + TypeUtility.GetTypeName(this.GetType()) + " , property : " + MethodBase.GetCurrentMethod().Name + " not supported setter"));
      }
    }

    [Inspected]
    public bool Resetable => false;

    [Inspected]
    public bool NeedSave => this.container.NeedSave;

    public object ValueData => (object) this.Value;

    public void SetValue(PriorityParameterEnum priority, T value)
    {
      this.CheckMutable();
      this.container.SetValue(priority, value);
    }

    public bool TryGetValue(PriorityParameterEnum priority, out T result)
    {
      return this.container.TryGetValue(priority, out result);
    }

    public void ResetValue(PriorityParameterEnum priority)
    {
      this.CheckMutable();
      this.container.ResetValue(priority);
    }

    private void CheckMutable()
    {
      if (this.mutableChecked)
        return;
      this.mutableChecked = true;
      this.storedValue = this.Value;
      ServiceLocator.GetService<ParametersUpdater>().AddParameter((IComputeParameter) this);
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
      this.mutableChecked = false;
      if (this.Compare(this.storedValue, this.Value))
        return;
      this.ChangeParameterInvoke((IParameter) this);
    }
  }
}
