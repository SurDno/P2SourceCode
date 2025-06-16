using System.Reflection;
using Cofe.Utility;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI;

public class APIPropertyInfo {
	public PropertyAttribute Attribute;
	public PropertyInfo PropertyInfo;
	public VMType PropertyType;
	public object PropertyDefValue;

	public APIPropertyInfo(PropertyInfo propertyInfo, VMType propertyType) {
		PropertyInfo = propertyInfo;
		PropertyType = propertyType;
		PropertyDefValue = TypeDefaultUtility.GetDefault(PropertyType.BaseType);
		var customAttributes = (PropertyAttribute[])propertyInfo.GetCustomAttributes(typeof(PropertyAttribute), true);
		if (customAttributes.Length == 0)
			return;
		Attribute = customAttributes[0];
	}

	public string PropertyName => PropertyInfo.Name;
}