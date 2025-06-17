using System;
using System.Collections.Generic;
using Cofe.Loggers;
using Cofe.Serializations.Converters;
using Engine.Common;
using Engine.Common.Comparers;
using Engine.Common.Services;
using Engine.Common.Types;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine
{
  public static class WorldEntityUtility
  {
    private static Dictionary<Guid, ILogicObject> dynamicGuidsToStaticWorldTemplatesDict = new(GuidComparer.Instance);
    private static List<VMWorldObject> staticWorldTemplatesList = [];
    private static Dictionary<Guid, VMEntity> dynamicEntityByEngineGuidDict = new(GuidComparer.Instance);
    private static Dictionary<HierarchyGuid, VMEntity> hierarchyGuidsToDynamicEntityDict = new(HierarchyGuidComparer.Instance);
    private static Dictionary<ulong, VMEntity> staticObjGuidsToDynamicEntityDict = new(UlongComparer.Instance);
    private static Dictionary<string, VMEntity> dynamicEntityByUniNameCache = new();

    public static bool IsDynamicGuidExist(Guid dynGuid)
    {
      return dynamicEntityByEngineGuidDict.ContainsKey(dynGuid);
    }

    public static void AddDynamicObjectEntityByEngineGuid(Guid engGuid, VMEntity entity)
    {
      if (!dynamicEntityByEngineGuidDict.ContainsKey(engGuid))
        dynamicEntityByEngineGuidDict.Add(engGuid, entity);
      else
        Logger.AddError("Error: Dynamic object guid duplication in dynamic fsm dict");
    }

    public static VMEntity GetDynamicObjectEntityByEngineGuid(Guid engGuid)
    {
      return dynamicEntityByEngineGuidDict.TryGetValue(engGuid, out VMEntity vmEntity) ? vmEntity : null;
    }

    public static VMEntity GetDynamicObjectEntityByHierarchyGuid(HierarchyGuid hGuid)
    {
      hierarchyGuidsToDynamicEntityDict.TryGetValue(hGuid, out VMEntity entityByHierarchyGuid);
      return entityByHierarchyGuid;
    }

    public static VMEntity GetDynamicObjectEntityByUniName(string uniName)
    {
      if (dynamicEntityByUniNameCache.TryGetValue(uniName, out VMEntity objectEntityByUniName))
        return objectEntityByUniName;
      switch (GuidUtility.GetGuidFormat(uniName))
      {
        case EGuidFormat.GT_BASE:
          ulong stguid = DefaultConverter.ParseUlong(uniName);
          if (stguid > 0UL)
          {
            objectEntityByUniName = GetDynamicObjectEntityByStaticGuid(stguid);
          }
          break;
        case EGuidFormat.GT_HIERARCHY:
          objectEntityByUniName = GetDynamicObjectEntityByHierarchyGuid(new HierarchyGuid(uniName));
          break;
        case EGuidFormat.GT_ENGINE:
          Guid guid = DefaultConverter.ParseGuid(uniName);
          if (guid != Guid.Empty)
          {
            objectEntityByUniName = GetDynamicObjectEntityByEngineGuid(guid);
          }
          break;
      }
      if (objectEntityByUniName == null)
        return null;
      dynamicEntityByUniNameCache.Add(uniName, objectEntityByUniName);
      return objectEntityByUniName;
    }

    public static VMEntity GetDynamicObjectEntityByStaticGuid(ulong stguid)
    {
      return staticObjGuidsToDynamicEntityDict.TryGetValue(stguid, out VMEntity vmEntity) ? vmEntity : null;
    }

    public static void AddDynamicObjectEntityByHierarchyGuid(
      HierarchyGuid hierarchyGuid,
      VMEntity entity)
    {
      if (!hierarchyGuidsToDynamicEntityDict.ContainsKey(hierarchyGuid))
        hierarchyGuidsToDynamicEntityDict.Add(hierarchyGuid, entity);
      else
        Logger.AddError(string.Format("Duplicate hierarchy guid {0} during registration dynamic object by template", hierarchyGuid));
    }

    public static void AddDynamicObjectEntityByStaticGuid(ulong stguid, VMEntity entity)
    {
      if (!staticObjGuidsToDynamicEntityDict.ContainsKey(stguid))
        staticObjGuidsToDynamicEntityDict.Add(stguid, entity);
      else
        Logger.AddError("Duplicate static object creation");
    }

    public static VMBaseEntity CreateWorldTemplateDynamicChildInstance(
      IEntity template,
      VMBaseEntity parentEntity,
      IEntity parentInstance)
    {
      if (template == null)
      {
        Logger.AddWarning(string.Format("Template for creating dynamic instance in {0} not defined", parentEntity.Name));
        return null;
      }
      IEntity entity = ServiceCache.Factory.Instantiate(template);
      ServiceCache.Simulation.Add(entity, parentInstance);
      IWorldBlueprint engineTemplateByGuid = ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetEngineTemplateByGuid(template.Id);
      if (engineTemplateByGuid == null)
      {
        Logger.AddWarning(string.Format("Template with engine guid = {0} not registered", GuidUtility.GetGuidString(template.Id)));
        return null;
      }
      Guid id = entity.Id;
      VMEntity dynamicChildInstance = new VMEntity();
      dynamicChildInstance.Initialize(engineTemplateByGuid, entity, true);
      parentEntity?.AddChildEntity(dynamicChildInstance);
      CreateEntityUtility.InitializeObject(dynamicChildInstance, (VMLogicObject) engineTemplateByGuid);
      IWorldBlueprint editorTemplate = engineTemplateByGuid;
      AddDynamicGuidsToStaticWorldTemplate(id, editorTemplate);
      return dynamicChildInstance;
    }

    public static VMBaseEntity CreateWorldTemplateDynamicChildInstance(
      IEntity template,
      IEntity instance,
      bool bOnSaveLoad = false)
    {
      if (template == null)
      {
        Logger.AddError(string.Format("Template of created instance {0} not defined at {1}", instance.Id, DynamicFSM.CurrentStateInfo));
        return null;
      }
      IWorldBlueprint engineTemplateByGuid = ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetEngineTemplateByGuid(template.Id);
      if (engineTemplateByGuid == null)
      {
        Logger.AddError(string.Format("Template with engine guid = {0} not registered at {1}", GuidUtility.GetGuidString(template.Id), DynamicFSM.CurrentStateInfo));
        return null;
      }
      VMEntity dynamicChildInstance = new VMEntity();
      dynamicChildInstance.Initialize(engineTemplateByGuid, instance, true);
      if (!bOnSaveLoad)
        CreateEntityUtility.InitializeObject(dynamicChildInstance, (VMLogicObject) engineTemplateByGuid);
      OnCreateWorldTemplateDynamicChildInstance(dynamicChildInstance, engineTemplateByGuid);
      return dynamicChildInstance;
    }

    public static IBlueprint GetEditorTemplateByEngineGuid(Guid engGuid)
    {
      dynamicGuidsToStaticWorldTemplatesDict.TryGetValue(engGuid, out ILogicObject logicObject);
      return logicObject?.Blueprint;
    }

    public static void RegistrStaticWorldTemplate(VMWorldObject template)
    {
      staticWorldTemplatesList.Add(template);
    }

    public static void ClearStatic()
    {
      dynamicGuidsToStaticWorldTemplatesDict.Clear();
      staticWorldTemplatesList.Clear();
    }

    public static void Clear()
    {
      foreach (KeyValuePair<Guid, VMEntity> keyValuePair in dynamicEntityByEngineGuidDict)
        keyValuePair.Value.PreLoading();
      dynamicEntityByEngineGuidDict.Clear();
      hierarchyGuidsToDynamicEntityDict.Clear();
      staticObjGuidsToDynamicEntityDict.Clear();
      dynamicEntityByUniNameCache.Clear();
      dynamicGuidsToStaticWorldTemplatesDict.Clear();
    }

    public static void AddDynamicGuidsToStaticWorldTemplate(
      Guid engGuid,
      ILogicObject editorTemplate)
    {
      if (dynamicGuidsToStaticWorldTemplatesDict.ContainsKey(engGuid))
        return;
      dynamicGuidsToStaticWorldTemplatesDict.Add(engGuid, editorTemplate);
    }

    public static void OnCreateWorldTemplateDynamicChildInstance(
      VMEntity dynChildEntity,
      IBlueprint dynChildEditorTemplate)
    {
      if (dynamicGuidsToStaticWorldTemplatesDict.ContainsKey(dynChildEntity.EngineGuid))
        return;
      dynamicGuidsToStaticWorldTemplatesDict.Add(dynChildEntity.EngineGuid, dynChildEditorTemplate);
    }
  }
}
