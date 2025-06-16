using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Binders;
using Engine.Common.Components.Movable;
using Engine.Common.MindMap;
using Engine.Common.Reflection;
using Engine.Common.Types;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.VMSpecialAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace PLVirtualMachine.Common.EngineAPI
{
  public class EngineAPIManager
  {
    protected static Dictionary<string, Dictionary<EContextVariableCategory, List<IVariable>>> componentsAbstractVarsDict = new Dictionary<string, Dictionary<EContextVariableCategory, List<IVariable>>>();
    protected static Dictionary<EObjectCategory, List<ComponentInfo>> objectDefaultComponentsDict = new Dictionary<EObjectCategory, List<ComponentInfo>>();
    protected static Dictionary<string, ComponentInfo> componentsInfoDict = new Dictionary<string, ComponentInfo>();
    protected static Dictionary<string, Type> componentTypesDict = new Dictionary<string, Type>();
    protected static Dictionary<Type, string> componentNamesByTypeDict = new Dictionary<Type, string>();
    protected static Dictionary<string, string> functionalDependencyDict = new Dictionary<string, string>();
    protected static Dictionary<TypeEnumKey, string> specMethodInfoDict = new Dictionary<TypeEnumKey, string>((IEqualityComparer<TypeEnumKey>) TypeEnumKeyEqualityComparer.Instance);
    protected static Dictionary<TypeEnumKey, string> specPropertyInfoDict = new Dictionary<TypeEnumKey, string>((IEqualityComparer<TypeEnumKey>) TypeEnumKeyEqualityComparer.Instance);
    protected static Dictionary<TypeEnumKey, string> specEventInfoDict = new Dictionary<TypeEnumKey, string>((IEqualityComparer<TypeEnumKey>) TypeEnumKeyEqualityComparer.Instance);
    private static bool isObjectCreationExtraDebugInfoMode = false;

    public static void Init()
    {
      EngineAPIManager.componentTypesDict.Clear();
      EngineAPIManager.componentNamesByTypeDict.Clear();
      EngineAPIManager.LoadEngineAPIComponents();
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_GAME, "Common");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_GAME, "Support");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_GAME, "Weather");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_GAME, "GlobalMarketManager");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_GAME, "GlobalStorageManager");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_GAME, "GlobalDoorsManager");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_GAME, "GameComponent");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_QUEST, "Common");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_QUEST, "QuestComponent");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_CHARACTER, "Common");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_CHARACTER, "Model");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_CHARACTER, "BehaviorComponent");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_CHARACTER, "Position");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_CHARACTER, "Interactive");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_CHARACTER, "Detector");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_CHARACTER, "Detectable");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_CHARACTER, "Speaking");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_ITEM, "Common");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_ITEM, "Model");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_ITEM, "Storable");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_GEOM, "Common");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_GEOM, "Milestone");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_GEOM, "Position");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_OTHERS, "Common");
      EngineAPIManager.RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_OTHERS, "Storable");
    }

    public static EngineAPIManager Instance { get; protected set; }

    public virtual string CurrentFSMStateInfo => "";

    public virtual VMBaseEntity CreateWorldTemplateDynamicChildInstance(
      IEntity template,
      VMBaseEntity parentEntity,
      IEntity parentInstance)
    {
      return (VMBaseEntity) null;
    }

    public virtual VMBaseEntity CreateWorldTemplateDynamicChildInstance(
      IEntity template,
      IEntity instance,
      bool bOnSaveLoad = false)
    {
      return (VMBaseEntity) null;
    }

    public virtual void InstantiateObject(
      VMBaseEntity obj,
      IEntity forceSpawnPos = null,
      AreaEnum areaType = AreaEnum.Unknown)
    {
    }

    public virtual void SetFatalError(string fatalError)
    {
    }

    public virtual IBlueprint GetEditorTemplateByEngineGuid(Guid guid) => (IBlueprint) null;

    public static ComponentInfo GetFunctionalComponentByName(string componentName)
    {
      ComponentInfo functionalComponentByName;
      if (EngineAPIManager.componentsInfoDict.TryGetValue(componentName, out functionalComponentByName))
        return functionalComponentByName;
      Logger.AddError(string.Format("Functional component with name {0} not found at {1}", (object) componentName, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
      return (ComponentInfo) null;
    }

    public static APIEventInfo GetAPIEventInfoByName(string componentName, string sEventName)
    {
      ComponentInfo componentInfo;
      if (EngineAPIManager.componentsInfoDict.TryGetValue(componentName, out componentInfo))
      {
        for (int index = 0; index < componentInfo.Events.Count; ++index)
        {
          if (componentInfo.Events[index].EventName == sEventName)
            return componentInfo.Events[index];
        }
      }
      return (APIEventInfo) null;
    }

    public static VMType GetEditorTypeByEngType(Type engType, string specTypeName, bool isTemplate)
    {
      if (typeof (IObjRef) == engType || typeof (VMBaseEntity).IsAssignableFrom(engType) || typeof (IEntity) == engType && !isTemplate)
        return new VMType(typeof (IObjRef), specTypeName);
      if (typeof (IBlueprintRef) == engType || typeof (IEntity) == engType & isTemplate)
        return new VMType(typeof (IBlueprintRef), specTypeName);
      if (typeof (ISampleRef) == engType || typeof (Engine.Common.IObject) == engType)
        return new VMType(typeof (ISampleRef), specTypeName, true);
      if (typeof (Engine.Common.IObject).IsAssignableFrom(engType) && !typeof (IEntity).IsAssignableFrom(engType))
      {
        string result;
        SampleAttribute.TryGetValue(engType, out result);
        return new VMType(typeof (ISampleRef), result, true);
      }
      if (typeof (IStateRef) == engType)
        return new VMType(typeof (IStateRef), specTypeName, false);
      if (typeof (ILogicMapRef) == engType)
        return new VMType(typeof (ILogicMapRef), specTypeName, false);
      if (typeof (ILogicMapNodeRef) == engType || typeof (IMMNode) == engType)
        return new VMType(typeof (ILogicMapNodeRef), specTypeName, false);
      return typeof (ICommonList).IsAssignableFrom(engType) ? new VMType(typeof (ICommonList), specTypeName, false) : new VMType(engType);
    }

    public static string GetSpecialFunctionName(
      ESpecialFunctionName specFuncName,
      Type ownerClassType,
      bool bWithComponent = false)
    {
      string specialFunctionInfo = EngineAPIManager.GetSpecialFunctionInfo(specFuncName, ownerClassType);
      return bWithComponent ? EngineAPIManager.GetComponentNameByType(ownerClassType) + "." + specialFunctionInfo : specialFunctionInfo;
    }

    public static string GetSpecialPropertyName(
      ESpecialPropertyName specPropertyName,
      Type ownerClassType,
      bool bWithComponent = false)
    {
      string specialPropertyInfo = EngineAPIManager.GetSpecialPropertyInfo(specPropertyName, ownerClassType);
      return bWithComponent ? EngineAPIManager.GetComponentNameByType(ownerClassType) + "." + specialPropertyInfo : specialPropertyInfo;
    }

    public static string GetSpecialEventName(
      ESpecialEventName specEventName,
      Type ownerClassType,
      bool bWithComponent = false)
    {
      string specialEventInfo = EngineAPIManager.GetSpecialEventInfo(specEventName, ownerClassType);
      return bWithComponent ? EngineAPIManager.GetComponentNameByType(ownerClassType) + "." + specialEventInfo : specialEventInfo;
    }

    public static string GetSpecialFunctionInfo(
      ESpecialFunctionName specFuncName,
      Type ownerClassType)
    {
      TypeEnumKey key = new TypeEnumKey()
      {
        Type = ownerClassType,
        Int = (int) specFuncName
      };
      string specialFunctionInfo;
      if (EngineAPIManager.specMethodInfoDict.TryGetValue(key, out specialFunctionInfo))
        return specialFunctionInfo;
      MethodInfo[] methods = ownerClassType.GetMethods();
      for (int index = 0; index < methods.Length; ++index)
      {
        foreach (SpecialFunctionAttribute customAttribute in methods[index].GetCustomAttributes(typeof (SpecialFunctionAttribute), true))
        {
          if (customAttribute.Name == specFuncName)
          {
            string name = methods[index].Name;
            EngineAPIManager.specMethodInfoDict.Add(key, name);
            return name;
          }
        }
      }
      Logger.AddError(string.Format("No special method info found for special function name {0} for class type {1}", (object) specFuncName, (object) ownerClassType));
      return (string) null;
    }

    public static string GetSpecialPropertyInfo(
      ESpecialPropertyName specPropertyName,
      Type ownerClassType)
    {
      TypeEnumKey key = new TypeEnumKey()
      {
        Type = ownerClassType,
        Int = (int) specPropertyName
      };
      string specialPropertyInfo;
      if (EngineAPIManager.specPropertyInfoDict.TryGetValue(key, out specialPropertyInfo))
        return specialPropertyInfo;
      PropertyInfo[] properties = ownerClassType.GetProperties();
      for (int index = 0; index < properties.Length; ++index)
      {
        foreach (SpecialPropertyAttribute customAttribute in properties[index].GetCustomAttributes(typeof (SpecialPropertyAttribute), true))
        {
          if (customAttribute.Name == specPropertyName)
          {
            string name = properties[index].Name;
            EngineAPIManager.specPropertyInfoDict.Add(key, name);
            return name;
          }
        }
      }
      Logger.AddError(string.Format("No special property info found for special property name {0} for class type {1}", (object) specPropertyName, (object) ownerClassType));
      return (string) null;
    }

    public static string GetSpecialEventInfo(ESpecialEventName specEventName, Type ownerClassType)
    {
      TypeEnumKey key = new TypeEnumKey()
      {
        Type = ownerClassType,
        Int = (int) specEventName
      };
      string specialEventInfo;
      if (EngineAPIManager.specEventInfoDict.TryGetValue(key, out specialEventInfo))
        return specialEventInfo;
      System.Reflection.EventInfo[] events = ownerClassType.GetEvents();
      for (int index = 0; index < events.Length; ++index)
      {
        SpecialEventAttribute specialEventAttribute = EngineAPIManager.GetSpecialEventAttribute(events[index]);
        if (specialEventAttribute != null && specialEventAttribute.Name == specEventName)
        {
          string name = events[index].Name;
          EngineAPIManager.specEventInfoDict.Add(key, name);
          return name;
        }
      }
      Logger.AddError(string.Format("No special event info found for special event name {0} for class type {1}", (object) specEventName, (object) ownerClassType));
      return (string) null;
    }

    private static SpecialEventAttribute GetSpecialEventAttribute(System.Reflection.EventInfo evntInfo)
    {
      object[] customAttributes = evntInfo.GetCustomAttributes(typeof (SpecialEventAttribute), true);
      return customAttributes.Length != 0 ? (SpecialEventAttribute) customAttributes[0] : (SpecialEventAttribute) null;
    }

    public static VMComponent GetObjectComponentByType(
      VMBaseEntity targetObj,
      Type baseComponentType)
    {
      if (targetObj == null || baseComponentType == (Type) null)
        return (VMComponent) null;
      foreach (VMComponent component in targetObj.Components)
      {
        if (baseComponentType.IsAssignableFrom(component.GetType()))
          return component;
      }
      return (VMComponent) null;
    }

    public static Type GetComponentTypeByName(string componentName)
    {
      Type componentTypeByName;
      EngineAPIManager.componentTypesDict.TryGetValue(componentName, out componentTypeByName);
      return componentTypeByName;
    }

    public static string GetComponentNameByType(Type type)
    {
      string str;
      return EngineAPIManager.componentNamesByTypeDict.TryGetValue(type, out str) ? str : "";
    }

    public static Type GetObjectTypeByName(string typeName)
    {
      Type result;
      if (VMTypeAttribute.TryGetValue(typeName, out result) || SampleAttribute.TryGetValue(typeName, out result) || EnumTypeAttribute.TryGetValue(typeName, out result))
        return result;
      Logger.AddError("Cannot get object Type for typename " + typeName + " from ECS at " + EngineAPIManager.Instance.CurrentFSMStateInfo);
      return (Type) null;
    }

    public static string GetObjectTypeNameByType(Type type)
    {
      string result;
      if (VMTypeAttribute.TryGetValue(type, out result))
        return result;
      if (!type.IsEnum)
        return "";
      EnumTypeAttribute.TryGetValue(type, out result);
      return result;
    }

    public static IEnumerable<IVariable> GetAbstractVariablesByFunctionalName(
      string functionalName,
      EContextVariableCategory varCategory)
    {
      if (EngineAPIManager.componentsAbstractVarsDict.ContainsKey(functionalName))
      {
        foreach (IVariable variable in EngineAPIManager.GetAbstractVariablesByRegisteredFunctionalName(functionalName, varCategory))
          yield return variable;
      }
      else
      {
        if (functionalName.StartsWith("I"))
        {
          functionalName = functionalName.Substring("I".Length);
          if (EngineAPIManager.componentsAbstractVarsDict.ContainsKey(functionalName))
          {
            foreach (IVariable variable in EngineAPIManager.GetAbstractVariablesByRegisteredFunctionalName(functionalName, varCategory))
              yield return variable;
          }
          else
            Logger.AddError(string.Format("Functional with name {0} not found in engine api at {1}", (object) functionalName, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
        }
        Logger.AddError(string.Format("Functional with name {0} not found in engine api at {1}", (object) functionalName, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
      }
    }

    public static IEnumerable<IVariable> GetAbstractParametricFunctionsByFunctionalName(
      string functionalName,
      List<VMType> functionParamList)
    {
      IEnumerable<IVariable> variables = (IEnumerable<IVariable>) null;
      if (EngineAPIManager.componentsAbstractVarsDict.ContainsKey(functionalName))
      {
        variables = EngineAPIManager.GetAbstractVariablesByRegisteredFunctionalName(functionalName, EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_APIFUNCTION);
      }
      else
      {
        if (functionalName.StartsWith("I"))
        {
          functionalName = functionalName.Substring("I".Length);
          if (EngineAPIManager.componentsAbstractVarsDict.ContainsKey(functionalName))
            variables = EngineAPIManager.GetAbstractVariablesByRegisteredFunctionalName(functionalName, EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_APIFUNCTION);
          else
            Logger.AddError(string.Format("Functional with name {0} not found in engine api at {1}", (object) functionalName, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
        }
        Logger.AddError(string.Format("Functional with name {0} not found in engine api at {1}", (object) functionalName, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
      }
      if (variables != null)
      {
        foreach (IVariable variable in variables)
        {
          if (variable is BaseFunction baseFunction && baseFunction.InputParams.Count == functionParamList.Count)
          {
            bool flag = true;
            for (int index = 0; index < baseFunction.InputParams.Count; ++index)
            {
              if (functionParamList[index].BaseType == typeof (object))
              {
                if ((baseFunction.InputParams[index].Type.BaseType.IsValueType || baseFunction.InputParams[index].Type.BaseType.IsEnum ? 1 : (baseFunction.InputParams[index].Type.BaseType == typeof (bool) ? 1 : 0)) == 0)
                  flag = false;
              }
              else if (!baseFunction.InputParams[index].Type.EqualsTo(functionParamList[index]))
                flag = false;
            }
            if (flag)
              yield return (IVariable) baseFunction;
          }
        }
      }
    }

    public static string GetEnumTypeName(Type enumType, object enumValue)
    {
      if (enumValue == null)
      {
        Logger.AddError(string.Format("Enum value not defined at {0}", (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
        return "";
      }
      if (!enumType.IsEnum)
      {
        Logger.AddError(string.Format("Type {0} isn't enum type at {1}", (object) enumType, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
        return "";
      }
      MemberInfo[] member = enumType.GetMember(enumValue.ToString());
      if (member != null && member.Length == 1)
      {
        object[] customAttributes = member[0].GetCustomAttributes(typeof (DescriptionAttribute), false);
        if (customAttributes.Length == 1)
          return ((DescriptionAttribute) customAttributes[0]).Description;
      }
      return enumValue.ToString();
    }

    public static LocalizedText CreateEngineTextInstance(ulong textGuid)
    {
      return textGuid != 0UL ? new LocalizedText(textGuid) : LocalizedText.Empty;
    }

    public static LocalizedText CreateEngineTextInstance(IGameString text)
    {
      return text != null ? new LocalizedText(text.BaseGuid) : LocalizedText.Empty;
    }

    public static LocalizedText CreateEngineTextInstance(ITextRef textRef)
    {
      return textRef != null && textRef.Text != null ? new LocalizedText(textRef.Text.BaseGuid) : LocalizedText.Empty;
    }

    protected static IEnumerable<IVariable> GetAbstractVariablesByRegisteredFunctionalName(
      string functionalName,
      EContextVariableCategory varCategory)
    {
      Dictionary<EContextVariableCategory, List<IVariable>> abstractVariablesCategoryDict = EngineAPIManager.componentsAbstractVarsDict[functionalName];
      if (abstractVariablesCategoryDict.ContainsKey(varCategory))
      {
        foreach (IVariable variable in abstractVariablesCategoryDict[varCategory])
          yield return variable;
      }
      else if (varCategory == EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_ALL)
      {
        foreach (KeyValuePair<EContextVariableCategory, List<IVariable>> keyValuePair in abstractVariablesCategoryDict)
        {
          foreach (IVariable variable in keyValuePair.Value)
            yield return variable;
        }
      }
    }

    private static void LoadEngineAPIComponents()
    {
      foreach (ComponentReplectionInfo component in InfoAttribute.Components)
      {
        if (component.DependedComponentName != null && component.DependedComponentName != "")
          EngineAPIManager.RegisterFunctionalDependency(component.Name, component.DependedComponentName);
        ComponentInfo componentInfo = new ComponentInfo(component.Name);
        EngineAPIManager.componentTypesDict.Add(component.Name, component.Type);
        EngineAPIManager.componentNamesByTypeDict.Add(component.Type, component.Name);
        List<APIMethodInfo> methods = EngineAPIManager.ComputeMethods(component);
        componentInfo.Methods.Clear();
        componentInfo.Methods.AddRange((IEnumerable<APIMethodInfo>) methods);
        List<APIEventInfo> events = EngineAPIManager.ComputeEvents(component);
        componentInfo.Events.Clear();
        componentInfo.Events.AddRange((IEnumerable<APIEventInfo>) events);
        List<APIPropertyInfo> properties = EngineAPIManager.ComputeProperties(component);
        componentInfo.Properties.Clear();
        componentInfo.Properties.AddRange((IEnumerable<APIPropertyInfo>) properties);
        EngineAPIManager.componentsInfoDict.Add(component.Name, componentInfo);
        EngineAPIManager.componentsAbstractVarsDict.Add(component.Name, new Dictionary<EContextVariableCategory, List<IVariable>>());
        EngineAPIManager.componentsAbstractVarsDict[component.Name].Add(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_APIFUNCTION, EngineAPIManager.GetAbstractFunctionsList(component, componentInfo.Methods));
        EngineAPIManager.componentsAbstractVarsDict[component.Name].Add(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM, EngineAPIManager.GetAbstractParamsList(component, componentInfo.Properties));
      }
    }

    private static List<APIPropertyInfo> ComputeProperties(ComponentReplectionInfo component)
    {
      List<APIPropertyInfo> properties = new List<APIPropertyInfo>();
      foreach (KeyValuePair<string, PropertyInfo> property in (IEnumerable<KeyValuePair<string, PropertyInfo>>) component.Properties)
      {
        PropertyInfo propertyInfo = property.Value;
        PropertyAttribute propertyAttribInfo = EngineAPIManager.GetPropertyAttribInfo(component.Type, propertyInfo);
        if (propertyAttribInfo != null)
        {
          VMType editorTypeByEngType = EngineAPIManager.GetEditorTypeByEngType(propertyInfo.PropertyType, propertyAttribInfo.SpecialTypeInfo, propertyInfo.IsDefined(typeof (TemplateAttribute), false));
          APIPropertyInfo apiPropertyInfo = new APIPropertyInfo(propertyInfo, editorTypeByEngType);
          apiPropertyInfo.PropertyDefValue = propertyAttribInfo.DefValue;
          if (apiPropertyInfo.PropertyDefValue != null && apiPropertyInfo.PropertyType != null && apiPropertyInfo.PropertyDefValue.GetType() != apiPropertyInfo.PropertyType.BaseType)
            Logger.AddError("Type : " + propertyInfo.DeclaringType.Name + " , property name : " + propertyInfo.Name + " , need type : " + apiPropertyInfo.PropertyType.BaseType.Name + " , has name : " + apiPropertyInfo.PropertyDefValue.GetType().Name);
          properties.Add(apiPropertyInfo);
        }
      }
      return properties;
    }

    private static List<APIEventInfo> ComputeEvents(ComponentReplectionInfo component)
    {
      List<APIEventInfo> events = new List<APIEventInfo>();
      foreach (KeyValuePair<string, System.Reflection.EventInfo> keyValuePair in (IEnumerable<KeyValuePair<string, System.Reflection.EventInfo>>) component.Events)
      {
        System.Reflection.EventInfo reflEventInfo = keyValuePair.Value;
        EventAttribute eventAttribInfo = EngineAPIManager.GetEventAttribInfo(component.Type, reflEventInfo);
        if (eventAttribInfo != null)
        {
          APIEventInfo apiEventInfo = new APIEventInfo(reflEventInfo.Name);
          apiEventInfo.AtOnce = eventAttribInfo.AtOnce;
          if (reflEventInfo.EventHandlerType.IsGenericType)
          {
            Type[] genericArguments = reflEventInfo.EventHandlerType.GetGenericArguments();
            if (genericArguments != null && eventAttribInfo.InputTypesDesc != "")
            {
              string[] strArray1 = eventAttribInfo.InputTypesDesc.Split(',');
              if (strArray1.Length != genericArguments.Length)
                Logger.AddError(string.Format("{0}: attribute event desc params count not corresponds to real params count", (object) apiEventInfo.EventName));
              for (int index = 0; index < genericArguments.Length; ++index)
              {
                string name = "param_" + index.ToString();
                string specTypeName = "";
                bool isTemplate = false;
                if (index < strArray1.Length)
                {
                  string[] strArray2 = strArray1[index].Split(':');
                  if (strArray2.Length != 0)
                    name = strArray2[0];
                  if (strArray2.Length > 1)
                  {
                    isTemplate = strArray2[1] == "template";
                    if (!isTemplate)
                      specTypeName = strArray2[1];
                    else if (strArray2.Length > 2)
                      specTypeName = strArray2[2];
                  }
                }
                try
                {
                  APIParamInfo apiParamInfo = new APIParamInfo(EngineAPIManager.GetEditorTypeByEngType(genericArguments[index], specTypeName, isTemplate), name);
                  apiEventInfo.MessageParams.Add(apiParamInfo);
                }
                catch (Exception ex)
                {
                  Logger.AddError(ex.ToString());
                }
              }
            }
          }
          else
          {
            List<ParameterInfo> parameterInfoList = new List<ParameterInfo>();
            foreach (MethodInfo method in reflEventInfo.EventHandlerType.GetMethods())
            {
              if (method.Name == "Invoke")
              {
                foreach (ParameterInfo parameter in method.GetParameters())
                  parameterInfoList.Add(parameter);
                break;
              }
            }
            if (eventAttribInfo.InputTypesDesc != "")
            {
              string[] strArray3 = eventAttribInfo.InputTypesDesc.Split(',');
              if (strArray3.Length != parameterInfoList.Count)
                Logger.AddError(string.Format("{0}: attribute event desc params count not corresponds to real params count", (object) apiEventInfo.EventName));
              for (int index = 0; index < parameterInfoList.Count; ++index)
              {
                string name = "param_" + index.ToString();
                string specTypeName = "";
                if (index < strArray3.Length)
                {
                  string[] strArray4 = strArray3[index].Split(':');
                  if (strArray4.Length != 0)
                    name = strArray4[0];
                  if (strArray4.Length > 1)
                    specTypeName = strArray4[1];
                }
                try
                {
                  bool isTemplate = parameterInfoList[index].IsDefined(typeof (TemplateAttribute), false);
                  APIParamInfo apiParamInfo = new APIParamInfo(EngineAPIManager.GetEditorTypeByEngType(parameterInfoList[index].ParameterType, specTypeName, isTemplate), name);
                  apiEventInfo.MessageParams.Add(apiParamInfo);
                }
                catch (Exception ex)
                {
                  Logger.AddError(ex.ToString());
                }
              }
            }
          }
          events.Add(apiEventInfo);
        }
      }
      return events;
    }

    private static List<APIMethodInfo> ComputeMethods(ComponentReplectionInfo component)
    {
      List<APIMethodInfo> methods = new List<APIMethodInfo>();
      foreach (KeyValuePair<string, MethodInfo> method in (IEnumerable<KeyValuePair<string, MethodInfo>>) component.Methods)
      {
        string key = method.Key;
        MethodInfo reflMethodInfo = method.Value;
        MethodAttribute methodAttribInfo = EngineAPIManager.GetMethodAttribInfo(component.Type, reflMethodInfo);
        if (methodAttribInfo != null)
        {
          APIMethodInfo apiMethodInfo = new APIMethodInfo(key);
          apiMethodInfo.ReturnParam = new APIParamInfo(EngineAPIManager.GetEditorTypeByEngType(reflMethodInfo.ReturnType, methodAttribInfo.OutputTypesSpecialInfo, false));
          ParameterInfo[] parameters = reflMethodInfo.GetParameters();
          string[] strArray1 = (string[]) null;
          if ("" != methodAttribInfo.InputTypesSpecialInfo)
            strArray1 = methodAttribInfo.InputTypesSpecialInfo.Split(',');
          if (strArray1 != null && strArray1.Length != parameters.Length)
            Logger.AddError(string.Format("Method {0} attribute params types info length don't match params count", (object) apiMethodInfo.MethodName));
          for (int index = 0; index < parameters.Length; ++index)
          {
            string name = "";
            string specTypeName = "";
            if (strArray1 != null && index < strArray1.Length)
            {
              string[] strArray2 = strArray1[index].Split(':');
              if (strArray2.Length != 0)
                name = strArray2[0];
              if (strArray2.Length > 1)
                specTypeName = strArray2[1];
            }
            APIParamInfo apiParamInfo = new APIParamInfo(EngineAPIManager.GetEditorTypeByEngType(parameters[index].ParameterType, specTypeName, parameters[index].IsDefined(typeof (TemplateAttribute), false)), name);
            apiMethodInfo.InputParams.Add(apiParamInfo);
          }
          methods.Add(apiMethodInfo);
        }
      }
      return methods;
    }

    private static void RegisterFunctionalDependency(
      string functionalName,
      string sDependedFunctionalName)
    {
      if (EngineAPIManager.functionalDependencyDict.ContainsKey(functionalName))
        return;
      EngineAPIManager.functionalDependencyDict.Add(functionalName, sDependedFunctionalName);
    }

    public static string GetDependedFunctional(string functionalName)
    {
      string str;
      return EngineAPIManager.functionalDependencyDict.TryGetValue(functionalName, out str) ? str : "";
    }

    public static bool ObjectCreationExtraDebugInfoMode
    {
      get => EngineAPIManager.isObjectCreationExtraDebugInfoMode;
      set => EngineAPIManager.isObjectCreationExtraDebugInfoMode = value;
    }

    private static List<IVariable> GetAbstractFunctionsList(
      ComponentReplectionInfo componentInfo,
      List<APIMethodInfo> methodList)
    {
      List<IVariable> abstractFunctionsList = new List<IVariable>();
      for (int index = 0; index < methodList.Count; ++index)
      {
        BaseFunction baseFunction = new BaseFunction(methodList[index].MethodName, componentInfo.Name);
        baseFunction.InitParams(methodList[index].InputParams, methodList[index].ReturnParam);
        abstractFunctionsList.Add((IVariable) baseFunction);
      }
      return abstractFunctionsList;
    }

    private static List<IVariable> GetAbstractParamsList(
      ComponentReplectionInfo componentInfo,
      List<APIPropertyInfo> propertiesList)
    {
      List<IVariable> abstractParamsList = new List<IVariable>();
      for (int index = 0; index < propertiesList.Count; ++index)
      {
        string propertyName = propertiesList[index].PropertyName;
        string name = componentInfo.Name;
        object propertyDefValue = propertiesList[index].PropertyDefValue;
        string componentName = name;
        VMType propertyType = propertiesList[index].PropertyType;
        object defValue = propertyDefValue;
        AbstractParameter abstractParameter = new AbstractParameter(propertyName, componentName, propertyType, defValue);
        abstractParamsList.Add((IVariable) abstractParameter);
      }
      return abstractParamsList;
    }

    private static void RegistrObjectDefaultComponent(
      EObjectCategory objCategory,
      string engineComponentName)
    {
      if (!EngineAPIManager.objectDefaultComponentsDict.ContainsKey(objCategory))
        EngineAPIManager.objectDefaultComponentsDict.Add(objCategory, new List<ComponentInfo>());
      ComponentInfo functionalComponentByName = EngineAPIManager.GetFunctionalComponentByName(engineComponentName);
      if (functionalComponentByName == null)
        Logger.AddError(string.Format("Functional component with name {0} not found", (object) engineComponentName));
      else
        EngineAPIManager.objectDefaultComponentsDict[objCategory].Add(functionalComponentByName);
    }

    private static MethodAttribute GetMethodAttribInfo(
      Type componentType,
      MethodInfo reflMethodInfo)
    {
      object[] customAttributes = reflMethodInfo.GetCustomAttributes(typeof (MethodAttribute), true);
      return customAttributes.Length != 0 ? (MethodAttribute) customAttributes[0] : (MethodAttribute) null;
    }

    private static EventAttribute GetEventAttribInfo(Type componentType, System.Reflection.EventInfo reflEventInfo)
    {
      object[] customAttributes = reflEventInfo.GetCustomAttributes(typeof (EventAttribute), true);
      return customAttributes.Length != 0 ? (EventAttribute) customAttributes[0] : (EventAttribute) null;
    }

    private static PropertyAttribute GetPropertyAttribInfo(
      Type componentType,
      PropertyInfo reflPropertyInfo)
    {
      object[] customAttributes = reflPropertyInfo.GetCustomAttributes(typeof (PropertyAttribute), true);
      return customAttributes.Length != 0 ? (PropertyAttribute) customAttributes[0] : (PropertyAttribute) null;
    }
  }
}
