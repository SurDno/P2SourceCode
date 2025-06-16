using Cofe.Meta;
using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class ComputeBytesAttribute : MemberAttribute
{
  public static readonly Guid Id = Guid.NewGuid();

  public override void ComputeMember(Container container, MemberInfo member)
  {
    container.GetHandler(ComputeBytesAttribute.Id).AddHandle((ComputeHandle) ((target, data) => ((MethodBase) member).Invoke(target, (object[]) null)));
  }
}
