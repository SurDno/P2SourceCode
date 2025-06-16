using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using Cofe.Loggers;

namespace PLVirtualMachine.Common.Data;

public static class ObjectUtility {
	public static T To<T>(object obj) {
		return (T)To(obj, typeof(T));
	}

	public static object To(object obj, Type type) {
		if (type == typeof(ulong))
			return Convert.ToUInt64(obj);
		if (type == typeof(long))
			return Convert.ToInt64(obj);
		if (type == typeof(uint))
			return Convert.ToUInt32(obj);
		if (type == typeof(int))
			return Convert.ToInt32(obj);
		if (type == typeof(ushort))
			return Convert.ToUInt16(obj);
		if (type == typeof(short))
			return Convert.ToInt16(obj);
		if (type == typeof(byte))
			return Convert.ToByte(obj);
		if (type == typeof(sbyte))
			return Convert.ToSByte(obj);
		if (type == typeof(double))
			return Convert.ToDouble(obj);
		if (type == typeof(float)) {
			var s = obj as string;
			if (!string.IsNullOrEmpty(s)) {
				float result;
				if (float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
					return result;
				Logger.AddWarning("Error parse , method : " + MethodBase.GetCurrentMethod().Name + " , value : " + s);
			}

			return 0.0f;
		}

		if (type == typeof(bool))
			return Convert.ToBoolean(obj);
		if (type == typeof(DateTime))
			return Convert.ToDateTime(obj);
		if (type == typeof(string))
			switch (obj) {
				case byte[] _:
					return To<string>((byte[])obj);
				case double num1:
					return num1.ToString("");
				case float num2:
					return num2.ToString("");
				default:
					return obj.ToString();
			}

		if (type == typeof(byte[]))
			return ToByteArray(obj);
		if (!(type == typeof(Enum)))
			throw new Exception("Error: Type " + type.Name + " == typeof not supported!");
		return obj is string ? Enum.Parse(type, (string)obj) : Convert.ChangeType(obj, type);
	}

	public static byte[] ToByteArray(object obj) {
		switch (obj) {
			case null:
			case DBNull _:
				throw new Exception("Error: Object == typeof(null!");
			default:
				byte[] byteArray = null;
				if (obj.GetType() == typeof(byte[]))
					byteArray = (byte[])obj;
				else if (obj.GetType() == typeof(object))
					byteArray = (byte[])obj;
				else
					switch (obj) {
						case ulong num1:
							byteArray = BitConverter.GetBytes(num1);
							break;
						case long num2:
							byteArray = BitConverter.GetBytes(num2);
							break;
						case uint num3:
							byteArray = BitConverter.GetBytes(num3);
							break;
						case int num4:
							byteArray = BitConverter.GetBytes(num4);
							break;
						case ushort num5:
							byteArray = BitConverter.GetBytes(num5);
							break;
						case short num6:
							byteArray = BitConverter.GetBytes(num6);
							break;
						case byte num7:
							byteArray = BitConverter.GetBytes(num7);
							break;
						case sbyte num8:
							byteArray = BitConverter.GetBytes(num8);
							break;
						case double num9:
							byteArray = BitConverter.GetBytes(num9);
							break;
						case float num10:
							byteArray = BitConverter.GetBytes(num10);
							break;
						case bool flag:
							byteArray = BitConverter.GetBytes(flag);
							break;
						case string _:
							byteArray = Encoding.UTF8.GetBytes((string)obj);
							break;
					}

				return byteArray;
		}
	}
}