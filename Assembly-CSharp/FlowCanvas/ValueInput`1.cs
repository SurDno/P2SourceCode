// Decompiled with JetBrains decompiler
// Type: FlowCanvas.ValueInput`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FlowCanvas
{
  public class ValueInput<T> : ValueInput, IValueInput<T>
  {
    private T _value;

    public ValueInput()
    {
    }

    public ValueInput(FlowNode parent, string name, string id)
      : base(parent, name, id)
    {
    }

    public ValueHandler<T> getter { get; set; }

    public T value => this.getter != null ? this.getter() : this._value;

    public override object GetValue() => (object) this.value;

    public override bool isDefaultValue
    {
      get => object.Equals((object) this._value, (object) default (T));
    }

    public override object serializedValue
    {
      get => (object) this._value;
      set => this._value = (T) value;
    }

    public override Type type => typeof (T);

    public override void BindTo(ValueOutput source)
    {
      if (source is ValueOutput<T>)
      {
        this.getter = (source as ValueOutput<T>).getter;
      }
      else
      {
        if (TypeConverter.TryConnect((object) this, source))
          return;
        ValueHandler<object> func = TypeConverter.GetConverterFuncFromTo(typeof (T), source);
        this.getter = (ValueHandler<T>) (() => (T) func());
      }
    }

    public void BindTo(ValueHandler<T> getter) => this.getter = getter;

    public override void UnBind() => this.getter = (ValueHandler<T>) null;
  }
}
