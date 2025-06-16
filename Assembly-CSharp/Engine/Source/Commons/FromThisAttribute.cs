// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.FromThisAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common;
using System;
using System.Reflection;

#nullable disable
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
