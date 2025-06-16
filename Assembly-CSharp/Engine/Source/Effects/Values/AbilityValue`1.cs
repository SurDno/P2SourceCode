// Decompiled with JetBrains decompiler
// Type: Engine.Source.Effects.Values.AbilityValue`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using Inspectors;

#nullable disable
namespace Engine.Source.Effects.Values
{
  public class AbilityValue<T> : IAbilityValue<T>, IAbilityValue where T : struct
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    [CopyableProxy(MemberEnum.None)]
    protected T value;

    public T Value
    {
      get => this.value;
      set => this.value = value;
    }
  }
}
