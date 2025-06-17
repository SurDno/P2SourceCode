using System.Collections.Generic;
using AssetDatabases;
using Inspectors;

namespace Engine.Impl.Services.HierarchyServices
{
  public struct HierarchyContainerInfo(HierarchyContainer container) {
    [Inspected(Header = true)]
    public string Name => AssetDatabaseUtility.GetFileName(Path);

    [Inspected]
    public string Path => AssetDatabaseService.Instance.GetPath(container.Id);

    [Inspected]
    public IEnumerable<HierarchyContainerInfo> Childs
    {
      get
      {
        foreach (HierarchyContainerInfo container1 in GetContainers(container.Items))
        {
          HierarchyContainerInfo child = container1;
          yield return child;
          child = new HierarchyContainerInfo();
        }
      }
    }

    private static IEnumerable<HierarchyContainerInfo> GetContainers(
      IEnumerable<HierarchyItem> items)
    {
      foreach (HierarchyItem item in items)
      {
        if (item.Container != null)
          yield return new HierarchyContainerInfo(item.Container);
        foreach (HierarchyContainerInfo container in GetContainers(item.Items))
        {
          HierarchyContainerInfo scene = container;
          yield return scene;
          scene = new HierarchyContainerInfo();
        }
      }
    }
  }
}
