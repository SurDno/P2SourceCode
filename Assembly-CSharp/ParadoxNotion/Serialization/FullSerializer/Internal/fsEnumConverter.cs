using System;
using System.Collections.Generic;
using System.Text;

namespace ParadoxNotion.Serialization.FullSerializer.Internal;

public class fsEnumConverter : fsConverter {
	public override bool CanProcess(Type type) {
		return type.Resolve().IsEnum;
	}

	public override bool RequestCycleSupport(Type storageType) {
		return false;
	}

	public override bool RequestInheritanceSupport(Type storageType) {
		return false;
	}

	public override object CreateInstance(fsData data, Type storageType) {
		return Enum.ToObject(storageType, (object)0);
	}

	public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType) {
		if (Serializer.Config.SerializeEnumsAsInteger)
			serialized = new fsData(Convert.ToInt64(instance));
		else if (fsPortableReflection.GetAttribute<FlagsAttribute>(storageType) != null) {
			var int64_1 = Convert.ToInt64(instance);
			var stringBuilder = new StringBuilder();
			var flag = true;
			foreach (var obj in Enum.GetValues(storageType)) {
				var int64_2 = Convert.ToInt64(obj);
				if ((int64_1 & int64_2) != 0L) {
					if (!flag)
						stringBuilder.Append(",");
					flag = false;
					stringBuilder.Append(obj);
				}
			}

			serialized = new fsData(stringBuilder.ToString());
		} else
			serialized = new fsData(Enum.GetName(storageType, instance));

		return fsResult.Success;
	}

	public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType) {
		if (data.IsString) {
			var strArray = data.AsString.Split(new char[1] {
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			long num1 = 0;
			for (var index = 0; index < strArray.Length; ++index) {
				var str = strArray[index];
				if (!ArrayContains(Enum.GetNames(storageType), str))
					return fsResult.Fail("Cannot find enum name " + str + " on type " + storageType);
				var num2 = (long)Convert.ChangeType(Enum.Parse(storageType, str), typeof(long));
				num1 |= num2;
			}

			instance = Enum.ToObject(storageType, (object)num1);
			return fsResult.Success;
		}

		if (!data.IsInt64)
			return fsResult.Fail("EnumConverter encountered an unknown JSON data type");
		var asInt64 = (int)data.AsInt64;
		instance = Enum.ToObject(storageType, (object)asInt64);
		return fsResult.Success;
	}

	private static bool ArrayContains<T>(T[] values, T value) {
		for (var index = 0; index < values.Length; ++index)
			if (EqualityComparer<T>.Default.Equals(values[index], value))
				return true;
		return false;
	}
}