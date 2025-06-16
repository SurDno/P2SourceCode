// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.WorldEntityUtility
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

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
using System;
using System.Collections.Generic;

#nullable disable
namespace PLVirtualMachine
{
  public static class WorldEntityUtility
  {
    private static Dictionary<Guid, ILogicObject> dynamicGuidsToStaticWorldTemplatesDict = new Dictionary<Guid, ILogicObject>((IEqualityComparer<Guid>) GuidComparer.Instance);
    private static List<VMWorldObject> staticWorldTemplatesList = new List<VMWorldObject>();
    private static Dictionary<Guid, VMEntity> dynamicEntityByEngineGuidDict = new Dictionary<Guid, VMEntity>((IEqualityComparer<Guid>) GuidComparer.Instance);
    private static Dictionary<HierarchyGuid, VMEntity> hierarchyGuidsToDynamicEntityDict = new Dictionary<HierarchyGuid, VMEntity>((IEqualityComparer<HierarchyGuid>) HierarchyGuidComparer.Instance);
    private static Dictionary<ulong, VMEntity> staticObjGuidsToDynamicEntityDict = new Dictionary<ulong, VMEntity>((IEqualityComparer<ulong>) UlongComparer.Instance);
    private static Dictionary<string, VMEntity> dynamicEntityByUniNameCache = new Dictionary<string, VMEntity>();

    public static bool IsDynamicGuidExist(Guid dynGuid)
    {
      return WorldEntityUtility.dynamicEntityByEngineGuidDict.ContainsKey(dynGuid);
    }

    public static void AddDynamicObjectEntityByEngineGuid(Guid engGuid, VMEntity entity)
    {
      if (!WorldEntityUtility.dynamicEntityByEngineGuidDict.ContainsKey(engGuid))
        WorldEntityUtility.dynamicEntityByEngineGuidDict.Add(engGuid, entity);
      else
        Logger.AddError(string.Format("Error: Dynamic object guid duplication in dynamic fsm dict"));
    }

    public static VMEntity GetDynamicObjectEntityByEngineGuid(Guid engGuid)
    {
      VMEntity vmEntity;
      return WorldEntityUtility.dynamicEntityByEngineGuidDict.TryGetValue(engGuid, out vmEntity) ? vmEntity : (VMEntity) null;
    }

    public static VMEntity GetDynamicObjectEntityByHierarchyGuid(HierarchyGuid hGuid)
    {
      VMEntity entityByHierarchyGuid;
      WorldEntityUtility.hierarchyGuidsToDynamicEntityDict.TryGetValue(hGuid, out entityByHierarchyGuid);
      return entityByHierarchyGuid;
    }

    public static VMEntity GetDynamicObjectEntityByUniName(string uniName)
    {
      VMEntity objectEntityByUniName = (VMEntity) null;
      if (WorldEntityUtility.dynamicEntityByUniNameCache.TryGetValue(uniName, out objectEntityByUniName))
        return objectEntityByUniName;
      switch (GuidUtility.GetGuidFormat(uniName))
      {
        case EGuidFormat.GT_BASE:
          ulong stguid = DefaultConverter.ParseUlong(uniName);
          if (stguid > 0UL)
          {
            objectEntityByUniName = WorldEntityUtility.GetDynamicObjectEntityByStaticGuid(stguid);
            break;
          }
          break;
        case EGuidFormat.GT_HIERARCHY:
          objectEntityByUniName = WorldEntityUtility.GetDynamicObjectEntityByHierarchyGuid(new HierarchyGuid(uniName));
          break;
        case EGuidFormat.GT_ENGINE:
          Guid guid = DefaultConverter.ParseGuid(uniName);
          if (guid != Guid.Empty)
          {
            objectEntityByUniName = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(guid);
            break;
          }
          break;
      }
      if (objectEntityByUniName == null)
        return (VMEntity) null;
      WorldEntityUtility.dynamicEntityByUniNameCache.Add(uniName, objectEntityByUniName);
      return objectEntityByUniName;
    }

    public static VMEntity GetDynamicObjectEntityByStaticGuid(ulong stguid)
    {
      VMEntity vmEntity;
      return WorldEntityUtility.staticObjGuidsToDynamicEntityDict.TryGetValue(stguid, out vmEntity) ? vmEntity : (VMEntity) null;
    }

    public static void AddDynamicObjectEntityByHierarchyGuid(
      HierarchyGuid hierarchyGuid,
      VMEntity entity)
    {
      if (!WorldEntityUtility.hierarchyGuidsToDynamicEntityDict.ContainsKey(hierarchyGuid))
        WorldEntityUtility.hierarchyGuidsToDynamicEntityDict.Add(hierarchyGuid, entity);
      else
        Logger.AddError(string.Format("Duplicate hierarchy guid {0} during registration dynamic object by template", (object) hierarchyGuid));
    }

    public static void AddDynamicObjectEntityByStaticGuid(ulong stguid, VMEntity entity)
    {
      if (!WorldEntityUtility.staticObjGuidsToDynamicEntityDict.ContainsKey(stguid))
        WorldEntityUtility.staticObjGuidsToDynamicEntityDict.Add(stguid, entity);
      else
        Logger.AddError(string.Format("Duplicate static object creation"));
    }

