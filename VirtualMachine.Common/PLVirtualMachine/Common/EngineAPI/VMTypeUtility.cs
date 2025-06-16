// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMTypeUtility
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI
{
  public static class VMTypeUtility
  {
    public static bool IsTypeNumber(Type type)
    {
      return VMTypeUtility.IsTypeIntegerNumber(type) || type == typeof (float) || type == typeof (double);
    }

    public static bool IsTypeIntegerNumber(Type type)
    {
      return type == typeof (byte) || type == typeof (ushort) || type == typeof (short) || type == typeof (int) || type == typeof (uint) || type == typeof (long) || type == typeof (ulong);
    }

    public static bool IsTypesCompatible(VMType firstType, VMType secondType, bool isWeak = false)
    {
      try
      {
        if (firstType == null)
          return true;
        if (secondType == null)
        {
          Logger.AddError(string.Format("Types compatibility checking error: second type not defined at {0})", (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
          return false;
        }
        if (firstType.BaseType == typeof (object) || secondType.BaseType == typeof (object))
          return true;
        if (firstType.IsNumber && secondType.IsNumber)
          return !firstType.IsIntegerNumber || secondType.IsIntegerNumber;
        if (firstType.BaseType.IsEnum && secondType.BaseType.IsEnum)
          return firstType.BaseType.ToString() == secondType.BaseType.ToString();
        if (firstType.BaseType == secondType.BaseType)
        {
          if (!typeof (IRef).IsAssignableFrom(firstType.BaseType) && !typeof (ICommonList).IsAssignableFrom(firstType.BaseType) || isWeak)
            return true;
          if (typeof (ICommonList).IsAssignableFrom(firstType.BaseType) && firstType.GetListElementType() != null && secondType.GetListElementType() != null)
            return VMTypeUtility.IsTypesCompatible(firstType.GetListElementType(), secondType.GetListElementType());
          string specialType1 = firstType.SpecialType;
          string specialType2 = secondType.SpecialType;
          if (specialType1 == "" || (firstType.BaseType == typeof (IObjRef) || firstType.BaseType == typeof (IBlueprintRef) || firstType.BaseType == typeof (ISampleRef)) && specialType2 == "" || specialType1 == specialType2)
            return true;
          string dependedFunctional = EngineAPIManager.GetDependedFunctional(specialType2);
          if (specialType1 == dependedFunctional)
            return true;
          if (firstType.IsFunctionalSpecial)
          {
            if (firstType.SpecialTypeBlueprint == null)
              return VMTypeUtility.CompareFunctionalList(firstType, secondType);
            return secondType.SpecialTypeBlueprint != null && ((long) secondType.SpecialTypeBlueprint.BaseGuid == (long) firstType.SpecialTypeBlueprint.BaseGuid || secondType.SpecialTypeBlueprint.IsDerivedFrom(firstType.SpecialTypeBlueprint.BaseGuid) || VMTypeUtility.CompareFunctionalList(firstType, secondType));
          }
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Types compatibility checking error: {0} at {1})", (object) ex.ToString(), (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
      }
      return false;
    }

    private static bool CompareFunctionalList(VMType firstType, VMType secondType)
    {
      IEnumerable<string> functionalParts = secondType.GetFunctionalParts(true);
      foreach (string functionalPart in firstType.GetFunctionalParts())
      {
        if (functionalPart != "Common" && !functionalParts.Contains<string>(functionalPart))
          return false;
      }
      return true;
    }
  }
}
