// Decompiled with JetBrains decompiler
// Type: Expressions.PolyOperation`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using Engine.Source.Commons.Effects;
using Inspectors;
using System.Collections.Generic;

#nullable disable
namespace Expressions
{
  public abstract class PolyOperation<T> : IValue<T> where T : struct
  {
    [DataReadProxy(MemberEnum.None, Name = "Parameters")]
    [DataWriteProxy(MemberEnum.None, Name = "Parameters")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Name = "values", Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<IValue<T>> values = new List<IValue<T>>();

    protected abstract T Compute(T a, T b);

    public T GetValue(IEffect context)
    {
      T a = default (T);
      foreach (IValue<T> obj in this.values)
      {
        if (obj != null)
          a = this.Compute(a, obj.GetValue(context));
      }
      return a;
    }

    protected abstract string OperatorView();

    public string ValueView
    {
      get
      {
        string str = "(";
        if (this.values.Count != 0)
        {
          for (int index = 0; index < this.values.Count; ++index)
          {
            IValue<T> obj = this.values[index];
            str = str + (obj != null ? obj.ValueView : "null") + (index < this.values.Count - 1 ? " " + this.OperatorView() + " " : "");
          }
        }
        return str + ")";
      }
    }

    public string TypeView
    {
      get
      {
        string str = "(";
        if (this.values.Count != 0)
        {
          for (int index = 0; index < this.values.Count; ++index)
          {
            IValue<T> obj = this.values[index];
            str = str + (obj != null ? obj.TypeView : "null") + (index < this.values.Count - 1 ? " " + this.OperatorView() + " " : "");
          }
        }
        return str + ")";
      }
    }
  }
}
