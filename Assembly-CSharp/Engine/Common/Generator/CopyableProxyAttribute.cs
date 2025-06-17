using System;

namespace Engine.Common.Generator
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public class CopyableProxyAttribute(MemberEnum detail = MemberEnum.None) : Attribute 
  {
    public MemberEnum Detail { get; set; } = detail;
  }
}
