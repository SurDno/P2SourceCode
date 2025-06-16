using Cofe.Meta;
using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class ComputeNpcAttribute : MemberAttribute
{
  public static readonly Guid Id = Guid.NewGuid();

  public override void ComputeMember(Container container, MemberInfo member)
  {
    container.GetHandler(ComputeNpcAttribute.Id).AddHandle((ComputeHandle) ((target, data) =>
    {
      ((MethodBase) member).Invoke(target, (object[]) null);
      ((Action) data)();
    }));
  }
}
