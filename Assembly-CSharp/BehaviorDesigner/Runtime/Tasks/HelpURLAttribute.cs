using System;

namespace BehaviorDesigner.Runtime.Tasks
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class HelpURLAttribute(string url) : Attribute 
  {
    public string URL => url;
  }
}
