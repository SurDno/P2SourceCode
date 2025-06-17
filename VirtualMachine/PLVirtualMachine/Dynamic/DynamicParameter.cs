using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using Engine.Common;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.GameLogic;
using PLVirtualMachine.Objects;
using IObject = Engine.Common.IObject;

namespace PLVirtualMachine.Dynamic
{
  public class DynamicParameter : 
    DynamicObject,
    IParam,
    IVariable,
    INamed,
    ISerializeStateSave,
    IDynamicLoadSerializable
  {
    private string name;
    private Type parentComponentType;
    private VMType paramType;
    private object currValue;
    private bool isImplicit;
    private bool firstSet = true;
    private DynamicFSM parentFSM;
    private PropertyInfo objPropertyInfo;
    private VMComponent objComponent;
    public static double ParamGetValueTimeMaxRT = 0.0;
    public static double ParamSetValueTimeMaxRT = 0.0;
    private static Dictionary<string, double> setParamValueTimingDict = new();
    private static Dictionary<string, int> setParamValueCountDict = new();
    private static Dictionary<ulong, double> execConditionTimingDict = new();
    private static Dictionary<ulong, int> execConditionCountDict = new();

    public DynamicParameter(VMEntity entity, DynamicFSM parentFSM, VMParameter stParam)
      : base(entity)
    {
      try
      {
        InitStatic(stParam);
        name = stParam.Name;
        paramType = stParam.Type;
        currValue = stParam.Value;
        if (stParam.Implicit && name.EndsWith("_Self"))
        {
          currValue = new VMObjRef();
          ((VMObjRef) currValue).InitializeInstance(entity);
        }
        if (typeof (ICommonList).IsAssignableFrom(paramType.BaseType) && stParam.Value != null && typeof (ICommonList).IsAssignableFrom(stParam.Value.GetType()))
          currValue = new VMDynamicCommonList((VMCommonList) stParam.Value);
        isImplicit = stParam.Implicit;
        this.parentFSM = parentFSM;
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Invalid dynamic parameter {0}.{1} creation: {2}", entity.Name, name, ex));
      }
    }

    public DynamicParameter(
      VMEntity entity,
      DynamicFSM parentFSM,
      VMComponent component,
      APIPropertyInfo apiPropertyInfo)
      : base(entity)
    {
      name = apiPropertyInfo.PropertyName;
      paramType = apiPropertyInfo.PropertyType;
      currValue = apiPropertyInfo.PropertyDefValue;
      isImplicit = false;
      this.parentFSM = parentFSM;
    }

    public virtual bool IsEqual(IVariable other)
    {
      return (!typeof (DynamicParameter).IsAssignableFrom(other.GetType()) || parentFSM.Equals(((DynamicParameter) other).parentFSM)) && other.Name == Name;
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "StaticID", StaticGuid);
      SaveManagerUtility.Save(writer, "Name", name);
      SaveManagerUtility.SaveCommon(writer, "Value", currValue);
      SaveManagerUtility.Save(writer, "FirstSet", firstSet);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        XmlElement childNode = (XmlElement) xmlNode.ChildNodes[i];
        if (childNode.Name == "Value")
        {
          Type realParamType = GetRealParamType(paramType.BaseType);
          currValue = VMSaveLoadManager.ReadValue(childNode, realParamType);
        }
        else if (childNode.Name == "FirstSet")
          firstSet = VMSaveLoadManager.ReadBool(childNode);
      }
    }

    public void InitStandartParam(VMComponent component)
    {
      if (IsCustom)
      {
        Logger.AddError(string.Format("Invalid standart param initialization: parameter {0} isn't standart", Name));
      }
      else
      {
        parentComponentType = component.GetType();
        PropertyInfo componentPropertyInfo = InfoAttribute.GetComponentPropertyInfo(component.GetComponentTypeName(), name);
        if (!(componentPropertyInfo != null))
          return;
        objPropertyInfo = componentPropertyInfo;
        objComponent = component;
      }
    }

    public bool Self => StaticObject != null && ((VMParameter) StaticObject).Implicit && ((VMParameter) StaticObject).Name.EndsWith(nameof (Self));

    public EContextVariableCategory Category => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM;

    public bool IsCustom => StaticObject != null && ((VMParameter) StaticObject).IsCustom;

    public string Name => name;

    public void UpdateStandartValueByPhysicalComponent()
    {
      if (objComponent == null)
      {
        if (IsCustom)
          return;
        Logger.AddError(string.Format("Invalid standart parameter {0} in entity {1} at {2}", Name, Entity.Name, DynamicFSM.CurrentStateInfo));
      }
      else
      {
        object vmReference = objPropertyInfo.GetValue(objComponent, null);
        if (VMTypeMathUtility.IsValuesEqual(currValue, vmReference))
          return;
        if (vmReference != null && typeof (IEntity).IsAssignableFrom(vmReference.GetType()))
          vmReference = VMEngineAPIManager.ConvertEngineEntityInstanceToVMReference((IEntity) vmReference);
        currValue = vmReference;
        DebugUtility.OnDebugParamValueChanged(this, currValue);
      }
    }

    public object Value
    {
      get => GetValueImpl();
      set
      {
        if (Entity.IsDisposed)
        {
          Logger.AddError(string.Format("Attempt to removed object {0} parameter accessing at  {1}", Entity.Name, DynamicFSM.CurrentStateInfo));
        }
        else
        {
          int num = DynamicCompatibleType(value) ? 1 : 0;
          bool flag = false;
          if (num == 0)
          {
            if (value == null)
            {
              Logger.AddError(string.Format("Attempt to set null value to param {0}", Name));
            }
            else
            {
              object obj = value;
              try
              {
                if (typeof (IObject).IsAssignableFrom(obj.GetType()) && typeof (IRef).IsAssignableFrom(paramType.BaseType))
                  obj = ExpressionUtility.GetRefByEngineInstance((IObject) obj, paramType.BaseType);
                else if (!typeof (IRef).IsAssignableFrom(paramType.BaseType))
                  obj = Convert.ChangeType(value, paramType.BaseType);
              }
              catch (Exception ex)
              {
                Logger.AddError(string.Format("Cannot direct convert type {0} to {1}", value.GetType(), paramType));
              }
              if (!DynamicCompatibleType(obj))
              {
                Logger.AddError(string.Format("Receiving value type {0} not compatible with param type {1} on setting param {2} at {3}", value.GetType(), paramType, Name, DynamicFSM.CurrentStateInfo));
                DynamicCompatibleType(obj);
                flag = true;
              }
              else
                DoSetValue(obj);
            }
          }
          else
            DoSetValue(value);
          if (flag)
            return;
          if (!IsCustom)
          {
            DoSetPhysicalStandartPropertyValue(value);
          }
          else
          {
            OnModify();
            DebugUtility.OnDebugParamValueChanged(this, currValue);
          }
        }
      }
    }

    private object GetValueImpl()
    {
      if (Entity.IsDisposed)
      {
        Logger.AddError(string.Format("Attempt to removed object {0} parameter accessing at  {1}", Entity.Name, DynamicFSM.CurrentStateInfo));
        return null;
      }
      if (!IsCustom && null != parentComponentType)
        UpdateStandartValueByPhysicalComponent();
      if (currValue == null)
        return null;
      if (typeof (IObjRef).IsAssignableFrom(currValue.GetType()) && ((VMObjRef) currValue).Static && !isImplicit)
      {
        if (((VMObjRef) currValue).Object == null)
        {
          Logger.AddError(string.Format("Not inited param handling, param name: {0}", Name));
          return currValue;
        }
        if (!((VMLogicObject) ((VMObjRef) currValue).Object).Static)
        {
          Logger.AddError(string.Format("Not inited param handling, param name: {0}", Name));
          return currValue;
        }
      }
      if (!isImplicit || !(Type.BaseType == typeof (IStateRef)) || !Name.EndsWith("_state"))
        return currValue;
      VMStateRef valueImpl = new VMStateRef();
      valueImpl.Initialize(parentFSM.CurrentState);
      return valueImpl;
    }

    public bool Implicit => StaticObject == null ? isImplicit : ((VMParameter) StaticObject).Implicit;

    public IGameObjectContext OwnerContext => StaticObject == null ? null : ((VMParameter) StaticObject).OwnerContext;

    private void DoSetValue(object newValue)
    {
      if (newValue != null && typeof (ContextVariable).IsAssignableFrom(newValue.GetType()))
        Logger.AddError(string.Format("Invalid param {0} value setting: attempt to use context variable {1} as param value", Name, newValue));
      if (this.currValue != null && typeof (IObjRef).IsAssignableFrom(this.currValue.GetType()))
        WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(((IEngineInstanced) this.currValue).EngineGuid)?.GetFSM().RemoveRefParam(this);
      if (firstSet)
      {
        OnUpdateParam(this.currValue, this.currValue);
        firstSet = false;
      }
      object currValue = this.currValue;
      this.currValue = newValue;
      OnUpdateParam(currValue, this.currValue);
      if (this.currValue == null || !typeof (IObjRef).IsAssignableFrom(this.currValue.GetType()) || StaticObject == null)
        return;
      WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(((IEngineInstanced) this.currValue).EngineGuid)?.GetFSM().AddRefParam(this);
    }

    private void DoSetPhysicalStandartPropertyValue(object value)
    {
      if (null != objPropertyInfo)
      {
        object obj = null;
        if (value != null)
          obj = CreateHierarchyHelper.ConvertEditorTypeToEngineType(value, objPropertyInfo.PropertyType, Type);
        try
        {
          objPropertyInfo.SetValue(objComponent, obj);
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Parameter {0} at component {1} vaue set error!: {2}", Name, objComponent.Name, ex));
        }
      }
      else
        Logger.AddWarning(string.Format("Standart param {0} property info  not inited", Name));
    }

    public void OnUpdateParam(object prevVal, object newVal)
    {
      if (StaticObject == null)
        return;
      List<IDependedEventRef> eventsByStaticGuid = DynamicParameterUtility.GetParameterDependedEventsByStaticGuid(StaticObject.BaseGuid);
      if (eventsByStaticGuid == null)
        return;
      int count = eventsByStaticGuid.Count;
      if (count <= 0)
        return;
      for (int index = 0; index < count; ++index)
      {
        IDependedEventRef dependedEventRef = eventsByStaticGuid[index];
        if (dependedEventRef != null)
        {
          bool flag = VMTypeMathUtility.IsValueEqual(prevVal, newVal);
          dependedEventRef.OnParamUpdate(!flag, this);
        }
      }
    }

    public void OnUpdateParam()
    {
      if (StaticObject == null)
        return;
      List<IDependedEventRef> eventsByStaticGuid = DynamicParameterUtility.GetParameterDependedEventsByStaticGuid(StaticObject.BaseGuid);
      if (eventsByStaticGuid == null)
        return;
      for (int index = 0; index < eventsByStaticGuid.Count; ++index)
        eventsByStaticGuid[index]?.OnParamUpdate(false, this);
    }

    public VMType Type => paramType;

    public override void Think()
    {
    }

    public void AfterSaveLoading()
    {
      if (!Self)
        return;
      currValue = new VMObjRef();
      ((VMObjRef) currValue).InitializeInstance(Entity);
    }

    private bool DynamicCompatibleType(object value) => DynamicCompatibleTypeImpl(value);

    private bool DynamicCompatibleTypeImpl(object value)
    {
      if (typeof (IObjRef).IsAssignableFrom(paramType.BaseType))
      {
        if (value == null)
          return true;
        if (!typeof (IObjRef).IsAssignableFrom(value.GetType()))
          return false;
        VMObjRef vmObjRef = (VMObjRef) value;
        if (currValue != null && typeof (IObjRef).IsAssignableFrom(currValue.GetType()) && (((IRef) currValue).StaticInstance != null && vmObjRef.StaticInstance != null && vmObjRef.StaticInstance.IsEqual(((IRef) currValue).StaticInstance) || ((VMObjRef) currValue).TypeTemplate != null && vmObjRef.TypeTemplate != null && DynamicTemplatesCompatible(((VMObjRef) currValue).TypeTemplate, vmObjRef.TypeTemplate)) || vmObjRef.Object == null && !WorldEntityUtility.IsDynamicGuidExist(vmObjRef.EngineGuid) || !vmObjRef.IsDynamic)
          return true;
        VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(vmObjRef.EngineGuid);
        if (entityByEngineGuid == null)
          return false;
        if (entityByEngineGuid.EditorTemplate != null)
        {
          VMParameter staticObject = (VMParameter) StaticObject;
          if (staticObject != null && staticObject.Value != null && ((VMObjRef) staticObject.Value).Object != null && !((VMLogicObject) ((VMObjRef) staticObject.Value).Object).Static)
            return ((VMObjRef) staticObject.Value).Object.GetCategory() != EObjectCategory.OBJECT_CATEGORY_CLASS ? entityByEngineGuid.EditorTemplate.IsEqual(((BaseRef) staticObject.Value).StaticInstance) : entityByEngineGuid.EditorTemplate.IsDerivedFrom(((BaseRef) staticObject.Value).StaticInstance.BaseGuid, true);
          if (paramType.SpecialType != "")
          {
            bool flag = true;
            foreach (string functionalPart in paramType.GetFunctionalParts())
            {
              if (!(!((VMLogicObject) entityByEngineGuid.EditorTemplate).DirectEngineCreated ? entityByEngineGuid.EditorTemplate.IsFunctionalSupport(functionalPart) : entityByEngineGuid.IsFunctionalSupport(functionalPart)))
              {
                flag = false;
                break;
              }
            }
            return flag;
          }
        }
        return true;
      }
      if (value == null)
        return typeof (IRef).IsAssignableFrom(paramType.BaseType);
      if (typeof (IVariable).IsAssignableFrom(value.GetType()) && ((IVariable) value).Type.BaseType == paramType.BaseType)
        return VMTypeUtility.IsTypesCompatible(paramType, ((IVariable) value).Type);
      VMType secondType = new VMType(value.GetType());
      return typeof (ICommonList).IsAssignableFrom(paramType.BaseType) && secondType.BaseType == typeof (VMDynamicCommonList) || VMTypeUtility.IsTypesCompatible(paramType, secondType);
    }

    private bool DynamicTemplatesCompatible(IBlueprint needTemplate, IBlueprint sourceTemplate)
    {
      foreach (KeyValuePair<string, IFunctionalComponent> functionalComponent in needTemplate.FunctionalComponents)
      {
        if (!sourceTemplate.IsFunctionalSupport(functionalComponent.Key))
          return false;
      }
      return true;
    }

    private Type GetRealParamType(Type type)
    {
      Type realRefType = BaseSerializer.GetRealRefType(type);
      return realRefType != null ? realRefType : type;
    }

    private static void MakeSetParamDetailTimingInfo(string paramName, double elapsedTime)
    {
      if (!setParamValueTimingDict.ContainsKey(paramName))
      {
        setParamValueTimingDict.Add(paramName, 0.0);
        setParamValueCountDict.Add(paramName, 0);
      }
      setParamValueTimingDict[paramName] += elapsedTime;
      ++setParamValueCountDict[paramName];
    }
  }
}
