using System.Reflection;

namespace Deploys;

public static class GetterUtility {
	public static object GetValue(object target, MemberInfo member) {
		var fieldInfo = member as FieldInfo;
		if (fieldInfo != null)
			return fieldInfo.GetValue(target);
		var propertyInfo = member as PropertyInfo;
		return propertyInfo != null ? propertyInfo.GetValue(target, null) : null;
	}
}