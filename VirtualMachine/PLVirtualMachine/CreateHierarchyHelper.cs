// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.CreateHierarchyHelper
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

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
using System;

#nullable disable
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
      return (IEntity) null;
    }

    public static object ConvertEditorTypeToEngineType(
      object vmValue,
      Type engType,
      VMType valueVMType)
    {
      if (vmValue == null)
      {
        Logger.AddWarning(string.Format("Converted to engine api value not defined at {0}", (object) DynamicFSM.CurrentStateInfo));
        return (object) null;
      }
      if (valueVMType == null)
      {
        Logger.AddError(string.Format("Cannot convert value {0}, value type not defined at {1}", vmValue, (object) DynamicFSM.CurrentStateInfo));
        return (object) null;
      }
      try
      {
        if (valueVMType.BaseType == typeof (IObjRef))
        {
          IObjRef engineType1;
          if (typeof (IObjRef).IsAssignableFrom(vmValue.GetType()))
            engineType1 = (IObjRef) vmValue;
          else if (typeof (Engine.Common.IObject).IsAssignableFrom(vmValue.GetType()))
          {
            engineType1 = (IObjRef) ExpressionUtility.GetRefByEngineInstance((Engine.Common.IObject) vmValue, valueVMType.BaseType);
          }
          else
          {
            Logger.AddError(string.Format("Cannot convert value with type {0} to IObjRef at {1}", (object) vmValue.GetType(), (object) DynamicFSM.CurrentStateInfo));
            return (object) null;
          }
          if (engineType1.Empty)
            return (object) null;
          if (engType == typeof (IObjRef))
            return (object) engineType1;
          if (engType == typeof (IEntity))
          {
            Guid engineGuid = engineType1.EngineGuid;
            IEntity engineType2 = ServiceCache.Simulation.Get(engineGuid);
            if (engineType2 == null)
              Logger.AddError(string.Format("Object with engine guid {0} not found in engine at {1}", (object) engineGuid, (object) DynamicFSM.CurrentStateInfo));
            return (object) engineType2;
          }
          if (engType == typeof (VMBaseEntity))
          {
            VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(engineType1.EngineGuid);
            if (entityByEngineGuid != null)
              return (object) entityByEngineGuid;
            Logger.AddError(string.Format("Dynamic object FSM with guid={0} not found in virtual machine at {1}", (object) engineType1.EngineGuid, (object) DynamicFSM.CurrentStateInfo));
            return (object) null;
          }
        }
        else if (valueVMType.BaseType == typeof (IBlueprintRef))
        {
          IBlueprintRef engineType = (IBlueprintRef) null;
          if (typeof (IBlueprintRef).IsAssignableFrom(vmValue.GetType()))
            engineType = (IBlueprintRef) vmValue;
          else if (typeof (IObjRef).IsAssignableFrom(vmValue.GetType()))
          {
            VMObjRef vmObjRef = (VMObjRef) vmValue;
            if (vmObjRef.Object != null && !((VMLogicObject) vmObjRef.Object).Static)
            {
              engineType = (IBlueprintRef) new VMBlueprintRef();
              ((VMBlueprintRef) engineType).Initialize(vmObjRef.Object);
            }
            if (engineType == null)
            {
              Logger.AddError(string.Format("Cannot convert object ref {0} to blueprint ref: must be not empty and not static at {1}", (object) vmObjRef.EngineGuid, (object) DynamicFSM.CurrentStateInfo));
              return (object) null;
            }
          }
          else
          {
            Logger.AddError(string.Format("Cannot convert value with type {0} to IBlueprintRef at {1}", (object) vmValue.GetType(), (object) DynamicFSM.CurrentStateInfo));
            return (object) null;
          }
          if (engType == typeof (IBlueprintRef))
            return (object) engineType;
          if (typeof (IEntity).IsAssignableFrom(engType))
          {
            VMBlueprint blueprint = (VMBlueprint) engineType.Blueprint;
            if (blueprint == null)
            {
              Logger.AddError(string.Format("Template not found by reference {0} at {1}", (object) engineType.Name, (object) DynamicFSM.CurrentStateInfo));
              return (object) null;
            }
            if (typeof (VMWorldObject).IsAssignableFrom(blueprint.GetType()))
            {
              Guid engineTemplateGuid = ((VMWorldObject) blueprint).EngineTemplateGuid;
              if (Guid.Empty != engineTemplateGuid)
              {
                IEntity template = ServiceCache.TemplateService.GetTemplate<IEntity>(engineTemplateGuid);
                if (template != null)
                  return (object) template;
                Logger.AddError(string.Format("Template with Guid={0} not found in engine at {1}", (object) engineTemplateGuid, (object) DynamicFSM.CurrentStateInfo));
                return (object) null;
              }
            }
            Logger.AddError(string.Format("Engine template blueprint {0} must be world object at {1}", (object) engineType.Name, (object) DynamicFSM.CurrentStateInfo));
            return (object) null;
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
                return (object) engineType3;
              if (typeof (Engine.Common.IObject).IsAssignableFrom(engType))
              {
                Guid engineTemplateGuid = engineType3.EngineTemplateGuid;
                if (!(engineTemplateGuid != Guid.Empty))
                  return (object) null;
                Engine.Common.IObject engineType4 = (Engine.Common.IObject) null;
                Type result;
                if (SampleAttribute.TryGetValue(valueVMType.SpecialType, out result))
                  engineType4 = ServiceCache.TemplateService.GetTemplate(result, engineTemplateGuid);
                if (engineType4 == null)
                  engineType4 = (Engine.Common.IObject) ServiceCache.TemplateService.GetTemplate<IEntity>(engineTemplateGuid);
                if (engineType4 != null)
                  return (object) engineType4;
                Logger.AddWarning(string.Format("Sample with Guid={0} not found in engine at {1}", (object) engineType3, (object) DynamicFSM.CurrentStateInfo));
                return (object) null;
              }
              Logger.AddError(string.Format("Cannot convert sample value to type {0} at {1}", (object) engType, (object) DynamicFSM.CurrentStateInfo));
              return (object) null;
            }
            Logger.AddError(string.Format("Cannot convert value with type {0} to ISampleRef at {1}", (object) vmValue.GetType(), (object) DynamicFSM.CurrentStateInfo));
            return (object) null;
          }
          if (valueVMType.BaseType == typeof (ILogicMapNodeRef))
          {
            if (typeof (ILogicMapNodeRef).IsAssignableFrom(vmValue.GetType()))
            {
              ILogicMapNodeRef engineType = (ILogicMapNodeRef) vmValue;
              if (engType == typeof (ILogicMapNodeRef))
                return (object) engineType;
              if (typeof (IMMNode).IsAssignableFrom(engType))
                return (object) DynamicMindMap.GetEngineNodeByStaticGuid(engineType.LogicMapNode.BaseGuid);
              Logger.AddError(string.Format("Cannot convert logic map node value to type {0} at {1}", (object) engType, (object) DynamicFSM.CurrentStateInfo));
              return (object) null;
            }
            Logger.AddError(string.Format("Cannot convert value with type {0} to ILogicMapNodeRef at {1}", (object) vmValue.GetType(), (object) DynamicFSM.CurrentStateInfo));
            return (object) null;
          }
          if (engType.IsAssignableFrom(vmValue.GetType()))
            return vmValue;
          if (engType.IsPrimitive)
            return Convert.ChangeType(vmValue, engType);
        }
        Logger.AddError(string.Format("Cannot convert value with type {0} to {1} at {2}", (object) vmValue.GetType(), (object) engType, (object) DynamicFSM.CurrentStateInfo));
        return (object) null;
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Cannot convert value {0}, error: {1} at {2}", vmValue, (object) ex.ToString(), (object) DynamicFSM.CurrentStateInfo));
        return (object) null;
      }
    }
  }
}
