using System;

namespace ParadoxNotion.Design
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Delegate, AllowMultiple = false, Inherited = true)]
  public class SpoofAOTAttribute : Attribute
  {
  }
}
