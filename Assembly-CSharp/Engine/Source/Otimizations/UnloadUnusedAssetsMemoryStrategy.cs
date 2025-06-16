// Decompiled with JetBrains decompiler
// Type: Engine.Source.Otimizations.UnloadUnusedAssetsMemoryStrategy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using AssetDatabases;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using System.Collections;
using UnityEngine;

#nullable disable
namespace Engine.Source.Otimizations
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class UnloadUnusedAssetsMemoryStrategy : IMemoryStrategy
  {
    public IEnumerator Compute(MemoryStrategyContextEnum context)
    {
      yield return (object) this.Compute();
    }

    private IEnumerator Compute()
    {
      while (!SceneController.CanLoad)
        yield return (object) null;
      SceneController.Disabled = true;
      yield return (object) Resources.UnloadUnusedAssets();
      SceneController.Disabled = false;
    }
  }
}
