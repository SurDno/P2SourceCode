// Decompiled with JetBrains decompiler
// Type: Engine.Impl.Services.HierarchyServices.HierarchyContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Assets.Internal.Reference;
using Engine.Common;
using Engine.Common.Comparers;
using Inspectors;
using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Impl.Services.HierarchyServices
{
  public class HierarchyContainer
  {
    private List<HierarchyItem> items = new List<HierarchyItem>();
    private Dictionary<Guid, HierarchyItem> hierarchyItems = new Dictionary<Guid, HierarchyItem>((IEqualityComparer<Guid>) GuidComparer.Instance);

    [Inspected]
    public Guid Id { get; private set; }

    [Inspected]
    public IEnumerable<HierarchyItem> Items => (IEnumerable<HierarchyItem>) this.items;

    public HierarchyItem GetItemByTemplateId(Guid templateId)
    {
      HierarchyItem itemByTemplateId;
      this.hierarchyItems.TryGetValue(templateId, out itemByTemplateId);
      return itemByTemplateId;
    }

    public HierarchyContainer(
      IScene scene,
      Dictionary<Guid, HierarchyContainer> containers,
      Dictionary<IEntity, HierarchyItem> templates)
    {
      containers.Add(scene.Id, this);
      this.Id = scene.Id;
      foreach (SceneObjectItem reference in ((SceneObject) scene).Items)
        this.items.Add(new HierarchyItem(reference, containers, templates, this.hierarchyItems));
    }
  }
}
