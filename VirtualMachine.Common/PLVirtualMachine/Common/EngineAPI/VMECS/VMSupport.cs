﻿using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Support")]
  public class VMSupport : VMComponent
  {
    public const string ComponentName = "Support";

    public override void Initialize(VMBaseEntity parent) => base.Initialize(parent);

    [Method("Random", "", "")]
    public virtual float Random() => 0.0f;

    [Method("Custom random", "Max value", "")]
    public virtual float Rand(float maxValue) => 0.0f;

    [Method("Custom random", "Min value,Max value", "")]
    public virtual float MinMaxRand(float minValue, float maxValue) => 0.0f;

    [Method("Custom int random", "Min value,Max value", "")]
    public virtual int MinMaxIntRand(int minValue, int maxValue) => 0;

    [Method("Custom int next random", "Min value,Max value,Previous value", "")]
    public virtual int MinMaxIntNextRand(int minValue, int maxValue, int prevValue) => 0;

    [Method("Poisson second random time", "Flow per second", "")]
    public virtual GameTime PoissonRandTime(float flowPerSecond) => null;

    [Method("Poisson hour random time", "Flow per hour", "")]
    public virtual GameTime PoissonHourRandTime(float flowPerHour)
    {
      return PoissonRandTime(flowPerHour / 3600f);
    }

    [Method("Poisson day random time", "Flow per day", "")]
    public virtual GameTime PoissonDayRandTime(float flowPerDay)
    {
      return PoissonRandTime(flowPerDay / 86400f);
    }

    [Method("Get list objects count", "list", "")]
    public virtual int GetListObjectsCount(VMCommonList objList) => 0;

    [Method("Get list object type", "list,index", "")]
    public virtual VMType GetListObjectType(VMCommonList objList, int index) => null;

    [Method("Get list object", "list,index", "")]
    public virtual object GetListObject(VMCommonList objList, int index) => null;

    [Method("Set list value", "list,value,index", "")]
    public virtual void SetListValue(VMCommonList objList, object val, int index)
    {
    }

    [Method("Get random list object", "list", "")]
    public virtual object GetRandomListObject(VMCommonList objList) => null;

    [Method("Clear list", "List", "")]
    public virtual void ClearList(VMCommonList objList)
    {
    }

    [Method("Add element to list", "List,element", "")]
    public virtual void AddObjectToList(VMCommonList objList, object obj)
    {
    }

    [Method("Remove list element by index", "List,index", "")]
    public virtual void RemoveElementFromList(VMCommonList objList, int index)
    {
    }

    [Method("Remove list object", "List, Object", "")]
    public virtual bool RemoveObjectFromList(VMCommonList list, object obj) => false;

    [Method("Merge lists", "Destination list,Source list", "")]
    public virtual void MergeLists(VMCommonList firstList, VMCommonList secondList)
    {
    }

    [Method("Check list object exist", "List, Object", "")]
    public virtual bool CheckListObjectExist(VMCommonList list, object obj) => false;

    [Method("Get list object index", "List, Object", "")]
    public virtual int GetListObjectIndex(VMCommonList list, object obj) => -1;

    [Method("Get list index of maximum value", "List", "")]
    public virtual int GetListIndexOfMaxValue(VMCommonList list) => -1;
  }
}
