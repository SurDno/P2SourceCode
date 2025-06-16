// Decompiled with JetBrains decompiler
// Type: FlowCanvas.ValueOutput
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion;
using System;

#nullable disable
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
