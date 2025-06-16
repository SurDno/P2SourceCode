using Cofe.Meta;
using Cofe.Utility;
using System;
using System.Reflection;
using UnityEngine;

namespace Engine.Source.Services.Consoles
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
  public class ConsoleCommandAttribute : InitialiseAttribute
  {
    private string name;

    public ConsoleCommandAttribute(string name) => this.name = name;

    public override void ComputeMember(Container container, MemberInfo member)
    {
      if (this.name.IsNullOrEmpty())
        return;
      MethodInfo method = member as MethodInfo;
      if (method == (MethodInfo) null)
        return;
      ParameterInfo[] parameters = method.GetParameters();
      if (method.ReturnType != typeof (string) || parameters.Length != 2 || parameters[0].ParameterType != typeof (string) || parameters[1].ParameterType != typeof (ConsoleParameter[]))
        Debug.LogError((object) ("Console command wrong parameters : " + this.name));
      else
        container.GetHandler(InitialiseAttribute.Id).AddHandle((ComputeHandle) ((target, data) => ConsoleService.RegisterCommand(this.name, (Func<string, ConsoleParameter[], string>) ((command, parameters2) =>
        {
          try
          {
            return (string) method.Invoke(target, new object[2]
            {
              (object) command,
              (object) parameters2
            });
          }
          catch (Exception ex)
          {
            return ex.ToString();
          }
        }))));
    }
  }
}
