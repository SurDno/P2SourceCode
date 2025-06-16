// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.APIPropertyInfo
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Utility;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System.Reflection;

#nullable disable
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
