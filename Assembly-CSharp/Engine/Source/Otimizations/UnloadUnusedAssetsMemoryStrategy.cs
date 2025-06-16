using AssetDatabases;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using System.Collections;
using UnityEngine;

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
