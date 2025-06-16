using System;
using Cofe.Loggers;
using Cofe.Serializations.Converters;
using Engine.Common.Types;
using PLVirtualMachine.Common.Data;

namespace PLVirtualMachine.Common;

public class ObjectCombinationVariant {
	public ulong ObjectGuid;
	public int MinCount;
	public int MaxCount;
	public int Weight;
	public CombinationItemParams CIParams = new();

	public ObjectCombinationVariant() {
		ObjectGuid = 0UL;
		MinCount = 1;
		MaxCount = 1;
		Weight = 1;
	}

	public ObjectCombinationVariant(ulong objGuid, int minCount, int maxCount) {
		ObjectGuid = objGuid;
		MinCount = minCount;
		MaxCount = maxCount;
		Weight = 1;
	}

	public ObjectCombinationVariant(string dataStr) {
		ObjectGuid = 0UL;
		MinCount = 0;
		MaxCount = 0;
		Weight = 1;
		var separator = new string[1] { "END&PAR" };
		var strArray = dataStr.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		if (strArray.Length != 0)
			ObjectGuid = ReadObjGuid(strArray[0]);
		if (strArray.Length > 1)
			MinCount = StringUtility.ToInt32(strArray[1]);
		if (strArray.Length > 2)
			MaxCount = StringUtility.ToInt32(strArray[2]);
		if (strArray.Length > 3)
			Weight = StringUtility.ToInt32(strArray[3]);
		if (strArray.Length <= 4)
			return;
		CIParams = new CombinationItemParams();
		CIParams.Read(strArray[4]);
	}

	public bool ContainsItem(IBlueprint item) {
		return ObjectGuid != 0UL && (long)item.BaseGuid == (long)ObjectGuid;
	}

	private ulong ReadObjGuid(string data) {
		if (GuidUtility.GetGuidFormat(data) == EGuidFormat.GT_BASE)
			return DefaultConverter.ParseUlong(data);
		Logger.AddError(string.Format("Invalid template base guid format {0} at object combination variant reading",
			data));
		return 0;
	}
}