using System;
using System.Collections.Generic;
using System.Reflection;
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
using IObject = Engine.Common.IObject;

namespace PLVirtualMachine.Base
{
  public class VMEngineAPIManager : EngineAPIManager
  {
    private static EntityMethodExecuteData lastEntityMethodExecData = new EntityMethodExecuteData();
    private static IDynamicGameObjectContext lastMethodExecInitiator;
    private static Dictionary<string, MethodExecuteContext> methodExecuteContextsDict = new Dictionary<string, MethodExecuteContext>();

    static VMEngineAPIManager()
    {
      Instance = new VMEngineAPIManager();
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

    public static void Start() => Init();

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
      if (componentBaseType == null)
      {
        Logger.AddError(string.Format("Component base type not defined for method {0}, component {1} at {2}", methodName, componentName, DynamicFSM.CurrentStateInfo));
        return null;
      }
      if (objGuid == Guid.Empty)
      {
        Logger.AddError(string.Format("Cannot exec engine method {0} for entity with id=0 at {1}", componentBaseType.Name + "." + methodName, DynamicFSM.CurrentStateInfo));
        return null;
      }
      VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(objGuid);
      if (entityByEngineGuid == null)
      {
        Logger.AddError(string.Format("Object with id={0} not found in virtual machine at {1}", objGuid, DynamicFSM.CurrentStateInfo));
        return null;
      }
      if (!entityByEngineGuid.Instantiated && methodName != "Instantiate")
      {
        Logger.AddError(string.Format("Cannot call method {0} at removed or not instantiated object {1} at {2}", methodName, entityByEngineGuid.Name, DynamicFSM.CurrentStateInfo));
        return null;
      }
      if (methodName == GetSpecialFunctionName(ESpecialFunctionName.SFN_OBJECT_INIT, typeof (VMCommon)))
      {
        CreateEntityUtility.InitializeObject(entityByEngineGuid, (VMLogicObject) entityByEngineGuid.EditorTemplate, entityByEngineGuid.GetFSM());
        return null;
      }
      lastMethodExecInitiator = initiator;
      VMComponent objectComponentByType = GetObjectComponentByType(entityByEngineGuid, componentBaseType);
      MethodExecuteContext methodExecuteContext = GetMethodExecuteContext(objectComponentByType, methodName, dParams.Count);
      for (int index = 0; index < methodExecuteContext.InputParamsInfo.Length; ++index)
      {
        try
        {
          methodExecuteContext.InputParams[index] = CreateHierarchyHelper.ConvertEditorTypeToEngineType(dParams[index], methodExecuteContext.InputParamsInfo[index].ParameterType, dParamTypes[index]);
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Exec method {0} params processing error: {1} at {2}", methodName, ex, DynamicFSM.CurrentStateInfo));
        }
      }
      try
      {
        object engValue = methodExecuteContext.ExecMethodInfo.Invoke(objectComponentByType, methodExecuteContext.InputParams);
        lastEntityMethodExecData.Initialize(entityByEngineGuid, objectComponentByType, methodExecuteContext.ExecMethodInfo, methodExecuteContext.InputParams);
        VMType vmType = outputType;
        return ConvertEngineTypeToEditorType(engValue, vmType);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Call method {0} error in object {1}: {2} at {3}", methodName, entityByEngineGuid.Name, ex, DynamicFSM.CurrentStateInfo));
        return null;
      }
    }

    public static object ExecMethod(EntityMethodExecuteData executeData)
    {
      if (executeData == null)
        return null;
      VMComponent targetComponent = executeData.TargetComponent;
      MethodInfo execMethodInfo = executeData.ExecMethodInfo;
      object[] inputParams = executeData.InputParams;
      VMComponent vmComponent = targetComponent;
      object[] parameters = inputParams;
      return ConvertEngineTypeToEditorType(execMethodInfo.Invoke(vmComponent, parameters));
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
        return componentMethodInfo.Invoke(funcComponent, dFactParams);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Call direct on component method {0} error in {1}.{2}, error: {3} at {4}", methodName, funcComponent.Parent.Name, name, ex, DynamicFSM.CurrentStateInfo));
        return null;
      }
    }

