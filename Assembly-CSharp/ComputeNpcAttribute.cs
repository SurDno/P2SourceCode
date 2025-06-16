// Decompiled with JetBrains decompiler
// Type: ComputeNpcAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using System;
using System.Reflection;

#nullable disable
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
