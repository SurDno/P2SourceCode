// Decompiled with JetBrains decompiler
// Type: Engine.Impl.Services.HierarchyServices.HierarchyItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Assets.Internal.Reference;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Components;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Impl.Services.HierarchyServices
{
  public class HierarchyItem
  {
    private List<HierarchyItem> items = new List<HierarchyItem>();

    [Inspected]
    public SceneObjectItem Reference { get; private set; }

    [Inspected]
    public HierarchyContainer Container { get; private set; }

    [Inspected]
    public IEnumerable<HierarchyItem> Items => (IEnumerable<HierarchyItem>) this.items;

    [Inspected]
    public IEntity Template { get; private set; }

    public HierarchyItem(HierarchyContainer container) => this.Container = container;

    public HierarchyItem(
      SceneObjectItem reference,
      Dictionary<Guid, HierarchyContainer> containers,
      Dictionary<IEntity, HierarchyItem> templates,
      Dictionary<Guid, HierarchyItem> hierarchyItems)
    {
      this.Reference = reference;
      foreach (SceneObjectItem reference1 in this.Reference.Items)
        this.items.Add(new HierarchyItem(reference1, containers, templates, hierarchyItems));
      this.ComputeItem(containers, templates, hierarchyItems);
    }

    private void ComputeItem(
      Dictionary<Guid, HierarchyContainer> containers,
      Dictionary<IEntity, HierarchyItem> templates,
      Dictionary<Guid, HierarchyItem> hierarchyItems)
    {
      this.Template = HierarchyItem.GetTemplate(this.Reference);
      if (this.Template == null)
      {
        Debug.LogWarning((object) ("Template not found : " + (object) this.Reference.Id));
      }
      else
      {
        hierarchyItems.Add(this.Template.Id, this);
        if (!templates.ContainsKey(this.Template))
          templates.Add(this.Template, this);
        StaticModelComponent component = this.Template.GetComponent<StaticModelComponent>();
        if (component == null)
          return;
        Guid id = component.Connection.Id;
        if (id != Guid.Empty)
        {
          HierarchyContainer hierarchyContainer;
          if (containers.TryGetValue(id, out hierarchyContainer))
          {
            this.Container = hierarchyContainer;
          }
          else
          {
            IScene template = ServiceLocator.GetService<ITemplateService>().GetTemplate<IScene>(id);
            if (template != null)
              this.Container = new HierarchyContainer(template, containers, templates);
            else
              Debug.LogError((object) (typeof (SceneObject).Name + " not found, id : " + (object) id + " , item : " + this.Template.GetInfo()));
          }
        }
      }
    }

    private static IEntity GetTemplate(SceneObjectItem item)
    {
      return ServiceLocator.GetService<ITemplateService>().GetTemplate<IEntity>(item.Id) ?? item.Template.Value;
    }
  }
}
