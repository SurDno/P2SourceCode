using Cofe.Loggers;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Common
{
  [VMType("CommonList")]
  public class VMCommonList : ICommonList, IVMStringSerializable
  {
    protected List<ListObjectInfo> objectsList = new List<ListObjectInfo>();

    public VMCommonList()
    {
    }

    public VMCommonList(VMCommonList copyList)
    {
      for (int index = 0; index < copyList.objectsList.Count; ++index)
        this.objectsList.Add(copyList.objectsList[index]);
    }

    public int ObjectsCount => this.objectsList.Count;

    public virtual object GetObject(int objIndex)
    {
      if (objIndex < 0)
        return (object) null;
      if (objIndex >= this.objectsList.Count)
      {
        Logger.AddWarning(string.Format("Invalid list index {0} in list of {1} values", (object) objIndex, (object) this.objectsList.Count));
        objIndex = this.objectsList.Count - 1;
      }
      if (this.objectsList[objIndex].ObjectValue == null)
        this.BindListElementVarInstance(objIndex);
      return this.objectsList[objIndex].ObjectValue != null ? this.objectsList[objIndex].ObjectValue : (object) this.objectsList[objIndex].ObjectInfoStr;
    }

    public virtual VMType GetType(int objIndex)
    {
      if (objIndex < 0 || objIndex >= this.objectsList.Count)
        return (VMType) null;
      if (this.objectsList[objIndex].ObjectType == null)
        this.BindListElementVarInstance(objIndex);
      return this.objectsList[objIndex].ObjectType != null ? this.objectsList[objIndex].ObjectType : new VMType(typeof (string));
    }

    public void SetObject(int objIndex, object obj)
    {
      if (objIndex < 0 || objIndex >= this.objectsList.Count)
        return;
      ListObjectInfo listObjectInfo = new ListObjectInfo(obj);
      this.objectsList[objIndex] = listObjectInfo;
    }

    public virtual void Clear() => this.objectsList.Clear();

    public virtual void AddObject(object obj)
    {
      if (obj == null)
        return;
      this.objectsList.Add(new ListObjectInfo(obj));
    }

    public virtual void RemoveObjectByIndex(int objIndex)
    {
      if (objIndex < 0 || objIndex >= this.objectsList.Count)
        return;
      this.objectsList.RemoveAt(objIndex);
    }

    public virtual int RemoveObjectInstanceByGuid(Guid objGuid)
    {
      int index1 = -1;
      for (int index2 = 0; index2 < this.objectsList.Count; ++index2)
      {
        if (this.objectsList[index2].ObjectValue != null && typeof (IEngineInstanced).IsAssignableFrom(this.objectsList[index2].ObjectValue.GetType()) && ((IEngineInstanced) this.objectsList[index2].ObjectValue).EngineGuid == objGuid)
        {
          index1 = index2;
          break;
        }
      }
      if (index1 > -1)
        this.objectsList.RemoveAt(index1);
      return index1;
    }

    public virtual void Merge(ICommonList mergeList)
    {
      VMCommonList vmCommonList = (VMCommonList) mergeList;
      for (int index = 0; index < vmCommonList.objectsList.Count; ++index)
        this.objectsList.Add(new ListObjectInfo(vmCommonList.objectsList[index]));
    }

    public bool CheckObjectExist(object obj) => this.GetObjectIndex(obj) >= 0;

    public bool RemoveObjectFromList(object obj)
    {
      int objectIndex = this.GetObjectIndex(obj);
      if (objectIndex < 0 || objectIndex >= this.objectsList.Count)
        return false;
      this.RemoveObjectByIndex(objectIndex);
      return true;
    }

    public int GetObjectIndex(object obj)
    {
      for (int objIndex = 0; objIndex < this.ObjectsCount; ++objIndex)
      {
        object otherVar = this.GetObject(objIndex);
        if (typeof (IRef).IsAssignableFrom(obj.GetType()) && typeof (IRef).IsAssignableFrom(otherVar.GetType()))
        {
          if (((IVariable) obj).IsEqual((IVariable) otherVar))
            return objIndex;
        }
        else if (otherVar.Equals(obj))
          return objIndex;
      }
      return -1;
    }

    public virtual string Write()
    {
      Logger.AddError("Not allowed serialization data struct in virtual machine!");
      return string.Empty;
    }

    public virtual void Read(string text)
    {
      this.objectsList.Clear();
      if (text.Length <= 1)
        return;
      string[] separator = new string[1]{ "LIST&ELEM" };
      foreach (string data in text.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        ListObjectInfo listObjectInfo = new ListObjectInfo();
        listObjectInfo.Read(data);
        if (listObjectInfo.IsLoaded)
          this.objectsList.Add(listObjectInfo);
      }
    }

    public int GetListIndexOfMaxValue()
    {
      int listIndexOfMaxValue = -1;
      float num = float.MinValue;
      for (int index = 0; index < this.objectsList.Count; ++index)
      {
        if (this.objectsList[index].ObjectValue != null && this.objectsList[index].ObjectValue.GetType().IsValueType)
        {
          float single = Convert.ToSingle(this.objectsList[index].ObjectValue);
          if ((double) single > (double) num)
          {
            num = single;
            listIndexOfMaxValue = index;
          }
        }
      }
      return listIndexOfMaxValue;
    }

    private void BindListElementVarInstance(int objIndex)
    {
      string objectInfoStr = this.objectsList[objIndex].ObjectInfoStr;
      CommonVariable commonVariable = new CommonVariable();
      commonVariable.Read(this.objectsList[objIndex].ObjectInfoStr);
      commonVariable.Bind((IContext) IStaticDataContainer.StaticDataContainer.GameRoot);
      if (commonVariable.IsBinded && commonVariable.Variable != null)
      {
        this.objectsList[objIndex].ObjectValue = commonVariable.Variable;
        Type type = commonVariable.Variable.GetType();
        if (typeof (IVariable).IsAssignableFrom(type))
          this.objectsList[objIndex].ObjectType = ((IVariable) commonVariable.Variable).Type;
        else
          this.objectsList[objIndex].ObjectType = new VMType(type);
      }
      else
        Logger.AddError(string.Format("List element binding error: cannot find variable for variable info {0}", (object) this.objectsList[objIndex].ObjectInfoStr));
    }
  }
}
