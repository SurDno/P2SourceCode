// Decompiled with JetBrains decompiler
// Type: Expressions.ConditionOperation`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using Engine.Source.Commons.Effects;
using Inspectors;

#nullable disable
namespace Expressions
{
  public abstract class ConditionOperation<T> : IValue<T> where T : struct
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Name = "condition", Mutable = true, Mode = ExecuteMode.Edit)]
    protected IValue<bool> condition;
    [DataReadProxy(MemberEnum.None, Name = "True")]
    [DataWriteProxy(MemberEnum.None, Name = "True")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Name = "true", Mutable = true, Mode = ExecuteMode.Edit)]
    protected IValue<T> trueResult;
    [DataReadProxy(MemberEnum.None, Name = "False")]
    [DataWriteProxy(MemberEnum.None, Name = "False")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Name = "false", Mutable = true, Mode = ExecuteMode.Edit)]
    protected IValue<T> falseResult;

    public T GetValue(IEffect context)
    {
      return this.condition != null && this.trueResult != null && this.falseResult != null ? (this.condition.GetValue(context) ? this.trueResult.GetValue(context) : this.falseResult.GetValue(context)) : default (T);
    }

    public string ValueView
    {
      get
      {
        return "(" + (this.condition != null ? this.condition.ValueView : "null") + " ? " + (this.trueResult != null ? this.trueResult.ValueView : "null") + " : " + (this.falseResult != null ? this.falseResult.ValueView : "null") + ")";
      }
    }

    public string TypeView
    {
      get
      {
        return "(" + (this.condition != null ? this.condition.TypeView : "null") + " ? " + (this.trueResult != null ? this.trueResult.TypeView : "null") + " : " + (this.falseResult != null ? this.falseResult.TypeView : "null") + ")";
      }
    }
  }
}
