using System.Reflection;

namespace Deploys
{
  public static class GetterUtility
  {
    public static object GetValue(object target, MemberInfo member)
    {
      FieldInfo fieldInfo = member as FieldInfo;
      if (fieldInfo != (FieldInfo) null)
        return fieldInfo.GetValue(target);
      PropertyInfo propertyInfo = member as PropertyInfo;
      return propertyInfo != (PropertyInfo) null ? propertyInfo.GetValue(target, (object[]) null) : (object) null;
    }
  }
}
