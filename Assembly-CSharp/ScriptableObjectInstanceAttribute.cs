using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ScriptableObjectInstanceAttribute : Attribute
{
  public string Path { get; private set; }

  public ScriptableObjectInstanceAttribute(string path) => Path = path;
}
