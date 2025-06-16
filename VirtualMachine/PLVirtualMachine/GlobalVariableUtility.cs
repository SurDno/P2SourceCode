using Cofe.Loggers;
using Engine.Common.Comparers;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Dynamic;
using PLVirtualMachine.GameLogic;
using System;
using System.Collections.Generic;

namespace PLVirtualMachine
{
  public static class GlobalVariableUtility
  {
    private static Dictionary<string, Dictionary<ulong, object>> globalVariablesDict = new Dictionary<string, Dictionary<ulong, object>>();

    public static void RegistrInGlobalVariables(VMEntity entity)
    {
      IBlueprint editorTemplate = entity.EditorTemplate;
      if (!editorTemplate.Static)
      {
        VMObjRef instanceRef = new VMObjRef();
        instanceRef.InitializeInstance((IEngineRTInstance) entity);
        GlobalVariableUtility.AddInstanceToGlobalTemplatedLists(editorTemplate, instanceRef);
      }
      List<IBlueprint> baseBlueprints = editorTemplate.BaseBlueprints;
      if (baseBlueprints == null || baseBlueprints.Count <= 0)
        return;
      VMObjRef instanceRef1 = new VMObjRef();
      for (int index = 0; index < baseBlueprints.Count; ++index)
      {
        instanceRef1.InitializeInstance((IEngineRTInstance) entity);
        GlobalVariableUtility.AddInstanceToGlobalTemplatedLists(baseBlueprints[index], instanceRef1);
      }
    }

    public static void RemoveInstanceFromGlobalTemplatedLists(IBlueprint template, Guid objGuid)
    {
      object globalVariable = GlobalVariableUtility.GetGlobalVariable(GlobalVariableUtility.GetInstanceListGlobalVarCategoryName(template.GetCategory()), template.BaseGuid);
      if (globalVariable != null)
      {
        if (typeof (ICommonList).IsAssignableFrom(globalVariable.GetType()))
          ((ICommonList) globalVariable).RemoveObjectInstanceByGuid(objGuid);
        else
          Logger.AddError(string.Format("Global variable for template {0} instances list has invalid type {1} at {2}", (object) template.Name, (object) globalVariable.GetType(), (object) DynamicFSM.CurrentStateInfo));
      }
      List<IBlueprint> baseBlueprints = template.BaseBlueprints;
      if (baseBlueprints == null)
        return;
      for (int index = 0; index < baseBlueprints.Count; ++index)
        GlobalVariableUtility.RemoveInstanceFromGlobalTemplatedLists(baseBlueprints[index], objGuid);
    }

    public static void RegistrGlobalTemplateInstancesList(IBlueprint templateObj)
    {
      GlobalVariableUtility.RegisterGlobalVariable(GlobalVariableUtility.GetInstanceListGlobalVarCategoryName(templateObj.GetCategory()), templateObj.BaseGuid);
    }

    public static void Clear()
    {
      foreach (KeyValuePair<string, Dictionary<ulong, object>> keyValuePair1 in GlobalVariableUtility.globalVariablesDict)
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
      GlobalVariableUtility.Clear();
      GlobalVariableUtility.globalVariablesDict.Clear();
    }

    public static object GetGlobalVariableValue(string globalVarName)
    {
      string[] strArray = globalVarName.Split('_');
      if (strArray.Length > 1)
      {
        ulong uint64 = StringUtility.ToUInt64(strArray[1]);
        if (uint64 > 0UL)
          return GlobalVariableUtility.GetGlobalVariableValue(strArray[0], uint64);
      }
      Logger.AddError(string.Format("Global variable with name {0} is incorrect at {1}", (object) globalVarName, (object) DynamicFSM.CurrentStateInfo));
      return (object) null;
    }

