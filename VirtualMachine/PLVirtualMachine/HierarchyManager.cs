using System;
using System.Collections.Generic;
using System.Text;
using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Services;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Objects;
using VirtualMachine.Data.Customs;
using IObject = PLVirtualMachine.Common.IObject;

namespace PLVirtualMachine
{
  public static class HierarchyManager
  {
    private static Dictionary<HierarchyGuid, IWorldHierarchyObject> worldHierarhyObjectsDict = new(HierarchyGuidComparer.Instance);
    private static IWorldHierarchyObject worldHierarchyRoot;
    private static int hierarchyBuildingCurrentSerialNum;
    private static Dictionary<HierarchyGuid, Guid> actualHierarchyEngineGuidsTable = new(HierarchyGuidComparer.Instance);

    public static bool BuildStaticHierarchy()
    {
      if (worldHierarchyRoot == null)
      {
        Logger.AddError("Static world hierarchy root not loaded, cannot build hierarchy");
        return false;
      }
      if (!Scenes.ContainsKey(worldHierarchyRoot.Blueprint.BaseGuid))
      {
        Logger.AddError("Game hierarchy scene structure (s_GameRoot.HierarchyScenesStructure.Scenes) is invalid");
        return false;
      }
      OnStartHierarchyBuilding();
      RegisterGameRootEngineInstance((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot);
      HierarchySceneInfoData scene = Scenes[worldHierarchyRoot.Blueprint.BaseGuid];
      CreateVMTemplateByEngTemplateHierarchyInfo(worldHierarchyRoot, scene);
      ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).OnBuildHierarchy();
      return true;
    }

    public static bool CreateVMTemplateByEngTemplateHierarchyInfo(
      IWorldHierarchyObject worldHierarchyObject,
      HierarchySceneInfoData objSceneInfo)
    {
      RegisterHierarchyObjectEngineInstance(worldHierarchyObject);
      for (int index = 0; index < objSceneInfo.Scenes.Count; ++index)
      {
        ulong scene = objSceneInfo.Scenes[index];
        IWorldBlueprint mountingSceneTemplate = IStaticDataContainer.StaticDataContainer.GameRoot.GetEngineTemplateByGuid(scene);
        if (mountingSceneTemplate == null)
        {
          Guid templateGuidByBaseGuid = IStaticDataContainer.StaticDataContainer.GameRoot.GetEngineTemplateGuidByBaseGuid(scene);
          IEntity template = ServiceCache.TemplateService.GetTemplate<IEntity>(templateGuidByBaseGuid);
          if (template != null)
            mountingSceneTemplate = CreateVMTemplateByEngineTemplate(scene, template);
          if (mountingSceneTemplate == null)
          {
            Logger.AddError(string.Format("Can't attach scene to mounting point {0}: scene template with Guid={1} not found", worldHierarchyObject.Name, templateGuidByBaseGuid));
            continue;
          }
        }
        if (!typeof (IWorldBlueprint).IsAssignableFrom(mountingSceneTemplate.GetType()))
        {
          Logger.AddError(string.Format("Can't attach scene to mounting point {0}: scene template {1} with engine id={2} isn't valid world object template!!!", worldHierarchyObject.Name, mountingSceneTemplate.Name, mountingSceneTemplate.EngineTemplateGuid));
        }
        else
        {
          IWorldHierarchyObject objectByTemplate = CreateVirtualObjectByTemplate(mountingSceneTemplate);
          worldHierarchyObject.AddHierarchyChild(objectByTemplate);
          objectByTemplate.Parent = worldHierarchyObject;
        }
      }
      foreach (IHierarchyObject hierarchyChild in worldHierarchyObject.HierarchyChilds)
      {
        IWorldHierarchyObject worldHierarchyObject1 = (IWorldHierarchyObject) hierarchyChild;
        if (Scenes.TryGetValue(worldHierarchyObject1.Blueprint.BaseGuid, out HierarchySceneInfoData objSceneInfo1))
          CreateVMTemplateByEngTemplateHierarchyInfo(worldHierarchyObject1, objSceneInfo1);
        else
          RegisterHierarchyObjectEngineInstance(hierarchyChild);
      }
      foreach (IHierarchyObject simpleChild in worldHierarchyObject.SimpleChilds)
        RegisterHierarchyObjectEngineInstance(simpleChild);
      return true;
    }

