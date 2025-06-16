// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.FSMParamsManager
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Cofe.Serializations.Data;
using Engine.Common.Types;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.GameLogic;
using System;
using System.Collections.Generic;
using System.Xml;

#nullable disable
namespace PLVirtualMachine.Dynamic
{
  public class FSMParamsManager
  {
    private DynamicFSM fsm;
    private Dictionary<ulong, IParam> contextParamsByStaticGuid;
    private Dictionary<string, IParam> contextParamsByName;
    protected Dictionary<ulong, DynamicParameter> dynObjContextParams;
    private List<DynamicParameter> fsmDynamicParams;

    public FSMParamsManager(DynamicFSM fsm)
    {
      this.fsm = fsm;
      try
      {
        if (fsm.FSMStaticObject.DirectEngineCreated)
          this.LoadParamsFromEngineDirect();
        else
          this.LoadParams();
      }
      catch (Exception ex)
      {
        Logger.AddError(ex.ToString());
      }
    }

    public void Clear()
    {
      if (this.fsmDynamicParams != null)
      {
        for (int index = 0; index < this.fsmDynamicParams.Count; ++index)
        {
          if (typeof (IObjRef).IsAssignableFrom(this.fsmDynamicParams[index].Type.BaseType) && this.fsmDynamicParams[index].Value != null && typeof (IObjRef).IsAssignableFrom(this.fsmDynamicParams[index].Value.GetType()))
          {
            VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(((IEngineInstanced) this.fsmDynamicParams[index].Value).EngineGuid);
            if (entityByEngineGuid != null && !entityByEngineGuid.IsDisposed && entityByEngineGuid.FSMExist)
              entityByEngineGuid.GetFSM().RemoveRefParam(this.fsmDynamicParams[index]);
          }
        }
      }
      if (this.contextParamsByStaticGuid != null)
        this.contextParamsByStaticGuid.Clear();
      if (this.contextParamsByName != null)
        this.contextParamsByName.Clear();
      if (this.dynObjContextParams != null)
        this.dynObjContextParams.Clear();
      if (this.fsmDynamicParams == null)
        return;
      this.fsmDynamicParams.Clear();
    }

    public IEnumerable<DynamicParameter> FSMDynamicParams
    {
      get
      {
        foreach (DynamicParameter fsmDynamicParam in this.fsmDynamicParams)
          yield return fsmDynamicParam;
      }
    }

    public IParam GetContextParam(ulong stGuid)
    {
      IParam obj;
      return this.contextParamsByStaticGuid.TryGetValue(stGuid, out obj) ? obj : (IParam) null;
    }

    public IParam GetContextParam(string paramName)
    {
      IParam contextParam;
      if (GuidUtility.GetGuidFormat(paramName) == EGuidFormat.GT_BASE && this.contextParamsByStaticGuid.TryGetValue(StringUtility.ToUInt64(paramName), out contextParam))
        return contextParam;
      string[] strArray = paramName.Split('.');
      if (strArray.Length > 1)
        paramName = strArray[strArray.Length - 1];
      return this.contextParamsByName.TryGetValue(paramName, out contextParam) ? contextParam : (IParam) null;
    }

    public DynamicParameter GetDynamicObjectParameter(ulong paramId)
    {
      DynamicParameter dynamicParameter;
      return this.dynObjContextParams.TryGetValue(paramId, out dynamicParameter) ? dynamicParameter : (DynamicParameter) null;
    }

    public void AfterSaveLoading()
    {
      for (int index = 0; index < this.fsmDynamicParams.Count; ++index)
        this.fsmDynamicParams[index].AfterSaveLoading();
    }

    public void StateSave(IDataWriter writer) => this.SaveParameters(writer, "Parameters");

    public void LoadFromXML(XmlElement xmlNode)
    {
      if (!(xmlNode.Name == "Parameters"))
        return;
      this.LoadParameters(xmlNode);
    }

    public bool HasActiveEvents { get; set; }

