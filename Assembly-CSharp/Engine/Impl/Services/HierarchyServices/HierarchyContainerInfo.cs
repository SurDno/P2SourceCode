using AssetDatabases;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Impl.Services.HierarchyServices
{
  public struct HierarchyContainerInfo
  {
    private HierarchyContainer container;

    [Inspected(Header = true)]
    public string Name => AssetDatabaseUtility.GetFileName(this.Path);

    [Inspected]
    public string Path => AssetDatabaseService.Instance.GetPath(this.container.Id);

    [Inspected]
    public IEnumerable<HierarchyContainerInfo> Childs
    {
      get
      {
        foreach (HierarchyContainerInfo container in HierarchyContainerInfo.GetContainers(this.container.Items))
        {
          HierarchyContainerInfo child = container;
          yield return child;
          child = new HierarchyContainerInfo();
        }
      }
    }

    public HierarchyContainerInfo(HierarchyContainer container) => this.container = container;

    private static IEnumerable<HierarchyContainerInfo> GetContainers(
      IEnumerable<HierarchyItem> items)
    {
      foreach (HierarchyItem item in items)
      {
        if (item.Container != null)
          yield return new HierarchyContainerInfo(item.Container);
        foreach (HierarchyContainerInfo container in HierarchyContainerInfo.GetContainers(item.Items))
        {
          HierarchyContainerInfo scene = container;
          yield return scene;
          scene = new HierarchyContainerInfo();
        }
      }
    }
  }
}
