using Cofe.Utility;
using Engine.Common.Generator;
using Engine.Source.Commons;
using Engine.Source.Commons.Effects;
using Engine.Source.Difficulties;
using Inspectors;

namespace Expressions
{
  [TypeName(TypeName = "[difficulty(name)] : float", MenuItem = "difficulty(name)/float")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class DifficultyFloatOperation : IValue<float>
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected(Header = true)]
    [Inspected(Name = "name", Mutable = true, Mode = ExecuteMode.Edit)]
    protected string name;

    public float GetValue(IEffect context)
    {
      Engine.Source.Settings.IValue<float> valueItem = InstanceByRequest<DifficultySettings>.Instance.GetValueItem(name);
      return valueItem != null ? valueItem.Value : 0.0f;
    }

    public string ValueView => "difficulty(" + name + ")";

    public string TypeView => TypeUtility.GetTypeName(GetType());
  }
}