    private void LoadParams()
    {
      IEnumerable<IVariable> contextVariables = this.fsm.FSMStaticObject.GetContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM);
      int capacity = this.fsm.FSMStaticObject.StandartParamsCount + this.fsm.FSMStaticObject.CustomParamsCount + 1;
      this.contextParamsByStaticGuid = new Dictionary<ulong, IParam>(capacity);
      this.fsmDynamicParams = new List<DynamicParameter>(capacity);
      this.contextParamsByName = new Dictionary<string, IParam>(this.fsm.FSMStaticObject.StandartParamsCount);
      foreach (IVariable variable in contextVariables)
      {
        try
        {
          VMParameter stParam = (VMParameter) variable;
          DynamicParameter dynamicParameter = new DynamicParameter(this.fsm.Entity, this.fsm, stParam);
          if (stParam.Implicit)
          {
            if (!this.contextParamsByStaticGuid.ContainsKey(stParam.BaseGuid))
              this.contextParamsByStaticGuid.Add(stParam.BaseGuid, (IParam) dynamicParameter);
            else
              Logger.AddError(string.Format("FSM {0} dynamic state param {1} guid {2} dublicated", (object) this.fsm.FSMStaticObject.Name, (object) stParam.Name, (object) stParam.BaseGuid));
          }
          else
          {
            if (!this.contextParamsByStaticGuid.ContainsKey(stParam.BaseGuid))
              this.contextParamsByStaticGuid.Add(stParam.BaseGuid, (IParam) dynamicParameter);
            else
              Logger.AddError(string.Format("FSM {0} dynamic context param {1} guid {2} dublicated", (object) this.fsm.FSMStaticObject.Name, (object) stParam.Name, (object) stParam.BaseGuid));
            if (!stParam.IsCustom)
            {
              VMComponent componentByName = this.fsm.Entity.GetComponentByName(stParam.ComponentName);
              if (componentByName == null)
              {
                Logger.AddError(string.Format("Standart parameter {0} not loaded in {1}, because component {2} not found in entity", (object) stParam.Name, (object) this.fsm.FSMStaticObject.Name, (object) stParam.ComponentName));
                continue;
              }
              dynamicParameter.InitStandartParam(componentByName);
              if (!this.contextParamsByName.ContainsKey(variable.Name))
                this.contextParamsByName.Add(variable.Name, (IParam) dynamicParameter);
            }
            this.fsmDynamicParams.Add(dynamicParameter);
            if (stParam.IsDynamicObject)
            {
              if (this.dynObjContextParams == null)
                this.dynObjContextParams = new Dictionary<ulong, DynamicParameter>();
              this.dynObjContextParams[stParam.BaseGuid] = dynamicParameter;
            }
          }
        }
        catch (Exception ex)
        {
          Logger.AddError(ex.ToString());
        }
      }
    }

    private void LoadParamsFromEngineDirect()
    {
      if (this.fsmDynamicParams == null)
        this.fsmDynamicParams = new List<DynamicParameter>();
      else
        this.fsmDynamicParams.Clear();
      if (this.contextParamsByName == null)
        this.contextParamsByName = new Dictionary<string, IParam>();
      else
        this.contextParamsByName.Clear();
      foreach (VMComponent component in this.fsm.Entity.Components)
      {
        ComponentInfo functionalComponentByName = EngineAPIManager.GetFunctionalComponentByName(component.Name);
        if (functionalComponentByName == null)
        {
          Logger.AddError(string.Format("Component with name {0} not found in virtual machine api", (object) component.Name));
        }
        else
        {
          for (int index = 0; index < functionalComponentByName.Properties.Count; ++index)
          {
            APIPropertyInfo property = functionalComponentByName.Properties[index];
            if (this.contextParamsByName.ContainsKey(property.PropertyName))
            {
              Logger.AddError(string.Format("FSM {0} dynamic context param {1} dublicated", (object) this.fsm.FSMStaticObject.Name, (object) property.PropertyName));
            }
            else
            {
              DynamicParameter dynamicParameter = new DynamicParameter(this.fsm.Entity, this.fsm, component, property);
              dynamicParameter.InitStandartParam(component);
              if (this.fsm.PropertyInitialized)
                dynamicParameter.UpdateStandartValueByPhysicalComponent();
              this.contextParamsByName.Add(property.PropertyName, (IParam) dynamicParameter);
              this.fsmDynamicParams.Add(dynamicParameter);
            }
          }
        }
      }
    }

    private void SaveParameters(IDataWriter writer, string name)
    {
      writer.Begin(name, (Type) null, true);
      for (int index = 0; index < this.fsmDynamicParams.Count; ++index)
      {
        DynamicParameter fsmDynamicParam = this.fsmDynamicParams[index];
        if (fsmDynamicParam.IsCustom && !fsmDynamicParam.Self)
          SaveManagerUtility.SaveDynamicSerializable(writer, "Item", (ISerializeStateSave) fsmDynamicParam);
      }
      writer.End(name, true);
    }

    private void LoadParameters(XmlElement rootNode)
    {
      for (int i = 0; i < rootNode.ChildNodes.Count; ++i)
      {
        XmlElement childNode = (XmlElement) rootNode.ChildNodes[i];
        string innerText = childNode.FirstChild.InnerText;
        ulong uint64 = StringUtility.ToUInt64(innerText);
        if (this.contextParamsByStaticGuid.ContainsKey(uint64))
          ((DynamicParameter) this.contextParamsByStaticGuid[uint64]).LoadFromXML(childNode);
        else
          Logger.AddError(string.Format("SaveLoad error: saved parameter by key {0} not found in fsm {1}", (object) innerText, (object) this.fsm.FSMStaticObject.Name));
      }
    }
  }
}
