using System;
using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using Engine.Common.Types;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.GameLogic;

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
          LoadParamsFromEngineDirect();
        else
          LoadParams();
      }
      catch (Exception ex)
      {
        Logger.AddError(ex.ToString());
      }
    }

    public void Clear()
    {
      if (fsmDynamicParams != null)
      {
        for (int index = 0; index < fsmDynamicParams.Count; ++index)
        {
          if (typeof (IObjRef).IsAssignableFrom(fsmDynamicParams[index].Type.BaseType) && fsmDynamicParams[index].Value != null && typeof (IObjRef).IsAssignableFrom(fsmDynamicParams[index].Value.GetType()))
          {
            VMEntity entityByEngineGuid = WorldEntityUtility.GetDynamicObjectEntityByEngineGuid(((IEngineInstanced) fsmDynamicParams[index].Value).EngineGuid);
            if (entityByEngineGuid != null && !entityByEngineGuid.IsDisposed && entityByEngineGuid.FSMExist)
              entityByEngineGuid.GetFSM().RemoveRefParam(fsmDynamicParams[index]);
          }
        }
      }
      if (contextParamsByStaticGuid != null)
        contextParamsByStaticGuid.Clear();
      if (contextParamsByName != null)
        contextParamsByName.Clear();
      if (dynObjContextParams != null)
        dynObjContextParams.Clear();
      if (fsmDynamicParams == null)
        return;
      fsmDynamicParams.Clear();
    }

    public IEnumerable<DynamicParameter> FSMDynamicParams
    {
      get
      {
        foreach (DynamicParameter fsmDynamicParam in fsmDynamicParams)
          yield return fsmDynamicParam;
      }
    }

    public IParam GetContextParam(ulong stGuid)
    {
      return contextParamsByStaticGuid.TryGetValue(stGuid, out IParam obj) ? obj : null;
    }

    public IParam GetContextParam(string paramName)
    {
      if (GuidUtility.GetGuidFormat(paramName) == EGuidFormat.GT_BASE && contextParamsByStaticGuid.TryGetValue(StringUtility.ToUInt64(paramName), out IParam contextParam))
        return contextParam;
      string[] strArray = paramName.Split('.');
      if (strArray.Length > 1)
        paramName = strArray[strArray.Length - 1];
      return contextParamsByName.TryGetValue(paramName, out contextParam) ? contextParam : null;
    }

    public DynamicParameter GetDynamicObjectParameter(ulong paramId)
    {
      return dynObjContextParams.TryGetValue(paramId, out DynamicParameter dynamicParameter) ? dynamicParameter : null;
    }

    public void AfterSaveLoading()
    {
      for (int index = 0; index < fsmDynamicParams.Count; ++index)
        fsmDynamicParams[index].AfterSaveLoading();
    }

    public void StateSave(IDataWriter writer) => SaveParameters(writer, "Parameters");

    public void LoadFromXML(XmlElement xmlNode)
    {
      if (!(xmlNode.Name == "Parameters"))
        return;
      LoadParameters(xmlNode);
    }

    public bool HasActiveEvents { get; set; }

    private void LoadParams()
    {
      IEnumerable<IVariable> contextVariables = fsm.FSMStaticObject.GetContextVariables(EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM);
      int capacity = fsm.FSMStaticObject.StandartParamsCount + fsm.FSMStaticObject.CustomParamsCount + 1;
      contextParamsByStaticGuid = new Dictionary<ulong, IParam>(capacity);
      fsmDynamicParams = new List<DynamicParameter>(capacity);
      contextParamsByName = new Dictionary<string, IParam>(fsm.FSMStaticObject.StandartParamsCount);
      foreach (IVariable variable in contextVariables)
      {
        try
        {
          VMParameter stParam = (VMParameter) variable;
          DynamicParameter dynamicParameter = new DynamicParameter(fsm.Entity, fsm, stParam);
          if (stParam.Implicit)
          {
            if (!contextParamsByStaticGuid.ContainsKey(stParam.BaseGuid))
              contextParamsByStaticGuid.Add(stParam.BaseGuid, dynamicParameter);
            else
              Logger.AddError(string.Format("FSM {0} dynamic state param {1} guid {2} dublicated", fsm.FSMStaticObject.Name, stParam.Name, stParam.BaseGuid));
          }
          else
          {
            if (!contextParamsByStaticGuid.ContainsKey(stParam.BaseGuid))
              contextParamsByStaticGuid.Add(stParam.BaseGuid, dynamicParameter);
            else
              Logger.AddError(string.Format("FSM {0} dynamic context param {1} guid {2} dublicated", fsm.FSMStaticObject.Name, stParam.Name, stParam.BaseGuid));
            if (!stParam.IsCustom)
            {
              VMComponent componentByName = fsm.Entity.GetComponentByName(stParam.ComponentName);
              if (componentByName == null)
              {
                Logger.AddError(string.Format("Standart parameter {0} not loaded in {1}, because component {2} not found in entity", stParam.Name, fsm.FSMStaticObject.Name, stParam.ComponentName));
                continue;
              }
              dynamicParameter.InitStandartParam(componentByName);
              if (!contextParamsByName.ContainsKey(variable.Name))
                contextParamsByName.Add(variable.Name, dynamicParameter);
            }
            fsmDynamicParams.Add(dynamicParameter);
            if (stParam.IsDynamicObject)
            {
              if (dynObjContextParams == null)
                dynObjContextParams = new Dictionary<ulong, DynamicParameter>();
              dynObjContextParams[stParam.BaseGuid] = dynamicParameter;
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
      if (fsmDynamicParams == null)
        fsmDynamicParams = [];
      else
        fsmDynamicParams.Clear();
      if (contextParamsByName == null)
        contextParamsByName = new Dictionary<string, IParam>();
      else
        contextParamsByName.Clear();
      foreach (VMComponent component in fsm.Entity.Components)
      {
        ComponentInfo functionalComponentByName = EngineAPIManager.GetFunctionalComponentByName(component.Name);
        if (functionalComponentByName == null)
        {
          Logger.AddError(string.Format("Component with name {0} not found in virtual machine api", component.Name));
        }
        else
        {
          for (int index = 0; index < functionalComponentByName.Properties.Count; ++index)
          {
            APIPropertyInfo property = functionalComponentByName.Properties[index];
            if (contextParamsByName.ContainsKey(property.PropertyName))
            {
              Logger.AddError(string.Format("FSM {0} dynamic context param {1} dublicated", fsm.FSMStaticObject.Name, property.PropertyName));
            }
            else
            {
              DynamicParameter dynamicParameter = new DynamicParameter(fsm.Entity, fsm, component, property);
              dynamicParameter.InitStandartParam(component);
              if (fsm.PropertyInitialized)
                dynamicParameter.UpdateStandartValueByPhysicalComponent();
              contextParamsByName.Add(property.PropertyName, dynamicParameter);
              fsmDynamicParams.Add(dynamicParameter);
            }
          }
        }
      }
    }

    private void SaveParameters(IDataWriter writer, string name)
    {
      writer.Begin(name, null, true);
      for (int index = 0; index < fsmDynamicParams.Count; ++index)
      {
        DynamicParameter fsmDynamicParam = fsmDynamicParams[index];
        if (fsmDynamicParam.IsCustom && !fsmDynamicParam.Self)
          SaveManagerUtility.SaveDynamicSerializable(writer, "Item", fsmDynamicParam);
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
        if (contextParamsByStaticGuid.ContainsKey(uint64))
          ((DynamicParameter) contextParamsByStaticGuid[uint64]).LoadFromXML(childNode);
        else
          Logger.AddError(string.Format("SaveLoad error: saved parameter by key {0} not found in fsm {1}", innerText, fsm.FSMStaticObject.Name));
      }
    }
  }
}
