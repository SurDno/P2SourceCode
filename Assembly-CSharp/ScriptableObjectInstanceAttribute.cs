using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ScriptableObjectInstanceAttribute : Attribute
{
  public string Path { get; private set; }

  public ScriptableObjectInstanceAttribute(string path) => this.Path = path;
}
