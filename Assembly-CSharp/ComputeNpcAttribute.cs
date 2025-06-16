using System;
using System.Reflection;
using Cofe.Meta;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class ComputeNpcAttribute : MemberAttribute
{
  public static readonly Guid Id = Guid.NewGuid();

  public override void ComputeMember(Container container, MemberInfo member)
  {
    container.GetHandler(Id).AddHandle((target, data) =>
    {
      ((MethodBase) member).Invoke(target, null);
      ((Action) data)();
    });
  }
}