    private static void RegisterGlobalVariable(string globalVarCategory, ulong globalVarStaticID)
    {
      if (!GlobalVariableUtility.globalVariablesDict.ContainsKey(globalVarCategory))
        GlobalVariableUtility.globalVariablesDict.Add(globalVarCategory, new Dictionary<ulong, object>((IEqualityComparer<ulong>) UlongComparer.Instance));
      if (globalVarCategory.Contains("InstanceList"))
      {
        if (GlobalVariableUtility.globalVariablesDict[globalVarCategory].ContainsKey(globalVarStaticID))
          return;
        GlobalVariableUtility.globalVariablesDict[globalVarCategory].Add(globalVarStaticID, (object) new VMDynamicCommonList());
      }
      else
        Logger.AddError(string.Format("Global variables category {0} is incorrect at {1}", (object) globalVarCategory, (object) DynamicFSM.CurrentStateInfo));
    }

    private static object GetGlobalVariable(string globalVarCategory, ulong globalVarStaticID)
    {
      Dictionary<ulong, object> dictionary;
      if (GlobalVariableUtility.globalVariablesDict.TryGetValue(globalVarCategory, out dictionary))
      {
        if (!dictionary.ContainsKey(globalVarStaticID))
          dictionary.Add(globalVarStaticID, (object) new VMDynamicCommonList());
        object globalVariable;
        if (dictionary.TryGetValue(globalVarStaticID, out globalVariable))
          return globalVariable;
        Logger.AddError(string.Format("Global variable with category {0} and static id {1} not found at {2}", (object) globalVarCategory, (object) globalVarStaticID, (object) DynamicFSM.CurrentStateInfo));
      }
      else
        Logger.AddError(string.Format("Global variables category {0} is incorrect at {1}", (object) globalVarCategory, (object) DynamicFSM.CurrentStateInfo));
      return (object) null;
    }

    private static object GetGlobalVariableValue(string globalVarCategory, ulong globalVarStaticID)
    {
      Dictionary<ulong, object> dictionary;
      if (GlobalVariableUtility.globalVariablesDict.TryGetValue(globalVarCategory, out dictionary))
      {
        object globalVariableValue;
        if (dictionary.TryGetValue(globalVarStaticID, out globalVariableValue))
          return globalVariableValue;
        Logger.AddError(string.Format("Global variable with static id {0} not found in global variables dictionary with category {1}", (object) globalVarStaticID, (object) globalVarCategory));
      }
      else
        Logger.AddError(string.Format("Global variable category {0} is incorrect", (object) globalVarCategory));
      return (object) null;
    }

    private static void AddInstanceToGlobalTemplatedLists(IBlueprint template, VMObjRef instanceRef)
    {
      object globalVariable = GlobalVariableUtility.GetGlobalVariable(GlobalVariableUtility.GetInstanceListGlobalVarCategoryName(template.GetCategory()), template.BaseGuid);
      if (globalVariable != null)
      {
        if (typeof (ICommonList).IsAssignableFrom(globalVariable.GetType()))
          ((ICommonList) globalVariable).AddObject((object) instanceRef);
        else
          Logger.AddError(string.Format("Global variable for template {0} instances list has invalid type {1} at {2}", (object) template.Name, (object) globalVariable.GetType(), (object) DynamicFSM.CurrentStateInfo));
      }
      List<IBlueprint> baseBlueprints = template.BaseBlueprints;
      if (baseBlueprints == null)
        return;
      for (int index = 0; index < baseBlueprints.Count; ++index)
        GlobalVariableUtility.AddInstanceToGlobalTemplatedLists(baseBlueprints[index], instanceRef);
    }

    public static GlobalVariable CreateGlobalTemplateInstancesListVariable(IBlueprint template)
    {
      string name = "global_TemplateInstanceList_" + (object) template.BaseGuid;
      if (template.GetCategory() == EObjectCategory.OBJECT_CATEGORY_CLASS)
        name = "global_ClassInstanceList_" + (object) template.BaseGuid;
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
