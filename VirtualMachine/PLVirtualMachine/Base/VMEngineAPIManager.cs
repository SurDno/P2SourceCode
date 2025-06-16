// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Base.VMEngineAPIManager
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Components.Movable;
using Engine.Common.Components.Prototype;
using Engine.Common.Services;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.VMSpecialAttributes;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.GameLogic;
using PLVirtualMachine.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace PLVirtualMachine.Base
{
  public class VMEngineAPIManager : EngineAPIManager
  {
    private static EntityMethodExecuteData lastEntityMethodExecData = new EntityMethodExecuteData();
    private static IDynamicGameObjectContext lastMethodExecInitiator;
    private static Dictionary<string, MethodExecuteContext> methodExecuteContextsDict = new Dictionary<string, MethodExecuteContext>();

    static VMEngineAPIManager()
    {
      EngineAPIManager.Instance = (EngineAPIManager) new VMEngineAPIManager();
    }

    public override VMBaseEntity CreateWorldTemplateDynamicChildInstance(
      IEntity template,
      VMBaseEntity parentEntity,
      IEntity parentInstance)
    {
      return WorldEntityUtility.CreateWorldTemplateDynamicChildInstance(template, parentEntity, parentInstance);
    }

    public override VMBaseEntity CreateWorldTemplateDynamicChildInstance(
      IEntity template,
      IEntity instance,
      bool bOnSaveLoad = false)
    {
      return WorldEntityUtility.CreateWorldTemplateDynamicChildInstance(template, instance, bOnSaveLoad);
    }

    public override IBlueprint GetEditorTemplateByEngineGuid(Guid guid)
    {
      return WorldEntityUtility.GetEditorTemplateByEngineGuid(guid);
    }

    public override void SetFatalError(string fatalError)
    {
      VirtualMachine.SetFatalError(fatalError);
    }

    public override string CurrentFSMStateInfo => DynamicFSM.CurrentStateInfo;

    public static void Start() => EngineAPIManager.Init();

    public static object ExecMethod(
      IDynamicGameObjectContext initiator,
      Guid objGuid,
      Type componentBaseType,
      string componentName,
      string methodName,
      List<object> dParams,
      List<VMType> dParamTypes,
      VMType outputType)
    {
      if (componentBaseType == (Type) null)
      {
        Logger.AddError(string.Format("Component base type not defined for method {0}, component {1} at {2}", (object) methodName, (object) componentName, (object) DynamicFSM.CurrentStateInfo));
        return (object) null;
      }
      if (objGuid == Guid.Empty)
      {
        Logger.AddError(string.Format("Cannot exec engine method {0} for entity with id=0 at {1}", (object) (componentBaseType.Name + "." + methodName), (object) DynamicFSM.CurrentStateInfo));
        return (object) null;
      }
      VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(objGuid);
      if (entityByEngineGuid == null)
      {
        Logger.AddError(string.Format("Object with id={0} not found in virtual machine at {1}", (object) objGuid, (object) DynamicFSM.CurrentStateInfo));
        return (object) null;
      }
      if (!entityByEngineGuid.Instantiated && methodName != "Instantiate")
      {
        Logger.AddError(string.Format("Cannot call method {0} at removed or not instantiated object {1} at {2}", (object) methodName, (object) entityByEngineGuid.Name, (object) DynamicFSM.CurrentStateInfo));
        return (object) null;
      }
      if (methodName == EngineAPIManager.GetSpecialFunctionName(ESpecialFunctionName.SFN_OBJECT_INIT, typeof (VMCommon)))
      {
        CreateEntityUtility.InitializeObject(entityByEngineGuid, (VMLogicObject) entityByEngineGuid.EditorTemplate, entityByEngineGuid.GetFSM());
        return (object) null;
      }
      VMEngineAPIManager.lastMethodExecInitiator = initiator;
      VMComponent objectComponentByType = EngineAPIManager.GetObjectComponentByType((VMBaseEntity) entityByEngineGuid, componentBaseType);
      MethodExecuteContext methodExecuteContext = VMEngineAPIManager.GetMethodExecuteContext(objectComponentByType, methodName, dParams.Count);
      for (int index = 0; index < methodExecuteContext.InputParamsInfo.Length; ++index)
      {
        try
        {
          methodExecuteContext.InputParams[index] = CreateHierarchyHelper.ConvertEditorTypeToEngineType(dParams[index], methodExecuteContext.InputParamsInfo[index].ParameterType, dParamTypes[index]);
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Exec method {0} params processing error: {1} at {2}", (object) methodName, (object) ex.ToString(), (object) DynamicFSM.CurrentStateInfo));
        }
      }
      try
      {
        object engValue = methodExecuteContext.ExecMethodInfo.Invoke((object) objectComponentByType, methodExecuteContext.InputParams);
        VMEngineAPIManager.lastEntityMethodExecData.Initialize(entityByEngineGuid, objectComponentByType, methodExecuteContext.ExecMethodInfo, methodExecuteContext.InputParams);
        VMType vmType = outputType;
        return VMEngineAPIManager.ConvertEngineTypeToEditorType(engValue, vmType);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Call method {0} error in object {1}: {2} at {3}", (object) methodName, (object) entityByEngineGuid.Name, (object) ex.ToString(), (object) DynamicFSM.CurrentStateInfo));
        return (object) null;
      }
    }

    public static object ExecMethod(EntityMethodExecuteData executeData)
    {
      if (executeData == null)
        return (object) null;
      VMComponent targetComponent = executeData.TargetComponent;
      MethodInfo execMethodInfo = executeData.ExecMethodInfo;
      object[] inputParams = executeData.InputParams;
      VMComponent vmComponent = targetComponent;
      object[] parameters = inputParams;
      return VMEngineAPIManager.ConvertEngineTypeToEditorType(execMethodInfo.Invoke((object) vmComponent, parameters));
    }

    public static object ExecMethodOnComponentDirect(
      VMComponent funcComponent,
      string methodName,
      object[] dFactParams)
    {
      string name = funcComponent.Name;
      MethodInfo componentMethodInfo = InfoAttribute.GetComponentMethodInfo(name, methodName);
      try
      {
        return componentMethodInfo.Invoke((object) funcComponent, dFactParams);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Call direct on component method {0} error in {1}.{2}, error: {3} at {4}", (object) methodName, (object) funcComponent.Parent.Name, (object) name, (object) ex, (object) DynamicFSM.CurrentStateInfo));
        return (object) null;
      }
    }

    public override void InstantiateObject(
      VMBaseEntity obj,
      IEntity forceSpawnPos = null,
      AreaEnum areaType = AreaEnum.Unknown)
    {
      if (obj == null)
      {
        Logger.AddError(string.Format("Instantiating object not defined at {0}!", (object) DynamicFSM.CurrentStateInfo));
      }
      else
      {
        obj.Instantiated = true;
        IBlueprint templateByEngineGuid = EngineAPIManager.Instance.GetEditorTemplateByEngineGuid(obj.EngineGuid);
        if (typeof (VMWorldObject).IsAssignableFrom(templateByEngineGuid.GetType()))
        {
          VMWorldObject vmWorldObject = (VMWorldObject) templateByEngineGuid;
          if (forceSpawnPos == null)
          {
            IEntity entityByPositionId = CreateHierarchyHelper.GetEngineMilestoneEntityByPositionID(vmWorldObject.WorldPositionGuid);
            if (entityByPositionId != null)
            {
              if (entityByPositionId == obj.Instance)
                return;
              VirtualMachine.Instance.SpawnObject((VMEntity) obj, entityByPositionId, areaType);
            }
            else
              Logger.AddError(string.Format("Cannot instantiate object {0}, because it's spawn position not defined at {1}", (object) templateByEngineGuid.Name, (object) DynamicFSM.CurrentStateInfo));
          }
          else
            VirtualMachine.Instance.SpawnObject((VMEntity) obj, forceSpawnPos, areaType);
        }
        else
          Logger.AddError(string.Format("Cannot instantiate not world object {0} at {1}", (object) templateByEngineGuid.Name, (object) DynamicFSM.CurrentStateInfo));
      }
    }

    public static VMObjRef ConvertEngineEntityInstanceToVMReference(IEntity engineEntity)
    {
      VMObjRef vmReference = new VMObjRef();
      vmReference.Initialize(engineEntity.Id);
      return vmReference;
    }

    public static object ConvertEngineTypeToEditorType(object engValue, VMType vmType = null)
    {
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      if (vmType != null)
      {
        if (typeof (IObjRef).IsAssignableFrom(vmType.BaseType))
          flag1 = true;
        else if (typeof (IBlueprintRef).IsAssignableFrom(vmType.BaseType))
          flag2 = true;
        else if (typeof (ISampleRef).IsAssignableFrom(vmType.BaseType))
          flag3 = true;
      }
      bool flag4 = !(flag1 | flag2 | flag3);
      if (flag1 | flag4)
      {
        if (engValue == null)
          return (object) new VMObjRef();
        if (typeof (IEntity).IsAssignableFrom(engValue.GetType()))
        {
          VMObjRef editorType = new VMObjRef();
          editorType.Initialize(((Engine.Common.IObject) engValue).Id);
          return (object) editorType;
        }
        if (typeof (IObjRef).IsAssignableFrom(engValue.GetType()))
          return engValue;
        if (typeof (VMBaseEntity).IsAssignableFrom(engValue.GetType()))
        {
          VMObjRef editorType = new VMObjRef();
          editorType.InitializeInstance((IEngineRTInstance) engValue);
          return (object) editorType;
        }
        if (flag1)
        {
          Logger.AddError(string.Format("Cannot convert value with type {0} to IObjRef at {1}", (object) engValue.GetType(), (object) DynamicFSM.CurrentStateInfo));
          return (object) null;
        }
      }
      if (flag2 | flag4)
      {
        if (engValue == null)
          return (object) new VMBlueprintRef();
        if (engValue is IEntity entity && ServiceCache.TemplateService.GetTemplate<IEntity>(entity.Id) != null)
        {
          VMBlueprintRef editorType = new VMBlueprintRef();
          editorType.Initialize(entity.Id);
          return (object) editorType;
        }
        if (typeof (IBlueprintRef).IsAssignableFrom(engValue.GetType()))
          return engValue;
        if (flag2)
        {
          Logger.AddError(string.Format("Cannot convert value with type {0} to IObjRef at {1}", (object) engValue.GetType(), (object) DynamicFSM.CurrentStateInfo));
          return (object) null;
        }
      }
      if (flag3 | flag4)
      {
        if (engValue == null)
          return (object) new VMSampleRef();
        if (typeof (Engine.Common.IObject).IsAssignableFrom(engValue.GetType()))
        {
          VMSampleRef editorType = new VMSampleRef();
          editorType.Initialize(((Engine.Common.IObject) engValue).Id);
          return (object) editorType;
        }
        if (typeof (ISampleRef).IsAssignableFrom(engValue.GetType()))
          return engValue;
        if (flag3)
        {
          Logger.AddError(string.Format("Cannot convert value with type {0} to ISampleRef at {1}", (object) engValue.GetType(), (object) DynamicFSM.CurrentStateInfo));
          return (object) null;
        }
      }
      if (engValue == null)
        return (object) null;
      if (engValue.GetType().IsEnum)
      {
        if (vmType == null)
          return engValue;
        if (typeof (string) == vmType.BaseType)
          return (object) engValue.ToString();
        int num = vmType.BaseType.IsEnum ? 1 : 0;
        return engValue;
      }
      if (typeof (ICommonList).IsAssignableFrom(engValue.GetType()))
      {
        for (int objIndex = 0; objIndex < ((ICommonList) engValue).ObjectsCount; ++objIndex)
        {
          object editorType = VMEngineAPIManager.ConvertEngineTypeToEditorType(((ICommonList) engValue).GetObject(objIndex));
          ((ICommonList) engValue).SetObject(objIndex, editorType);
        }
        return engValue;
      }
      return typeof (Message) == engValue.GetType() ? ((Message) engValue).Content : engValue;
    }

    public static EntityMethodExecuteData EntityMethodExecuteData
    {
      get
      {
        return VMEngineAPIManager.lastEntityMethodExecData.TargetEntity != null ? VMEngineAPIManager.lastEntityMethodExecData : (EntityMethodExecuteData) null;
      }
    }

    public static IDynamicGameObjectContext LastMethodExecInitiator
    {
      get => VMEngineAPIManager.lastMethodExecInitiator;
    }

    public static void OnRestart() => VMEngineAPIManager.lastEntityMethodExecData.Clear();

    public static void Clear()
    {
      VMEngineAPIManager.lastMethodExecInitiator = (IDynamicGameObjectContext) null;
      VMEngineAPIManager.lastEntityMethodExecData = new EntityMethodExecuteData();
      VMEngineAPIManager.methodExecuteContextsDict.Clear();
      foreach (KeyValuePair<string, Dictionary<EContextVariableCategory, List<IVariable>>> keyValuePair1 in EngineAPIManager.componentsAbstractVarsDict)
      {
        foreach (KeyValuePair<EContextVariableCategory, List<IVariable>> keyValuePair2 in keyValuePair1.Value)
        {
          List<IVariable> variableList = keyValuePair2.Value;
          for (int index = 0; index < variableList.Count; ++index)
          {
            IVariable variable = variableList[index];
            if (typeof (AbstractParameter) == variable.GetType())
              ((AbstractParameter) variable).Clear();
          }
        }
      }
    }

    private static MethodExecuteContext GetMethodExecuteContext(
      VMComponent funcComponent,
      string methodName,
      int paramsCount)
    {
      string str = funcComponent.Name + methodName + paramsCount.ToString();
      MethodExecuteContext methodExecuteContext1 = (MethodExecuteContext) null;
      if (VMEngineAPIManager.methodExecuteContextsDict.TryGetValue(str, out methodExecuteContext1))
        return methodExecuteContext1;
      MethodInfo componentMethodInfo = InfoAttribute.GetComponentMethodInfo(funcComponent.Name, methodName);
      MethodExecuteContext methodExecuteContext2 = new MethodExecuteContext();
      methodExecuteContext2.Initialize(str, componentMethodInfo);
      VMEngineAPIManager.methodExecuteContextsDict.Add(str, methodExecuteContext2);
      return methodExecuteContext2;
    }
  }
}
