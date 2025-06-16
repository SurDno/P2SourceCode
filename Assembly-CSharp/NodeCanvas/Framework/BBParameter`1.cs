// Decompiled with JetBrains decompiler
// Type: NodeCanvas.Framework.BBParameter`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace NodeCanvas.Framework
{
  [Serializable]
  public class BBParameter<T> : BBParameter
  {
    private Func<T> getter;
    private Action<T> setter;
    [SerializeField]
    protected T _value;

    public BBParameter()
    {
    }

    public BBParameter(T value) => this._value = value;

    public T value
    {
      get
      {
        if (this.getter != null)
          return this.getter();
        if (!Application.isPlaying || !((UnityEngine.Object) this.bb != (UnityEngine.Object) null) || string.IsNullOrEmpty(this.name))
          return this._value;
        this.varRef = this.bb.GetVariable(this.name, typeof (T));
        return this.getter != null ? this.getter() : default (T);
      }
      set
      {
        if (this.setter != null)
        {
          this.setter(value);
        }
        else
        {
          if (this.isNone)
            return;
          if ((UnityEngine.Object) this.bb != (UnityEngine.Object) null && !string.IsNullOrEmpty(this.name))
          {
            this.varRef = this.PromoteToVariable(this.bb);
            Action<T> setter = this.setter;
            if (setter == null)
              return;
            setter(value);
          }
          else
            this._value = value;
        }
      }
    }

    protected override object objectValue
    {
      get => (object) this.value;
      set => this.value = (T) value;
    }

    public override System.Type varType => typeof (T);

    protected override void Bind(Variable variable)
    {
      if (variable == null)
      {
        this.getter = (Func<T>) null;
        this.setter = (Action<T>) null;
        this._value = default (T);
      }
      else
      {
        this.BindGetter(variable);
        this.BindSetter(variable);
      }
    }

    private bool BindGetter(Variable variable)
    {
      if (variable is Variable<T>)
      {
        this.getter = new Func<T>((variable as Variable<T>).GetValue);
        return true;
      }
      if (!variable.CanConvertTo(this.varType))
        return false;
      Func<object> func = variable.GetGetConverter(this.varType);
      this.getter = (Func<T>) (() => (T) func());
      return true;
    }

    private bool BindSetter(Variable variable)
    {
      if (variable is Variable<T>)
      {
        this.setter = new Action<T>((variable as Variable<T>).SetValue);
        return true;
      }
      if (!variable.CanConvertFrom(this.varType))
        return false;
      Action<object> func = variable.GetSetConverter(this.varType);
      this.setter = (Action<T>) (value => func((object) value));
      return true;
    }

    public static implicit operator BBParameter<T>(T value)
    {
      return new BBParameter<T>() { value = value };
    }
  }
}
