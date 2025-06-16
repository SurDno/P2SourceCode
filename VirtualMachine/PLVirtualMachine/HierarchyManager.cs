// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.HierarchyManager
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Services;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using VirtualMachine.Data.Customs;

#nullable disable
namespace PLVirtualMachine
{
  public static class HierarchyManager
  {
    private static Dictionary<HierarchyGuid, IWorldHierarchyObject> worldHierarhyObjectsDict = new Dictionary<HierarchyGuid, IWorldHierarchyObject>((IEqualityComparer<HierarchyGuid>) HierarchyGuidComparer.Instance);
    private static IWorldHierarchyObject worldHierarchyRoot = (IWorldHierarchyObject) null;
    private static int hierarchyBuildingCurrentSerialNum;
    private static Dictionary<HierarchyGuid, Guid> actualHierarchyEngineGuidsTable = new Dictionary<HierarchyGuid, Guid>((IEqualityComparer<HierarchyGuid>) HierarchyGuidComparer.Instance);

    public static bool BuildStaticHierarchy()
    {
      if (HierarchyManager.worldHierarchyRoot == null)
      {
        Logger.AddError(string.Format("Static world hierarchy root not loaded, cannot build hierarchy"));
        return false;
      }
      if (!HierarchyManager.Scenes.ContainsKey(HierarchyManager.worldHierarchyRoot.Blueprint.BaseGuid))
      {
        Logger.AddError(string.Format("Game hierarchy scene structure (s_GameRoot.HierarchyScenesStructure.Scenes) is invalid"));
        return false;
      }
      HierarchyManager.OnStartHierarchyBuilding();
      HierarchyManager.RegisterGameRootEngineInstance((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot);
      HierarchySceneInfoData scene = HierarchyManager.Scenes[HierarchyManager.worldHierarchyRoot.Blueprint.BaseGuid];
      HierarchyManager.CreateVMTemplateByEngTemplateHierarchyInfo(HierarchyManager.worldHierarchyRoot, scene);
      ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).OnBuildHierarchy();
      return true;
    }

    public static bool CreateVMTemplateByEngTemplateHierarchyInfo(
      IWorldHierarchyObject worldHierarchyObject,
      HierarchySceneInfoData objSceneInfo)
    {
      HierarchyManager.RegisterHierarchyObjectEngineInstance((IHierarchyObject) worldHierarchyObject);
      for (int index = 0; index < objSceneInfo.Scenes.Count; ++index)
      {
        ulong scene = objSceneInfo.Scenes[index];
        IWorldBlueprint mountingSceneTemplate = IStaticDataContainer.StaticDataContainer.GameRoot.GetEngineTemplateByGuid(scene);
        if (mountingSceneTemplate == null)
        {
          Guid templateGuidByBaseGuid = IStaticDataContainer.StaticDataContainer.GameRoot.GetEngineTemplateGuidByBaseGuid(scene);
          IEntity template = ServiceCache.TemplateService.GetTemplate<IEntity>(templateGuidByBaseGuid);
          if (template != null)
            mountingSceneTemplate = (IWorldBlueprint) HierarchyManager.CreateVMTemplateByEngineTemplate(scene, template);
          if (mountingSceneTemplate == null)
          {
            Logger.AddError(string.Format("Can't attach scene to mounting point {0}: scene template with Guid={1} not found", (object) worldHierarchyObject.Name, (object) templateGuidByBaseGuid));
            continue;
          }
        }
        if (!typeof (IWorldBlueprint).IsAssignableFrom(mountingSceneTemplate.GetType()))
        {
          Logger.AddError(string.Format("Can't attach scene to mounting point {0}: scene template {1} with engine id={2} isn't valid world object template!!!", (object) worldHierarchyObject.Name, (object) mountingSceneTemplate.Name, (object) mountingSceneTemplate.EngineTemplateGuid));
        }
        else
        {
          IWorldHierarchyObject objectByTemplate = HierarchyManager.CreateVirtualObjectByTemplate(mountingSceneTemplate);
          worldHierarchyObject.AddHierarchyChild((IHierarchyObject) objectByTemplate);
          objectByTemplate.Parent = worldHierarchyObject;
        }
      }
      foreach (IHierarchyObject hierarchyChild in worldHierarchyObject.HierarchyChilds)
      {
        IWorldHierarchyObject worldHierarchyObject1 = (IWorldHierarchyObject) hierarchyChild;
        HierarchySceneInfoData objSceneInfo1;
        if (HierarchyManager.Scenes.TryGetValue(worldHierarchyObject1.Blueprint.BaseGuid, out objSceneInfo1))
          HierarchyManager.CreateVMTemplateByEngTemplateHierarchyInfo(worldHierarchyObject1, objSceneInfo1);
        else
          HierarchyManager.RegisterHierarchyObjectEngineInstance(hierarchyChild);
      }
      foreach (IHierarchyObject simpleChild in worldHierarchyObject.SimpleChilds)
        HierarchyManager.RegisterHierarchyObjectEngineInstance(simpleChild);
      return true;
    }

