// Decompiled with JetBrains decompiler
// Type: Expressions.DifficultyFloatOperation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Engine.Common.Generator;
using Engine.Source.Commons;
using Engine.Source.Commons.Effects;
using Engine.Source.Difficulties;
using Inspectors;

#nullable disable
namespace Expressions
{
  [TypeName(TypeName = "[difficulty(name)] : float", MenuItem = "difficulty(name)/float")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class DifficultyFloatOperation : IValue<float>
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Header = true)]
    [Inspected(Name = "name", Mutable = true, Mode = ExecuteMode.Edit)]
    protected string name;

    public float GetValue(IEffect context)
    {
      Engine.Source.Settings.IValue<float> valueItem = InstanceByRequest<DifficultySettings>.Instance.GetValueItem(this.name);
      return valueItem != null ? valueItem.Value : 0.0f;
    }

    public string ValueView => "difficulty(" + this.name + ")";

    public string TypeView => TypeUtility.GetTypeName(this.GetType());
  }
}
