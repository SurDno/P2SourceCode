using System;

namespace Engine.Common.Generator
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public class StateSaveProxyAttribute : Attribute
  {
    public string Name { get; set; }

    public MemberEnum Detail { get; set; }

    public StateSaveProxyAttribute(MemberEnum detail = MemberEnum.None) => Detail = detail;
  }
}
