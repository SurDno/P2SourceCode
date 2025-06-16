using System;
using System.Reflection;
using Cofe.Meta;

namespace Engine.Common.Services
{
  [AttributeUsage(AttributeTargets.Field)]
  public class FromLocatorAttribute : MemberAttribute
  {
    public static readonly Guid Id = Guid.NewGuid();

    public override void ComputeMember(Container container, MemberInfo member)
    {
      Handler handler = container.GetHandler(Id);
      FieldInfo field = member as FieldInfo;
      Type type = field.FieldType;
      ComputeHandle handle = (target, data) =>
      {
        object service = ServiceLocator.GetService(type);
        field.SetValue(target, service);
      };
      handler.AddHandle(handle);
    }
  }
}
