using System;

namespace Engine.Common.Generator
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public class StateLoadProxyAttribute(MemberEnum detail = MemberEnum.None) : Attribute 
  {
    public string Name { get; set; }

    public MemberEnum Detail { get; set; } = detail;
  }
}