    public override void InstantiateObject(
      VMBaseEntity obj,
      IEntity forceSpawnPos = null,
      AreaEnum areaType = AreaEnum.Unknown)
    {
      if (obj == null)
      {
        Logger.AddError(string.Format("Instantiating object not defined at {0}!", DynamicFSM.CurrentStateInfo));
      }
      else
      {
        obj.Instantiated = true;
        IBlueprint templateByEngineGuid = Instance.GetEditorTemplateByEngineGuid(obj.EngineGuid);
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
              Logger.AddError(string.Format("Cannot instantiate object {0}, because it's spawn position not defined at {1}", templateByEngineGuid.Name, DynamicFSM.CurrentStateInfo));
          }
          else
            VirtualMachine.Instance.SpawnObject((VMEntity) obj, forceSpawnPos, areaType);
        }
        else
          Logger.AddError(string.Format("Cannot instantiate not world object {0} at {1}", templateByEngineGuid.Name, DynamicFSM.CurrentStateInfo));
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
          return new VMObjRef();
        if (typeof (IEntity).IsAssignableFrom(engValue.GetType()))
        {
          VMObjRef editorType = new VMObjRef();
          editorType.Initialize(((IObject) engValue).Id);
          return editorType;
        }
        if (typeof (IObjRef).IsAssignableFrom(engValue.GetType()))
          return engValue;
        if (typeof (VMBaseEntity).IsAssignableFrom(engValue.GetType()))
        {
          VMObjRef editorType = new VMObjRef();
          editorType.InitializeInstance((IEngineRTInstance) engValue);
          return editorType;
        }
        if (flag1)
        {
          Logger.AddError(string.Format("Cannot convert value with type {0} to IObjRef at {1}", engValue.GetType(), DynamicFSM.CurrentStateInfo));
          return null;
        }
      }
      if (flag2 | flag4)
      {
        if (engValue == null)
          return new VMBlueprintRef();
        if (engValue is IEntity entity && ServiceCache.TemplateService.GetTemplate<IEntity>(entity.Id) != null)
        {
          VMBlueprintRef editorType = new VMBlueprintRef();
          editorType.Initialize(entity.Id);
          return editorType;
        }
        if (typeof (IBlueprintRef).IsAssignableFrom(engValue.GetType()))
          return engValue;
        if (flag2)
        {
          Logger.AddError(string.Format("Cannot convert value with type {0} to IObjRef at {1}", engValue.GetType(), DynamicFSM.CurrentStateInfo));
          return null;
        }
      }
      if (flag3 | flag4)
      {
        if (engValue == null)
          return new VMSampleRef();
        if (typeof (IObject).IsAssignableFrom(engValue.GetType()))
        {
          VMSampleRef editorType = new VMSampleRef();
          editorType.Initialize(((IObject) engValue).Id);
          return editorType;
        }
        if (typeof (ISampleRef).IsAssignableFrom(engValue.GetType()))
          return engValue;
        if (flag3)
        {
          Logger.AddError(string.Format("Cannot convert value with type {0} to ISampleRef at {1}", engValue.GetType(), DynamicFSM.CurrentStateInfo));
          return null;
        }
      }
      if (engValue == null)
        return null;
      if (engValue.GetType().IsEnum)
      {
        if (vmType == null)
          return engValue;
        if (typeof (string) == vmType.BaseType)
          return engValue.ToString();
        int num = vmType.BaseType.IsEnum ? 1 : 0;
        return engValue;
      }
      if (typeof (ICommonList).IsAssignableFrom(engValue.GetType()))
      {
        for (int objIndex = 0; objIndex < ((ICommonList) engValue).ObjectsCount; ++objIndex)
        {
          object editorType = ConvertEngineTypeToEditorType(((ICommonList) engValue).GetObject(objIndex));
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
        return lastEntityMethodExecData.TargetEntity != null ? lastEntityMethodExecData : null;
      }
    }

    public static IDynamicGameObjectContext LastMethodExecInitiator
    {
      get => lastMethodExecInitiator;
    }

    public static void OnRestart() => lastEntityMethodExecData.Clear();

    public static void Clear()
    {
      lastMethodExecInitiator = null;
      lastEntityMethodExecData = new EntityMethodExecuteData();
      methodExecuteContextsDict.Clear();
      foreach (KeyValuePair<string, Dictionary<EContextVariableCategory, List<IVariable>>> keyValuePair1 in componentsAbstractVarsDict)
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
      string str = funcComponent.Name + methodName + paramsCount;
      MethodExecuteContext methodExecuteContext1 = null;
      if (methodExecuteContextsDict.TryGetValue(str, out methodExecuteContext1))
        return methodExecuteContext1;
      MethodInfo componentMethodInfo = InfoAttribute.GetComponentMethodInfo(funcComponent.Name, methodName);
      MethodExecuteContext methodExecuteContext2 = new MethodExecuteContext();
      methodExecuteContext2.Initialize(str, componentMethodInfo);
      methodExecuteContextsDict.Add(str, methodExecuteContext2);
      return methodExecuteContext2;
    }
  }
}
