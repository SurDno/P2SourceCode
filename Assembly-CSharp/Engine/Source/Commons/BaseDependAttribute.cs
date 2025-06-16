// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.BaseDependAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace Engine.Source.Commons
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public abstract class BaseDependAttribute : Attribute
  {
    public Type Type { get; private set; }

    public BaseDependAttribute(Type type) => this.Type = type;
  }
}
