using System;

namespace Engine.Common.Generator
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class DataReadProxyAttribute : Attribute
  {
    public string Name { get; set; }

    public MemberEnum Detail { get; set; }

    public DataReadProxyAttribute(MemberEnum detail = MemberEnum.None) => this.Detail = detail;
  }
}
