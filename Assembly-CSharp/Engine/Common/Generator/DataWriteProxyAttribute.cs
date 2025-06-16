using System;

namespace Engine.Common.Generator
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class DataWriteProxyAttribute : Attribute
  {
    public MemberEnum Detail;

    public string Name { get; set; }

    public DataWriteProxyAttribute(MemberEnum detail = MemberEnum.None) => this.Detail = detail;
  }
}
