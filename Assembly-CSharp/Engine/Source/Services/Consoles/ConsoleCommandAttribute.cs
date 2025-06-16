// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Consoles.ConsoleCommandAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Cofe.Utility;
using System;
using System.Reflection;
using UnityEngine;

#nullable disable
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
