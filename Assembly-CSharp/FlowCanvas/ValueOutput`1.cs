// Decompiled with JetBrains decompiler
// Type: FlowCanvas.ValueOutput`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FlowCanvas
{
  public class ValueOutput<T> : ValueOutput
  {
    public ValueOutput()
    {
    }

    public ValueOutput(FlowNode parent, string name, string ID, ValueHandler<T> getter)
      : base(parent, name, ID)
    {
      this.getter = getter;
    }

    public ValueOutput(FlowNode parent, string name, string ID, ValueHandler<object> getter)
      : base(parent, name, ID)
    {
      this.getter = getter as ValueHandler<T>;
      if (this.getter != null)
        return;
      this.getter = (ValueHandler<T>) (() => (T) getter());
    }

    public ValueHandler<T> getter { get; set; }

    public override object GetValue() => (object) this.getter();

    public override Type type => typeof (T);
  }
}
