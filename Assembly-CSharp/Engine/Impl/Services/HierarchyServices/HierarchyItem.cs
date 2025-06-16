using System;
using System.Collections.Generic;
using Engine.Assets.Internal.Reference;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Components;
using Inspectors;

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
    public IEnumerable<HierarchyItem> Items => items;

    [Inspected]
    public IEntity Template { get; private set; }

    public HierarchyItem(HierarchyContainer container) => Container = container;

    public HierarchyItem(
      SceneObjectItem reference,
      Dictionary<Guid, HierarchyContainer> containers,
      Dictionary<IEntity, HierarchyItem> templates,
      Dictionary<Guid, HierarchyItem> hierarchyItems)
    {
      Reference = reference;
      foreach (SceneObjectItem reference1 in Reference.Items)
        items.Add(new HierarchyItem(reference1, containers, templates, hierarchyItems));
      ComputeItem(containers, templates, hierarchyItems);
    }

    private void ComputeItem(
      Dictionary<Guid, HierarchyContainer> containers,
      Dictionary<IEntity, HierarchyItem> templates,
      Dictionary<Guid, HierarchyItem> hierarchyItems)
    {
      Template = GetTemplate(Reference);
      if (Template == null)
      {
        Debug.LogWarning((object) ("Template not found : " + Reference.Id));
      }
      else
      {
        hierarchyItems.Add(Template.Id, this);
        if (!templates.ContainsKey(Template))
          templates.Add(Template, this);
        StaticModelComponent component = Template.GetComponent<StaticModelComponent>();
        if (component == null)
          return;
        Guid id = component.Connection.Id;
        if (id != Guid.Empty)
        {
          HierarchyContainer hierarchyContainer;
          if (containers.TryGetValue(id, out hierarchyContainer))
          {
            Container = hierarchyContainer;
          }
          else
          {
            IScene template = ServiceLocator.GetService<ITemplateService>().GetTemplate<IScene>(id);
            if (template != null)
              Container = new HierarchyContainer(template, containers, templates);
            else
              Debug.LogError((object) (typeof (SceneObject).Name + " not found, id : " + id + " , item : " + Template.GetInfo()));
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
