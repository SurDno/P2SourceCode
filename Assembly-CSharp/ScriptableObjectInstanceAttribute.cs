using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ScriptableObjectInstanceAttribute(string path) : Attribute 
  {
  public string Path { get; private set; } = path;
}
