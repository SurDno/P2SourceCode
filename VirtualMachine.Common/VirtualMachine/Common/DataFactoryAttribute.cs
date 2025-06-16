// Decompiled with JetBrains decompiler
// Type: VirtualMachine.Common.DataFactoryAttribute
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Meta;
using System;
using System.Collections.Generic;

#nullable disable
namespace VirtualMachine.Common
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class DataFactoryAttribute : TypeAttribute
  {
    private static Dictionary<string, Type> types = new Dictionary<string, Type>();

    public string TypeName { get; set; }

    public DataFactoryAttribute(string typeName) => this.TypeName = typeName;

    public override void ComputeType(Type type)
    {
      DataFactoryAttribute.types.Add(this.TypeName, type);
    }

    public static Type GetTypeByName(string typeName)
    {
      Type typeByName;
      DataFactoryAttribute.types.TryGetValue(typeName, out typeByName);
      return typeByName;
    }

    public static IEnumerable<KeyValuePair<string, Type>> Items
    {
      get => (IEnumerable<KeyValuePair<string, Type>>) DataFactoryAttribute.types;
    }
  }
}
