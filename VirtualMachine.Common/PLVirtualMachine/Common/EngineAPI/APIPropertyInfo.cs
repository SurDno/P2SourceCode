using Cofe.Utility;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System.Reflection;

namespace PLVirtualMachine.Common.EngineAPI
{
  public class APIPropertyInfo
  {
    public PropertyAttribute Attribute;
    public PropertyInfo PropertyInfo;
    public VMType PropertyType;
    public object PropertyDefValue;

    public APIPropertyInfo(PropertyInfo propertyInfo, VMType propertyType)
    {
      this.PropertyInfo = propertyInfo;
      this.PropertyType = propertyType;
      this.PropertyDefValue = TypeDefaultUtility.GetDefault(this.PropertyType.BaseType);
      PropertyAttribute[] customAttributes = (PropertyAttribute[]) propertyInfo.GetCustomAttributes(typeof (PropertyAttribute), true);
      if (customAttributes.Length == 0)
        return;
      this.Attribute = customAttributes[0];
    }

    public string PropertyName => this.PropertyInfo.Name;
  }
}