    public static void CreateChilds(VMWorldHierarchyObject worldHierarchyObject)
    {
      if (!Scenes.TryGetValue(worldHierarchyObject.BaseGuid, out HierarchySceneInfoData hierarchySceneInfoData))
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
            templateObject = CreateVMTemplateByEngineTemplate(child1, template);
          if (templateObject == null)
          {
            templateObject = CreateVMTemplateByEngineTemplate(child1, template);
            Logger.AddError(string.Format("Can't add child to scene {0}: child template with id={1} not found. Phantom hierarchy object will be created.", worldHierarchyObject.Name, templateGuidByBaseGuid));
          }
        }
        if (templateObject != null)
        {
          VMWorldHierarchyObject child2 = new VMWorldHierarchyObject();
          child2.Initialize(templateObject);
          worldHierarchyObject.AddHierarchyChild(child2);
        }
        else
        {
          IHierarchyObject phantomHierarchyObject = CreatePhantomHierarchyObject(child1);
          worldHierarchyObject.AddHierarchyChild(phantomHierarchyObject);
        }
      }
    }

    public static void CreateSimpleChilds(VMWorldHierarchyObject worldHierarchyObject)
    {
      if (!Scenes.TryGetValue(worldHierarchyObject.BaseGuid, out HierarchySceneInfoData hierarchySceneInfoData))
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
            simpleChiTemplate = CreateVMTemplateByEngineTemplate(simpleChild, template);
          if (simpleChiTemplate == null)
          {
            Logger.AddError(string.Format("Can't add simple child to scene {0}: child template with Guid={1} not found", worldHierarchyObject.Name, templateGuidByBaseGuid));
            continue;
          }
        }
        HierarchySimpleChildInfo child = new HierarchySimpleChildInfo(simpleChiTemplate);
        worldHierarchyObject.AddHierarchySimpleChild(child);
      }
    }

    public static VMWorldObject CreateVMTemplateByEngineTemplate(
      ulong templateBaseID,
      IEntity entity)
    {
      IObject @object = IStaticDataContainer.StaticDataContainer.GetOrCreateObject(templateBaseID);
      if (!typeof (VMWorldObject).IsAssignableFrom(@object.GetType()))
      {
        Logger.AddError(string.Format("Guid {0} has invalid template type {1}", templateBaseID, @object.GetType()));
        return null;
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
      if (GameHierarchyRoot == null)
      {
        Logger.AddError("Game hierarchy root not inited!");
      }
      else
      {
        worldHierarchyRoot.ClearHierarchy();
        worldHierarhyObjectsDict.Clear();
        actualHierarchyEngineGuidsTable.Clear();
        worldHierarchyRoot = null;
      }
    }

    private static void RegisterGameRootEngineInstance(VMGameRoot gameRoot)
    {
      Guid empty = Guid.Empty;
      Guid guid;
      if (hierarchyBuildingCurrentSerialNum < gameRoot.HierarchyEngineGuidsTable.Count)
      {
        guid = gameRoot.HierarchyEngineGuidsTable[hierarchyBuildingCurrentSerialNum];
      }
      else
      {
        guid = Guid.NewGuid();
        Logger.AddError("Hierarchy engine guids table not loaded !");
      }
      gameRoot.InitInstanceGuid(guid);
      RegistrEngineGuidInHierarchy(new HierarchyGuid(gameRoot.BaseGuid), guid, false);
      ++hierarchyBuildingCurrentSerialNum;
    }

    public static void RegisterHierarchyObjectEngineInstance(IHierarchyObject hierarchyObject)
    {
      VMGameRoot gameRoot = (VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot;
      Guid empty = Guid.Empty;
      Guid guid;
      if (hierarchyBuildingCurrentSerialNum < gameRoot.HierarchyEngineGuidsTable.Count)
      {
        guid = gameRoot.HierarchyEngineGuidsTable[hierarchyBuildingCurrentSerialNum];
      }
      else
      {
        guid = Guid.NewGuid();
        Logger.AddError(string.Format("Hierarchy engine guids table is inconsistency with hierarchy data ! Engine instance guid for object {0} with hierarchyGuid={1} will be generated new!", hierarchyObject.EditorTemplate.BaseGuid, hierarchyObject.HierarchyGuid));
      }
      hierarchyObject.InitInstanceGuid(guid);
      RegistrEngineGuidInHierarchy(hierarchyObject.HierarchyGuid, guid, false);
      if (typeof (IWorldHierarchyObject).IsAssignableFrom(hierarchyObject.GetType()))
        worldHierarhyObjectsDict.Add(hierarchyObject.HierarchyGuid, (IWorldHierarchyObject) hierarchyObject);
      ++hierarchyBuildingCurrentSerialNum;
    }

    public static List<IWorldHierarchyObject> GetObjectPathByHierarhyGuid(
      HierarchyGuid hierarchyGuid)
    {
      List<IWorldHierarchyObject> pathByHierarhyGuid = [];
      for (int index = 0; index < hierarchyGuid.Guids.Length; ++index)
      {
        IObject objectByGuid = IStaticDataContainer.StaticDataContainer.GetObjectByGuid(hierarchyGuid.Guids[index]);
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
      hierarchyBuildingCurrentSerialNum = 0;
      actualHierarchyEngineGuidsTable.Clear();
    }

    public static void RegistrEngineGuidInHierarchy(
      HierarchyGuid hGuid,
      Guid whObjectEngineInstanceGuid,
      bool memoryActualTable = true)
    {
      if (actualHierarchyEngineGuidsTable.ContainsKey(hGuid))
      {
        Logger.AddError(string.Format("Hierarchy building inconsistency error: engine instance with hierarchy guid {0} has already been registered !!!", hGuid.ToString()));
      }
      else
      {
        if (!memoryActualTable)
          return;
        actualHierarchyEngineGuidsTable.Add(hGuid, whObjectEngineInstanceGuid);
      }
    }

    public static IWorldHierarchyObject GetWorldHierarhyObjectByGuid(HierarchyGuid hGuid)
    {
      worldHierarhyObjectsDict.TryGetValue(hGuid, out IWorldHierarchyObject hierarhyObjectByGuid);
      return hierarhyObjectByGuid;
    }

    public static void RegistrGameHierarchyRoot(IWorldHierarchyObject hierarchyRootObject)
    {
      worldHierarchyRoot = hierarchyRootObject;
    }

    public static IWorldHierarchyObject GameHierarchyRoot => worldHierarchyRoot;

    private static Dictionary<ulong, HierarchySceneInfoData> Scenes => ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).HierarchyScenesStructure;

    private static IHierarchyObject CreatePhantomHierarchyObject(ulong templateID)
    {
      IObject templateObject = IStaticDataContainer.StaticDataContainer.GetOrCreateObject(templateID);
      if (templateObject != null && typeof (IWorldBlueprint).IsAssignableFrom(templateObject.GetType()))
      {
        VMWorldHierarchyObject phantomHierarchyObject = new VMWorldHierarchyObject();
        phantomHierarchyObject.Initialize((IWorldBlueprint) templateObject);
        return phantomHierarchyObject;
      }
      Logger.AddError(string.Format("Cannot create phantom hierarchy object: invalid template guid {0}", templateID));
      return null;
    }

    private static IWorldHierarchyObject CreateVirtualObjectByTemplate(
      IWorldBlueprint mountingSceneTemplate,
      bool bScene = false)
    {
      try
      {
        VMWorldHierarchyObject objectByTemplate = new VMWorldHierarchyObject();
        objectByTemplate.Initialize(mountingSceneTemplate);
        return objectByTemplate;
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Virtual object creation error: {0}", ex));
      }
      return null;
    }
  }
}
