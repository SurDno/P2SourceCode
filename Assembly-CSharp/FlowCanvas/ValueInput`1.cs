using System;

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

    public T value => getter != null ? getter() : _value;

    public override object GetValue() => value;

    public override bool isDefaultValue
    {
      get => Equals(_value, default (T));
    }

    public override object serializedValue
    {
      get => _value;
      set => _value = (T) value;
    }

    public override Type type => typeof (T);

    public override void BindTo(ValueOutput source)
    {
      if (source is ValueOutput<T>)
      {
        getter = (source as ValueOutput<T>).getter;
      }
      else
      {
        if (TypeConverter.TryConnect(this, source))
          return;
        ValueHandler<object> func = TypeConverter.GetConverterFuncFromTo(typeof (T), source);
        getter = () => (T) func();
      }
    }

    public void BindTo(ValueHandler<T> getter) => this.getter = getter;

    public override void UnBind() => getter = null;
  }
}
