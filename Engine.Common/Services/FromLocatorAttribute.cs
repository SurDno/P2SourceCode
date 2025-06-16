// Decompiled with JetBrains decompiler
// Type: Engine.Common.Services.FromLocatorAttribute
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Cofe.Meta;
using System;
using System.Reflection;

#nullable disable
namespace Engine.Common.Services
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class FromLocatorAttribute : MemberAttribute
  {
    public static readonly Guid Id = Guid.NewGuid();

    public override void ComputeMember(Container container, MemberInfo member)
    {
      Handler handler = container.GetHandler(FromLocatorAttribute.Id);
      FieldInfo field = member as FieldInfo;
      Type type = field.FieldType;
      ComputeHandle handle = (ComputeHandle) ((target, data) =>
      {
        object service = ServiceLocator.GetService(type);
        field.SetValue(target, service);
      });
      handler.AddHandle(handle);
    }
  }
}
