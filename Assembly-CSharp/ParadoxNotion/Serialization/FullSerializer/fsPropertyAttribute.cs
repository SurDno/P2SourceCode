// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.fsPropertyAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace ParadoxNotion.Serialization.FullSerializer
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public sealed class fsPropertyAttribute : Attribute
  {
    public string Name;
    public Type Converter;

    public fsPropertyAttribute()
      : this(string.Empty)
    {
    }

    public fsPropertyAttribute(string name) => this.Name = name;
  }
}
