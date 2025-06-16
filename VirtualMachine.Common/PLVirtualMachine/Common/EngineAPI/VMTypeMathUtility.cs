// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMTypeMathUtility
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using Engine.Common;
using System;
using System.Reflection;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI
{
  public static class VMTypeMathUtility
  {
    public static bool IsValueLarger(object firstValue, object secondValue, bool bEqual = false)
    {
      if (firstValue == null)
      {
        Logger.AddWarning(string.Format("first of IsValueLarger comparing values is null"));
        return false;
      }
      if (secondValue == null)
      {
        Logger.AddWarning(string.Format("second of IsValueLarger comparing values is null"));
        return false;
      }
      try
      {
        float num1 = !(firstValue.GetType() == typeof (float)) ? (!(firstValue.GetType() == typeof (GameTime)) ? Convert.ToSingle(firstValue) : (float) ((GameTime) firstValue).TotalSeconds) : (float) firstValue;
        float num2 = !(secondValue.GetType() == typeof (float)) ? (!(secondValue.GetType() == typeof (GameTime)) ? Convert.ToSingle(secondValue) : (float) ((GameTime) secondValue).TotalSeconds) : (float) secondValue;
        return !bEqual ? (double) num1 > (double) num2 : (double) num1 > (double) num2 - 9.9999997473787516E-05;
      }
      catch (Exception ex)
      {
        firstValue.GetType();
        secondValue.GetType();
        Logger.AddError(string.Format("Error value comparing ' IsValueLarger': {0} at {1})", (object) ex.ToString(), (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
        return false;
      }
    }

    public static bool IsValueLess(object firstValue, object secondValue, bool bEqual = false)
    {
      if (firstValue == null)
      {
        Logger.AddWarning(string.Format("first of IsValueLarger comparing values is null"));
        return false;
      }
      if (secondValue == null)
      {
        Logger.AddWarning(string.Format("second of IsValueLarger comparing values is null"));
        return false;
      }
      try
      {
        float num1 = !(firstValue.GetType() == typeof (GameTime)) ? Convert.ToSingle(firstValue) : (float) ((GameTime) firstValue).TotalSeconds;
        float num2 = !(secondValue.GetType() == typeof (GameTime)) ? Convert.ToSingle(secondValue) : (float) ((GameTime) secondValue).TotalSeconds;
        return !bEqual ? (double) num1 < (double) num2 : (double) num1 < (double) num2 + 9.9999997473787516E-05;
      }
      catch (Exception ex)
      {
        firstValue.GetType();
        secondValue.GetType();
        Logger.AddError(string.Format("Error value comparing ' IsValueLess': {0} at {1})", (object) ex.ToString(), (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
        return false;
      }
    }

    public static object DoMathAdd(object firstValue, object secondValue)
    {
      if (firstValue.GetType() != secondValue.GetType())
        Logger.AddError(MethodBase.GetCurrentMethod().Name + " , type is different, first : " + (object) firstValue.GetType() + " , second : " + (object) secondValue.GetType());
      if (firstValue.GetType() == typeof (float) && secondValue.GetType() == typeof (float))
        return (object) (float) ((double) (float) firstValue + (double) (float) secondValue);
      if (firstValue.GetType() == typeof (long) && secondValue.GetType() == typeof (long))
        return (object) ((long) firstValue + (long) secondValue);
      if (firstValue.GetType() == typeof (int) && secondValue.GetType() == typeof (int))
        return (object) ((int) firstValue + (int) secondValue);
      if (firstValue.GetType() == typeof (string) && secondValue.GetType() == typeof (string))
        return (object) ((string) firstValue + (string) secondValue);
      if (firstValue.GetType() == typeof (GameTime))
        return (object) new GameTime(((GameTime) firstValue).TotalSeconds + ((GameTime) secondValue).TotalSeconds);
      Logger.AddError(MethodBase.GetCurrentMethod().Name + " , type is unknown, first : " + (object) firstValue.GetType() + " , second : " + (object) secondValue.GetType());
      return (object) (float) ((double) Convert.ToSingle(firstValue) + (double) Convert.ToSingle(secondValue));
    }

    public static object DoMathSubstarct(object firstValue, object secondValue)
    {
      if (firstValue.GetType() != secondValue.GetType())
        Logger.AddError(MethodBase.GetCurrentMethod().Name + " , type is different, first : " + (object) firstValue.GetType() + " , second : " + (object) secondValue.GetType());
      if (firstValue.GetType() == typeof (float) && secondValue.GetType() == typeof (float))
        return (object) (float) ((double) (float) firstValue - (double) (float) secondValue);
      if (firstValue.GetType() == typeof (long) && secondValue.GetType() == typeof (long))
        return (object) ((long) firstValue - (long) secondValue);
      if (firstValue.GetType() == typeof (int) && secondValue.GetType() == typeof (int))
        return (object) ((int) firstValue - (int) secondValue);
      Logger.AddError(MethodBase.GetCurrentMethod().Name + " , type is unknown, first : " + (object) firstValue.GetType() + " , second : " + (object) secondValue.GetType());
      return (object) (float) ((double) Convert.ToSingle(firstValue) - (double) Convert.ToSingle(secondValue));
    }

    public static object DoMathMultiply(object firstValue, object secondValue)
    {
      if (firstValue.GetType() != secondValue.GetType())
        Logger.AddError(MethodBase.GetCurrentMethod().Name + " , type is different, first : " + (object) firstValue.GetType() + " , second : " + (object) secondValue.GetType());
      if (firstValue.GetType() == typeof (float) && secondValue.GetType() == typeof (float))
        return (object) (float) ((double) (float) firstValue * (double) (float) secondValue);
      if (firstValue.GetType() == typeof (long) && secondValue.GetType() == typeof (long))
        return (object) ((long) firstValue * (long) secondValue);
      if (firstValue.GetType() == typeof (int) && secondValue.GetType() == typeof (int))
        return (object) ((int) firstValue * (int) secondValue);
      Logger.AddError(MethodBase.GetCurrentMethod().Name + " , type is unknown, first : " + (object) firstValue.GetType() + " , second : " + (object) secondValue.GetType());
      return (object) (float) ((double) Convert.ToSingle(firstValue) * (double) Convert.ToSingle(secondValue));
    }

    public static object DoMathDivide(object firstValue, object secondValue)
    {
      if (firstValue.GetType() != secondValue.GetType())
        Logger.AddError(MethodBase.GetCurrentMethod().Name + " , type is different, first : " + (object) firstValue.GetType() + " , second : " + (object) secondValue.GetType());
      if (firstValue.GetType() == typeof (float) && secondValue.GetType() == typeof (float))
        return (object) (float) ((double) (float) firstValue / (double) (float) secondValue);
      if (firstValue.GetType() == typeof (long) && secondValue.GetType() == typeof (long))
        return (object) ((long) firstValue / (long) secondValue);
      if (firstValue.GetType() == typeof (int) && secondValue.GetType() == typeof (int))
        return (object) ((int) firstValue / (int) secondValue);
      Logger.AddError(MethodBase.GetCurrentMethod().Name + " , type is unknown, first : " + (object) firstValue.GetType() + " , second : " + (object) secondValue.GetType());
      return (object) (float) ((double) Convert.ToSingle(firstValue) / (double) Convert.ToSingle(secondValue));
    }

    public static object MakeInversion(object value)
    {
      if (value == null)
        return value;
      if (value.GetType() == typeof (bool))
        return (object) !(bool) value;
      if (value.GetType() == typeof (short))
        return (object) (int) -(short) value;
      if (value.GetType() == typeof (int))
        return (object) -(int) value;
      if (value.GetType() == typeof (long))
        return (object) -(long) value;
      if (value.GetType() == typeof (float))
        return (object) (float) -(double) (float) value;
      if (value.GetType() == typeof (double))
        return (object) -(double) value;
      Logger.AddError(MethodBase.GetCurrentMethod().Name + " , type is unknown, value : " + (object) value.GetType());
      return value;
    }

    public static bool IsValuesEqual(object oldVal, object newVal)
    {
      if (oldVal == null && newVal == null)
        return true;
      if (oldVal == null && newVal != null || oldVal != null && newVal == null)
        return false;
      return typeof (IEntity).IsAssignableFrom(newVal.GetType()) && typeof (IObjRef).IsAssignableFrom(oldVal.GetType()) ? ((Engine.Common.IObject) newVal).Id == ((IEngineInstanced) oldVal).EngineGuid : VMTypeMathUtility.IsValueEqual(oldVal, newVal);
    }

    public static bool IsValueEqual(object firstValue, object secondValue)
    {
      if (VMTypeMathUtility.IsValueNone(firstValue) && VMTypeMathUtility.IsValueNone(secondValue))
        return true;
      if (firstValue == null || secondValue == null)
      {
        Logger.AddWarning(string.Format("One from comparing values is null"));
        return false;
      }
      Type type1 = firstValue.GetType();
      Type type2 = secondValue.GetType();
      if (typeof (IRef).IsAssignableFrom(type1) && typeof (IRef).IsAssignableFrom(type2))
        return ((IVariable) firstValue).IsEqual((IVariable) secondValue);
      if (typeof (string).IsAssignableFrom(type1) && typeof (string).IsAssignableFrom(type2))
        return (string) firstValue == (string) secondValue;
      if (type1 == typeof (bool) && type2 == typeof (bool))
        return (bool) firstValue == (bool) secondValue;
      if (type1.IsValueType && type2.IsValueType)
        return (double) Math.Abs(Convert.ToSingle(firstValue) - Convert.ToSingle(secondValue)) < 9.9999999747524271E-07;
      if (type1.IsEnum && type2 == typeof (string))
        return firstValue.ToString() == (string) secondValue;
      if (type1 == typeof (string) && type2.IsEnum)
        return secondValue.ToString() == (string) firstValue;
      if (typeof (IRef).IsAssignableFrom(type1) && typeof (Engine.Common.IObject).IsAssignableFrom(type2))
        return VMTypeMathUtility.CompareVMRefWithEngineObject((IRef) firstValue, (Engine.Common.IObject) secondValue);
      return typeof (IRef).IsAssignableFrom(type2) && typeof (Engine.Common.IObject).IsAssignableFrom(type1) ? VMTypeMathUtility.CompareVMRefWithEngineObject((IRef) secondValue, (Engine.Common.IObject) firstValue) : firstValue == secondValue;
    }

    private static bool CompareVMRefWithEngineObject(IRef refValue, Engine.Common.IObject engineObjValue)
    {
      if (typeof (IObjRef).IsAssignableFrom(refValue.GetType()))
        return ((IEngineInstanced) refValue).EngineGuid == engineObjValue.Id;
      if (typeof (IBlueprintRef).IsAssignableFrom(refValue.GetType()))
        return ((IEngineTemplated) refValue).EngineTemplateGuid == engineObjValue.Id;
      return typeof (ISampleRef).IsAssignableFrom(refValue.GetType()) && ((IEngineTemplated) refValue).EngineTemplateGuid == engineObjValue.Id;
    }

    private static bool IsValueNone(object value)
    {
      if (value == null)
        return true;
      return typeof (IRef).IsAssignableFrom(value.GetType()) && !((IRef) value).Exist;
    }
  }
}
