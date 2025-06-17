using Engine.Common.Generator;
using Engine.Source.Commons.Effects;
using Inspectors;

namespace Expressions
{
  public abstract class ConditionOperation<T> : IValue<T> where T : struct
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Name = "condition", Mutable = true, Mode = ExecuteMode.Edit)]
    protected IValue<bool> condition;
    [DataReadProxy(Name = "True")]
    [DataWriteProxy(Name = "True")]
    [CopyableProxy]
    [Inspected]
    [Inspected(Name = "true", Mutable = true, Mode = ExecuteMode.Edit)]
    protected IValue<T> trueResult;
    [DataReadProxy(Name = "False")]
    [DataWriteProxy(Name = "False")]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Name = "false", Mutable = true, Mode = ExecuteMode.Edit)]
    protected IValue<T> falseResult;

    public T GetValue(IEffect context)
    {
      return condition != null && trueResult != null && falseResult != null ? (condition.GetValue(context) ? trueResult.GetValue(context) : falseResult.GetValue(context)) : default (T);
    }

    public string ValueView => "(" + (condition != null ? condition.ValueView : "null") + " ? " + (trueResult != null ? trueResult.ValueView : "null") + " : " + (falseResult != null ? falseResult.ValueView : "null") + ")";

    public string TypeView => "(" + (condition != null ? condition.TypeView : "null") + " ? " + (trueResult != null ? trueResult.TypeView : "null") + " : " + (falseResult != null ? falseResult.TypeView : "null") + ")";
  }
}
