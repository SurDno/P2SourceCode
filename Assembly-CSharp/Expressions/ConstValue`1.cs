using Cofe.Utility;
using Engine.Common.Generator;
using Engine.Source.Commons.Effects;
using Inspectors;

namespace Expressions
{
  public abstract class ConstValue<T> : IValue<T> where T : struct
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Header = true)]
    [Inspected(Name = "value", Mutable = true, Mode = ExecuteMode.Edit)]
    protected T value;

    public T GetValue(IEffect context) => this.value;

    public string ValueView => this.value.ToString();

    public string TypeView => TypeUtility.GetTypeName(this.GetType());
  }
}