    public static void CreateChilds(VMWorldHierarchyObject worldHierarchyObject)
    {
      HierarchySceneInfoData hierarchySceneInfoData;
      if (!HierarchyManager.Scenes.TryGetValue(worldHierarchyObject.BaseGuid, out hierarchySceneInfoData))
        return;
      for (int index = 0; index < hierarchySceneInfoData.Childs.Count; ++index)
      {
        ulong child1 = hierarchySceneInfoData.Childs[index];
        IWorldBlueprint templateObject = ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetEngineTemplateByGuid(child1);
        if (templateObject == null)
        {
          Guid templateGuidByBaseGuid = ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetEngineTemplateGuidByBaseGuid(child1);
          IEntity template = ServiceCache.TemplateService.GetTemplate<IEntity>(templateGuidByBaseGuid);
          if (template != null)
            templateObject = (IWorldBlueprint) HierarchyManager.CreateVMTemplateByEngineTemplate(child1, template);
          if (templateObject == null)
          {
            templateObject = (IWorldBlueprint) HierarchyManager.CreateVMTemplateByEngineTemplate(child1, template);
            Logger.AddError(string.Format("Can't add child to scene {0}: child template with id={1} not found. Phantom hierarchy object will be created.", (object) worldHierarchyObject.Name, (object) templateGuidByBaseGuid));
          }
        }
        if (templateObject != null)
        {
          VMWorldHierarchyObject child2 = new VMWorldHierarchyObject();
          child2.Initialize(templateObject);
          worldHierarchyObject.AddHierarchyChild((IHierarchyObject) child2);
        }
        else
        {
          IHierarchyObject phantomHierarchyObject = HierarchyManager.CreatePhantomHierarchyObject(child1);
          worldHierarchyObject.AddHierarchyChild(phantomHierarchyObject);
        }
      }
    }

    public static void CreateSimpleChilds(VMWorldHierarchyObject worldHierarchyObject)
    {
      HierarchySceneInfoData hierarchySceneInfoData;
      if (!HierarchyManager.Scenes.TryGetValue(worldHierarchyObject.BaseGuid, out hierarchySceneInfoData))
        return;
      for (int index = 0; index < hierarchySceneInfoData.SimpleChilds.Count; ++index)
      {
        ulong simpleChild = hierarchySceneInfoData.SimpleChilds[index];
        IWorldBlueprint simpleChiTemplate = ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetEngineTemplateByGuid(simpleChild);
        if (simpleChiTemplate == null)
        {
          Guid templateGuidByBaseGuid = ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetEngineTemplateGuidByBaseGuid(simpleChild);
          IEntity template = ServiceCache.TemplateService.GetTemplate<IEntity>(templateGuidByBaseGuid);
          if (template != null)
            simpleChiTemplate = (IWorldBlueprint) HierarchyManager.CreateVMTemplateByEngineTemplate(simpleChild, template);
          if (simpleChiTemplate == null)
          {
            Logger.AddError(string.Format("Can't add simple child to scene {0}: child template with Guid={1} not found", (object) worldHierarchyObject.Name, (object) templateGuidByBaseGuid));
            continue;
          }
        }
        HierarchySimpleChildInfo child = new HierarchySimpleChildInfo(simpleChiTemplate);
        worldHierarchyObject.AddHierarchySimpleChild((IHierarchyObject) child);
      }
    }

