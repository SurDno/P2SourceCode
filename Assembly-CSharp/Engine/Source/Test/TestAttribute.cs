using Cofe.Meta;
using System;
using System.Reflection;

namespace Engine.Source.Test
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
  public class TestAttribute : MemberAttribute
  {
    public static readonly Guid Id = Guid.NewGuid();

    public override void ComputeMember(Container container, MemberInfo member)
    {
      container.GetHandler(TestAttribute.Id).AddHandle((ComputeHandle) ((target, data) => ((MethodBase) member).Invoke(target, (object[]) null)));
    }
  }
}
