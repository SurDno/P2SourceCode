using System;

namespace Engine.Common.Generator
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public class DataReadProxyAttribute : Attribute
  {
    public string Name { get; set; }

    public MemberEnum Detail { get; set; }

    public DataReadProxyAttribute(MemberEnum detail = MemberEnum.None) => Detail = detail;
  }
}
