using System;

namespace Engine.Common.Generator
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class StateSaveProxyAttribute : Attribute
  {
    public string Name { get; set; }

    public MemberEnum Detail { get; set; }

    public StateSaveProxyAttribute(MemberEnum detail = MemberEnum.None) => this.Detail = detail;
  }
}
