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
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

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
    private System.Type parentComponentType;
    private VMType paramType;
    private object currValue;
    private bool isImplicit;
    private bool firstSet = true;
    private DynamicFSM parentFSM;
    private PropertyInfo objPropertyInfo;
    private VMComponent objComponent;
    public static double ParamGetValueTimeMaxRT = 0.0;
    public static double ParamSetValueTimeMaxRT = 0.0;
    private static Dictionary<string, double> setParamValueTimingDict = new Dictionary<string, double>();
    private static Dictionary<string, int> setParamValueCountDict = new Dictionary<string, int>();
    private static Dictionary<ulong, double> execConditionTimingDict = new Dictionary<ulong, double>();
    private static Dictionary<ulong, int> execConditionCountDict = new Dictionary<ulong, int>();

    public DynamicParameter(VMEntity entity, DynamicFSM parentFSM, VMParameter stParam)
      : base(entity)
    {
      try
      {
        this.InitStatic((PLVirtualMachine.Common.IObject) stParam);
        this.name = stParam.Name;
        this.paramType = stParam.Type;
        this.currValue = stParam.Value;
        if (stParam.Implicit && this.name.EndsWith("_Self"))
        {
          this.currValue = (object) new VMObjRef();
          ((VMObjRef) this.currValue).InitializeInstance((IEngineRTInstance) entity);
        }
        if (typeof (ICommonList).IsAssignableFrom(this.paramType.BaseType) && stParam.Value != null && typeof (ICommonList).IsAssignableFrom(stParam.Value.GetType()))
          this.currValue = (object) new VMDynamicCommonList((VMCommonList) stParam.Value);
        this.isImplicit = stParam.Implicit;
        this.parentFSM = parentFSM;
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Invalid dynamic parameter {0}.{1} creation: {2}", (object) entity.Name, (object) this.name, (object) ex.ToString()));
      }
    }

    public DynamicParameter(
      VMEntity entity,
      DynamicFSM parentFSM,
      VMComponent component,
      APIPropertyInfo apiPropertyInfo)
      : base(entity)
    {
      this.name = apiPropertyInfo.PropertyName;
      this.paramType = apiPropertyInfo.PropertyType;
      this.currValue = apiPropertyInfo.PropertyDefValue;
      this.isImplicit = false;
      this.parentFSM = parentFSM;
    }

    public virtual bool IsEqual(IVariable other)
    {
      return (!typeof (DynamicParameter).IsAssignableFrom(other.GetType()) || this.parentFSM.Equals((object) ((DynamicParameter) other).parentFSM)) && other.Name == this.Name;
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "StaticID", this.StaticGuid);
      SaveManagerUtility.Save(writer, "Name", this.name);
      SaveManagerUtility.SaveCommon(writer, "Value", this.currValue);
      SaveManagerUtility.Save(writer, "FirstSet", this.firstSet);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        XmlElement childNode = (XmlElement) xmlNode.ChildNodes[i];
        if (childNode.Name == "Value")
        {
          System.Type realParamType = this.GetRealParamType(this.paramType.BaseType);
          this.currValue = VMSaveLoadManager.ReadValue((XmlNode) childNode, realParamType);
        }
        else if (childNode.Name == "FirstSet")
          this.firstSet = VMSaveLoadManager.ReadBool((XmlNode) childNode);
      }
    }

    public void InitStandartParam(VMComponent component)
    {
      if (this.IsCustom)
      {
        Logger.AddError(string.Format("Invalid standart param initialization: parameter {0} isn't standart", (object) this.Name));
      }
      else
      {
        this.parentComponentType = component.GetType();
        PropertyInfo componentPropertyInfo = InfoAttribute.GetComponentPropertyInfo(component.GetComponentTypeName(), this.name);
        if (!(componentPropertyInfo != (PropertyInfo) null))
          return;
        this.objPropertyInfo = componentPropertyInfo;
        this.objComponent = component;
      }
    }

    public bool Self
    {
      get
      {
        return this.StaticObject != null && ((VMParameter) this.StaticObject).Implicit && ((VMParameter) this.StaticObject).Name.EndsWith(nameof (Self));
      }
    }

    public EContextVariableCategory Category
    {
      get => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM;
    }

    public bool IsCustom => this.StaticObject != null && ((VMParameter) this.StaticObject).IsCustom;

    public string Name => this.name;

    public void UpdateStandartValueByPhysicalComponent()
    {
      if (this.objComponent == null)
      {
        if (this.IsCustom)
          return;
        Logger.AddError(string.Format("Invalid standart parameter {0} in entity {1} at {2}", (object) this.Name, (object) this.Entity.Name, (object) DynamicFSM.CurrentStateInfo));
      }
      else
      {
        object vmReference = this.objPropertyInfo.GetValue((object) this.objComponent, (object[]) null);
        if (VMTypeMathUtility.IsValuesEqual(this.currValue, vmReference))
          return;
        if (vmReference != null && typeof (IEntity).IsAssignableFrom(vmReference.GetType()))
          vmReference = (object) VMEngineAPIManager.ConvertEngineEntityInstanceToVMReference((IEntity) vmReference);
        this.currValue = vmReference;
        DebugUtility.OnDebugParamValueChanged(this, this.currValue);
      }
    }

    public object Value
    {
      get => this.GetValueImpl();
      set
      {
        if (this.Entity.IsDisposed)
        {
          Logger.AddError(string.Format("Attempt to removed object {0} parameter accessing at  {1}", (object) this.Entity.Name, (object) DynamicFSM.CurrentStateInfo));
        }
        else
        {
          int num = this.DynamicCompatibleType(value) ? 1 : 0;
          bool flag = false;
          if (num == 0)
          {
            if (value == null)
            {
              Logger.AddError(string.Format("Attempt to set null value to param {0}", (object) this.Name));
            }
            else
            {
              object obj = value;
              try
              {
                if (typeof (Engine.Common.IObject).IsAssignableFrom(obj.GetType()) && typeof (IRef).IsAssignableFrom(this.paramType.BaseType))
                  obj = (object) ExpressionUtility.GetRefByEngineInstance((Engine.Common.IObject) obj, this.paramType.BaseType);
                else if (!typeof (IRef).IsAssignableFrom(this.paramType.BaseType))
                  obj = Convert.ChangeType(value, this.paramType.BaseType);
              }
              catch (Exception ex)
              {
                Logger.AddError(string.Format("Cannot direct convert type {0} to {1}", (object) value.GetType(), (object) this.paramType));
              }
              if (!this.DynamicCompatibleType(obj))
              {
                Logger.AddError(string.Format("Receiving value type {0} not compatible with param type {1} on setting param {2} at {3}", (object) value.GetType(), (object) this.paramType, (object) this.Name, (object) DynamicFSM.CurrentStateInfo));
                this.DynamicCompatibleType(obj);
                flag = true;
              }
              else
                this.DoSetValue(obj);
            }
          }
          else
            this.DoSetValue(value);
          if (flag)
            return;
          if (!this.IsCustom)
          {
            this.DoSetPhysicalStandartPropertyValue(value);
          }
          else
          {
            this.OnModify();
            DebugUtility.OnDebugParamValueChanged(this, this.currValue);
          }
        }
      }
    }

    private object GetValueImpl()
    {
      if (this.Entity.IsDisposed)
      {
        Logger.AddError(string.Format("Attempt to removed object {0} parameter accessing at  {1}", (object) this.Entity.Name, (object) DynamicFSM.CurrentStateInfo));
        return (object) null;
      }
      if (!this.IsCustom && (System.Type) null != this.parentComponentType)
        this.UpdateStandartValueByPhysicalComponent();
      if (this.currValue == null)
        return (object) null;
      if (typeof (IObjRef).IsAssignableFrom(this.currValue.GetType()) && ((VMObjRef) this.currValue).Static && !this.isImplicit)
      {
        if (((VMObjRef) this.currValue).Object == null)
        {
          Logger.AddError(string.Format("Not inited param handling, param name: {0}", (object) this.Name));
          return this.currValue;
        }
        if (!((VMLogicObject) ((VMObjRef) this.currValue).Object).Static)
        {
          Logger.AddError(string.Format("Not inited param handling, param name: {0}", (object) this.Name));
          return this.currValue;
        }
      }
      if (!this.isImplicit || !(this.Type.BaseType == typeof (IStateRef)) || !this.Name.EndsWith("_state"))
        return this.currValue;
      VMStateRef valueImpl = new VMStateRef();
      valueImpl.Initialize(this.parentFSM.CurrentState);
      return (object) valueImpl;
    }

    public bool Implicit
    {
      get
      {
        return this.StaticObject == null ? this.isImplicit : ((VMParameter) this.StaticObject).Implicit;
      }
    }

    public IGameObjectContext OwnerContext
    {
      get
      {
        return this.StaticObject == null ? (IGameObjectContext) null : ((VMParameter) this.StaticObject).OwnerContext;
      }
    }

    private void DoSetValue(object newValue)
    {
      if (newValue != null && typeof (ContextVariable).IsAssignableFrom(newValue.GetType()))
        Logger.AddError(string.Format("Invalid param {0} value setting: attempt to use context variable {1} as param value", (object) this.Name, (object) newValue.ToString()));
      if (this.currValue != null && typeof (IObjRef).IsAssignableFrom(this.currValue.GetType()))
        WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(((IEngineInstanced) this.currValue).EngineGuid)?.GetFSM().RemoveRefParam(this);
      if (this.firstSet)
      {
        this.OnUpdateParam(this.currValue, this.currValue);
        this.firstSet = false;
      }
      object currValue = this.currValue;
      this.currValue = newValue;
      this.OnUpdateParam(currValue, this.currValue);
      if (this.currValue == null || !typeof (IObjRef).IsAssignableFrom(this.currValue.GetType()) || this.StaticObject == null)
        return;
      WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(((IEngineInstanced) this.currValue).EngineGuid)?.GetFSM().AddRefParam(this);
    }

    private void DoSetPhysicalStandartPropertyValue(object value)
    {
      if ((PropertyInfo) null != this.objPropertyInfo)
      {
        object obj = (object) null;
        if (value != null)
          obj = CreateHierarchyHelper.ConvertEditorTypeToEngineType(value, this.objPropertyInfo.PropertyType, this.Type);
        try
        {
          this.objPropertyInfo.SetValue((object) this.objComponent, obj);
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Parameter {0} at component {1} vaue set error!: {2}", (object) this.Name, (object) this.objComponent.Name, (object) ex.ToString()));
        }
      }
      else
        Logger.AddWarning(string.Format("Standart param {0} property info  not inited", (object) this.Name));
    }

    public void OnUpdateParam(object prevVal, object newVal)
    {
      if (this.StaticObject == null)
        return;
      List<IDependedEventRef> eventsByStaticGuid = DynamicParameterUtility.GetParameterDependedEventsByStaticGuid(this.StaticObject.BaseGuid);
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
      if (this.StaticObject == null)
        return;
      List<IDependedEventRef> eventsByStaticGuid = DynamicParameterUtility.GetParameterDependedEventsByStaticGuid(this.StaticObject.BaseGuid);
      if (eventsByStaticGuid == null)
        return;
      for (int index = 0; index < eventsByStaticGuid.Count; ++index)
        eventsByStaticGuid[index]?.OnParamUpdate(false, this);
    }

    public VMType Type => this.paramType;

    public override void Think()
    {
    }

    public void AfterSaveLoading()
    {
      if (!this.Self)
        return;
      this.currValue = (object) new VMObjRef();
      ((VMObjRef) this.currValue).InitializeInstance((IEngineRTInstance) this.Entity);
    }

    private bool DynamicCompatibleType(object value) => this.DynamicCompatibleTypeImpl(value);

    private bool DynamicCompatibleTypeImpl(object value)
    {
      if (typeof (IObjRef).IsAssignableFrom(this.paramType.BaseType))
      {
        if (value == null)
          return true;
        if (!typeof (IObjRef).IsAssignableFrom(value.GetType()))
          return false;
        VMObjRef vmObjRef = (VMObjRef) value;
        if (this.currValue != null && typeof (IObjRef).IsAssignableFrom(this.currValue.GetType()) && (((IRef) this.currValue).StaticInstance != null && vmObjRef.StaticInstance != null && vmObjRef.StaticInstance.IsEqual(((IRef) this.currValue).StaticInstance) || ((VMObjRef) this.currValue).TypeTemplate != null && vmObjRef.TypeTemplate != null && this.DynamicTemplatesCompatible(((VMObjRef) this.currValue).TypeTemplate, vmObjRef.TypeTemplate)) || vmObjRef.Object == null && !WorldEntityUtility.IsDynamicGuidExist(vmObjRef.EngineGuid) || !vmObjRef.IsDynamic)
          return true;
        VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(vmObjRef.EngineGuid);
        if (entityByEngineGuid == null)
          return false;
        if (entityByEngineGuid.EditorTemplate != null)
        {
          VMParameter staticObject = (VMParameter) this.StaticObject;
          if (staticObject != null && staticObject.Value != null && ((VMObjRef) staticObject.Value).Object != null && !((VMLogicObject) ((VMObjRef) staticObject.Value).Object).Static)
            return ((VMObjRef) staticObject.Value).Object.GetCategory() != EObjectCategory.OBJECT_CATEGORY_CLASS ? entityByEngineGuid.EditorTemplate.IsEqual(((BaseRef) staticObject.Value).StaticInstance) : entityByEngineGuid.EditorTemplate.IsDerivedFrom(((BaseRef) staticObject.Value).StaticInstance.BaseGuid, true);
          if (this.paramType.SpecialType != "")
          {
            bool flag = true;
            foreach (string functionalPart in this.paramType.GetFunctionalParts())
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
        return typeof (IRef).IsAssignableFrom(this.paramType.BaseType);
      if (typeof (IVariable).IsAssignableFrom(value.GetType()) && ((IVariable) value).Type.BaseType == this.paramType.BaseType)
        return VMTypeUtility.IsTypesCompatible(this.paramType, ((IVariable) value).Type);
      VMType secondType = new VMType(value.GetType());
      return typeof (ICommonList).IsAssignableFrom(this.paramType.BaseType) && secondType.BaseType == typeof (VMDynamicCommonList) || VMTypeUtility.IsTypesCompatible(this.paramType, secondType);
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

    private System.Type GetRealParamType(System.Type type)
    {
      System.Type realRefType = BaseSerializer.GetRealRefType(type);
      return realRefType != (System.Type) null ? realRefType : type;
    }

    private static void MakeSetParamDetailTimingInfo(string paramName, double elapsedTime)
    {
      if (!DynamicParameter.setParamValueTimingDict.ContainsKey(paramName))
      {
        DynamicParameter.setParamValueTimingDict.Add(paramName, 0.0);
        DynamicParameter.setParamValueCountDict.Add(paramName, 0);
      }
      DynamicParameter.setParamValueTimingDict[paramName] += elapsedTime;
      ++DynamicParameter.setParamValueCountDict[paramName];
    }
  }
}
