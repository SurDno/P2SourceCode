using Cofe.Meta;
using Engine.Common;
using System;
using System.Reflection;

namespace Engine.Source.Commons
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class FromThisAttribute : MemberAttribute
  {
    public static readonly Guid Id = Guid.NewGuid();
    public static readonly Guid ClearId = Guid.NewGuid();

    public override void ComputeMember(Container container, MemberInfo member)
    {
      Handler handler = container.GetHandler(FromThisAttribute.Id);
      FieldInfo field = member as FieldInfo;
      Type type = field.FieldType;
      handler.AddHandle((ComputeHandle) ((target, data) =>
      {
        if (!(target is IComponent component2))
          return;
        IComponent component3 = component2.Owner.GetComponent(type);
        field.SetValue(target, (object) component3);
      }));
      container.GetHandler(FromThisAttribute.ClearId).AddHandle((ComputeHandle) ((target, data) => field.SetValue(target, (object) null)));
    }
  }
}
