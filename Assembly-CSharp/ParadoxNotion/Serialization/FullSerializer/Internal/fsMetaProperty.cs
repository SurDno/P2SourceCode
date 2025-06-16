using System;
using System.Reflection;

namespace ParadoxNotion.Serialization.FullSerializer.Internal;

public class fsMetaProperty {
	private MemberInfo _memberInfo;

	internal fsMetaProperty(fsConfig config, FieldInfo field) {
		_memberInfo = field;
		StorageType = field.FieldType;
		MemberName = field.Name;
		IsPublic = field.IsPublic;
		CanRead = true;
		CanWrite = true;
		CommonInitialize(config);
	}

	internal fsMetaProperty(fsConfig config, PropertyInfo property) {
		_memberInfo = property;
		StorageType = property.PropertyType;
		MemberName = property.Name;
		IsPublic = property.GetGetMethod() != null && property.GetGetMethod().IsPublic &&
		           property.GetSetMethod() != null && property.GetSetMethod().IsPublic;
		CanRead = property.CanRead;
		CanWrite = property.CanWrite;
		CommonInitialize(config);
	}

	private void CommonInitialize(fsConfig config) {
		var attribute = fsPortableReflection.GetAttribute<fsPropertyAttribute>(_memberInfo);
		if (attribute != null) {
			JsonName = attribute.Name;
			OverrideConverterType = attribute.Converter;
		}

		if (!string.IsNullOrEmpty(JsonName))
			return;
		JsonName = config.GetJsonNameFromMemberName(MemberName, _memberInfo);
	}

	public Type StorageType { get; private set; }

	public Type OverrideConverterType { get; private set; }

	public bool CanRead { get; private set; }

	public bool CanWrite { get; private set; }

	public string JsonName { get; private set; }

	public string MemberName { get; private set; }

	public bool IsPublic { get; private set; }

	public void Write(object context, object value) {
		var memberInfo1 = _memberInfo as FieldInfo;
		var memberInfo2 = _memberInfo as PropertyInfo;
		if (memberInfo1 != null)
			memberInfo1.SetValue(context, value);
		else {
			if (!(memberInfo2 != null))
				return;
			var setMethod = memberInfo2.GetSetMethod(true);
			if (setMethod != null)
				setMethod.Invoke(context, new object[1] { value });
		}
	}

	public object Read(object context) {
		return _memberInfo is PropertyInfo
			? ((PropertyInfo)_memberInfo).GetValue(context, new object[0])
			: ((FieldInfo)_memberInfo).GetValue(context);
	}
}