    public static VMWorldObject CreateVMTemplateByEngineTemplate(
      ulong templateBaseID,
      IEntity entity)
    {
      PLVirtualMachine.Common.IObject @object = IStaticDataContainer.StaticDataContainer.GetOrCreateObject(templateBaseID);
      if (!typeof (VMWorldObject).IsAssignableFrom(@object.GetType()))
      {
        Logger.AddError(string.Format("Guid {0} has invalid template type {1}", (object) templateBaseID, (object) @object.GetType()));
        return (VMWorldObject) null;
      }
      VMWorldObject byEngineTemplate = (VMWorldObject) @object;
      byEngineTemplate.InitTemplateFromEngineDirect(entity);
      return byEngineTemplate;
    }

    public static IEntity GetBaseTemplate(IEntity entity)
    {
      IEntity entity1 = entity;
      IEntity template;
      do
      {
        template = (IEntity) entity1.Template;
        entity1 = template;
      }
      while (entity1 != null && entity1.Template != null);
      return template;
    }

    public static void Clear()
    {
      if (HierarchyManager.GameHierarchyRoot == null)
      {
        Logger.AddError(string.Format("Game hierarchy root not inited!"));
      }
      else
      {
        HierarchyManager.worldHierarchyRoot.ClearHierarchy();
        HierarchyManager.worldHierarhyObjectsDict.Clear();
        HierarchyManager.actualHierarchyEngineGuidsTable.Clear();
        HierarchyManager.worldHierarchyRoot = (IWorldHierarchyObject) null;
      }
    }

    private static void RegisterGameRootEngineInstance(VMGameRoot gameRoot)
    {
      Guid empty = Guid.Empty;
      Guid guid;
      if (HierarchyManager.hierarchyBuildingCurrentSerialNum < gameRoot.HierarchyEngineGuidsTable.Count)
      {
        guid = gameRoot.HierarchyEngineGuidsTable[HierarchyManager.hierarchyBuildingCurrentSerialNum];
      }
      else
      {
        guid = Guid.NewGuid();
        Logger.AddError(string.Format("Hierarchy engine guids table not loaded !"));
      }
      gameRoot.InitInstanceGuid(guid);
      HierarchyManager.RegistrEngineGuidInHierarchy(new HierarchyGuid(gameRoot.BaseGuid), guid, false);
      ++HierarchyManager.hierarchyBuildingCurrentSerialNum;
    }

    public static void RegisterHierarchyObjectEngineInstance(IHierarchyObject hierarchyObject)
    {
      VMGameRoot gameRoot = (VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot;
      Guid empty = Guid.Empty;
      Guid guid;
      if (HierarchyManager.hierarchyBuildingCurrentSerialNum < gameRoot.HierarchyEngineGuidsTable.Count)
      {
        guid = gameRoot.HierarchyEngineGuidsTable[HierarchyManager.hierarchyBuildingCurrentSerialNum];
      }
      else
      {
        guid = Guid.NewGuid();
        Logger.AddError(string.Format("Hierarchy engine guids table is inconsistency with hierarchy data ! Engine instance guid for object {0} with hierarchyGuid={1} will be generated new!", (object) hierarchyObject.EditorTemplate.BaseGuid, (object) hierarchyObject.HierarchyGuid));
      }
      hierarchyObject.InitInstanceGuid(guid);
      HierarchyManager.RegistrEngineGuidInHierarchy(hierarchyObject.HierarchyGuid, guid, false);
      if (typeof (IWorldHierarchyObject).IsAssignableFrom(hierarchyObject.GetType()))
        HierarchyManager.worldHierarhyObjectsDict.Add(hierarchyObject.HierarchyGuid, (IWorldHierarchyObject) hierarchyObject);
      ++HierarchyManager.hierarchyBuildingCurrentSerialNum;
    }

    public static List<IWorldHierarchyObject> GetObjectPathByHierarhyGuid(
      HierarchyGuid hierarchyGuid)
    {
      List<IWorldHierarchyObject> pathByHierarhyGuid = new List<IWorldHierarchyObject>();
      for (int index = 0; index < hierarchyGuid.Guids.Length; ++index)
      {
        PLVirtualMachine.Common.IObject objectByGuid = IStaticDataContainer.StaticDataContainer.GetObjectByGuid(hierarchyGuid.Guids[index]);
        if (objectByGuid != null && typeof (IWorldHierarchyObject).IsAssignableFrom(objectByGuid.GetType()))
          pathByHierarhyGuid.Add((IWorldHierarchyObject) objectByGuid);
      }
      return pathByHierarhyGuid;
    }

    public static string GetHierarchyObjectPathName(List<IWorldHierarchyObject> dHierarhyObjectsPath)
    {
      StringBuilder stringBuilder = new StringBuilder(1000);
      for (int index = 0; index < dHierarhyObjectsPath.Count; ++index)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append(".");
        stringBuilder.Append(dHierarhyObjectsPath[index].Name);
      }
      return stringBuilder.ToString();
    }

