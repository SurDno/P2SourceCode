// Decompiled with JetBrains decompiler
// Type: FlowCanvas.ValueInput
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion;
using System;

#nullable disable
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
      }), (object) parent, (object) name, (object) ID);
    }

    public object value => this.GetValue();

    public abstract void BindTo(ValueOutput target);

    public abstract void UnBind();

    public abstract object GetValue();

    public abstract object serializedValue { get; set; }

    public abstract bool isDefaultValue { get; }

    public abstract override Type type { get; }
  }
}
