using System;
using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.Serialization;

namespace PLVirtualMachine.Dynamic
{
  [VMType("CommonList")]
  [VMFactory(typeof (ICommonList))]
  public class VMDynamicCommonList : VMCommonList, ISerializeStateSave, IDynamicLoadSerializable
  {
    private List<object> realObjectsList = [];

    public VMDynamicCommonList()
    {
    }

    public VMDynamicCommonList(VMCommonList copyList)
      : base(copyList)
    {
      for (int index = 0; index < ObjectsCount; ++index)
        realObjectsList.Add(null);
    }

    public override void Read(string data)
    {
      base.Read(data);
      for (int index = 0; index < ObjectsCount; ++index)
        realObjectsList.Add(null);
    }

    public override object GetObject(int objIndex)
    {
      if (objIndex < 0)
        return null;
      if (objIndex >= realObjectsList.Count)
      {
        Logger.AddWarning(string.Format("Invalid list index {0} in list of {1} values", objIndex, realObjectsList.Count));
        objIndex = realObjectsList.Count - 1;
      }
      if (realObjectsList[objIndex] != null)
        return typeof (IParam).IsAssignableFrom(realObjectsList[objIndex].GetType()) ? ((IParam) realObjectsList[objIndex]).Value : realObjectsList[objIndex];
      object data = base.GetObject(objIndex);
      CommonVariable variable = null;
      if (data is CommonVariable)
        variable = (CommonVariable) data;
      else if (typeof (string) == data.GetType())
      {
        variable = new CommonVariable();
        variable.Read((string) data);
      }
      if (variable != null)
      {
        variable.Bind(IStaticDataContainer.StaticDataContainer.GameRoot);
        if (variable.IsBinded)
        {
          IParam dynamicParam = ((VMVariableService) IVariableService.Instance).GetDynamicParam(variable, VMEngineAPIManager.LastMethodExecInitiator);
          realObjectsList[objIndex] = !(typeof (DynamicParameter) == dynamicParam.GetType()) ? dynamicParam.Value : (DynamicParameter) dynamicParam;
          return dynamicParam.Value;
        }
      }
      realObjectsList[objIndex] = data;
      return data;
    }

    public override void Clear()
    {
      base.Clear();
      realObjectsList.Clear();
    }

    public override void AddObject(object obj)
    {
      if (obj == null)
      {
        Logger.AddError("Adding null to common list attemption!");
      }
      else
      {
        base.AddObject(obj);
        realObjectsList.Add(null);
      }
    }

    public override void RemoveObjectByIndex(int objIndex)
    {
      try
      {
        base.RemoveObjectByIndex(objIndex);
        if (objIndex < 0 || objIndex >= realObjectsList.Count)
          return;
        realObjectsList.RemoveAt(objIndex);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Removing dynamic list object by index error: {0}!", ex));
      }
    }

    public override int RemoveObjectInstanceByGuid(Guid objGuid)
    {
      int num = -1;
      for (int index = 0; index < realObjectsList.Count; ++index)
      {
        IEngineInstanced engineInstanced = null;
        if (realObjectsList[index] != null)
        {
          if (typeof (IParam).IsAssignableFrom(realObjectsList[index].GetType()))
          {
            if (((IParam) realObjectsList[index]).Value != null)
            {
              object obj = ((IParam) realObjectsList[index]).Value;
              if (typeof (IEngineInstanced).IsAssignableFrom(obj.GetType()))
                engineInstanced = (IEngineInstanced) obj;
            }
          }
          else if (typeof (IEngineInstanced).IsAssignableFrom(realObjectsList[index].GetType()))
            engineInstanced = (IEngineInstanced) realObjectsList[index];
          if (engineInstanced != null && engineInstanced.EngineGuid == objGuid)
          {
            num = index;
            break;
          }
        }
      }
      if (num >= 0 && num < realObjectsList.Count)
      {
        RemoveObjectByIndex(num);
      }
      else
      {
        num = base.RemoveObjectInstanceByGuid(objGuid);
        if (num >= 0 && num < realObjectsList.Count)
          realObjectsList.RemoveAt(num);
      }
      return num;
    }

    public override void Merge(ICommonList mergeList)
    {
      int count = realObjectsList.Count;
      base.Merge(mergeList);
      for (int index = count; index < ObjectsCount; ++index)
        realObjectsList.Add(null);
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.SaveDynamicSerializableList(writer, "CommonListElementsList", objectsList);
    }

    public void LoadFromXML(XmlElement objNode)
    {
      objectsList.Clear();
      for (int i = 0; i < objNode.ChildNodes.Count; ++i)
      {
        if (objNode.ChildNodes[i].Name == "CommonListElementsList")
        {
          XmlElement childNode1 = (XmlElement) objNode.ChildNodes[i];
          objectsList.Capacity = childNode1.ChildNodes.Count;
          foreach (XmlElement childNode2 in childNode1.ChildNodes)
          {
            ListObjectInfo listObjectInfo = new ListObjectInfo();
            listObjectInfo.LoadFromXML(childNode2);
            objectsList.Add(listObjectInfo);
          }
          for (int index = 0; index < ObjectsCount; ++index)
            realObjectsList.Add(null);
        }
      }
    }
  }
}
