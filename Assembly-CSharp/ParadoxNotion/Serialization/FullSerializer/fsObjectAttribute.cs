using System;

namespace ParadoxNotion.Serialization.FullSerializer
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
  public sealed class fsObjectAttribute : Attribute
  {
    public Type[] PreviousModels;
    public string VersionString;
    public fsMemberSerialization MemberSerialization = fsMemberSerialization.Default;
    public Type Converter;
    public Type Processor;

    public fsObjectAttribute()
    {
    }

    public fsObjectAttribute(string versionString, params Type[] previousModels)
    {
      this.VersionString = versionString;
      this.PreviousModels = previousModels;
    }
  }
}
