using Cofe.Meta;
using Cofe.Serializations.Data;
using System;

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
