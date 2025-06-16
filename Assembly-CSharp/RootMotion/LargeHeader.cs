using System;
using UnityEngine;

namespace RootMotion
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class LargeHeader : PropertyAttribute
  {
    public string name;
    public string color = "white";

    public LargeHeader(string name)
    {
      this.name = name;
      this.color = "white";
    }

    public LargeHeader(string name, string color)
    {
      this.name = name;
      this.color = color;
    }
  }
}
