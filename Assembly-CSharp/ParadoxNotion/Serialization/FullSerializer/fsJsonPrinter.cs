using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ParadoxNotion.Serialization.FullSerializer;

public static class fsJsonPrinter {
	private static void InsertSpacing(TextWriter stream, int count) {
		for (var index = 0; index < count; ++index)
			stream.Write("    ");
	}

	private static string EscapeString(string str) {
		var flag = false;
		for (var index = 0; index < str.Length; ++index) {
			var ch = str[index];
			var int32 = Convert.ToInt32(ch);
			if (int32 < 0 || int32 > sbyte.MaxValue) {
				flag = true;
				break;
			}

			switch (ch) {
				case char.MinValue:
				case '\a':
				case '\b':
				case '\t':
				case '\n':
				case '\f':
				case '\r':
				case '"':
				case '\\':
					flag = true;
					break;
			}

			if (flag)
				break;
		}

		if (!flag)
			return str;
		var stringBuilder = new StringBuilder();
		for (var index = 0; index < str.Length; ++index) {
			var ch = str[index];
			var int32 = Convert.ToInt32(ch);
			if (int32 < 0 || int32 > sbyte.MaxValue)
				stringBuilder.Append(string.Format("\\u{0:x4} ", int32).Trim());
			else
				switch (ch) {
					case char.MinValue:
						stringBuilder.Append("\\0");
						continue;
					case '\a':
						stringBuilder.Append("\\a");
						continue;
					case '\b':
						stringBuilder.Append("\\b");
						continue;
					case '\t':
						stringBuilder.Append("\\t");
						continue;
					case '\n':
						stringBuilder.Append("\\n");
						continue;
					case '\f':
						stringBuilder.Append("\\f");
						continue;
					case '\r':
						stringBuilder.Append("\\r");
						continue;
					case '"':
						stringBuilder.Append("\\\"");
						continue;
					case '\\':
						stringBuilder.Append("\\\\");
						continue;
					default:
						stringBuilder.Append(ch);
						continue;
				}
		}

		return stringBuilder.ToString();
	}

	private static void BuildCompressedString(fsData data, TextWriter stream) {
		switch (data.Type) {
			case fsDataType.Array:
				stream.Write('[');
				var flag1 = false;
				foreach (var data1 in data.AsList) {
					if (flag1)
						stream.Write(',');
					flag1 = true;
					BuildCompressedString(data1, stream);
				}

				stream.Write(']');
				break;
			case fsDataType.Object:
				stream.Write('{');
				var flag2 = false;
				foreach (var keyValuePair in data.AsDictionary)
					if (fsGlobalConfig.SerializeDefaultValues || !keyValuePair.Value.IsNull) {
						if (flag2)
							stream.Write(',');
						flag2 = true;
						stream.Write('"');
						stream.Write(keyValuePair.Key);
						stream.Write('"');
						stream.Write(":");
						BuildCompressedString(keyValuePair.Value, stream);
					}

				stream.Write('}');
				break;
			case fsDataType.Double:
				stream.Write(ConvertDoubleToString(data.AsDouble));
				break;
			case fsDataType.Int64:
				stream.Write(data.AsInt64);
				break;
			case fsDataType.Boolean:
				if (data.AsBool) {
					stream.Write("true");
					break;
				}

				stream.Write("false");
				break;
			case fsDataType.String:
				stream.Write('"');
				stream.Write(EscapeString(data.AsString));
				stream.Write('"');
				break;
			case fsDataType.Null:
				stream.Write("null");
				break;
		}
	}

	private static void BuildPrettyString(fsData data, TextWriter stream, int depth) {
		switch (data.Type) {
			case fsDataType.Array:
				if (data.AsList.Count == 0) {
					stream.Write("[]");
					break;
				}

				var flag1 = false;
				stream.Write('[');
				stream.WriteLine();
				foreach (var data1 in data.AsList) {
					if (flag1) {
						stream.Write(',');
						stream.WriteLine();
					}

					flag1 = true;
					InsertSpacing(stream, depth + 1);
					BuildPrettyString(data1, stream, depth + 1);
				}

				stream.WriteLine();
				InsertSpacing(stream, depth);
				stream.Write(']');
				break;
			case fsDataType.Object:
				stream.Write('{');
				stream.WriteLine();
				var flag2 = false;
				foreach (var keyValuePair in data.AsDictionary)
					if (fsGlobalConfig.SerializeDefaultValues || !keyValuePair.Value.IsNull) {
						if (flag2) {
							stream.Write(',');
							stream.WriteLine();
						}

						flag2 = true;
						InsertSpacing(stream, depth + 1);
						stream.Write('"');
						stream.Write(keyValuePair.Key);
						stream.Write('"');
						stream.Write(": ");
						BuildPrettyString(keyValuePair.Value, stream, depth + 1);
					}

				stream.WriteLine();
				InsertSpacing(stream, depth);
				stream.Write('}');
				break;
			case fsDataType.Double:
				stream.Write(ConvertDoubleToString(data.AsDouble));
				break;
			case fsDataType.Int64:
				stream.Write(data.AsInt64);
				break;
			case fsDataType.Boolean:
				if (data.AsBool) {
					stream.Write("true");
					break;
				}

				stream.Write("false");
				break;
			case fsDataType.String:
				stream.Write('"');
				stream.Write(EscapeString(data.AsString));
				stream.Write('"');
				break;
			case fsDataType.Null:
				stream.Write("null");
				break;
		}
	}

	public static void PrettyJson(fsData data, TextWriter outputStream) {
		BuildPrettyString(data, outputStream, 0);
	}

	public static string PrettyJson(fsData data) {
		var sb = new StringBuilder();
		using (var stream = new StringWriter(sb)) {
			BuildPrettyString(data, stream, 0);
			return sb.ToString();
		}
	}

	public static void CompressedJson(fsData data, StreamWriter outputStream) {
		BuildCompressedString(data, outputStream);
	}

	public static string CompressedJson(fsData data) {
		var sb = new StringBuilder();
		using (var stream = new StringWriter(sb)) {
			BuildCompressedString(data, stream);
			return sb.ToString();
		}
	}

	private static string ConvertDoubleToString(double d) {
		if (double.IsInfinity(d) || double.IsNaN(d))
			return d.ToString(CultureInfo.InvariantCulture);
		var str = d.ToString(CultureInfo.InvariantCulture);
		if (!str.Contains(".") && !str.Contains("e") && !str.Contains("E"))
			str += ".0";
		return str;
	}
}