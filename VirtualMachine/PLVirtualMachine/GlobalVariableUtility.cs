using System;
using System.Collections.Generic;
using Cofe.Loggers;
using Engine.Common.Comparers;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.GameLogic;

namespace PLVirtualMachine
{
  public static class GlobalVariableUtility
  {
    private static Dictionary<string, Dictionary<ulong, object>> globalVariablesDict = new();

    public static void RegistrInGlobalVariables(VMEntity entity)
    {
      IBlueprint editorTemplate = entity.EditorTemplate;
      if (!editorTemplate.Static)
      {
        VMObjRef instanceRef = new VMObjRef();
        instanceRef.InitializeInstance(entity);
        AddInstanceToGlobalTemplatedLists(editorTemplate, instanceRef);
      }
      List<IBlueprint> baseBlueprints = editorTemplate.BaseBlueprints;
      if (baseBlueprints == null || baseBlueprints.Count <= 0)
        return;
      VMObjRef instanceRef1 = new VMObjRef();
      for (int index = 0; index < baseBlueprints.Count; ++index)
      {
        instanceRef1.InitializeInstance(entity);
        AddInstanceToGlobalTemplatedLists(baseBlueprints[index], instanceRef1);
      }
    }

    public static void RemoveInstanceFromGlobalTemplatedLists(IBlueprint template, Guid objGuid)
    {
      object globalVariable = GetGlobalVariable(GetInstanceListGlobalVarCategoryName(template.GetCategory()), template.BaseGuid);
      if (globalVariable != null)
      {
        if (typeof (ICommonList).IsAssignableFrom(globalVariable.GetType()))
          ((ICommonList) globalVariable).RemoveObjectInstanceByGuid(objGuid);
        else
          Logger.AddError(string.Format("Global variable for template {0} instances list has invalid type {1} at {2}", template.Name, globalVariable.GetType(), DynamicFSM.CurrentStateInfo));
      }
      List<IBlueprint> baseBlueprints = template.BaseBlueprints;
      if (baseBlueprints == null)
        return;
      for (int index = 0; index < baseBlueprints.Count; ++index)
        RemoveInstanceFromGlobalTemplatedLists(baseBlueprints[index], objGuid);
    }

    public static void RegistrGlobalTemplateInstancesList(IBlueprint templateObj)
    {
      RegisterGlobalVariable(GetInstanceListGlobalVarCategoryName(templateObj.GetCategory()), templateObj.BaseGuid);
    }

    public static void Clear()
    {
      foreach (KeyValuePair<string, Dictionary<ulong, object>> keyValuePair1 in globalVariablesDict)
      {
        foreach (KeyValuePair<ulong, object> keyValuePair2 in keyValuePair1.Value)
        {
          object obj = keyValuePair2.Value;
          if (typeof (ICommonList).IsAssignableFrom(obj.GetType()))
            ((ICommonList) obj).Clear();
        }
      }
    }

    public static void ClearAll()
    {
      Clear();
      globalVariablesDict.Clear();
    }

    public static object GetGlobalVariableValue(string globalVarName)
    {
      string[] strArray = globalVarName.Split('_');
      if (strArray.Length > 1)
      {
        ulong uint64 = StringUtility.ToUInt64(strArray[1]);
        if (uint64 > 0UL)
          return GetGlobalVariableValue(strArray[0], uint64);
      }
      Logger.AddError(string.Format("Global variable with name {0} is incorrect at {1}", globalVarName, DynamicFSM.CurrentStateInfo));
      return null;
    }

    private static void RegisterGlobalVariable(string globalVarCategory, ulong globalVarStaticID)
    {
      if (!globalVariablesDict.ContainsKey(globalVarCategory))
        globalVariablesDict.Add(globalVarCategory, new Dictionary<ulong, object>(UlongComparer.Instance));
      if (globalVarCategory.Contains("InstanceList"))
      {
        if (globalVariablesDict[globalVarCategory].ContainsKey(globalVarStaticID))
          return;
        globalVariablesDict[globalVarCategory].Add(globalVarStaticID, new VMDynamicCommonList());
      }
      else
        Logger.AddError(string.Format("Global variables category {0} is incorrect at {1}", globalVarCategory, DynamicFSM.CurrentStateInfo));
    }

    private static object GetGlobalVariable(string globalVarCategory, ulong globalVarStaticID)
    {
      if (globalVariablesDict.TryGetValue(globalVarCategory, out Dictionary<ulong, object> dictionary))
      {
        if (!dictionary.ContainsKey(globalVarStaticID))
          dictionary.Add(globalVarStaticID, new VMDynamicCommonList());
        if (dictionary.TryGetValue(globalVarStaticID, out object globalVariable))
          return globalVariable;
        Logger.AddError(string.Format("Global variable with category {0} and static id {1} not found at {2}", globalVarCategory, globalVarStaticID, DynamicFSM.CurrentStateInfo));
      }
      else
        Logger.AddError(string.Format("Global variables category {0} is incorrect at {1}", globalVarCategory, DynamicFSM.CurrentStateInfo));
      return null;
    }

    private static object GetGlobalVariableValue(string globalVarCategory, ulong globalVarStaticID)
    {
      if (globalVariablesDict.TryGetValue(globalVarCategory, out Dictionary<ulong, object> dictionary))
      {
        if (dictionary.TryGetValue(globalVarStaticID, out object globalVariableValue))
          return globalVariableValue;
        Logger.AddError(string.Format("Global variable with static id {0} not found in global variables dictionary with category {1}", globalVarStaticID, globalVarCategory));
      }
      else
        Logger.AddError(string.Format("Global variable category {0} is incorrect", globalVarCategory));
      return null;
    }

    private static void AddInstanceToGlobalTemplatedLists(IBlueprint template, VMObjRef instanceRef)
    {
      object globalVariable = GetGlobalVariable(GetInstanceListGlobalVarCategoryName(template.GetCategory()), template.BaseGuid);
      if (globalVariable != null)
      {
        if (typeof (ICommonList).IsAssignableFrom(globalVariable.GetType()))
          ((ICommonList) globalVariable).AddObject(instanceRef);
        else
          Logger.AddError(string.Format("Global variable for template {0} instances list has invalid type {1} at {2}", template.Name, globalVariable.GetType(), DynamicFSM.CurrentStateInfo));
      }
      List<IBlueprint> baseBlueprints = template.BaseBlueprints;
      if (baseBlueprints == null)
        return;
      for (int index = 0; index < baseBlueprints.Count; ++index)
        AddInstanceToGlobalTemplatedLists(baseBlueprints[index], instanceRef);
    }

    public static GlobalVariable CreateGlobalTemplateInstancesListVariable(IBlueprint template)
    {
      string name = "global_TemplateInstanceList_" + template.BaseGuid;
      if (template.GetCategory() == EObjectCategory.OBJECT_CATEGORY_CLASS)
        name = "global_ClassInstanceList_" + template.BaseGuid;
      VMType typeByElementType = VMType.CreateListTypeByElementType(VMType.CreateBlueprintSpecialType(template));
      GlobalVariable instancesListVariable = new GlobalVariable();
      instancesListVariable.Initialize(name, typeByElementType);
      return instancesListVariable;
    }

    public static string GetInstanceListGlobalVarCategoryName(EObjectCategory instanceCategory)
    {
      return instanceCategory == EObjectCategory.OBJECT_CATEGORY_CLASS ? "ClassInstanceList" : "TemplateInstanceList";
    }
  }
}
