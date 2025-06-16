using System.Collections;
using AssetDatabases;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using UnityEngine;

namespace Engine.Source.Otimizations
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class UnloadUnusedAssetsMemoryStrategy : IMemoryStrategy
  {
    public IEnumerator Compute(MemoryStrategyContextEnum context)
    {
      yield return Compute();
    }

    private IEnumerator Compute()
    {
      while (!SceneController.CanLoad)
        yield return null;
      SceneController.Disabled = true;
      yield return Resources.UnloadUnusedAssets();
      SceneController.Disabled = false;
    }
  }
}
