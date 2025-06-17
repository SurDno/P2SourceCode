using System;
using System.Reflection;
using Cofe.Meta;
using Cofe.Utility;
using UnityEngine;

namespace Engine.Source.Services.Consoles
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
  public class ConsoleCommandAttribute(string name) : InitialiseAttribute 
  {
    public override void ComputeMember(Container container, MemberInfo member)
    {
      if (name.IsNullOrEmpty())
        return;
      MethodInfo method = member as MethodInfo;
      if (method == null)
        return;
      ParameterInfo[] parameters = method.GetParameters();
      if (method.ReturnType != typeof (string) || parameters.Length != 2 || parameters[0].ParameterType != typeof (string) || parameters[1].ParameterType != typeof (ConsoleParameter[]))
        Debug.LogError("Console command wrong parameters : " + name);
      else
        container.GetHandler(Id).AddHandle((target, data) => ConsoleService.RegisterCommand(name, (command, parameters2) =>
        {
          try
          {
            return (string) method.Invoke(target, [
              command,
              parameters2
            ]);
          }
          catch (Exception ex)
          {
            return ex.ToString();
          }
        }));
    }
  }
}
