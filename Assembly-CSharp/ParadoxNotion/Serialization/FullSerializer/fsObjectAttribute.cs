// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.fsObjectAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
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
