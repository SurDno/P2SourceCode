using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
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

namespace PLVirtualMachine.Common.EngineAPI
{
  public class EngineAPIManager
  {
    protected static Dictionary<string, Dictionary<EContextVariableCategory, List<IVariable>>> componentsAbstractVarsDict = new();
    protected static Dictionary<EObjectCategory, List<ComponentInfo>> objectDefaultComponentsDict = new();
    protected static Dictionary<string, ComponentInfo> componentsInfoDict = new();
    protected static Dictionary<string, Type> componentTypesDict = new();
    protected static Dictionary<Type, string> componentNamesByTypeDict = new();
    protected static Dictionary<string, string> functionalDependencyDict = new();
    protected static Dictionary<TypeEnumKey, string> specMethodInfoDict = new(TypeEnumKeyEqualityComparer.Instance);
    protected static Dictionary<TypeEnumKey, string> specPropertyInfoDict = new(TypeEnumKeyEqualityComparer.Instance);
    protected static Dictionary<TypeEnumKey, string> specEventInfoDict = new(TypeEnumKeyEqualityComparer.Instance);
    private static bool isObjectCreationExtraDebugInfoMode;

    public static void Init()
    {
      componentTypesDict.Clear();
      componentNamesByTypeDict.Clear();
      LoadEngineAPIComponents();
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_GAME, "Common");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_GAME, "Support");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_GAME, "Weather");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_GAME, "GlobalMarketManager");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_GAME, "GlobalStorageManager");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_GAME, "GlobalDoorsManager");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_GAME, "GameComponent");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_QUEST, "Common");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_QUEST, "QuestComponent");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_CHARACTER, "Common");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_CHARACTER, "Model");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_CHARACTER, "BehaviorComponent");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_CHARACTER, "Position");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_CHARACTER, "Interactive");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_CHARACTER, "Detector");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_CHARACTER, "Detectable");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_CHARACTER, "Speaking");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_ITEM, "Common");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_ITEM, "Model");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_ITEM, "Storable");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_GEOM, "Common");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_GEOM, "Milestone");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_GEOM, "Position");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_OTHERS, "Common");
      RegistrObjectDefaultComponent(EObjectCategory.OBJECT_CATEGORY_OTHERS, "Storable");
    }

    public static EngineAPIManager Instance { get; protected set; }

    public virtual string CurrentFSMStateInfo => "";

    public virtual VMBaseEntity CreateWorldTemplateDynamicChildInstance(
      IEntity template,
      VMBaseEntity parentEntity,
      IEntity parentInstance)
    {
      return null;
    }

    public virtual VMBaseEntity CreateWorldTemplateDynamicChildInstance(
      IEntity template,
      IEntity instance,
      bool bOnSaveLoad = false)
    {
      return null;
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

    public virtual IBlueprint GetEditorTemplateByEngineGuid(Guid guid) => null;

    public static ComponentInfo GetFunctionalComponentByName(string componentName)
    {
      if (componentsInfoDict.TryGetValue(componentName, out ComponentInfo functionalComponentByName))
        return functionalComponentByName;
      Logger.AddError(string.Format("Functional component with name {0} not found at {1}", componentName, Instance.CurrentFSMStateInfo));
      return null;
    }

    public static APIEventInfo GetAPIEventInfoByName(string componentName, string sEventName)
    {
      if (componentsInfoDict.TryGetValue(componentName, out ComponentInfo componentInfo))
      {
        for (int index = 0; index < componentInfo.Events.Count; ++index)
        {
          if (componentInfo.Events[index].EventName == sEventName)
            return componentInfo.Events[index];
        }
      }
      return null;
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
        SampleAttribute.TryGetValue(engType, out string result);
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
      string specialFunctionInfo = GetSpecialFunctionInfo(specFuncName, ownerClassType);
      return bWithComponent ? GetComponentNameByType(ownerClassType) + "." + specialFunctionInfo : specialFunctionInfo;
    }

    public static string GetSpecialPropertyName(
      ESpecialPropertyName specPropertyName,
      Type ownerClassType,
      bool bWithComponent = false)
    {
      string specialPropertyInfo = GetSpecialPropertyInfo(specPropertyName, ownerClassType);
      return bWithComponent ? GetComponentNameByType(ownerClassType) + "." + specialPropertyInfo : specialPropertyInfo;
    }

    public static string GetSpecialEventName(
      ESpecialEventName specEventName,
      Type ownerClassType,
      bool bWithComponent = false)
    {
      string specialEventInfo = GetSpecialEventInfo(specEventName, ownerClassType);
      return bWithComponent ? GetComponentNameByType(ownerClassType) + "." + specialEventInfo : specialEventInfo;
    }

    public static string GetSpecialFunctionInfo(
      ESpecialFunctionName specFuncName,
      Type ownerClassType)
    {
      TypeEnumKey key = new TypeEnumKey {
        Type = ownerClassType,
        Int = (int) specFuncName
      };
      if (specMethodInfoDict.TryGetValue(key, out string specialFunctionInfo))
        return specialFunctionInfo;
      MethodInfo[] methods = ownerClassType.GetMethods();
      for (int index = 0; index < methods.Length; ++index)
      {
        foreach (SpecialFunctionAttribute customAttribute in methods[index].GetCustomAttributes(typeof (SpecialFunctionAttribute), true))
        {
          if (customAttribute.Name == specFuncName)
          {
            string name = methods[index].Name;
            specMethodInfoDict.Add(key, name);
            return name;
          }
        }
      }
      Logger.AddError(string.Format("No special method info found for special function name {0} for class type {1}", specFuncName, ownerClassType));
      return null;
    }

    public static string GetSpecialPropertyInfo(
      ESpecialPropertyName specPropertyName,
      Type ownerClassType)
    {
      TypeEnumKey key = new TypeEnumKey {
        Type = ownerClassType,
        Int = (int) specPropertyName
      };
      if (specPropertyInfoDict.TryGetValue(key, out string specialPropertyInfo))
        return specialPropertyInfo;
      PropertyInfo[] properties = ownerClassType.GetProperties();
      for (int index = 0; index < properties.Length; ++index)
      {
        foreach (SpecialPropertyAttribute customAttribute in properties[index].GetCustomAttributes(typeof (SpecialPropertyAttribute), true))
        {
          if (customAttribute.Name == specPropertyName)
          {
            string name = properties[index].Name;
            specPropertyInfoDict.Add(key, name);
            return name;
          }
        }
      }
      Logger.AddError(string.Format("No special property info found for special property name {0} for class type {1}", specPropertyName, ownerClassType));
      return null;
    }

    public static string GetSpecialEventInfo(ESpecialEventName specEventName, Type ownerClassType)
    {
      TypeEnumKey key = new TypeEnumKey {
        Type = ownerClassType,
        Int = (int) specEventName
      };
      if (specEventInfoDict.TryGetValue(key, out string specialEventInfo))
        return specialEventInfo;
      System.Reflection.EventInfo[] events = ownerClassType.GetEvents();
      for (int index = 0; index < events.Length; ++index)
      {
        SpecialEventAttribute specialEventAttribute = GetSpecialEventAttribute(events[index]);
        if (specialEventAttribute != null && specialEventAttribute.Name == specEventName)
        {
          string name = events[index].Name;
          specEventInfoDict.Add(key, name);
          return name;
        }
      }
      Logger.AddError(string.Format("No special event info found for special event name {0} for class type {1}", specEventName, ownerClassType));
      return null;
    }

    private static SpecialEventAttribute GetSpecialEventAttribute(System.Reflection.EventInfo evntInfo)
    {
      object[] customAttributes = evntInfo.GetCustomAttributes(typeof (SpecialEventAttribute), true);
      return customAttributes.Length != 0 ? (SpecialEventAttribute) customAttributes[0] : null;
    }

    public static VMComponent GetObjectComponentByType(
      VMBaseEntity targetObj,
      Type baseComponentType)
    {
      if (targetObj == null || baseComponentType == null)
        return null;
      foreach (VMComponent component in targetObj.Components)
      {
        if (baseComponentType.IsAssignableFrom(component.GetType()))
          return component;
      }
      return null;
    }

    public static Type GetComponentTypeByName(string componentName)
    {
      componentTypesDict.TryGetValue(componentName, out Type componentTypeByName);
      return componentTypeByName;
    }

    public static string GetComponentNameByType(Type type)
    {
      return componentNamesByTypeDict.TryGetValue(type, out string str) ? str : "";
    }

    public static Type GetObjectTypeByName(string typeName)
    {
      if (VMTypeAttribute.TryGetValue(typeName, out Type result) || SampleAttribute.TryGetValue(typeName, out result) || EnumTypeAttribute.TryGetValue(typeName, out result))
        return result;
      Logger.AddError("Cannot get object Type for typename " + typeName + " from ECS at " + Instance.CurrentFSMStateInfo);
      return null;
    }

    public static string GetObjectTypeNameByType(Type type)
    {
      if (VMTypeAttribute.TryGetValue(type, out string result))
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
      if (componentsAbstractVarsDict.ContainsKey(functionalName))
      {
        foreach (IVariable variable in GetAbstractVariablesByRegisteredFunctionalName(functionalName, varCategory))
          yield return variable;
      }
      else
      {
        if (functionalName.StartsWith("I"))
        {
          functionalName = functionalName.Substring("I".Length);
          if (componentsAbstractVarsDict.ContainsKey(functionalName))
          {
            foreach (IVariable variable in GetAbstractVariablesByRegisteredFunctionalName(functionalName, varCategory))
              yield return variable;
          }
          else
            Logger.AddError(string.Format("Functional with name {0} not found in engine api at {1}", functionalName, Instance.CurrentFSMStateInfo));
        }
        Logger.AddError(string.Format("Functional with name {0} not found in engine api at {1}", functionalName, Instance.CurrentFSMStateInfo));
      }
    }

    public static IEnumerable<IVariable> GetAbstractParametricFunctionsByFunctionalName(
      string functionalName,
      List<VMType> functionParamList)
    {
      IEnumerable<IVariable> variables = null;
      if (componentsAbstractVarsDict.ContainsKey(functionalName))
      {
        variables = GetAbstractVariablesByRegisteredFunctionalName(functionalName, EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_APIFUNCTION);
      }
      else
      {
        if (functionalName.StartsWith("I"))
        {
          functionalName = functionalName.Substring("I".Length);
          if (componentsAbstractVarsDict.ContainsKey(functionalName))
            variables = GetAbstractVariablesByRegisteredFunctionalName(functionalName, EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_APIFUNCTION);
          else
            Logger.AddError(string.Format("Functional with name {0} not found in engine api at {1}", functionalName, Instance.CurrentFSMStateInfo));
        }
        Logger.AddError(string.Format("Functional with name {0} not found in engine api at {1}", functionalName, Instance.CurrentFSMStateInfo));
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
              yield return baseFunction;
          }
        }
      }
    }

    public static string GetEnumTypeName(Type enumType, object enumValue)
    {
      if (enumValue == null)
      {
        Logger.AddError(string.Format("Enum value not defined at {0}", Instance.CurrentFSMStateInfo));
        return "";
      }
      if (!enumType.IsEnum)
      {
        Logger.AddError(string.Format("Type {0} isn't enum type at {1}", enumType, Instance.CurrentFSMStateInfo));
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
      Dictionary<EContextVariableCategory, List<IVariable>> abstractVariablesCategoryDict = componentsAbstractVarsDict[functionalName];
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
          RegisterFunctionalDependency(component.Name, component.DependedComponentName);
        ComponentInfo componentInfo = new ComponentInfo(component.Name);
        componentTypesDict.Add(component.Name, component.Type);
        componentNamesByTypeDict.Add(component.Type, component.Name);
        List<APIMethodInfo> methods = ComputeMethods(component);
        componentInfo.Methods.Clear();
        componentInfo.Methods.AddRange(methods);
        List<APIEventInfo> events = ComputeEvents(component);
        componentInfo.Events.Clear();
        componentInfo.Events.AddRange(events);
        List<APIPropertyInfo> properties = ComputeProperties(component);
        componentInfo.Properties.Clear();
        componentInfo.Properties.AddRange(properties);
        componentsInfoDict.Add(component.Name, componentInfo);
        componentsAbstractVarsDict.Add(component.Name, new Dictionary<EContextVariableCategory, List<IVariable>>());
        componentsAbstractVarsDict[component.Name].Add(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_APIFUNCTION, GetAbstractFunctionsList(component, componentInfo.Methods));
        componentsAbstractVarsDict[component.Name].Add(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM, GetAbstractParamsList(component, componentInfo.Properties));
      }
    }

    private static List<APIPropertyInfo> ComputeProperties(ComponentReplectionInfo component)
    {
      List<APIPropertyInfo> properties = [];
      foreach (KeyValuePair<string, PropertyInfo> property in component.Properties)
      {
        PropertyInfo propertyInfo = property.Value;
        PropertyAttribute propertyAttribInfo = GetPropertyAttribInfo(component.Type, propertyInfo);
        if (propertyAttribInfo != null)
        {
          VMType editorTypeByEngType = GetEditorTypeByEngType(propertyInfo.PropertyType, propertyAttribInfo.SpecialTypeInfo, propertyInfo.IsDefined(typeof (TemplateAttribute), false));
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
      List<APIEventInfo> events = [];
      foreach (KeyValuePair<string, System.Reflection.EventInfo> keyValuePair in component.Events)
      {
        System.Reflection.EventInfo reflEventInfo = keyValuePair.Value;
        EventAttribute eventAttribInfo = GetEventAttribInfo(component.Type, reflEventInfo);
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
                Logger.AddError(string.Format("{0}: attribute event desc params count not corresponds to real params count", apiEventInfo.EventName));
              for (int index = 0; index < genericArguments.Length; ++index)
              {
                string name = "param_" + index;
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
                  APIParamInfo apiParamInfo = new APIParamInfo(GetEditorTypeByEngType(genericArguments[index], specTypeName, isTemplate), name);
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
            List<ParameterInfo> parameterInfoList = [];
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
                Logger.AddError(string.Format("{0}: attribute event desc params count not corresponds to real params count", apiEventInfo.EventName));
              for (int index = 0; index < parameterInfoList.Count; ++index)
              {
                string name = "param_" + index;
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
                  APIParamInfo apiParamInfo = new APIParamInfo(GetEditorTypeByEngType(parameterInfoList[index].ParameterType, specTypeName, isTemplate), name);
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
      List<APIMethodInfo> methods = [];
      foreach (KeyValuePair<string, MethodInfo> method in component.Methods)
      {
        string key = method.Key;
        MethodInfo reflMethodInfo = method.Value;
        MethodAttribute methodAttribInfo = GetMethodAttribInfo(component.Type, reflMethodInfo);
        if (methodAttribInfo != null)
        {
          APIMethodInfo apiMethodInfo = new APIMethodInfo(key);
          apiMethodInfo.ReturnParam = new APIParamInfo(GetEditorTypeByEngType(reflMethodInfo.ReturnType, methodAttribInfo.OutputTypesSpecialInfo, false));
          ParameterInfo[] parameters = reflMethodInfo.GetParameters();
          string[] strArray1 = null;
          if ("" != methodAttribInfo.InputTypesSpecialInfo)
            strArray1 = methodAttribInfo.InputTypesSpecialInfo.Split(',');
          if (strArray1 != null && strArray1.Length != parameters.Length)
            Logger.AddError(string.Format("Method {0} attribute params types info length don't match params count", apiMethodInfo.MethodName));
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
            APIParamInfo apiParamInfo = new APIParamInfo(GetEditorTypeByEngType(parameters[index].ParameterType, specTypeName, parameters[index].IsDefined(typeof (TemplateAttribute), false)), name);
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
      if (functionalDependencyDict.ContainsKey(functionalName))
        return;
      functionalDependencyDict.Add(functionalName, sDependedFunctionalName);
    }

    public static string GetDependedFunctional(string functionalName)
    {
      return functionalDependencyDict.TryGetValue(functionalName, out string str) ? str : "";
    }

    public static bool ObjectCreationExtraDebugInfoMode
    {
      get => isObjectCreationExtraDebugInfoMode;
      set => isObjectCreationExtraDebugInfoMode = value;
    }

    private static List<IVariable> GetAbstractFunctionsList(
      ComponentReplectionInfo componentInfo,
      List<APIMethodInfo> methodList)
    {
      List<IVariable> abstractFunctionsList = [];
      for (int index = 0; index < methodList.Count; ++index)
      {
        BaseFunction baseFunction = new BaseFunction(methodList[index].MethodName, componentInfo.Name);
        baseFunction.InitParams(methodList[index].InputParams, methodList[index].ReturnParam);
        abstractFunctionsList.Add(baseFunction);
      }
      return abstractFunctionsList;
    }

    private static List<IVariable> GetAbstractParamsList(
      ComponentReplectionInfo componentInfo,
      List<APIPropertyInfo> propertiesList)
    {
      List<IVariable> abstractParamsList = [];
      for (int index = 0; index < propertiesList.Count; ++index)
      {
        string propertyName = propertiesList[index].PropertyName;
        string name = componentInfo.Name;
        object propertyDefValue = propertiesList[index].PropertyDefValue;
        string componentName = name;
        VMType propertyType = propertiesList[index].PropertyType;
        object defValue = propertyDefValue;
        AbstractParameter abstractParameter = new AbstractParameter(propertyName, componentName, propertyType, defValue);
        abstractParamsList.Add(abstractParameter);
      }
      return abstractParamsList;
    }

    private static void RegistrObjectDefaultComponent(
      EObjectCategory objCategory,
      string engineComponentName)
    {
      if (!objectDefaultComponentsDict.ContainsKey(objCategory))
        objectDefaultComponentsDict.Add(objCategory, []);
      ComponentInfo functionalComponentByName = GetFunctionalComponentByName(engineComponentName);
      if (functionalComponentByName == null)
        Logger.AddError(string.Format("Functional component with name {0} not found", engineComponentName));
      else
        objectDefaultComponentsDict[objCategory].Add(functionalComponentByName);
    }

    private static MethodAttribute GetMethodAttribInfo(
      Type componentType,
      MethodInfo reflMethodInfo)
    {
      object[] customAttributes = reflMethodInfo.GetCustomAttributes(typeof (MethodAttribute), true);
      return customAttributes.Length != 0 ? (MethodAttribute) customAttributes[0] : null;
    }

    private static EventAttribute GetEventAttribInfo(Type componentType, System.Reflection.EventInfo reflEventInfo)
    {
      object[] customAttributes = reflEventInfo.GetCustomAttributes(typeof (EventAttribute), true);
      return customAttributes.Length != 0 ? (EventAttribute) customAttributes[0] : null;
    }

    private static PropertyAttribute GetPropertyAttribInfo(
      Type componentType,
      PropertyInfo reflPropertyInfo)
    {
      object[] customAttributes = reflPropertyInfo.GetCustomAttributes(typeof (PropertyAttribute), true);
      return customAttributes.Length != 0 ? (PropertyAttribute) customAttributes[0] : null;
    }
  }
}
