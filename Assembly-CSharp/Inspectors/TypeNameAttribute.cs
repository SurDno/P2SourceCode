// Decompiled with JetBrains decompiler
// Type: Inspectors.TypeNameAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Cofe.Serializations.Data;
using System;

#nullable disable
namespace Inspectors
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class TypeNameAttribute : TypeAttribute
  {
    public string TypeName { get; set; }

    public string MenuItem { get; set; }

    public override void PrepareType(Type type)
    {
      TypeResolver.AddType(type);
      TypeNameService.RegisterTypeName(type, this.TypeName, this.MenuItem);
    }
  }
}
