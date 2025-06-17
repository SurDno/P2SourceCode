using System;
using System.Collections.Generic;
using System.Reflection;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Comparers;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;

namespace Engine.Impl.Services.HierarchyServices
{
  [Depend(typeof (ITemplateService))]
  [RuntimeService(typeof (HierarchyService))]
  public class HierarchyService : IInitialisable
  {
    private Dictionary<IEntity, HierarchyItem> templates = new();
    private Dictionary<Guid, HierarchyContainer> containers = new(GuidComparer.Instance);
    private HierarchyContainer mainContainer;
    private bool initialise;

    [Inspected]
    public HierarchyContainer MainContainer
    {
      get
      {
        if (mainContainer == null)
          throw new Exception("!!! " + GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        if (!initialise)
          throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        return mainContainer;
      }
    }

    [Inspected]
    private HierarchyContainerInfo SceneHierarchy
    {
      get
      {
        if (mainContainer == null)
          throw new Exception("!!! " + GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        if (!initialise)
          throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        return new HierarchyContainerInfo(MainContainer);
      }
    }

    public void CreateMainContainer()
    {
      mainContainer = null;
      templates.Clear();
      containers.Clear();
      mainContainer = new HierarchyContainer(ServiceLocator.GetService<ITemplateService>().GetTemplate<IScene>(Ids.MainId), containers, templates);
    }

    public void Initialise() => initialise = true;

    public void Terminate()
    {
      mainContainer = null;
      templates.Clear();
      containers.Clear();
      initialise = false;
    }

    public HierarchyItem GetItem(IEntity template)
    {
      if (mainContainer == null)
        throw new Exception("!!! " + GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      if (!initialise)
        throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      templates.TryGetValue(template, out HierarchyItem hierarchyItem);
      return hierarchyItem;
    }

    public HierarchyContainer GetContainer(Guid id)
    {
      if (mainContainer == null)
        throw new Exception("!!! " + GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      if (!initialise)
        throw new Exception(GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      containers.TryGetValue(id, out HierarchyContainer container);
      return container;
    }
  }
}