    public static void OnStartHierarchyBuilding()
    {
      HierarchyManager.hierarchyBuildingCurrentSerialNum = 0;
      HierarchyManager.actualHierarchyEngineGuidsTable.Clear();
    }

    public static void RegistrEngineGuidInHierarchy(
      HierarchyGuid hGuid,
      Guid whObjectEngineInstanceGuid,
      bool memoryActualTable = true)
    {
      if (HierarchyManager.actualHierarchyEngineGuidsTable.ContainsKey(hGuid))
      {
        Logger.AddError(string.Format("Hierarchy building inconsistency error: engine instance with hierarchy guid {0} has already been registered !!!", (object) hGuid.ToString()));
      }
      else
      {
        if (!memoryActualTable)
          return;
        HierarchyManager.actualHierarchyEngineGuidsTable.Add(hGuid, whObjectEngineInstanceGuid);
      }
    }

    public static IWorldHierarchyObject GetWorldHierarhyObjectByGuid(HierarchyGuid hGuid)
    {
      IWorldHierarchyObject hierarhyObjectByGuid;
      HierarchyManager.worldHierarhyObjectsDict.TryGetValue(hGuid, out hierarhyObjectByGuid);
      return hierarhyObjectByGuid;
    }

    public static void RegistrGameHierarchyRoot(IWorldHierarchyObject hierarchyRootObject)
    {
      HierarchyManager.worldHierarchyRoot = hierarchyRootObject;
    }

    public static IWorldHierarchyObject GameHierarchyRoot => HierarchyManager.worldHierarchyRoot;

    private static Dictionary<ulong, HierarchySceneInfoData> Scenes
    {
      get
      {
        return ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).HierarchyScenesStructure;
      }
    }

    private static IHierarchyObject CreatePhantomHierarchyObject(ulong templateID)
    {
      PLVirtualMachine.Common.IObject templateObject = IStaticDataContainer.StaticDataContainer.GetOrCreateObject(templateID);
      if (templateObject != null && typeof (IWorldBlueprint).IsAssignableFrom(templateObject.GetType()))
      {
        VMWorldHierarchyObject phantomHierarchyObject = new VMWorldHierarchyObject();
        phantomHierarchyObject.Initialize((IWorldBlueprint) templateObject);
        return (IHierarchyObject) phantomHierarchyObject;
      }
      Logger.AddError(string.Format("Cannot create phantom hierarchy object: invalid template guid {0}", (object) templateID));
      return (IHierarchyObject) null;
    }

    private static IWorldHierarchyObject CreateVirtualObjectByTemplate(
      IWorldBlueprint mountingSceneTemplate,
      bool bScene = false)
    {
      try
      {
        VMWorldHierarchyObject objectByTemplate = new VMWorldHierarchyObject();
        objectByTemplate.Initialize(mountingSceneTemplate);
        return (IWorldHierarchyObject) objectByTemplate;
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Virtual object creation error: {0}", (object) ex.ToString()));
      }
      return (IWorldHierarchyObject) null;
    }
  }
}
