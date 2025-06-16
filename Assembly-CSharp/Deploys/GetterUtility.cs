// Decompiled with JetBrains decompiler
// Type: Deploys.GetterUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Reflection;

#nullable disable
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
