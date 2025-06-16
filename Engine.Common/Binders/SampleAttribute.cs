// Decompiled with JetBrains decompiler
// Type: Engine.Common.Binders.SampleAttribute
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Cofe.Meta;
using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Common.Binders
{
  [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
  public class SampleAttribute : TypeAttribute
  {
    public readonly string Name;
    private static Dictionary<string, Type> types = new Dictionary<string, Type>();
    private static Dictionary<Type, string> names = new Dictionary<Type, string>();

    public SampleAttribute(string name) => this.Name = name;

    public override void ComputeType(Type type)
    {
      SampleAttribute.RegisterSampleType(this.Name, type);
    }

    private static void RegisterSampleType(string name, Type type)
    {
      SampleAttribute.types.Add(name, type);
      SampleAttribute.names.Add(type, name);
    }

    public static bool TryGetValue(Type type, out string result)
    {
      return SampleAttribute.names.TryGetValue(type, out result);
    }

    public static bool TryGetValue(string name, out Type result)
    {
      return SampleAttribute.types.TryGetValue(name, out result);
    }
  }
}
