using ParadoxNotion;
using System;

namespace FlowCanvas
{
  public abstract class ValueOutput : Port
  {
    public ValueOutput()
    {
    }

    public ValueOutput(FlowNode parent, string name, string id)
      : base(parent, name, id)
    {
    }

    public static ValueOutput CreateInstance(
      Type type,
      FlowNode parent,
      string name,
      string id,
      ValueHandler<object> getter)
    {
      if (getter == null)
        getter = (ValueHandler<object>) (() => (object) null);
      return (ValueOutput) Activator.CreateInstance(typeof (ValueOutput<>).RTMakeGenericType(new Type[1]
      {
        type
      }), (object) parent, (object) name, (object) id, (object) getter);
    }

    public abstract object GetValue();
  }
}