    public static VMBaseEntity CreateWorldTemplateDynamicChildInstance(
      IEntity template,
      VMBaseEntity parentEntity,
      IEntity parentInstance)
    {
      if (template == null)
      {
        Logger.AddWarning(string.Format("Template for creating dynamic instance in {0} not defined", (object) parentEntity.Name));
        return (VMBaseEntity) null;
      }
      IEntity entity = ServiceCache.Factory.Instantiate<IEntity>(template);
      ServiceCache.Simulation.Add(entity, parentInstance);
      IWorldBlueprint engineTemplateByGuid = ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetEngineTemplateByGuid(template.Id);
      if (engineTemplateByGuid == null)
      {
        Logger.AddWarning(string.Format("Template with engine guid = {0} not registered", (object) GuidUtility.GetGuidString(template.Id)));
        return (VMBaseEntity) null;
      }
      Guid id = entity.Id;
      VMEntity dynamicChildInstance = new VMEntity();
      dynamicChildInstance.Initialize((ILogicObject) engineTemplateByGuid, entity, true);
      parentEntity?.AddChildEntity((VMBaseEntity) dynamicChildInstance);
      CreateEntityUtility.InitializeObject(dynamicChildInstance, (VMLogicObject) engineTemplateByGuid);
      IWorldBlueprint editorTemplate = engineTemplateByGuid;
      WorldEntityUtility.AddDynamicGuidsToStaticWorldTemplate(id, (ILogicObject) editorTemplate);
      return (VMBaseEntity) dynamicChildInstance;
    }

    public static VMBaseEntity CreateWorldTemplateDynamicChildInstance(
      IEntity template,
      IEntity instance,
      bool bOnSaveLoad = false)
    {
      if (template == null)
      {
        Logger.AddError(string.Format("Template of created instance {0} not defined at {1}", (object) instance.Id, (object) DynamicFSM.CurrentStateInfo));
        return (VMBaseEntity) null;
      }
      IWorldBlueprint engineTemplateByGuid = ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).GetEngineTemplateByGuid(template.Id);
      if (engineTemplateByGuid == null)
      {
        Logger.AddError(string.Format("Template with engine guid = {0} not registered at {1}", (object) GuidUtility.GetGuidString(template.Id), (object) DynamicFSM.CurrentStateInfo));
        return (VMBaseEntity) null;
      }
      VMEntity dynamicChildInstance = new VMEntity();
      dynamicChildInstance.Initialize((ILogicObject) engineTemplateByGuid, instance, true);
      if (!bOnSaveLoad)
        CreateEntityUtility.InitializeObject(dynamicChildInstance, (VMLogicObject) engineTemplateByGuid);
      WorldEntityUtility.OnCreateWorldTemplateDynamicChildInstance(dynamicChildInstance, (IBlueprint) engineTemplateByGuid);
      return (VMBaseEntity) dynamicChildInstance;
    }

    public static IBlueprint GetEditorTemplateByEngineGuid(Guid engGuid)
    {
      ILogicObject logicObject;
      WorldEntityUtility.dynamicGuidsToStaticWorldTemplatesDict.TryGetValue(engGuid, out logicObject);
      return logicObject?.Blueprint;
    }

    public static void RegistrStaticWorldTemplate(VMWorldObject template)
    {
      WorldEntityUtility.staticWorldTemplatesList.Add(template);
    }

    public static void ClearStatic()
    {
      WorldEntityUtility.dynamicGuidsToStaticWorldTemplatesDict.Clear();
      WorldEntityUtility.staticWorldTemplatesList.Clear();
    }

    public static void Clear()
    {
      foreach (KeyValuePair<Guid, VMEntity> keyValuePair in WorldEntityUtility.dynamicEntityByEngineGuidDict)
        keyValuePair.Value.PreLoading();
      WorldEntityUtility.dynamicEntityByEngineGuidDict.Clear();
      WorldEntityUtility.hierarchyGuidsToDynamicEntityDict.Clear();
      WorldEntityUtility.staticObjGuidsToDynamicEntityDict.Clear();
      WorldEntityUtility.dynamicEntityByUniNameCache.Clear();
      WorldEntityUtility.dynamicGuidsToStaticWorldTemplatesDict.Clear();
    }

    public static void AddDynamicGuidsToStaticWorldTemplate(
      Guid engGuid,
      ILogicObject editorTemplate)
    {
      if (WorldEntityUtility.dynamicGuidsToStaticWorldTemplatesDict.ContainsKey(engGuid))
        return;
      WorldEntityUtility.dynamicGuidsToStaticWorldTemplatesDict.Add(engGuid, editorTemplate);
    }

    public static void OnCreateWorldTemplateDynamicChildInstance(
      VMEntity dynChildEntity,
      IBlueprint dynChildEditorTemplate)
    {
      if (WorldEntityUtility.dynamicGuidsToStaticWorldTemplatesDict.ContainsKey(dynChildEntity.EngineGuid))
        return;
      WorldEntityUtility.dynamicGuidsToStaticWorldTemplatesDict.Add(dynChildEntity.EngineGuid, (ILogicObject) dynChildEditorTemplate);
    }
  }
}
