using System;
using Cofe.Loggers;
using Cofe.Proxies;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMSupport))]
  public class Support : VMSupport, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    protected static VMSupport instance;

    public override string GetComponentTypeName() => nameof (Support);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      string name = target.Name;
    }

    public override void Initialize(VMBaseEntity parent)
    {
      base.Initialize(parent);
      if (instance == null)
        instance = this;
      else
        Logger.AddError("Support component creation dublicate!");
    }

    public static void ResetInstance() => instance = null;

    public override float Random() => (float) VMMath.GetRandomDouble();

    public override float Rand(float maxVal) => maxVal * (float) VMMath.GetRandomDouble();

    public override float MinMaxRand(float minVal, float maxVal)
    {
      return minVal + (float) ((maxVal - (double) minVal) * VMMath.GetRandomDouble());
    }

    public override int MinMaxIntRand(int minVal, int maxVal)
    {
      return minVal + VMMath.GetRandomInt(maxVal - minVal);
    }

    public override int MinMaxIntNextRand(int minVal, int maxVal, int prevVal)
    {
      int num = prevVal + 1 + VMMath.GetRandomInt(maxVal - minVal - 1);
      if (num >= maxVal)
        num -= maxVal - minVal;
      return num;
    }

    public override GameTime PoissonRandTime(float flowPerSecond)
    {
      if (flowPerSecond <= 0.0)
        return new GameTime(ulong.MaxValue);
      double d = 1E-06 + VMMath.GetRandomDouble();
      if (d > 1.0)
        d = 1.0;
      return new GameTime((ulong) Math.Round(-(1.0 / flowPerSecond) * Math.Log(d)));
    }

    public override int GetListObjectsCount(VMCommonList objList)
    {
      if (objList != null)
        return objList.ObjectsCount;
      Logger.AddError(string.Format("GetListObjectsCount error: list instance is null !!! at {0}", DynamicFSM.CurrentStateInfo));
      return 0;
    }

    public override VMType GetListObjectType(VMCommonList objList, int index)
    {
      if (objList != null)
        return objList.GetType(index);
      Logger.AddError(string.Format("GetListObjectsCount error: list instance is null !!! at {0}", DynamicFSM.CurrentStateInfo));
      return null;
    }

    public override object GetListObject(VMCommonList objList, int index)
    {
      if (objList == null)
      {
        Logger.AddError(string.Format("GetListObjectsCount error: list instance is null !!! at {0}", DynamicFSM.CurrentStateInfo));
        return null;
      }
      try
      {
        return objList.GetObject(index);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Get list object error: {0}", ex));
        return null;
      }
    }

    public override void SetListValue(VMCommonList objList, object val, int index)
    {
      try
      {
        objList.SetObject(index, val);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Get list object error: {0}", ex));
      }
    }

    [Method("Get random list object", "list", "")]
    public override object GetRandomListObject(VMCommonList objList)
    {
      if (objList == null)
      {
        Logger.AddError(string.Format("Cannot get random object from list: list instance not defined at {0}", DynamicFSM.CurrentStateInfo));
        return null;
      }
      int index = VMMath.GetRandomInt(objList.ObjectsCount);
      if (index >= objList.ObjectsCount)
      {
        Logger.AddError(string.Format("Random index generating error: probably list is empty at {0}", DynamicFSM.CurrentStateInfo));
        index = objList.ObjectsCount - 1;
      }
      return GetListObject(objList, index);
    }

    public override int GetListIndexOfMaxValue(VMCommonList list)
    {
      try
      {
        return list.GetListIndexOfMaxValue();
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Get list max value index error: {0}", ex));
        return -1;
      }
    }

    public override void ClearList(VMCommonList objList)
    {
      if (objList != null)
        objList.Clear();
      else
        Logger.AddError("Target list for clear function is null");
    }

    public override void AddObjectToList(VMCommonList objList, object obj)
    {
      if (objList != null)
        objList.AddObject(obj);
      else
        Logger.AddError("Target list for adding object function is null");
    }

    public override void RemoveElementFromList(VMCommonList objList, int index)
    {
      if (objList != null)
        objList.RemoveObjectByIndex(index);
      else
        Logger.AddError("Target list for adding object function is null");
    }

    public override bool RemoveObjectFromList(VMCommonList list, object obj)
    {
      return list.RemoveObjectFromList(obj);
    }

    public override void MergeLists(VMCommonList firstList, VMCommonList secondList)
    {
      if (firstList == null || secondList == null)
        return;
      firstList.Merge(secondList);
    }

    public override bool CheckListObjectExist(VMCommonList list, object obj)
    {
      return list.CheckObjectExist(obj);
    }

    public override int GetListObjectIndex(VMCommonList list, object obj)
    {
      return list.GetObjectIndex(obj);
    }

    public static VMSupport Instance => instance;
  }
}
