using System;
using System.Collections.Generic;
using Cofe.Loggers;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.Common
{
  [VMType("CommonList")]
  public class VMCommonList : ICommonList, IVMStringSerializable
  {
    protected List<ListObjectInfo> objectsList = [];

    public VMCommonList()
    {
    }

    public VMCommonList(VMCommonList copyList)
    {
      for (int index = 0; index < copyList.objectsList.Count; ++index)
        objectsList.Add(copyList.objectsList[index]);
    }

    public int ObjectsCount => objectsList.Count;

    public virtual object GetObject(int objIndex)
    {
      if (objIndex < 0)
        return null;
      if (objIndex >= objectsList.Count)
      {
        Logger.AddWarning(string.Format("Invalid list index {0} in list of {1} values", objIndex, objectsList.Count));
        objIndex = objectsList.Count - 1;
      }
      if (objectsList[objIndex].ObjectValue == null)
        BindListElementVarInstance(objIndex);
      return objectsList[objIndex].ObjectValue != null ? objectsList[objIndex].ObjectValue : objectsList[objIndex].ObjectInfoStr;
    }

    public virtual VMType GetType(int objIndex)
    {
      if (objIndex < 0 || objIndex >= objectsList.Count)
        return null;
      if (objectsList[objIndex].ObjectType == null)
        BindListElementVarInstance(objIndex);
      return objectsList[objIndex].ObjectType != null ? objectsList[objIndex].ObjectType : new VMType(typeof (string));
    }

    public void SetObject(int objIndex, object obj)
    {
      if (objIndex < 0 || objIndex >= objectsList.Count)
        return;
      ListObjectInfo listObjectInfo = new ListObjectInfo(obj);
      objectsList[objIndex] = listObjectInfo;
    }

    public virtual void Clear() => objectsList.Clear();

    public virtual void AddObject(object obj)
    {
      if (obj == null)
        return;
      objectsList.Add(new ListObjectInfo(obj));
    }

    public virtual void RemoveObjectByIndex(int objIndex)
    {
      if (objIndex < 0 || objIndex >= objectsList.Count)
        return;
      objectsList.RemoveAt(objIndex);
    }

    public virtual int RemoveObjectInstanceByGuid(Guid objGuid)
    {
      int index1 = -1;
      for (int index2 = 0; index2 < objectsList.Count; ++index2)
      {
        if (objectsList[index2].ObjectValue != null && typeof (IEngineInstanced).IsAssignableFrom(objectsList[index2].ObjectValue.GetType()) && ((IEngineInstanced) objectsList[index2].ObjectValue).EngineGuid == objGuid)
        {
          index1 = index2;
          break;
        }
      }
      if (index1 > -1)
        objectsList.RemoveAt(index1);
      return index1;
    }

    public virtual void Merge(ICommonList mergeList)
    {
      VMCommonList vmCommonList = (VMCommonList) mergeList;
      for (int index = 0; index < vmCommonList.objectsList.Count; ++index)
        objectsList.Add(new ListObjectInfo(vmCommonList.objectsList[index]));
    }

    public bool CheckObjectExist(object obj) => GetObjectIndex(obj) >= 0;

    public bool RemoveObjectFromList(object obj)
    {
      int objectIndex = GetObjectIndex(obj);
      if (objectIndex < 0 || objectIndex >= objectsList.Count)
        return false;
      RemoveObjectByIndex(objectIndex);
      return true;
    }

    public int GetObjectIndex(object obj)
    {
      for (int objIndex = 0; objIndex < ObjectsCount; ++objIndex)
      {
        object otherVar = GetObject(objIndex);
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
      objectsList.Clear();
      if (text.Length <= 1)
        return;
      string[] separator = ["LIST&ELEM"];
      foreach (string data in text.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        ListObjectInfo listObjectInfo = new ListObjectInfo();
        listObjectInfo.Read(data);
        if (listObjectInfo.IsLoaded)
          objectsList.Add(listObjectInfo);
      }
    }

    public int GetListIndexOfMaxValue()
    {
      int listIndexOfMaxValue = -1;
      float num = float.MinValue;
      for (int index = 0; index < objectsList.Count; ++index)
      {
        if (objectsList[index].ObjectValue != null && objectsList[index].ObjectValue.GetType().IsValueType)
        {
          float single = Convert.ToSingle(objectsList[index].ObjectValue);
          if (single > (double) num)
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
      string objectInfoStr = objectsList[objIndex].ObjectInfoStr;
      CommonVariable commonVariable = new CommonVariable();
      commonVariable.Read(objectsList[objIndex].ObjectInfoStr);
      commonVariable.Bind(IStaticDataContainer.StaticDataContainer.GameRoot);
      if (commonVariable.IsBinded && commonVariable.Variable != null)
      {
        objectsList[objIndex].ObjectValue = commonVariable.Variable;
        Type type = commonVariable.Variable.GetType();
        if (typeof (IVariable).IsAssignableFrom(type))
          objectsList[objIndex].ObjectType = ((IVariable) commonVariable.Variable).Type;
        else
          objectsList[objIndex].ObjectType = new VMType(type);
      }
      else
        Logger.AddError(string.Format("List element binding error: cannot find variable for variable info {0}", objectsList[objIndex].ObjectInfoStr));
    }
  }
}
