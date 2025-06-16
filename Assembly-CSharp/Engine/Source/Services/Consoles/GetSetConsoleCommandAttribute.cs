// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Consoles.GetSetConsoleCommandAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Cofe.Utility;
using Engine.Assets.Internal;
using Engine.Source.Services.Consoles.Binds;
using System;
using System.Reflection;

#nullable disable
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
