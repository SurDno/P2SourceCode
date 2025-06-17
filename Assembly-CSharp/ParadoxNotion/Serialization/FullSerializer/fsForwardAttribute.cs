using System;

namespace ParadoxNotion.Serialization.FullSerializer
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
  public sealed class fsForwardAttribute(string memberName) : Attribute 
  {
    public string MemberName = memberName;
  }
}
