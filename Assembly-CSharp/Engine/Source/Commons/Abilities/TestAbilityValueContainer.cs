// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Abilities.TestAbilityValueContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Connections;
using Engine.Source.Effects.Values;
using Engine.Source.Services;
using Inspectors;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace Engine.Source.Commons.Abilities
{
  [Factory(typeof (TestAbilityValueContainer))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class TestAbilityValueContainer : EngineObject, IAbilityValueContainer
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    [CopyableProxy(MemberEnum.None)]
    protected List<AbilityValueInfo> values = new List<AbilityValueInfo>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    [CopyableProxy(MemberEnum.None)]
    protected UnityAsset<GameObject> blueprint;

    public IAbilityValue<T> GetAbilityValue<T>(AbilityValueNameEnum name) where T : struct
    {
      AbilityValueInfo abilityValueInfo = this.values.FirstOrDefault<AbilityValueInfo>((Func<AbilityValueInfo, bool>) (o => o.Name == name));
      return abilityValueInfo != null ? abilityValueInfo.Value as IAbilityValue<T> : (IAbilityValue<T>) null;
    }

    [Inspected]
    private void CreateEffect()
    {
      BlueprintServiceUtility.Start(this.blueprint, (IAbilityValueContainer) this, (IEntity) null);
    }
  }
}
