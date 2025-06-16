// Decompiled with JetBrains decompiler
// Type: SRF.Service.ServiceAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace SRF.Service
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class ServiceAttribute : Attribute
  {
    public ServiceAttribute(Type serviceType) => this.ServiceType = serviceType;

    public Type ServiceType { get; private set; }
  }
}
