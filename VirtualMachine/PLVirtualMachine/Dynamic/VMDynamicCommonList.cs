// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.VMDynamicCommonList
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.Serialization;
using System;
using System.Collections.Generic;
using System.Xml;

#nullable disable
namespace PLVirtualMachine.Dynamic
{
  [VMType("CommonList")]
  [VMFactory(typeof (ICommonList))]
  public class VMDynamicCommonList : VMCommonList, ISerializeStateSave, IDynamicLoadSerializable
  {
    private List<object> realObjectsList = new List<object>();

    public VMDynamicCommonList()
    {
    }

    public VMDynamicCommonList(VMCommonList copyList)
      : base(copyList)
    {
      for (int index = 0; index < this.ObjectsCount; ++index)
        this.realObjectsList.Add((object) null);
    }

    public override void Read(string data)
    {
      base.Read(data);
      for (int index = 0; index < this.ObjectsCount; ++index)
        this.realObjectsList.Add((object) null);
    }

    public override object GetObject(int objIndex)
    {
      if (objIndex < 0)
        return (object) null;
      if (objIndex >= this.realObjectsList.Count)
      {
        Logger.AddWarning(string.Format("Invalid list index {0} in list of {1} values", (object) objIndex, (object) this.realObjectsList.Count));
        objIndex = this.realObjectsList.Count - 1;
      }
      if (this.realObjectsList[objIndex] != null)
        return typeof (IParam).IsAssignableFrom(this.realObjectsList[objIndex].GetType()) ? ((IParam) this.realObjectsList[objIndex]).Value : this.realObjectsList[objIndex];
      object data = base.GetObject(objIndex);
      CommonVariable variable = (CommonVariable) null;
      if (data is CommonVariable)
        variable = (CommonVariable) data;
      else if (typeof (string) == data.GetType())
      {
        variable = new CommonVariable();
        variable.Read((string) data);
      }
      if (variable != null)
      {
        variable.Bind((IContext) IStaticDataContainer.StaticDataContainer.GameRoot);
        if (variable.IsBinded)
        {
          IParam dynamicParam = ((VMVariableService) IVariableService.Instance).GetDynamicParam(variable, VMEngineAPIManager.LastMethodExecInitiator);
          this.realObjectsList[objIndex] = !(typeof (DynamicParameter) == dynamicParam.GetType()) ? dynamicParam.Value : (object) (DynamicParameter) dynamicParam;
          return dynamicParam.Value;
        }
      }
      this.realObjectsList[objIndex] = data;
      return data;
    }

    public override void Clear()
    {
      base.Clear();
      this.realObjectsList.Clear();
    }

    public override void AddObject(object obj)
    {
      if (obj == null)
      {
        Logger.AddError(string.Format("Adding null to common list attemption!"));
      }
      else
      {
        base.AddObject(obj);
        this.realObjectsList.Add((object) null);
      }
    }

    public override void RemoveObjectByIndex(int objIndex)
    {
      try
      {
        base.RemoveObjectByIndex(objIndex);
        if (objIndex < 0 || objIndex >= this.realObjectsList.Count)
          return;
        this.realObjectsList.RemoveAt(objIndex);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Removing dynamic list object by index error: {0}!", (object) ex.ToString()));
      }
    }

    public override int RemoveObjectInstanceByGuid(Guid objGuid)
    {
      int num = -1;
      for (int index = 0; index < this.realObjectsList.Count; ++index)
      {
        IEngineInstanced engineInstanced = (IEngineInstanced) null;
        if (this.realObjectsList[index] != null)
        {
          if (typeof (IParam).IsAssignableFrom(this.realObjectsList[index].GetType()))
          {
            if (((IParam) this.realObjectsList[index]).Value != null)
            {
              object obj = ((IParam) this.realObjectsList[index]).Value;
              if (typeof (IEngineInstanced).IsAssignableFrom(obj.GetType()))
                engineInstanced = (IEngineInstanced) obj;
            }
          }
          else if (typeof (IEngineInstanced).IsAssignableFrom(this.realObjectsList[index].GetType()))
            engineInstanced = (IEngineInstanced) this.realObjectsList[index];
          if (engineInstanced != null && engineInstanced.EngineGuid == objGuid)
          {
            num = index;
            break;
          }
        }
      }
      if (num >= 0 && num < this.realObjectsList.Count)
      {
        this.RemoveObjectByIndex(num);
      }
      else
      {
        num = base.RemoveObjectInstanceByGuid(objGuid);
        if (num >= 0 && num < this.realObjectsList.Count)
          this.realObjectsList.RemoveAt(num);
      }
      return num;
    }

    public override void Merge(ICommonList mergeList)
    {
      int count = this.realObjectsList.Count;
      base.Merge(mergeList);
      for (int index = count; index < this.ObjectsCount; ++index)
        this.realObjectsList.Add((object) null);
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.SaveDynamicSerializableList<ListObjectInfo>(writer, "CommonListElementsList", this.objectsList);
    }

    public void LoadFromXML(XmlElement objNode)
    {
      this.objectsList.Clear();
      for (int i = 0; i < objNode.ChildNodes.Count; ++i)
      {
        if (objNode.ChildNodes[i].Name == "CommonListElementsList")
        {
          XmlElement childNode1 = (XmlElement) objNode.ChildNodes[i];
          this.objectsList.Capacity = childNode1.ChildNodes.Count;
          foreach (XmlElement childNode2 in childNode1.ChildNodes)
          {
            ListObjectInfo listObjectInfo = new ListObjectInfo();
            listObjectInfo.LoadFromXML(childNode2);
            this.objectsList.Add(listObjectInfo);
          }
          for (int index = 0; index < this.ObjectsCount; ++index)
            this.realObjectsList.Add((object) null);
        }
      }
    }
  }
}
