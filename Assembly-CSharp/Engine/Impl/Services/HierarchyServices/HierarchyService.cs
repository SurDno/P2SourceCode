// Decompiled with JetBrains decompiler
// Type: Engine.Impl.Services.HierarchyServices.HierarchyService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Comparers;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace Engine.Impl.Services.HierarchyServices
{
  [Depend(typeof (ITemplateService))]
  [RuntimeService(new Type[] {typeof (HierarchyService)})]
  public class HierarchyService : IInitialisable
  {
    private Dictionary<IEntity, HierarchyItem> templates = new Dictionary<IEntity, HierarchyItem>();
    private Dictionary<Guid, HierarchyContainer> containers = new Dictionary<Guid, HierarchyContainer>((IEqualityComparer<Guid>) GuidComparer.Instance);
    private HierarchyContainer mainContainer;
    private bool initialise;

    [Inspected]
    public HierarchyContainer MainContainer
    {
      get
      {
        if (this.mainContainer == null)
          throw new Exception("!!! " + this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        if (!this.initialise)
          throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        return this.mainContainer;
      }
    }

    [Inspected]
    private HierarchyContainerInfo SceneHierarchy
    {
      get
      {
        if (this.mainContainer == null)
          throw new Exception("!!! " + this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        if (!this.initialise)
          throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
        return new HierarchyContainerInfo(this.MainContainer);
      }
    }

    public void CreateMainContainer()
    {
      this.mainContainer = (HierarchyContainer) null;
      this.templates.Clear();
      this.containers.Clear();
      this.mainContainer = new HierarchyContainer(ServiceLocator.GetService<ITemplateService>().GetTemplate<IScene>(Ids.MainId), this.containers, this.templates);
    }

    public void Initialise() => this.initialise = true;

    public void Terminate()
    {
      this.mainContainer = (HierarchyContainer) null;
      this.templates.Clear();
      this.containers.Clear();
      this.initialise = false;
    }

    public HierarchyItem GetItem(IEntity template)
    {
      if (this.mainContainer == null)
        throw new Exception("!!! " + this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      HierarchyItem hierarchyItem;
      this.templates.TryGetValue(template, out hierarchyItem);
      return hierarchyItem;
    }

    public HierarchyContainer GetContainer(Guid id)
    {
      if (this.mainContainer == null)
        throw new Exception("!!! " + this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      if (!this.initialise)
        throw new Exception(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);
      HierarchyContainer container;
      this.containers.TryGetValue(id, out container);
      return container;
    }
  }
}
