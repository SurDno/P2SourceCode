using System;
using System.Collections.Generic;
using System.Linq;
using Cofe.Loggers;

namespace PLVirtualMachine.Common.EngineAPI;

public static class VMTypeUtility {
	public static bool IsTypeNumber(Type type) {
		return IsTypeIntegerNumber(type) || type == typeof(float) || type == typeof(double);
	}

	public static bool IsTypeIntegerNumber(Type type) {
		return type == typeof(byte) || type == typeof(ushort) || type == typeof(short) || type == typeof(int) ||
		       type == typeof(uint) || type == typeof(long) || type == typeof(ulong);
	}

	public static bool IsTypesCompatible(VMType firstType, VMType secondType, bool isWeak = false) {
		try {
			if (firstType == null)
				return true;
			if (secondType == null) {
				Logger.AddError(string.Format("Types compatibility checking error: second type not defined at {0})",
					EngineAPIManager.Instance.CurrentFSMStateInfo));
				return false;
			}

			if (firstType.BaseType == typeof(object) || secondType.BaseType == typeof(object))
				return true;
			if (firstType.IsNumber && secondType.IsNumber)
				return !firstType.IsIntegerNumber || secondType.IsIntegerNumber;
			if (firstType.BaseType.IsEnum && secondType.BaseType.IsEnum)
				return firstType.BaseType.ToString() == secondType.BaseType.ToString();
			if (firstType.BaseType == secondType.BaseType) {
				if ((!typeof(IRef).IsAssignableFrom(firstType.BaseType) &&
				     !typeof(ICommonList).IsAssignableFrom(firstType.BaseType)) || isWeak)
					return true;
				if (typeof(ICommonList).IsAssignableFrom(firstType.BaseType) &&
				    firstType.GetListElementType() != null && secondType.GetListElementType() != null)
					return IsTypesCompatible(firstType.GetListElementType(), secondType.GetListElementType());
				var specialType1 = firstType.SpecialType;
				var specialType2 = secondType.SpecialType;
				if (specialType1 == "" ||
				    ((firstType.BaseType == typeof(IObjRef) || firstType.BaseType == typeof(IBlueprintRef) ||
				      firstType.BaseType == typeof(ISampleRef)) && specialType2 == "") || specialType1 == specialType2)
					return true;
				var dependedFunctional = EngineAPIManager.GetDependedFunctional(specialType2);
				if (specialType1 == dependedFunctional)
					return true;
				if (firstType.IsFunctionalSpecial) {
					if (firstType.SpecialTypeBlueprint == null)
						return CompareFunctionalList(firstType, secondType);
					return secondType.SpecialTypeBlueprint != null &&
					       ((long)secondType.SpecialTypeBlueprint.BaseGuid ==
					        (long)firstType.SpecialTypeBlueprint.BaseGuid ||
					        secondType.SpecialTypeBlueprint.IsDerivedFrom(firstType.SpecialTypeBlueprint.BaseGuid) ||
					        CompareFunctionalList(firstType, secondType));
				}
			}
		} catch (Exception ex) {
			Logger.AddError(string.Format("Types compatibility checking error: {0} at {1})", ex,
				EngineAPIManager.Instance.CurrentFSMStateInfo));
		}

		return false;
	}

	private static bool CompareFunctionalList(VMType firstType, VMType secondType) {
		var functionalParts = secondType.GetFunctionalParts(true);
		foreach (var functionalPart in firstType.GetFunctionalParts())
			if (functionalPart != "Common" && !functionalParts.Contains(functionalPart))
				return false;
		return true;
	}
}