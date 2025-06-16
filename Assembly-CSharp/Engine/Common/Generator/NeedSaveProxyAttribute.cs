using System;

namespace Engine.Common.Generator
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class NeedSaveProxyAttribute : Attribute
  {
  }
}
