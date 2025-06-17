using System;

namespace Engine.Common.Generator
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public class DataWriteProxyAttribute(MemberEnum detail = MemberEnum.None) : Attribute 
  {
    public MemberEnum Detail = detail;

    public string Name { get; set; }
  }
}
