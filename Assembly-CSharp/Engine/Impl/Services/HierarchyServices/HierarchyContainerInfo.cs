// Decompiled with JetBrains decompiler
// Type: Engine.Impl.Services.HierarchyServices.HierarchyContainerInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using AssetDatabases;
using Inspectors;
using System.Collections.Generic;

#nullable disable
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
