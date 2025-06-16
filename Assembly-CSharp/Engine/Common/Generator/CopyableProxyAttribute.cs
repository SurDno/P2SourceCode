using System;

namespace Engine.Common.Generator
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class CopyableProxyAttribute : Attribute
  {
    public MemberEnum Detail { get; set; }

    public CopyableProxyAttribute(MemberEnum detail = MemberEnum.None) => this.Detail = detail;
  }
}
