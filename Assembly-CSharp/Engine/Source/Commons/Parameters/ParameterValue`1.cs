// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Parameters.ParameterValue`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.Parameters;
using Inspectors;
using System;

#nullable disable
namespace Engine.Source.Commons.Parameters
{
  public class ParameterValue<T> : 
    IParameterValue<T>,
    IParameterValueSet<T>,
    IChangeParameterListener
    where T : struct
  {
    [Inspected]
    private IParameter<T> parameter;

    public void Set(IParameter<T> parameter)
    {
      if (this.parameter != null)
        this.parameter.RemoveListener((IChangeParameterListener) this);
      this.parameter = parameter;
      if (this.parameter == null)
        return;
      this.parameter.AddListener((IChangeParameterListener) this);
    }

    public void OnParameterChanged(IParameter parameter)
    {
      Action<T> changeValueEvent = this.ChangeValueEvent;
      if (changeValueEvent == null)
        return;
      changeValueEvent(((IParameter<T>) parameter).Value);
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
  }
}
