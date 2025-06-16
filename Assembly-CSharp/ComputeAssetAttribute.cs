using Cofe.Meta;
using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class ComputeAssetAttribute : MemberAttribute
{
  public static readonly Guid Id = Guid.NewGuid();

  public override void ComputeMember(Container container, MemberInfo member)
  {
    container.GetHandler(ComputeAssetAttribute.Id).AddHandle((ComputeHandle) ((target, data) => ((MethodBase) member).Invoke(target, (object[]) null)));
  }
}
