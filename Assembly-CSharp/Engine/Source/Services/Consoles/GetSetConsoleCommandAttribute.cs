using Cofe.Meta;
using Cofe.Utility;
using Engine.Assets.Internal;
using Engine.Source.Services.Consoles.Binds;
using System;
using System.Reflection;

namespace Engine.Source.Services.Consoles
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class GetSetConsoleCommandAttribute : InitialiseAttribute
  {
    private string name;

    public GetSetConsoleCommandAttribute(string name) => this.name = name;

    public override void ComputeMember(Container container, MemberInfo member)
    {
      if (this.name.IsNullOrEmpty())
        return;
      Type valueType = member.GetValueType();
      Type type = member.DeclaringType;
      container.GetHandler(InitialiseAttribute.Id).AddHandle((ComputeHandle) ((target, data) =>
      {
        GetConsoleCommand.AddBind(type, valueType, this.name, true, (Func<object, object>) (target2 => member.GetValue(target2)));
        SetConsoleCommand.AddBind(type, valueType, this.name, true, (Action<object, object>) ((target2, value) => member.SetValue(target2, value)));
      }));
    }
  }
}
