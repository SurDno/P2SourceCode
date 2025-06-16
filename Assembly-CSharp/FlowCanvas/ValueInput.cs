using System;
using ParadoxNotion;

namespace FlowCanvas
{
  public abstract class ValueInput : Port
  {
    public ValueInput()
    {
    }

    public ValueInput(FlowNode parent, string name, string ID)
      : base(parent, name, ID)
    {
    }

    public static ValueInput CreateInstance(Type t, FlowNode parent, string name, string ID)
    {
      return (ValueInput) Activator.CreateInstance(typeof (ValueInput<>).RTMakeGenericType(new Type[1]
      {
        t
      }), parent, name, ID);
    }

    public object value => GetValue();

    public abstract void BindTo(ValueOutput target);

    public abstract void UnBind();

    public abstract object GetValue();

    public abstract object serializedValue { get; set; }

    public abstract bool isDefaultValue { get; }

    public abstract override Type type { get; }
  }
}
