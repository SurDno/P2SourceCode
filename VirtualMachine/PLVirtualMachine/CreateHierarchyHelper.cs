using System;
using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Binders;
using Engine.Common.MindMap;
using Engine.Common.Services;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.GameLogic;
using PLVirtualMachine.Objects;
using IObject = Engine.Common.IObject;

namespace PLVirtualMachine
{
  public static class CreateHierarchyHelper
  {
    public static IEntity GetEngineMilestoneEntityByPositionID(HierarchyGuid worldPositionGuid)
    {
      VMEntity entityByHierarchyGuid = WorldEntityUtility.GetDynamicObjectEntityByHierarchyGuid(worldPositionGuid);
      if (entityByHierarchyGuid != null)
      {
        Guid engineGuid = entityByHierarchyGuid.EngineGuid;
        if (engineGuid != Guid.Empty)
          return ServiceCache.Simulation.Get(engineGuid);
      }
      return null;
    }

    public static object ConvertEditorTypeToEngineType(
      object vmValue,
      Type engType,
      VMType valueVMType)
    {
      if (vmValue == null)
      {
        Logger.AddWarning(string.Format("Converted to engine api value not defined at {0}", DynamicFSM.CurrentStateInfo));
        return null;
      }
      if (valueVMType == null)
      {
        Logger.AddError(string.Format("Cannot convert value {0}, value type not defined at {1}", vmValue, DynamicFSM.CurrentStateInfo));
        return null;
      }
      try
      {
        if (valueVMType.BaseType == typeof (IObjRef))
        {
          IObjRef engineType1;
          if (typeof (IObjRef).IsAssignableFrom(vmValue.GetType()))
            engineType1 = (IObjRef) vmValue;
          else if (typeof (IObject).IsAssignableFrom(vmValue.GetType()))
          {
            engineType1 = (IObjRef) ExpressionUtility.GetRefByEngineInstance((IObject) vmValue, valueVMType.BaseType);
          }
          else
          {
            Logger.AddError(string.Format("Cannot convert value with type {0} to IObjRef at {1}", vmValue.GetType(), DynamicFSM.CurrentStateInfo));
            return null;
          }
          if (engineType1.Empty)
            return null;
          if (engType == typeof (IObjRef))
            return engineType1;
          if (engType == typeof (IEntity))
          {
            Guid engineGuid = engineType1.EngineGuid;
            IEntity engineType2 = ServiceCache.Simulation.Get(engineGuid);
            if (engineType2 == null)
              Logger.AddError(string.Format("Object with engine guid {0} not found in engine at {1}", engineGuid, DynamicFSM.CurrentStateInfo));
            return engineType2;
          }
          if (engType == typeof (VMBaseEntity))
          {
            VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(engineType1.EngineGuid);
            if (entityByEngineGuid != null)
              return entityByEngineGuid;
            Logger.AddError(string.Format("Dynamic object FSM with guid={0} not found in virtual machine at {1}", engineType1.EngineGuid, DynamicFSM.CurrentStateInfo));
            return null;
          }
        }
        else if (valueVMType.BaseType == typeof (IBlueprintRef))
        {
          IBlueprintRef engineType = null;
          if (typeof (IBlueprintRef).IsAssignableFrom(vmValue.GetType()))
            engineType = (IBlueprintRef) vmValue;
          else if (typeof (IObjRef).IsAssignableFrom(vmValue.GetType()))
          {
            VMObjRef vmObjRef = (VMObjRef) vmValue;
            if (vmObjRef.Object != null && !((VMLogicObject) vmObjRef.Object).Static)
            {
              engineType = new VMBlueprintRef();
              ((VMBlueprintRef) engineType).Initialize(vmObjRef.Object);
            }
            if (engineType == null)
            {
              Logger.AddError(string.Format("Cannot convert object ref {0} to blueprint ref: must be not empty and not static at {1}", vmObjRef.EngineGuid, DynamicFSM.CurrentStateInfo));
              return null;
            }
          }
          else
          {
            Logger.AddError(string.Format("Cannot convert value with type {0} to IBlueprintRef at {1}", vmValue.GetType(), DynamicFSM.CurrentStateInfo));
            return null;
          }
          if (engType == typeof (IBlueprintRef))
            return engineType;
          if (typeof (IEntity).IsAssignableFrom(engType))
          {
            VMBlueprint blueprint = (VMBlueprint) engineType.Blueprint;
            if (blueprint == null)
            {
              Logger.AddError(string.Format("Template not found by reference {0} at {1}", engineType.Name, DynamicFSM.CurrentStateInfo));
              return null;
            }
            if (typeof (VMWorldObject).IsAssignableFrom(blueprint.GetType()))
            {
              Guid engineTemplateGuid = ((VMWorldObject) blueprint).EngineTemplateGuid;
              if (Guid.Empty != engineTemplateGuid)
              {
                IEntity template = ServiceCache.TemplateService.GetTemplate<IEntity>(engineTemplateGuid);
                if (template != null)
                  return template;
                Logger.AddError(string.Format("Template with Guid={0} not found in engine at {1}", engineTemplateGuid, DynamicFSM.CurrentStateInfo));
                return null;
              }
            }
            Logger.AddError(string.Format("Engine template blueprint {0} must be world object at {1}", engineType.Name, DynamicFSM.CurrentStateInfo));
            return null;
          }
        }
        else
        {
          if (valueVMType.BaseType == typeof (ISampleRef))
          {
            if (typeof (ISampleRef).IsAssignableFrom(vmValue.GetType()))
            {
              ISampleRef engineType3 = (ISampleRef) vmValue;
              if (engType == typeof (ISampleRef))
                return engineType3;
              if (typeof (IObject).IsAssignableFrom(engType))
              {
                Guid engineTemplateGuid = engineType3.EngineTemplateGuid;
                if (!(engineTemplateGuid != Guid.Empty))
                  return null;
                IObject engineType4 = null;
                if (SampleAttribute.TryGetValue(valueVMType.SpecialType, out Type result))
                  engineType4 = ServiceCache.TemplateService.GetTemplate(result, engineTemplateGuid);
                if (engineType4 == null)
                  engineType4 = ServiceCache.TemplateService.GetTemplate<IEntity>(engineTemplateGuid);
                if (engineType4 != null)
                  return engineType4;
                Logger.AddWarning(string.Format("Sample with Guid={0} not found in engine at {1}", engineType3, DynamicFSM.CurrentStateInfo));
                return null;
              }
              Logger.AddError(string.Format("Cannot convert sample value to type {0} at {1}", engType, DynamicFSM.CurrentStateInfo));
              return null;
            }
            Logger.AddError(string.Format("Cannot convert value with type {0} to ISampleRef at {1}", vmValue.GetType(), DynamicFSM.CurrentStateInfo));
            return null;
          }
          if (valueVMType.BaseType == typeof (ILogicMapNodeRef))
          {
            if (typeof (ILogicMapNodeRef).IsAssignableFrom(vmValue.GetType()))
            {
              ILogicMapNodeRef engineType = (ILogicMapNodeRef) vmValue;
              if (engType == typeof (ILogicMapNodeRef))
                return engineType;
              if (typeof (IMMNode).IsAssignableFrom(engType))
                return DynamicMindMap.GetEngineNodeByStaticGuid(engineType.LogicMapNode.BaseGuid);
              Logger.AddError(string.Format("Cannot convert logic map node value to type {0} at {1}", engType, DynamicFSM.CurrentStateInfo));
              return null;
            }
            Logger.AddError(string.Format("Cannot convert value with type {0} to ILogicMapNodeRef at {1}", vmValue.GetType(), DynamicFSM.CurrentStateInfo));
            return null;
          }
          if (engType.IsAssignableFrom(vmValue.GetType()))
            return vmValue;
          if (engType.IsPrimitive)
            return Convert.ChangeType(vmValue, engType);
        }
        Logger.AddError(string.Format("Cannot convert value with type {0} to {1} at {2}", vmValue.GetType(), engType, DynamicFSM.CurrentStateInfo));
        return null;
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Cannot convert value {0}, error: {1} at {2}", vmValue, ex, DynamicFSM.CurrentStateInfo));
        return null;
      }
    }
  }
}
