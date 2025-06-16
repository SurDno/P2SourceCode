using System;
using Cofe.Loggers;
using PLVirtualMachine.Common.Data;

namespace PLVirtualMachine.Common;

public class TagInfo : ITagInfo, IVMStringSerializable {
	private string tagStr;
	private int percentageValue;

	public string Tag => tagStr;

	public int Percentage => percentageValue;

	public void Read(string data) {
		switch (data) {
			case null:
				Logger.AddError("Attempt to read null tag info data");
				break;
			case "":
				break;
			case "0":
				break;
			default:
				var separator = new string[1] { "&=&" };
				var strArray = data.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				if (strArray.Length < 2) {
					Logger.AddError(
						string.Format("Tags distribution info data read error: {0} isn't full tag info data", data));
					break;
				}

				tagStr = strArray[0];
				percentageValue = StringUtility.ToInt32(strArray[1]);
				break;
		}
	}

	public string Write() {
		Logger.AddError("Not allowed serialization data struct in virtual machine!");
		return string.Empty;
	}
}