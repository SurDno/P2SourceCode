// Decompiled with JetBrains decompiler
// Type: Engine.Common.Generator.GenerateProxyAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace Engine.Common.Generator
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class GenerateProxyAttribute : Attribute
  {
    public Type Type { get; set; }

    public TypeEnum Detail { get; set; }

    public GenerateProxyAttribute(TypeEnum detail) => this.Detail = detail;
  }
}
