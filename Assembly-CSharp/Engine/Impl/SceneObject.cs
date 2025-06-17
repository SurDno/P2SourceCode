using System.Collections.Generic;
using Engine.Assets.Internal.Reference;
using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;

namespace Engine.Impl
{
  [Factory(typeof (IScene))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SceneObject : EngineObject, IScene, IObject
  {
    [DataReadProxy]
    [DataWriteProxy()]
    [Inspected]
    public List<SceneObjectItem> Items = [];

    public void Sort() => Sort(Items);

    private void Sort(List<SceneObjectItem> items)
    {
      items.Sort((a, b) => a.Id.CompareTo(b.Id));
      foreach (SceneObjectItem sceneObjectItem in items)
        Sort(sceneObjectItem.Items);
    }

    public IEnumerable<SceneObjectItem> GetChilds()
    {
      foreach (SceneObjectItem child in Items)
      {
        yield return child;
        foreach (SceneObjectItem child2 in GetChilds(child))
          yield return child2;
      }
    }

    private static IEnumerable<SceneObjectItem> GetChilds(SceneObjectItem item)
    {
      foreach (SceneObjectItem child in item.Items)
      {
        yield return child;
        foreach (SceneObjectItem child2 in GetChilds(child))
          yield return child2;
      }
    }
  }
}
