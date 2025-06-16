// Decompiled with JetBrains decompiler
// Type: Engine.Impl.Services.Factories.RequiredAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace Engine.Impl.Services.Factories
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
  public sealed class RequiredAttribute : Attribute
  {
    public Type Type { get; private set; }

    public RequiredAttribute(Type Type) => this.Type = Type;
  }
}
