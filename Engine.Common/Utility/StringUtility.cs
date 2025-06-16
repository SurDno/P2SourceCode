namespace Engine.Common.Utility;

public static class StringUtility {
	public static bool NextSubstring(
		string value,
		string separator,
		ref int index,
		ref string result) {
		if (index == -1)
			return false;
		var num = value.IndexOf(separator, index);
		if (num == -1) {
			result = value.Substring(index);
			index = -1;
			return true;
		}

		result = value.Substring(index, num - index);
		index = num + separator.Length;
		return true;
	}
}