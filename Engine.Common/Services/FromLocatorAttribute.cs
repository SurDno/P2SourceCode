using Cofe.Meta;
using System;
using System.Reflection;

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
