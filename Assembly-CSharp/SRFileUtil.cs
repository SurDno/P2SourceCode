using System.IO;
using System.Threading;

public static class SRFileUtil {
	public static void DeleteDirectory(string path) {
		try {
			Directory.Delete(path, true);
		} catch (IOException ex) {
			Thread.Sleep(0);
			Directory.Delete(path, true);
		}
	}

	public static string GetBytesReadable(long i) {
		var str1 = i < 0L ? "-" : "";
		var num1 = i < 0L ? -i : (double)i;
		string str2;
		double num2;
		if (i >= 1152921504606846976L) {
			str2 = "EB";
			num2 = i >> 50;
		} else if (i >= 1125899906842624L) {
			str2 = "PB";
			num2 = i >> 40;
		} else if (i >= 1099511627776L) {
			str2 = "TB";
			num2 = i >> 30;
		} else if (i >= 1073741824L) {
			str2 = "GB";
			num2 = i >> 20;
		} else if (i >= 1048576L) {
			str2 = "MB";
			num2 = i >> 10;
		} else {
			if (i < 1024L)
				return i.ToString(str1 + "0 B");
			str2 = "KB";
			num2 = i;
		}

		var num3 = num2 / 1024.0;
		return str1 + num3.ToString("0.### ") + str2;
	}
}